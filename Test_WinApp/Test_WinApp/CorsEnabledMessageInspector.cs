using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace Test_WinApp
{
    internal class CorsEnabledMessageInspector : IDispatchMessageInspector
    {
        private List<string> corsEnabledOperationNames;

        public CorsEnabledMessageInspector(List<OperationDescription> corsEnabledOperations)
        {
            this.corsEnabledOperationNames = (
                from o in corsEnabledOperations
                select o.Name).ToList<string>();
        }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            object operationName;
            object obj;
            HttpRequestMessageProperty httpProp = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
            request.Properties.TryGetValue("HttpOperationName", out operationName);
            if ((httpProp == null || operationName == null ? false : this.corsEnabledOperationNames.Contains((string)operationName)))
            {
                string origin = httpProp.Headers["Origin"];
                if (origin != null)
                {
                    obj = origin;
                    return obj;
                }
            }
            obj = null;
            return obj;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            string origin = correlationState as string;
            if (origin != null)
            {
                HttpResponseMessageProperty httpProp = null;
                if (!reply.Properties.ContainsKey(HttpResponseMessageProperty.Name))
                {
                    httpProp = new HttpResponseMessageProperty();
                    reply.Properties.Add(HttpResponseMessageProperty.Name, httpProp);
                }
                else
                {
                    httpProp = (HttpResponseMessageProperty)reply.Properties[HttpResponseMessageProperty.Name];
                }
                httpProp.Headers.Add("Access-Control-Allow-Origin", origin);
            }
        }
    }
}
