using B_Service;
using Morpho.MorphoSmart;
using System;
using System.Drawing;
using System.Windows.Forms;
using Test_WinApp;
using Test_WinApp.TCapture.Core.Device;

namespace BCAP
{
    internal class BiometricService //eval_ae
    {
        private int _dwFlags = 1;
        private int _minQuality; //eval_a
        private Form form; // eval_b
        private IEventService eventService; //eval_d;
        private DeviceVendor _vendor; //eval_e
        private int _ftrHandler = -1;
        private int _dwMask;
        private bool IsDeviceFound;

        private MorphoSmartDevice morphoSmart;

        internal BiometricService(int minQuality)
        {
            morphoSmart = new MorphoSmartDevice();
            this._minQuality = minQuality;
            //this.form = form;
            this.IsDeviceFound = false;
        }

        internal bool GetMorphoDevice_Main()
        {
            bool result = false;
            try
            {
                var connectedDevice = morphoSmart.GetConnectedDevices();
                var connected = connectedDevice?.Length ?? 0;

                if (connected > 0)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }


        internal string GetDeviceVendor() // eval_e()
        {
            return !this.IsDeviceFound ? string.Empty : this._vendor.ToString();
        }

        internal bool IsDeviceConnected() //internal bool eval_f()
        {
            try
            {
                if (GetMorphoDevice())
                {
                    this.IsDeviceFound = true;
                }
                else
                {
                    this.IsDeviceFound = false;
                }
            }
            catch (Exception ex)
            {
                this.IsDeviceFound = false;
            }
            return this.IsDeviceFound;
        }

        internal bool ScanSetOptions(int dwMask, int dwFlags) //internal bool eval_a(int A_0, int A_1)
        {
            this._dwMask = dwMask;
            this._dwFlags = dwFlags;

            return this.eventService.EventH(this._dwMask, this._dwFlags);
        }

        internal bool eval_b(int dwMask, int dwFlags)
        {
            this._dwMask = dwMask;
            this._dwFlags = dwFlags;
            this.eventService.EventH(this._dwMask, this._dwFlags);

            return this.eventService.EventF();
        }

        internal bool IsReadyToCapture() //internal bool eval_m()
        {
            bool flag = false;
            try
            {
            label_3:
                this.eventService.DeviceInit();
                int num = 2;
                while (true)
                {
                    switch (num)
                    {
                        case 0:
                        case 1:
                            num = 4;
                            continue;
                        case 2:
                            if (!this.ScanSetOptions(this._dwMask, this._dwFlags))
                            {
                                num = 3;
                                continue;
                            }

                            this.eventService.EventI();
                            flag = this.eventService.EventF();
                            num = 1;
                            continue;
                        case 3:
                            this.eventService.DeviceRelease();
                            num = 0;
                            continue;
                        case 4:
                            goto label_11;
                        default:
                            goto label_3;
                    }
                }
            }
            catch (Exception ex)
            {

            }

        label_11:
            return flag;
        }

        internal void ReleaseResource() //internal void eval_j()
        {
            try
            {
                this.eventService.FingerRelease();
                this.eventService.DeviceRelease();
            }
            catch (Exception ex)
            {

            }
        }

        internal Bitmap eval_k()
        {
            return this.eventService.EventL();
        }

        internal CaptureData eval_g()
        {
            return this.eventService.EventM();
        }

        internal Bitmap CaptureBmpFrame() //internal Bitmap eval_h()
        {
            return this.eventService.EventK();
        }

        internal bool IsFingerPresent => this.eventService.FingerDetected(); // eval_i()

        private bool GetMorphoDevice()
        {
            bool flag = false;
            this.eventService = new MorphoDeviceService();
            this._vendor = DeviceVendor.MORPHO;
            try
            {
            label_3:
                this._ftrHandler = this.eventService.DeviceAccess();
                int num = 1;
                while (true)
                {
                    switch (num)
                    {
                        case 0:
                            num = 3;
                            continue;
                        case 1:
                            if (this._ftrHandler > 0)
                            {
                                num = 2;
                                continue;
                            }
                            goto case 0;
                        case 2:
                            this.eventService.DeviceInit();
                            flag = true;
                            num = 0;
                            continue;
                        case 3:
                            goto label_10;
                        default:
                            goto label_3;
                    }
                }
            }
            catch (Exception ex)
            {

            }

        label_10:
            return flag;
        }
    }
}
