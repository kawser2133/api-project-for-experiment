using System;

namespace FingerPrint.Core
{
    internal class MorphoDeviceService     //internal class eval_o : IEventService
    {
        private CapturedData capturedData; //eval_p
        private byte[] imageBytes; // eval_q

        public MorphoDeviceService()
        {
            this.capturedData = new CapturedData();
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

        public CapturedData CaptureFrame()
        {
            int num = 0;
            int num1 = 0;
            int num2 = 0;

            if (MorphoDevice.GetImage(WSQHelper.DSAKey, new MorphoDevice.eval_a(this.eval_a), this.imageBytes, ref num1, ref num2, num))
            {

                this.capturedData.BmpImage = WSQHelper.ByteToBitmap(this.imageBytes, num1, num2);
                var bitmapToBytes = WSQHelper.BitmapToBytes(this.capturedData.BmpImage);

                this.capturedData.FingerprintImageScore = WSQHelper.ImageQuelity(this.imageBytes, num1, num2);
                this.capturedData.FingerprintImage = WSQHelper.BmpToBase64(WSQHelper.ByteToBitmap(this.imageBytes, num1, num2));
                this.capturedData.FingerprintData = Convert.ToBase64String(TCapHelper.RSAEncrypt(WSQHelper.GenWSQImage(this.imageBytes, num1, num2), TCapHelper.GetToken(), 128));

                MorphoDevice.ReleaseResource(WSQHelper.DSAKey);
            }

            return this.capturedData;
        }

    }

}