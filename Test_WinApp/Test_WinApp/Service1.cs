using BCAP;
using Morpho.MorphoSmart;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Script.Serialization;

namespace Test_WinApp
{

    public class Service1 : IService1
    {
        public Stream Capture(string fingerId)
        {

            MorphoSmartDevice morphoSmart = new MorphoSmartDevice();

            var connectedDevice = morphoSmart.GetConnectedDevices();
            var connected = connectedDevice?.Length ?? 0;


            FingerCaptureResult result = new FingerCaptureResult();

            result.MSG = "Device not found, please plug your device properly.";

            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
            return new MemoryStream(Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(result)));
        }


    }

    public class FingerCaptureResult
    {
        public string MSG { get; set; }
    }
}
