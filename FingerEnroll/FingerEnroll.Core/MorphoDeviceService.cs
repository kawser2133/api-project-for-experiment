using System;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace FingerEnroll.Core
{
    internal class MorphoDeviceService     //internal class eval_o : IEventService
    {
        private CaptureData captureData; //eval_p
        private byte[] imageBytes; // eval_q

        public MorphoDeviceService()
        {
            this.captureData = new CaptureData();
            this.imageBytes = new byte[MorphoDevice.GetCaptureImageSize(WSQHelper.DSAKey)];
        }

        public int DeviceAccess()
        {
            return MorphoDevice.InitSdk(WSQHelper.DSAKey) ? 1 : 0;
        }

        public void DeviceInit() //public void EventB()
        {
            MorphoDevice.InitDevice(WSQHelper.DSAKey);
        }

        public void EventC()
        {
            MorphoDevice.CancelCapture(WSQHelper.DSAKey);
            MorphoDevice.ReleaseResource(WSQHelper.DSAKey);
        }

        public int eval_a(IntPtr ftrHandler, MorphoDevice.DataBit bit, IntPtr A_2)
        {
            return 0;
        }

        public CaptureData CaptureFrame()
        {
            int num = 0;
            int num1 = 0;
            int num2 = 0;

            if (MorphoDevice.GetImage(WSQHelper.DSAKey, new MorphoDevice.eval_a(this.eval_a), this.imageBytes, ref num1, ref num2, num))
            {

                this.captureData.BmpImage = this.ImageByteToBmp(this.imageBytes, num1, num2);
                var bitmapToBytes = WSQHelper.BitmapToBytes(this.captureData.BmpImage);

                this.captureData.FingerprintImageScore = ImageQuelity(this.imageBytes, num1, num2);
                this.captureData.FingerprintImage = WSQHelper.BmpToBase64(WSQHelper.ByteToBitmap(this.imageBytes, num1, num2));
                this.captureData.FingerprintData = Convert.ToBase64String(TCapHelper.RSAEncrypt(WSQHelper.GenWSQImage(bitmapToBytes, num1, num2), TCapHelper.GetToken(), 128));

                if (this.captureData.BmpImage.Width % 4 == 0)
                    this.captureData.Template = Convert.ToBase64String(WSQHelper.GenerateIsoBytes(bitmapToBytes));
                else
                    this.captureData.Template = Convert.ToBase64String(WSQHelper.GenerateAnsiBytes(bitmapToBytes));

                MorphoDevice.ReleaseResource(WSQHelper.DSAKey);
            }

            return this.captureData;
        }

        public int ImageQuelity(byte[] imageBytes, int width, int height)
        {
            int img_score;
            IntPtr intPtr = Marshal.AllocHGlobal((int)imageBytes.Length);
            int num1 = 0;
            float single = 0f;
            Marshal.Copy(imageBytes, 0, intPtr, (int)imageBytes.Length);
            WSQHelper.CheckQuality(WSQHelper.DSAKey, intPtr, width, height, out num1, out single);
            Marshal.FreeHGlobal(intPtr);
            img_score = num1 * 20;
            img_score = 120 - img_score;

            return img_score;
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

    }

}