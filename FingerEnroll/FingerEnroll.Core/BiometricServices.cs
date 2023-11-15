using System;
using System.Drawing;

namespace FingerEnroll.Core
{
    public static class BiometricServices
    {
        public static CaptureData DeviceAccessMorpho()
        {
            MorphoDeviceService morphoDevice = new MorphoDeviceService();

            morphoDevice.DeviceAccess();
            morphoDevice.DeviceInit();
            var captureData = morphoDevice.CaptureFrame();

            return captureData;
        }

        public static int GetAnsiMatchingScore(string template1, string template2)
        {
            int score = 0;
            if (!string.IsNullOrEmpty(template1) && !string.IsNullOrEmpty(template2))
            {
                byte[] tp1 = Convert.FromBase64String(template1);
                byte[] tp2 = Convert.FromBase64String(template2);

                bool res = WSQHelper.MatchingScore(tp1, tp2, ref score);
            }

            return score;
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