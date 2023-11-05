using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Script.Services;

namespace Test_WinApp
{
    
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "/capture/{fingerId}", Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        Stream Capture(string fingerId);
    }
}
