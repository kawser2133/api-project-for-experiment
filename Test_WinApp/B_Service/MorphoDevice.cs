using Morpho.MorphoAcquisition;
using Morpho.MorphoSmart;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace B_Service
{
    public static class MorphoDeviceTest
    {
        private static string[] connectedDevice;
        private static byte[] image_buffer;
        private static int quality;
        private static byte[] imageBytes; // eval_q

        public static string DeviceAccessMorpho()
        {
            MorphoDeviceService ss = new MorphoDeviceService();

            ss.DeviceAccess();
            ss.DeviceInit();
            var imageFile = ss.CaptureFrame();

            return imageFile;
        }

        public static string DeviceAccessMorphoSdk()
        {
            MorphoSmartDevice morphoSmart = new MorphoSmartDevice();
            connectedDevice = morphoSmart.GetConnectedDevices();

            //DeviceInit();
            string templateFormat = "ANSI378_0";

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

            var enrollData = morphoSmart.Enroll();
            string imageFile = Convert.ToBase64String(enrollData.ImageList[0].Image);


            var finger_image = WSQCodecAPI.BmpToBase64(WSQCodecAPI.ByteToBitmap(enrollData.ImageList[0].Image, enrollData.ImageList[0].Width, enrollData.ImageList[0].Height));
            var wsq_image = Convert.ToBase64String(EncoderHelper.RSAEncrypt(WSQCodecAPI.GenWSQImage(enrollData.ImageList[0].Image, enrollData.ImageList[0].Width, enrollData.ImageList[0].Height), EncoderHelper.GetToken(), 128));

            return imageFile;
        }
        private static int eval_a(IntPtr ftrHandler, MorphoDevice.DataBit bit, IntPtr A_2)
        {
            return 0;
        }

        private static void CaptureFrame()
        {
            int num = 0;
            int num1 = 0;
            int num2 = 0;
            int num3;
            int num4 = 6;

            if (MorphoDevice.GetImage(WSQCodec.DSAKey, new MorphoDevice.eval_a(eval_a), imageBytes, ref num1, ref num2, num))
            {
                int num5 = 0;
                var bmpImage = ImageByteToBmp(imageBytes, num1, num2);
                var imageQuality = num5;
            }

        }

        private static Bitmap ImageByteToBmp(byte[] imageBytes, int width, int height) //private Bitmap eval_a(byte[] imageBytes, int width, int height)
        {
            switch (0)
            {
                default:
                label_2:
                    byte[] source = new byte[imageBytes.Length * 3];
                    int index = 0;
                    int num1 = 7;
                    BitmapData bitmapdata = new BitmapData();
                    int num2 = 0;
                    Bitmap bitmap = null;
                    while (true)
                    {
                        switch (num1)
                        {
                            case 0:
                                bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                                bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                                num2 = 0;
                                num1 = 6;
                                continue;
                            case 1:
                            case 7:
                                num1 = 5;
                                continue;
                            case 2:
                            case 6:
                                num1 = 4;
                                continue;
                            case 3:
                                goto label_13;
                            case 4:
                                if (num2 <= bitmap.Height - 1)
                                {
                                    IntPtr destination = new IntPtr(bitmapdata.Scan0.ToInt64() + (long)(bitmapdata.Stride * num2));
                                    Marshal.Copy(source, num2 * bitmap.Width * 3, destination, bitmap.Width * 3);
                                    ++num2;
                                    num1 = 2;
                                    continue;
                                }
                                num1 = 3;
                                continue;
                            case 5:
                                if (index <= imageBytes.Length - 1)
                                {
                                    source[index * 3] = imageBytes[index];
                                    source[index * 3 + 1] = imageBytes[index];
                                    source[index * 3 + 2] = imageBytes[index];
                                    ++index;
                                    num1 = 1;
                                    continue;
                                }
                                num1 = 0;
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

        private static void ImageHandler(byte[] ImageBuffer, int NbLine, int NbColumn, int Resolution)
        {
            image_buffer = ImageBuffer;
        }

        private static void QualityHandler(byte Quality)
        {
            quality = Quality;
        }

    }

    public class CaptureData
    {
        internal Bitmap BmpImage; //eval_a
        internal int ImageQuality;
        public CaptureData()
        {
        }
    }

    internal class MorphoDeviceService     //internal class eval_o : IEventService
    {
        private bool bool_h; //eval_h
        private Thread thread; //eval_n
        private bool bool_o; // eval_o
        private CaptureData captureData; //eval_p
        private byte[] imageBytes; // eval_q
        private bool eval_r;
        private bool eval_s;

        internal MorphoDeviceService()
        {
            //this.thread = new Thread(new ThreadStart(this.eval_m_o));
            //this.thread = new Thread(new ThreadStart(this.CaptureFrame));
            this.captureData = new CaptureData();
            this.imageBytes = new byte[MorphoDevice.GetCaptureImageSize(WSQCodec.DSAKey)];
        }

        public int DeviceAccess()
        {
            return MorphoDevice.InitSdk(WSQCodec.DSAKey) ? 1 : 0;
        }

        public void DeviceInit() //public void EventB()
        {
            MorphoDevice.InitDevice(WSQCodec.DSAKey);
            this.eval_s = true;
        }

        public void EventC()
        {
            MorphoDevice.CancelCapture(WSQCodec.DSAKey);
            MorphoDevice.ReleaseResource(WSQCodec.DSAKey);
        }

        public bool FingerDetected() //public bool EventD()
        {
            return this.bool_h;
        }

        public bool EventE()
        {
            return true;
        }

        public bool EventF()
        {
            bool flag;
            try
            {
                this.thread.Start();
                flag = true;
            }
            catch (Exception ex)
            {
                flag = false;
            }

            return flag;
        }

        public bool FingerRelease()
        {
            (new Thread(new ThreadStart(this.StopCapturing))).Start();
            return true;
        }

        public bool EventH(int A_0, int A_1)
        {
            return true;
        }

        public void EventI()
        {
        }

        public bool DeviceRelease() //public bool EventJ()
        {
            (new Thread(new ThreadStart(this.ReleaseResource))).Start();
            return true;
        }

        private void ReleaseResource()
        {
            int num = 2;
            while (true)
            {
                switch (num)
                {
                    case 1:
                        goto label_7;
                    case 4:
                        this.bool_o = true;
                        MorphoDevice.CancelCapture(WSQCodec.DSAKey);
                        this.thread.Join();
                        break;
                    default:
                        if (this.eval_s && this.thread.IsAlive)
                        {
                            num = 4;
                            continue;
                        }
                        goto label_11;
                }

                MorphoDevice.ReleaseResource(WSQCodec.DSAKey);
                this.eval_s = false;
                num = 1;
            }

        label_7:
            return;
        label_11:;
        }

        public Bitmap EventK()
        {
            return this.captureData.BmpImage;
        }

        public Bitmap EventL()
        {
            return this.captureData.BmpImage;
        }

        public CaptureData EventM()
        {
            return this.captureData;
        }

        public void EventN(Image img)
        {
        }

        public int eval_a(IntPtr ftrHandler, MorphoDevice.DataBit bit, IntPtr A_2)
        {
            return 0;
        }

        public string CaptureFrame()
        {
            int num = 0;
            int num1 = 0;
            int num2 = 0;
            int num3;
            int num4 = 5;

            if (MorphoDevice.GetImage(WSQCodec.DSAKey, new MorphoDevice.eval_a(this.eval_a), this.imageBytes, ref num1, ref num2, num))
            {
                this.bool_h = true;
                this.eval_r = true;
                int num5 = 0;
                this.captureData.BmpImage = this.ImageByteToBmp(this.imageBytes, num1, num2);
                this.captureData.ImageQuality = num5;
                num4 = 6;
                MorphoDevice.ReleaseResource(WSQCodec.DSAKey);
            }
            string file = Convert.ToBase64String(this.imageBytes);

            return file;
        }

        private void eval_m_o()
        {
            switch (0)
            {
                default:
                    int num1 = 7;
                    while (true)
                    {
                        int width = 0;
                        int height = 0;
                        int A_5 = 0;
                        int num2;
                        switch (num1)
                        {
                            case 0:
                                num1 = this.eval_r ? 8 : 1;
                                continue;
                            case 1:
                                num1 = 3;
                                continue;
                            case 2:
                                goto default;
                            case 3:
                                num2 = 0;
                                break;
                            case 4:
                                this.bool_h = false;
                                this.eval_r = false;
                                this.captureData = new CaptureData();
                                num1 = 2;
                                continue;
                            case 5:
                                if (MorphoDevice.GetImage(WSQCodec.DSAKey, new MorphoDevice.eval_a(this.eval_a), this.imageBytes, ref width, ref height, A_5))
                                {
                                    this.bool_h = true;
                                    this.eval_r = true;
                                    int num3 = 0;
                                    this.captureData.BmpImage = this.ImageByteToBmp(this.imageBytes, width, height);
                                    this.captureData.ImageQuality = num3;
                                    num1 = 10;
                                    continue;
                                }
                                num1 = 4;
                                continue;
                            case 6:
                                num1 = !this.bool_o ? 0 : 9;
                                continue;
                            case 8:
                                num2 = 1;
                                break;
                            case 9:
                                goto label_15;
                            default:
                                num1 = 6;
                                continue;
                        }

                        A_5 = num2;
                        Array.Clear((Array)this.imageBytes, 0, this.imageBytes.Length);
                        width = 0;
                        height = 0;
                        num1 = 5;
                    }

                label_15:
                    break;
            }
        }

        private Bitmap ImageByteToBmp(byte[] imageBytes, int width, int height) //private Bitmap eval_a(byte[] imageBytes, int width, int height)
        {
            switch (0)
            {
                default:
                label_2:
                    byte[] source = new byte[imageBytes.Length * 3];
                    int index = 0;
                    int num1 = 7;
                    BitmapData bitmapdata = new BitmapData();
                    int num2 = 0;
                    Bitmap bitmap = null;
                    while (true)
                    {
                        switch (num1)
                        {
                            case 0:
                                bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                                bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                                num2 = 0;
                                num1 = 6;
                                continue;
                            case 1:
                            case 7:
                                num1 = 5;
                                continue;
                            case 2:
                            case 6:
                                num1 = 4;
                                continue;
                            case 3:
                                goto label_13;
                            case 4:
                                if (num2 <= bitmap.Height - 1)
                                {
                                    IntPtr destination = new IntPtr(bitmapdata.Scan0.ToInt64() + (long)(bitmapdata.Stride * num2));
                                    Marshal.Copy(source, num2 * bitmap.Width * 3, destination, bitmap.Width * 3);
                                    ++num2;
                                    num1 = 2;
                                    continue;
                                }
                                num1 = 3;
                                continue;
                            case 5:
                                if (index <= imageBytes.Length - 1)
                                {
                                    source[index * 3] = imageBytes[index];
                                    source[index * 3 + 1] = imageBytes[index];
                                    source[index * 3 + 2] = imageBytes[index];
                                    ++index;
                                    num1 = 1;
                                    continue;
                                }
                                num1 = 0;
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

        private void StopCapturing() //private void eval_n()
        {
            //-------------------------------------
            if (this.thread.IsAlive)
            {
                this.bool_o = true;
                MorphoDevice.CancelCapture(WSQCodec.DSAKey);
            }
            //-------------------------------------

            /*            int num = 1;
                        while (true)
                        {
                            switch (num)
                            {
                                case 0:
                                    this.bool_o = true;
                                    MorphoDevice.CancelCapture(WSQCodec.DSAKey);
                                    num = 2;
                                    continue;
                                case 2:
                                    goto label_7;
                                default:
                                    if (this.thread.IsAlive)
                                    {
                                        num = 0;
                                        continue;
                                    }
                                    goto label_8;
                            }
                        }

                        label_8:
                            return;

                        label_7: ;*/
        }
    }

    public class WSQCodec //eval_b
    {
        public static string DSAKey = "<DSAKeyValue><P>18hCCdNJQWg0kw4B9iwpsHw2SCgLnuk4sbcZCY8vCasHaUh6+Nzxs7MH4PcfVqQTw3ToORp4/CvIWYC9SEmwvPPL2HJUb0pOo5b1ldnyD+LQaKD5thd6AhkBdyvrATdD6tXEin9bgLIyo98Gf0uLqG922dyF4MfvMM8cbwWPmGE=</P><Q>iNZ0oAOA451JhSC35hlKU//IAmc=</Q><G>sDP6WUGLehQ+EJOFnTtBE/ubvlR8aJnV43hxI35nojc3dC5xJsW4Y+ur3Xkg/GF9+ldZ8KwxhuwPvx0Wp0aRFJib1fpMS4fMqG3Jd9QDrTZx+XE7iTSO6TPfyCLzZBsrFC4jwJewpdYKoi8KGhbMdKxhRgNWL8wwfegk2hVTRZo=</G><Y>HzSbzGwv0y9r9pxtN4TzX4GFZRYGNhSkCiy4mF7uVzxHAW+FboCDR6gITfgMp0nZF1D2eyfuc6L+Qg4Eo2/GjOUYW0F1eijUBYBJ4Z8ru3Ps0Nih5sNcuM9KChMHEFt8JY7g/46qQd4Ust69jc+g+EcifcUf74244DCZCmDLOY8=</Y><J>AAAAAZOxFsQun6/hxZqOI6F8MFDvs7mQDdu35zD+0v3ebXQ+3+p0ItdFbtKQwV9GI1dSqgAtK43ElLNM2kR77teete8/93rhX8/Y7lqLV5UjuXB7Nza3gjMv73D7+zTa+UYWeJJSh6A+TesWpaIooA==</J><Seed>/yE+ihF8NohTaIbibqgHyg/R1ow=</Seed><PgenCounter>AVA=</PgenCounter><X>SLNUSLRaeVdS6tCY+8QZc8Fxi30=</X></DSAKeyValue>";
    }

    internal static class MorphoDevice
    {
        [DllImport("morpho_core.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern bool InitSdk(string A_0);

        [DllImport("morpho_core.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern bool InitDevice(string A_0);

        [DllImport("morpho_core.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern bool GetImage(string key, MorphoDevice.eval_a A_1, byte[] imageBytes, ref int width, ref int height, int A_5);

        [DllImport("morpho_core.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern bool CancelCapture(string A_0);

        [DllImport("morpho_core.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern int GetPreviewImageSize(string A_0);

        [DllImport("morpho_core.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern void GetPreviewImageDimension(string A_0, ref int A_1, ref int A_2);

        [DllImport("morpho_core.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern int GetMorphoImageHeaderSize(string A_0);

        [DllImport("morpho_core.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern int GetCaptureImageSize(string A_0);

        [DllImport("morpho_core.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern bool ReleaseResource(string A_0);

        internal delegate int eval_a(IntPtr A_0, MorphoDevice.DataBit A_1, IntPtr A_2);

        internal enum DataBit
        {
            eval_a = 1,
            eval_b = 2,
            eval_c = 4,
            eval_d = 8,
            eval_e = 16, // 0x00000010
            eval_f = 32, // 0x00000020
            eval_g = 64, // 0x00000040
            eval_h = 128, // 0x00000080
            eval_i = 256, // 0x00000100
            eval_j = 512, // 0x00000200
            eval_k = 1024, // 0x00000400
            eval_l = 2048, // 0x00000800
            eval_m = 4096, // 0x00001000
            eval_n = 8192, // 0x00002000
            eval_o = 16384, // 0x00004000
            eval_p = 32768, // 0x00008000
            eval_q = 65536, // 0x00010000
            eval_r = 131072, // 0x00020000
            eval_s = 262144, // 0x00040000
            eval_t = 524288, // 0x00080000
            eval_u = 1048576, // 0x00100000
            eval_v = 2097152, // 0x00200000
            eval_w = 4194304, // 0x00400000
            eval_x = 8388608, // 0x00800000
            eval_y = 16777216, // 0x01000000
            eval_z = 33554432, // 0x02000000
            eval_aa = 67108864, // 0x04000000
            eval_ab = 134217728, // 0x08000000
            eval_ac = 268435456, // 0x10000000
            eval_ad = 536870912, // 0x20000000
            eval_ae = 1073741824, // 0x40000000
        }
    }
}