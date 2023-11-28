using System.Drawing;

namespace FingerPrint.Core
{
    public static class BiometricServices
    {
        public static CapturedData FingerCapture()
        {
            var capturedData = new CapturedData();

            #region Morpho Device

            MorphoDeviceService morphoDevice = new MorphoDeviceService();
            morphoDevice.DeviceAccess();
            morphoDevice.DeviceInit();
            capturedData = morphoDevice.CaptureFrame();

            #endregion

            return capturedData;
        }
    }
}