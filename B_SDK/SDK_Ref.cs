﻿using Morpho.MorphoAcquisition;
using Morpho.MorphoSmart;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace B_SDK
{
    public class BService
    {
        private bool _isDeviceFound;
        private string MSG;
        private string[] connectedDevice;
        private byte[] image_buffer;
        private int quality;
        private ProductList deviceName;
        private MorphoSmartDevice morphoSmart;

        public BService()
        {
            morphoSmart = new MorphoSmartDevice();
        }


        // Step 1 (event service)
        public bool IsDeviceConnected()
        {
            try
            {
                if (GetMorphoDevice())
                {
                    _isDeviceFound = true;
                }
                else
                {
                    _isDeviceFound = false;
                }
            }
            catch (Exception ex)
            {
                _isDeviceFound = false;
                MSG = ex.Message;
            }

            return _isDeviceFound;
        }


        // Step 2 (event service)
        public List<Fingerprint> Capture(string fingerId)
        {
            int[] flist = new int[]
            {
                Convert.ToInt32(fingerId)
            };

            //List<Fingerprint> result = this.device.StartCapturing(flist);
            List<Fingerprint> result = StartCapturing(flist);

            return result;
        }

        #region Supporting Parts
        public class Fingerprint
        {
            internal Bitmap BmpImage;
            public int quality;
            internal int score;
            internal byte[] Template;
            public string wsq_image;
            public string finger_image;
            public string finger_template;
            internal string connection_error;
            internal string device_error;
            public string success;
            public string fail;
            public string error;
            public string instruction;
        }

        internal enum ProductList
        {
            FUTRONIC = 0,
            CROSSMATCH = 1,
            SECUGEN = 2,
            IDENTIX500 = 3,
            DIGITALPERSONA = 4,
            LUMIDIGMVENUS = 5,
            LUMIDIGMMERCURY = 6,
            MORPHO = 7,
            REALSCAND = 20, // 0x00000014
        }

        #endregion

        #region Device Connection
        // Step 1.1  (event service)
        private bool GetMorphoDevice()
        {
            bool result = false;
            try
            {
                //var device = new MorphoService();
                //if (device.DeviceAccess() > 0)
                if (DeviceAccess() > 0)
                {
                    //device.DeviceInit(TemplateType);
                    DeviceInit();
                    deviceName = ProductList.MORPHO;
                    result = true;
                    MSG = "Morpho device found";
                }
            }
            catch (Exception ex)
            {
                result = false;
                MSG = ex.Message;
            }

            return result;
        }

        // Step 1.2 (morpho service)
        public int DeviceAccess()
        {
            connectedDevice = morphoSmart.GetConnectedDevices();
            return connectedDevice?.Length ?? 0;
        }

        // Step 1.3 (morpho service)
        public void DeviceInit(string templateFormat = "ANSI378_0")
        {
            TemplateFormat format;
            switch (templateFormat)
            {
                case "ANSI378_0":
                    format = TemplateFormat.ANSI_378;
                    break;
                case "ANSI378_1":
                    format = TemplateFormat.ISO_19794_2_Card_Format_Normal_Size;
                    break;
                case "ANSI378_2":
                    format = TemplateFormat.ISO_19794_2_Card_Format_Compact_Size;
                    break;
                case "ISO19794_0":
                    format = TemplateFormat.ISO_19794_2_FMR;
                    break;
                default:
                    format = TemplateFormat.ANSI_378;
                    break;
            }

            morphoSmart.SetActiveDevice(connectedDevice[0]);
            morphoSmart.SetTemplateFormat(format);
            morphoSmart.SetCoderAlgorithm(CoderAlgorithm.EmbeddedCoder);
            morphoSmart.SetConsolidation(false);
            morphoSmart.SetExtendedSecurityLevel(ExtendedSecurityLevel.STANDARD);
            morphoSmart.SetTimeout(30);
            morphoSmart.FireQualityEvent((byte)80);
            morphoSmart.SetJuvenileMode(false);
            morphoSmart.SetLiveQualityThreshold(80);
            morphoSmart.RegisterImageEvent(ImageHandler);
            morphoSmart.RegisterQualityEvent(QualityHandler);
        }

        // Step 1.4 (morpho service)
        private void ImageHandler(byte[] ImageBuffer, int NbLine, int NbColumn, int Resolution)
        {
            image_buffer = ImageBuffer;
        }

        // Step 1.5 (morpho service)
        private void QualityHandler(byte Quality)
        {
            quality = Quality;
        }
        #endregion

        #region Capture
        // Step 2.1 (morpho service)
        public List<Fingerprint> StartCapturing(int[] flist)
        {
            var result = new Fingerprint();
            try
            {
                EnrollmentResult enrollment = morphoSmart.Enroll();
                switch (enrollment.Status)
                {
                    case ErrorCodes.IED_ERR_INVALID_FINGER:
                        result.fail = "Invalid finger detected!";
                        result.instruction = "FINGER NOT OK";
                        break;
                    case ErrorCodes.IED_NO_ERROR:
                        int index = enrollment.BestImageIndex - 1;

                        result.error = MSG;
                        result.Template = enrollment.Template;
                        result.finger_template = Convert.ToBase64String(enrollment.Template);
                        result.finger_image = WSQCodecAPI.BmpToBase64(WSQCodecAPI.ByteToBitmap(enrollment.ImageList[index].Image, enrollment.ImageList[index].Width, enrollment.ImageList[index].Height));
                        result.quality = (int)enrollment.TemplateQuality;
                        //result.wsq_image = Convert.ToBase64String(EncoderHelper.RSAEncrypt(WSQCodecAPI.GenWSQImage(
                        //        enrollment.ImageList[index].Image, enrollment.ImageList[index].Width, enrollment.ImageList[index].Height), EncoderHelper.GetToken(), 128));
                        result.wsq_image = Convert.ToBase64String(RSAEncrypt(WSQCodecAPI.GenWSQImage(
                                enrollment.ImageList[index].Image, enrollment.ImageList[index].Width, enrollment.ImageList[index].Height), GetToken(), 128));
                        result.success = "Ok";
                        result.instruction = "FINGER OK";

                        break;
                }
            }
            catch (Morpho.MorphoAcquisition.TimeoutException ex)
            {
                result.fail = ex.Message;
                result.instruction = "FINGER NOT OK";
            }
            catch (Exception ex)
            {
                result.error = ex.Message;
                result.instruction = "FINGER NOT OK";
            }

            return new List<Fingerprint> { result };
        }
        #endregion

        #region Encoding

        // Step 2.2 (encoder service)
        internal static byte[] GetToken(string token = "g86v5s4g5se84g5sfd4g5werx25sdf4f")
        {
            switch (0)
            {
                default:
                label_2:
                    string[] SerialKeies = TCapEncrypt.SerialKeies;
                    int index = 0;
                    int num = 4;
                    string SerialKey = String.Empty;
                    while (true)
                    {
                        switch (num)
                        {
                            case 0:
                                num = 3;
                                continue;
                            case 1:
                                goto label_8;
                            case 2:
                                try
                                {
                                    return TCapEncrypt.FindBytes(Convert.FromBase64String(SerialKey),
                                        Encoding.UTF8.GetBytes(token));
                                }
                                catch (Exception ex)
                                {
                                }
                                ++index;
                                num = 0;
                                continue;
                            case 3:
                                if (index >= SerialKeies.Length)
                                {
                                    num = 1;
                                    continue;
                                }

                                SerialKey = SerialKeies[index];
                                num = 2;
                                continue;
                            case 4:
                                goto case 0;
                            default:
                                goto label_2;
                        }
                    }

                label_8:
                    throw new Exception("Invalid access token provided");
            }
        }

        // Step 2.4 (encoder service)
        internal static byte[] RSAEncrypt(byte[] source, byte[] token, int size)
        {
            byte[] numArray1 = null;
            byte[] numArray2 = new byte[16];
            new Random().NextBytes(numArray2);
            FingerDataEncode.Key = numArray2;
            byte[] numArray3 = FingerDataEncode.Encode(source, size);
            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)new X509Certificate2(token).PublicKey.Key;
            try
            {
                byte[] numArray4 = rsa.Encrypt(numArray2, false);
                numArray1 = new byte[numArray4.Length + numArray3.Length];
                Array.Copy(numArray4, numArray1, numArray4.Length);
                Array.Copy(numArray3, 0, numArray1, numArray4.Length, numArray3.Length);
            }
            finally
            {
                rsa.Dispose();
            }

            return numArray1;
        }

        #endregion
    }

    internal static class TCapEncrypt
    {
        private static string Pin;
        private static readonly byte[] InitVec;
        internal static readonly string[] SerialKeies;

        internal static byte[] FindBytes(byte[] A_0, byte[] A_1)
        {
            switch (0)
            {
                default:
                    int num1 = 0;
                    Rijndael rijndael = new RijndaelManaged();
                    while (true)
                    {
                        switch (num1)
                        {
                            case 1:
                                goto label_11;
                            case 2:
                                num1 = 3;
                                continue;
                            case 3:
                                num1 = A_0.Length != 0 ? 7 : 6;
                                continue;
                            case 4:
                                goto label_7;
                            case 5:
                                if (A_1.Length != 0)
                                {
                                    rijndael = Rijndael.Create();
                                    num1 = 1;
                                    continue;
                                }
                                num1 = 4;
                                continue;
                            case 6:
                                goto label_8;
                            case 7:
                                if (A_1 != null)
                                {
                                    num1 = 8;
                                    continue;
                                }
                                goto label_7;
                            case 8:
                                num1 = 5;
                                continue;
                            default:
                                if (A_0 != null)
                                {
                                    num1 = 2;
                                    continue;
                                }
                                goto label_8;
                        }
                    }
                label_7:
                    throw new ArgumentNullException("key");
                label_8:
                    throw new ArgumentNullException("encryptedText");
                label_11:
                    try
                    {
                        rijndael.BlockSize = 128;
                        rijndael.KeySize = 256;
                        rijndael.Key = A_1;
                        rijndael.IV = TCapEncrypt.InitVec;
                        rijndael.Padding = PaddingMode.PKCS7;
                        rijndael.Mode = CipherMode.CBC;
                        ICryptoTransform decryptor = rijndael.CreateDecryptor(rijndael.Key, rijndael.IV);
                        MemoryStream memoryStream = new MemoryStream();
                        try
                        {
                            CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Write);
                            try
                            {
                                cryptoStream.Write(A_0, 0, A_0.Length);
                            }
                            finally
                            {
                                int num2 = 2;
                                while (true)
                                {
                                    switch (num2)
                                    {
                                        case 0:
                                            cryptoStream.Dispose();
                                            num2 = 1;
                                            continue;
                                        case 1:
                                            goto label_19;
                                        default:
                                            if (cryptoStream != null)
                                            {
                                                num2 = 0;
                                                continue;
                                            }
                                            goto label_19;
                                    }
                                }
                            label_19:;
                            }
                            return memoryStream.ToArray();
                        }
                        finally
                        {
                            int num2 = 2;
                            while (true)
                            {
                                switch (num2)
                                {
                                    case 0:
                                        goto label_26;
                                    case 1:
                                        memoryStream.Dispose();
                                        num2 = 0;
                                        continue;
                                    default:
                                        if (memoryStream != null)
                                        {
                                            num2 = 1;
                                            continue;
                                        }
                                        goto label_26;
                                }
                            }
                        label_26:;
                        }
                    }
                    finally
                    {
                        int num2 = 0;
                        while (true)
                        {
                            switch (num2)
                            {
                                case 1:
                                    goto label_32;
                                case 2:
                                    rijndael.Dispose();
                                    num2 = 1;
                                    continue;
                                default:
                                    if (rijndael != null)
                                    {
                                        num2 = 2;
                                        continue;
                                    }
                                    goto label_32;
                            }
                        }
                    label_32:;
                    }
            }
        }

        static TCapEncrypt()
        {
            Pin = "6@c&5#4*s#48^9)g";
            InitVec = Encoding.UTF8.GetBytes(Pin);
            SerialKeies = new string[7]
            {
                "VbCYTGw9FKZebDmNTEO4A/zwsSGpEmK06mEJ3NzsZUAFldWEKXhSEF7wb5SeUw3SMMqM8i1raooJpKob4t2ckHCNCI/qdnaxb0XuHUb+nk5GS863Mh5h9z5N26/9WnHGxCaEGFONu4wIxpuMIDs3mTGuD3yaVRNLOYH4Q3Z1xDXYK+yPdBa5rAVXhHvvd4ZCNswSFDpSQXJrXjZdKK1KqD5HRfK7Kza0KiWX7MDfZhtSX3LA/GaobglEQoF2JY4AI7JdC4VOeGrnew0n8T7mvuLrNe64cnO9CSFv4vS+lLYCkov572orIFvhKNGaRGDfRpQkjsk9uI4/9Ie+2h0KoqTO7rQJbvqwC/MK6Qh6CejdNIvp0U6+oZZly2WLlbd4FOC9GTftwwVrdEUghm1LijFGGtqU2YS5JaZeyBIGmULOvHGoQ7qsxdJ5+8smV3TJBmjAptfd4wrbPOy1QPB2eOJXxj9rYVz6VQLqi3bhyVRBTc3647zhRv7IDWbWNqE5LO2Y+dbPpwFgZNOcSpJAbSHWn42svwijxkvl63Mn2pIeqAL3/mJwJJFpOew8S4lqGaMSilzgxFxqSYoL+VdQ8hgDRR9d2El9hnBWLqO1Gk6IKZpFQNi1YIl21c7WkiQQkCGaMsFAjzcAXFqMkEJpvkxSQKbNC7kGJdWHzGrk4LmeVkAbS9kKB5+hAVFVRBVtyn/vyLRsrvf9J+FrAGv2KUKQXXDYhxL+5JN01CO3EpOWcXTjvTxLBR3KwZBPhHvMuSbT1fsc/PPdbPcHH+yvXbMjWVTmWPFbAspiIN4SEDB7TQcoDNXIEMij/4jyZsDxiooOQVuepzlAEhX1Ec73H2cnjGfq0TH9gP6xyYPS0UrPuzKvqC7rtn2aNAqjnSgQHIyrRTtL8waUsTNfyBsd1AjxThbGnHb1RkbBmiAV/mTpfZB/uzIpi86TpTRaQoTX8NRxuruZDBa0hnlB06Z/h3QUWxzLl7s7JMpGcyqQpKsdj4YoNv6+qCPWkekevHEMn94HbE6iFZdfhaxreYaefA==",
                "bpP0yc7J6LHEU5UgUlYND9L7YmKUrLjV/G1HghOqS4pk/xC2PF7OHHTYNgwhLjIaqJ8qIFGr52csCVw4OMmfiUSFISuUAjZVICTOZDRtE9ikPFK8zz6VCdvMG9xaBef1s8xD9apZ/J7BU4B+/jYPa01MN4ZSQ0wcFQvxae9pomd/mn+9SScNMa55IXRm7xoLMqFAOlmbfOjd/7Gq/4IMzopK05K1Q7Kr3PeUXHE6CA/4oBWAiRIsdu07/JzJcaUSw8nLtxQj6cbqlwThbat7gbK30oPItCmrVvh8b2b0PphSU92+80QroXPz/Qi4zSNgekdA+HXRXSD/a1J+LXR0zMLUmn3xrFGnmu6+7Bu3dF+DeUSDHPF7wf2KIsWhTeFOX5rlMwGql/D4FuAHM5fF+F3ll4hN2ZZMEhpsn0ieEirhxC6uXVUdTqkklxQVamRxzlin/HOm8VsMNoHLBuJasct5sR1u+VzXo6P2ZfQw7j5cIbcog6zxoT5CKNe4KF2YG6EHXD0+2HPjIYckOT2y6bCrVkeSDzMGP0VycFeareHOh/N9VJzbv7mdDnEKkPvJW9t1N2bkAs09FuP5PkHpsR77URw75wn6H0C0U78D4pBl8krhMpIkg4TK0maeV7iRfCPx0Bcnm735oYVci1/mO32MCnDagyvOU/RMtTYzlkAeAZhMCKAAv/BBy3ZCu2T+y8NQNh/YYeZMKoNLOEZwxaIk9KOwWaO7o3rNqI2tCRf0w+0PehYW/iU/7kbFolXqrjbNK4A10vyGehE5pbOGkIqPabyBqW12gdNA244YPLQoiAJT9g2aAA+t59GaBXQY+YjpyD6trkwpO5SalSAylOsKsk7ceXOeZBFXN5MEZ8oE0/xxVySyfjxLlv3kn/v4x8Zn8iqC/uKOINI3GHRgcl03ADbTXy2OLqveRectVNs7i/lNmzn/z5Gktc9qmGexwE0qmZul29ZDkUlCaMElszSzb/BhrMlQmmvydJTlSHCcWZ3evySdKiboT4WL62DX/8gWrFCii3GqsHOBrJKkIQ==",
                "m5z0tuiTnxPG+1ERx24qTXmopUP4iDnXDTn7d54x2KnGvhyfdtzWVdNGUSVJ60x3Hn5xPkDemek5JHlRKxT9cdpvB+VCqyWUk+Q204rOni7Yi2o1W5gwKMmUlA3wLQxFu1ZZVgDpL2MB0BpZFxIqGm4PPVmENpkLmST5BPTFvdYHlTQo0rYjnhyD6hAIwPov/q75sW2gljoNskxfV1C3HsGtENKkSgr54jUyTcnen4lLgyN9vkeiHgD2jzk/qutePTSWKwaA1uXxJ4+CsSBke1gY/ROE5ev1JpjuokXLNn+LeLVPmCx2G5lCWDHHTJYfsXokVGSOSoRUaJgm2R7Im9uL64NucYVSZGOn6rvaq8juxWZXN7tzUoCyr8ja01Zbg84sicyqEOVU8nEW/zcibg+UpKrBIQs5Bdk+o3nNvzLDp9k/vEavR3etKzI6qFEbK4SwHv1KQaseyCK6BM1UF01CAmobuvPEJqNrX/yju+g+kopOL/HUorVica4oZqI2hAHN8/H5VRWpucsXLRWPoNxxzn7e2awcQX0duHqSDThdqjJCl2WK4/vgOz4h0Km/wpO/zfigQJpdqDKTI7thjIsD18GQd5QgEEiQETEGF81N/S0nRWzcG5bODsA8bkfEcJHYtFlIzCDFQWjtEmXKBGUQ1VdL2O4jobWikjp3yWAgLmtPePAP516rTsd2mZ53WSxx395xQyMDgLrtGs1LL6V3WEC8AIXAcmMpcGy4i8o3VQNHAZwkKjeBNnu581gu41zsNjN8indzX65GWB9QOsJbEpgvMTdBHWaYaMbIcyDrL2tGRqQTCEhCOl3+Iqg4gFGidTd9F4JpiG+r1okgsRYT+BnV95OIEPIvaJwrBQIsHaHcp68H7aJn5iOFVSkPTOw+i6TJSfEvVTSxDr2JeYxHYx3fh5NoaUzQi2Tmrg4kXBiW7nuz3gPGq/kW20SprUKyzhnNdJTdAMvOzgzELcDdvJVXHEHV6cWohtFBhcXKLGMayBGWIwNmeqww+jFPA1tc1XQSd+QyS/DZgtyKAg==",
                "LJm6RgRxJZj+kvfrdHzJPzytrM19YKTwEaBGJD9OuEk5CwOfGoSIFmxUWYQ49mtWTFaPNP+fR37DQcuI8hkkn7cL9sBLckX3WJAbtSZTmxyhH6vnCTRG28X0J9c2BNiseHwH6NpBxLihEOnmCvPumed38s9/+rmU2aI1zzuawV51kej83ze4zO5zQj4oD65UMQDppIm2E7fIJqzVLCFmYo8w8iL9f767T4n9jZ9BELcBIApnFG8KveFB+xkSiLK4aGUNXNZhnl03UxhAoN3hEuSp/6oF1ICHf2ZcgMViSHG0zvZDgvv3ZHFKch4hIIuNd5MLJ/1Ascz4VuvAux9BUTB7J+aqLN3csSpdQWoeFKuVZ2ZsHPkTuEy/E29dAN1TByJt/no0BZeiPC/z5vpoRMB4mvHVsWlF/tKpGPSmEtsyxIr48qckHml8dPRrS7k5UMZ1cAKlF+PDKZoZ1yT/W4XJi1HHjcaeb7tgCok/f6sj4acSaCjwC9olNjWuIPhZb8pRI6pIwlf9SPlwCizbxjmjaQE7+fPbXVnWTkupIeql20t7gEMtp+0/DKUkronrFeZdm+8wA0nH6fb50XNZzK2sSN7qR0Z5F/4mEEOZiAIIfIhp96xdA9TWUKidz7LVawbwtxVjIB2Y4Mo0Kh3ViMvGUwjEjlvOmOaEHmOUiwiY2ggzHHoW7luVkFV7X3N87prcPpbnY/N12vps9b+AR4QFg7LFdXJT4CxmKFxFKjSdR/oVZJTiKVLw3JOEHtvfDE1ulFOjX38dtx/V9x/XLw+epquoH5fO1SawkdIXNJ+sI/aEQhcpAnR8a2kLidJ/4MKcYENz4c9kO7Ub1UVJ7mXFmkBNYwwygYR2g5NzUh61a0UuA30cZkh80ifF3t0NXTaxq7cqXekkMdPnOaAzAwO9uDlE68vcFIkd1asI/hisBHF/HMoXroD3qet9m06H2ZFBAlEm+MSJyA4egLskktvEnBtewqrw6LybTBHSp/ACR+g1/C0oqEnSkKO0qqzRLGCP/RHAysyCLFMnaL6y1g==",
                "Mx7m836klv2vCPTkCN2/nOMoq4XWJfEgXt1C2b+I61hvmd37axdFTKzmd8/oi8AzrCpH0KSldAWSVKtn47VkwEcXKKWvL6vv3oXZ6a54b4F+TiyYkkm/6RMWWk47cmPO3f80KXZbNwTABIiugNSMc28/Yy5CFSDdNLlMueMre71ABsQFnXorAfy81qpMtmxp+PGMnULF6csuivWs8lIiyL44M4WYL6le6twz1Of1zckKuwgVx/TglQ/pl4jsRpGUeWpvq3OaOYRglDJxC80ATGu68Pgdb1PR0w1VuGiD67OGNpjYgefHx4W2/H8t4d90/UhDxlJmiR2mrv0pebtNWoL+gF8LM3OfjGnfxkjmRAO/pnPpzi/UQj55sbkt+hb61qFx882bJ5bKdjUdjGU6zhnSk9ZHQH7x1qSwHypvsqiZXyROmvVt0BHWFeNlRM0cSL/d+L5NqttMMaV/CU7x+fNICdGA7rDB+TohMqmBfayP5ul66ag8T5l8ll0S40y3Y0pd6U9vWUt1A/PDnP3BnFB0lKChNzWdhG1MNzTGsEqVBYFQwC37iz8wcGuKz8zQ0MEfXP9gNnYp9ky6QilM3k5Wp6EfV/2wNE3Bxam7jIG1dtGK/4GKJ2y8NLUSu9+yBiw5XRsuvKZtUi/VOz/Z8DcPaQOSfz1vd1RwkTVeAdyeYkQlMgfK1f4ti3MjTIr0uBzl0dTOvbh06SPfTXH+1dkhxc4YfDxgl2xv9Su8xuHqlU87YdSXqZTckl8eJZM/t8VrRLZ9CYpx9CIDky1crQwTVDB2EAa94Akj1vA3gNOx80T2m0gnI8aCmIw/g+3z1xEQ65eM2mLMnsyPgQ0hM4IyuJDa9CBGfjXA3ChZrl9mrdWpIsB5gPyizlE6Ag9Thb44ymgCZXVgp8cx02T8b3iSKQRuaMCVtV4l7h4qw2kcuN9uq7iWR+EH0BLLFOQoHI0W5pO3OgvKeQoGg4cpsYSQ01Jwk14Pt3Rha5oxkM7rzlDDOJqqSUCCLMWrNMKmFv1tAH3b5fJ/qYytfGrpzw==",
                "DgnUwV/sMImCqWV5It9Nng0DL4TT4b2dqcedGts3Zzury7o/2jhCbcPoaLQ6Vb/bImbPqTVKDomjXPetbTXaAJPOUwgV3w6WNYdI1DjtiFo8YzXyRZcSDLTX+V7hLRlrCWsRNDy2OC1qsxv3Cb/UQ8JC8a84cfCJ7KS4BAoVq0/0iIWKcJB3e5tHxQL5gnpwqH+LvwYZpSfCaK1vI6KOYnfy3pvdTG9t5BCfIgQXRI0orwtrBZdQ71i9PoMjOrkv2Pn2qqx2XaKN0p3PsdRxnd/NLvx5T5UXVfX3T+PK1MXmy2DEE5aKqst8tsPxvBCbvMvWeGiJ/VsDbOrdAu5rOSclnvkFMb2zr5ec+KT2rvar43/R7PLSW72x0J4XIW7CBBht4EeK+Q3+rjJQ/duKt+jRm+DrfOgSM/CIFC0laCVi7lzNzfs3ylW63758DhwSQw453puPMy9KSFaDYYk111snoQJ4oPBReNyvDLp8DMof6RBbJpu6WqnbSI3/F7KKQcRP5qiiN9JHjg6DA52qhjt/ZTEkZOwzyfU9KNe2pfwMr+lNvrOluLu5cvlpUAExR5OrJS8pUewLNeQ3jGeTH78b+963ug7pVjhLanJ1UQePAK+Xj9XURLgyy/RFL3Kjws3k3qlxdxlyyz0ZwY89JYrlTWYcrh53USOj+sEioPrLew9BA6FQfOPECxHcIY86C+PYg2yHV1zV97SRs35VkeYV7W09zkzX7nK+eTi5ttfFryezdCCmGRXmWqmwVSS+6JoBQXlW7mXCmErxyOnhKtqhdPVw+9Vf3XX7SUk6Zl6PRS7lw20bPDm3Kt58Ay4ioUrNtSb+DY4YVfqUOheWjKKmKPp4qhV9CKr1zQ4/5zY/hyv5x32jYJFbRr1KOmgmZBR0NlLF4pSWym/G5kG+lCGDw1zknoOrBVUSoqlqvDER2Q5u9UKHo8CejXuiTVFJ1Xk90WCBIggYD4cDRpGOQIy1CmXMuNZQpJCDMy11qoG9fEGS5qIMWKT5DxuKMn+jvOlK+JVZqOeIay5MclP7nA==",
                "elMUGlhdlsZntF7beFwO9syZ47F8n0Qc5+WrTofA5Imm5JwDXy5ay25PSvzFqCR6+18tWs/NWmwuXDOartmW1LbA0g48n+PxBEw1XKyjTt66Z14Nkte3YbWDnDnfA2HvhboR/L/Mydrg4zB8NoaO4KA/XxyKSMUPGOxJkF/0Idy3Wge9Jk90Bxjlf53F2+E+jLkUSxjlAr4IhKaxFtT6RZ/CeaUa5hdN3IaWqPlI6jniEtn+xTcDwD2UgWWogQxYO0hrt+MXABEioA/0u9Lyq8IBPTGvDq5x5UCHV2rw7RPxnNbYOkC1Gn3wBtw+c37VjgcVrQAooiEnQJTT9uUdt3GY2HhEPTj2urD9AZCR3ihUBe7/GzRydsp7V6U8mVrJ5EKul75wpN9S8uCXlmCP7b4bYADwXcMZPuoB8/7SoKdOh7S1TgymDFQvGNiZYQ86AnSs05w/0+I0bXLCWZKAlnmHbtweuz2us8XXieJ5oQ40RYYIbWbk7ad/kLe0vHDOXZh0ite/bphgfvUGQT+vHIYKz7x66ANN8xH5ejNIJ3GEG5R5H4eOIZa6rHT8RwCPfr47CLgpuAwnhndGfz8cRBqRBrLf/JNeJC1v5MKP7zJ46AHhI9LgSaDZQvPToNSrHyFO6TectzzfUmW7P7WZAYAGqKuHTNRactCzjSwvWGQ2z3C4BuNxHGTjMRTH5j2X1YxtJnwUaGeLhBVT9xjO6tAyxe+zWSB55oEpmsacW0+I2iSGoD63o78DZHxmmEw6D972O2Wj9wHsZCXO8dmrLUzUUQUszZisnyQVyVecz5gICceXUhkgQBu1TSZrPiJ7jSphIHnpmKNM82b1qYBijvwpnB9w7tDMI7AXthRI4Iti4+5nAhxUvl50l3e+GQxIDXDQ/LOOOoiwM7Qh+tlcKQkIEjyu0LrzBXZpc+jHSlnrdkBeL2QIy5WtorpM/Dq9MtQgFc+seDY5s9CosnUSBtB/1pSGXn2QrWuO9nkbFhZzhWGraczORwwqQM1WlgLciil9Ou7Ns1y/ezob3JmI3g=="
            };
        }
    }

    internal static class FingerDataEncode
    {
        internal static byte[] Key;
        private static string Ciphertext;
        private static readonly byte[] Iv;

        internal static byte[] Encode(byte[] source, int size)
        {
            if (source == null || source.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }

            if (Key == null || Key.Length <= 0 || Iv == null || Iv.Length <= 0)
            {
                throw new ArgumentNullException("Key");
            }

            Rijndael rj = new RijndaelManaged
            {
                BlockSize = 128,
                KeySize = 128,
                Key = Key,
                IV = Iv,
                Padding = PaddingMode.PKCS7,
                Mode = CipherMode.CBC
            };

            byte[] array1 = source;
            try
            {
                ICryptoTransform encryptor = rj.CreateEncryptor(rj.Key, rj.IV);
                MemoryStream memoryStream1 = new MemoryStream();
                try
                {
                label_13:
                    int sourceIndex = 0;
                    int num2 = 0;
                    byte[] buffer = source;
                    MemoryStream memoryStream2 = new MemoryStream();
                    while (true)
                    {
                        switch (num2)
                        {
                            case 0:
                                if (sourceIndex < source.Length)
                                {
                                    buffer = new byte[size * 32];
                                    num2 = 1;
                                    continue;
                                }

                                array1 = memoryStream1.ToArray();
                                goto label_59;
                            case 1:
                                Array.Copy((Array)source, sourceIndex, (Array)buffer, 0,
                                    source.Length - sourceIndex < size * 32 ? source.Length - sourceIndex : size * 32);
                                memoryStream2 = new MemoryStream();
                                try
                                {
                                    CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream2, encryptor,
                                        CryptoStreamMode.Write);
                                    try
                                    {
                                        cryptoStream.Write(buffer, 0, buffer.Length);
                                    }
                                    finally
                                    {
                                        cryptoStream.Dispose();
                                    }

                                    byte[] array2 = memoryStream2.ToArray();
                                    memoryStream1.Write(array2, 0, array2.Length);
                                }
                                finally
                                {
                                    memoryStream2.Dispose();
                                }

                                sourceIndex += size * 32;
                                num2 = 0;
                                continue;
                            default:
                                goto label_13;
                        }
                    }
                }
                finally
                {
                    memoryStream1.Dispose();
                }
            }
            finally
            {
                rj.Dispose();
            }

        label_59:
            return array1;
        }

        static FingerDataEncode()
        {
            Key = new byte[32]
            {
                87,
                175,
                102,
                232,
                214,
                9,
                116,
                42,
                125,
                176,
                96,
                17,
                22,
                0,
                13,
                23,
                138,
                188,
                165,
                34,
                74,
                217,
                6,
                195,
                20,
                96,
                131,
                124,
                90,
                181,
                187,
                31
            };
            Ciphertext = "cell$THJcell$THJ";
            Iv = Encoding.UTF8.GetBytes(Ciphertext);
        }
    }

    internal static class WSQCodecAPI
    {
        internal static string DSAKey = "<DSAKeyValue><P>18hCCdNJQWg0kw4B9iwpsHw2SCgLnuk4sbcZCY8vCasHaUh6+Nzxs7MH4PcfVqQTw3ToORp4/CvIWYC9SEmwvPPL2HJUb0pOo5b1ldnyD+LQaKD5thd6AhkBdyvrATdD6tXEin9bgLIyo98Gf0uLqG922dyF4MfvMM8cbwWPmGE=</P><Q>iNZ0oAOA451JhSC35hlKU//IAmc=</Q><G>sDP6WUGLehQ+EJOFnTtBE/ubvlR8aJnV43hxI35nojc3dC5xJsW4Y+ur3Xkg/GF9+ldZ8KwxhuwPvx0Wp0aRFJib1fpMS4fMqG3Jd9QDrTZx+XE7iTSO6TPfyCLzZBsrFC4jwJewpdYKoi8KGhbMdKxhRgNWL8wwfegk2hVTRZo=</G><Y>HzSbzGwv0y9r9pxtN4TzX4GFZRYGNhSkCiy4mF7uVzxHAW+FboCDR6gITfgMp0nZF1D2eyfuc6L+Qg4Eo2/GjOUYW0F1eijUBYBJ4Z8ru3Ps0Nih5sNcuM9KChMHEFt8JY7g/46qQd4Ust69jc+g+EcifcUf74244DCZCmDLOY8=</Y><J>AAAAAZOxFsQun6/hxZqOI6F8MFDvs7mQDdu35zD+0v3ebXQ+3+p0ItdFbtKQwV9GI1dSqgAtK43ElLNM2kR77teete8/93rhX8/Y7lqLV5UjuXB7Nza3gjMv73D7+zTa+UYWeJJSh6A+TesWpaIooA==</J><Seed>/yE+ihF8NohTaIbibqgHyg/R1ow=</Seed><PgenCounter>AVA=</PgenCounter><X>SLNUSLRaeVdS6tCY+8QZc8Fxi30=</X></DSAKeyValue>";
        private static float _flot_b = 0.75f;

        [DllImport("WSQCodec.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern bool CreateWSQ2(IntPtr A_0, int width, int height, int A_3, out IntPtr A_4, out int A_5, float A_6, string A_7);

        [DllImport("WSQCodec.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern bool FreeMem(IntPtr A_0);

        internal static byte[] GenWSQImage(byte[] imageBytes, int width, int height)
        {
            #region MyRegion
            switch (0)
            {
                default:
                    byte[] destination = (byte[])null;
                    IntPtr num1 = IntPtr.Zero;
                    IntPtr A_4 = IntPtr.Zero;
                    int A_5 = 0;
                    bool flag = false;
                    try
                    {
                        num1 = Marshal.AllocHGlobal(imageBytes.Length);
                        Marshal.Copy(imageBytes, 0, num1, imageBytes.Length);
                        flag = CreateWSQ2(num1, width, height, 16, out A_4, out A_5, _flot_b, DSAKey);
                    }
                    catch (Exception ex)
                    {
                        //
                    }

                    try
                    {
                        int num2 = 7;
                        while (true)
                        {
                            switch (num2)
                            {
                                case 0:
                                    num2 = 8;
                                    continue;
                                case 1:
                                    if (A_4 != IntPtr.Zero)
                                    {
                                        num2 = 4;
                                        continue;
                                    }
                                    goto case 0;
                                case 2:
                                    num2 = 5;
                                    continue;
                                case 3:
                                    Marshal.FreeHGlobal(num1);
                                    num2 = 6;
                                    continue;
                                case 4:
                                    destination = new byte[A_5];
                                    Marshal.Copy(A_4, destination, 0, A_5);
                                    FreeMem(A_4);
                                    num2 = 0;
                                    continue;
                                case 5:
                                    if (num1 != IntPtr.Zero)
                                    {
                                        num2 = 3;
                                        continue;
                                    }
                                    goto case 6;
                                case 6:
                                    num2 = 1;
                                    continue;
                                case 8:
                                    goto label_18;
                                default:
                                    if (flag)
                                    {
                                        num2 = 2;
                                        continue;
                                    }
                                    goto case 0;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //
                    }

                label_18:
                    return destination;
            }
            #endregion
        }

        internal static Bitmap ByteToBitmap(byte[] sourceBytes, int width, int height)
        {
            switch (0)
            {
                default:
                label_2:
                    byte[] source = new byte[sourceBytes.Length * 3];
                    int index = 0;
                    int num1 = 7;
                    BitmapData bitmapdata = new BitmapData();
                    int num2 = 0;
                    Bitmap bitmap = null;
                    while (true)
                    {
                        switch (num1)
                        {
                            case 1:
                                bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                                bitmapdata = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                                num2 = 0;
                                num1 = 4;
                                continue;
                            case 4:
                                if (num2 <= bitmap.Height - 1)
                                {
                                    IntPtr destination = new IntPtr(bitmapdata.Scan0.ToInt64() + (long)(bitmapdata.Stride * num2));
                                    Marshal.Copy(source, num2 * bitmap.Width * 3, destination, bitmap.Width * 3);
                                    ++num2;
                                    num1 = 4;
                                    continue;
                                }
                                goto label_13;
                            case 7:
                                if (index <= sourceBytes.Length - 1)
                                {
                                    source[index * 3] = sourceBytes[index];
                                    source[index * 3 + 1] = sourceBytes[index];
                                    source[index * 3 + 2] = sourceBytes[index];
                                    ++index;
                                    num1 = 7;
                                    continue;
                                }
                                num1 = 1;
                                continue;
                            default:
                                goto label_2;
                        }
                    }
                label_13:
                    bitmap.UnlockBits(bitmapdata);
                    bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                    return bitmap;
            }
        }

        internal static string BmpToBase64(Bitmap newImage)
        {
            string SigBase64 = string.Empty;
            using (var ms = new MemoryStream())
            {
                using (var bitmap = new Bitmap(newImage))
                {
                    bitmap.Save(ms, ImageFormat.Jpeg);
                    SigBase64 = "data:image/png;base64, " + Convert.ToBase64String(ms.GetBuffer());
                }
            }

            return SigBase64;
        }

        internal struct Eval_A
        {
            internal int PointX;
            internal int PointY;
            internal int c;
            internal int d;
            internal int e;
            internal int f;
            internal int g;
            internal int h;
            internal int i;
            internal int j;
            internal int k;
            internal int l;
            internal int m;
            internal int n;
            internal float o;
            internal int p;
            internal int q;
            internal int r;
            internal int s;
            internal int t;
        }
    }
}
