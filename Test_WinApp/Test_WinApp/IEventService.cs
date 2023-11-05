using System.Drawing;
using Test_WinApp.TCapture.Core.Device;

namespace Test_WinApp
{
    internal interface IEventService
    {
        /// <summary>
        /// Get device access
        /// </summary>
        /// <returns></returns>
        int DeviceAccess(); //int EventA();

        /// <summary>
        /// Device initialize
        /// </summary>
        void DeviceInit();
        void EventC();

        /// <summary>
        /// Finger detection
        /// </summary>
        /// <returns></returns>
        bool FingerDetected(); //bool EventD();

        bool EventE();

        bool EventF();
        
        /// <summary>
        /// Finger Release
        /// </summary>
        /// <returns></returns>
        bool FingerRelease(); //bool EventG();

        bool EventH(int a0, int a1);

        void EventI();

        /// <summary>
        /// Device release
        /// </summary>
        /// <returns></returns>
        bool DeviceRelease();

        Bitmap EventK();
        Bitmap EventL();

        CaptureData EventM();

        void EventN(Image img);
    }
}
