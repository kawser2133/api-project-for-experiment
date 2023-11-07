using B_Service;
using System.IO;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Script.Serialization;

namespace Test_WinApp
{

    public class Service1 : IService1
    {
        public Stream Capture(string fingerId)
        {
            try
            {
                var morphoDll = MorphoDeviceTest.DeviceAccessMorpho();
                var morphoSDK = MorphoDeviceTest.DeviceAccessMorphoSdk();

                FingerCaptureResult result = new FingerCaptureResult();
                result.imageFileDll = morphoDll;
                result.imageFileSDK = morphoSDK;

                WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
                return new MemoryStream(Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(result)));
            }
            catch (System.Exception ex)
            {
                WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
                return new MemoryStream(Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(ex.Message)));
            }

        }


    }

    public class FingerCaptureResult
    {
        public string imageFileDll { get; set; }
        public string imageFileSDK { get; set; }
        public string MSG { get; set; }
    }
}
