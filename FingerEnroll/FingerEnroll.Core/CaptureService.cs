using System.Drawing;

namespace FingerEnroll.Core
{
    public static class CaptureService
    {
        public static CaptureData DeviceAccessMorpho()
        {
            MorphoDeviceService morphoDevice = new MorphoDeviceService();

            morphoDevice.DeviceAccess();
            morphoDevice.DeviceInit();
            var captureData = morphoDevice.CaptureFrame();

            return captureData;
        }
    }

    public class CaptureData
    {
        internal Bitmap BmpImage; //eval_a

        public CaptureData()
        {
        }

        public int FingerID { get; set; }
        public int FingerprintImageScore { get; set; }
        public string FingerprintImage { get; set; }
        public string FingerprintData { get; set; }
        public string Template { get; set; }
        public string Message { get; set; }
    }

}