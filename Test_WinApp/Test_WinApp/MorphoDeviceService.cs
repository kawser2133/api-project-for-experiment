using B_Service;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using Test_WinApp.TCapture.Core.Device;

namespace Test_WinApp
{
    internal class MorphoDeviceService : IEventService   //internal class eval_o : IEventService
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
            this.thread = new Thread(new ThreadStart(this.CaptureFrame));
            this.captureData = new CaptureData();
            //this.imageBytes = new byte[MorphoDevice.GetCaptureImageSize(WSQCodec.DSAKey)];
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

        private int eval_a(IntPtr ftrHandler, MorphoDevice.DataBit bit, IntPtr A_2)
        {
            return 0;
        }

        private void CaptureFrame()
        {
            int num = 0;
            int num1 = 0;
            int num2 = 0;
            int num3;
            int num4 = 6;

            while (true)
            {
                switch (num4)
                {
                    case 3:
                        {
                            num3 = 0;
                            num = num3;
                            Array.Clear(this.imageBytes, 0, (int)this.imageBytes.Length);
                            num1 = 0;
                            num2 = 0;
                            num4 = 5;
                            continue;
                        }
                    case 4:
                        {
                            this.bool_h = false;
                            this.eval_r = false;
                            this.captureData = new CaptureData();
                            num4 = 6;
                            continue;
                        }
                    case 5:
                        {
                            if (MorphoDevice.GetImage(WSQCodec.DSAKey, new MorphoDevice.eval_a(this.eval_a), this.imageBytes, ref num1, ref num2, num))
                            {
                                this.bool_h = true;
                                this.eval_r = true;
                                int num5 = 0;
                                this.captureData.BmpImage = this.ImageByteToBmp(this.imageBytes, num1, num2);
                                this.captureData.ImageQuality = num5;
                                num4 = 6;
                                continue;
                            }

                            num4 = 4;
                            continue;
                        }
                    case 6:
                        {
                            if (!this.bool_o)
                            {
                                num4 = this.eval_r ? 8 : 3;
                                continue;
                            }
                            else
                            {
                                return;
                            }
                        }
                    case 8:
                        {
                            num3 = 1;
                            num = num3;
                            Array.Clear(this.imageBytes, 0, (int)this.imageBytes.Length);
                            num1 = 0;
                            num2 = 0;
                            num4 = 5;
                            continue;
                        }
                    default:
                        return;
                }
            }

            return;
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
}