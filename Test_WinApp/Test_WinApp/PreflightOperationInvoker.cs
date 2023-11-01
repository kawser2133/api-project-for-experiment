using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace Test_WinApp
{
    internal class PreflightOperationInvoker : IOperationInvoker
    {
        private string replyAction;

        private List<string> allowedHttpMethods;

        public bool IsSynchronous
        {
            get
            {
                return true;
            }
        }

        public PreflightOperationInvoker(string replyAction, List<string> allowedHttpMethods)
        {
            this.replyAction = replyAction;
            this.allowedHttpMethods = allowedHttpMethods;
        }

        public object[] AllocateInputs()
        {
            return new object[1];
        }

        private Message HandlePreflight(Message input)
        {
            HttpRequestMessageProperty httpRequest = (HttpRequestMessageProperty)input.Properties[HttpRequestMessageProperty.Name];
            string origin = httpRequest.Headers["Origin"];
            string requestMethod = httpRequest.Headers["Access-Control-Request-Method"];
            string requestHeaders = httpRequest.Headers["Access-Control-Request-Headers"];
            Message reply = Message.CreateMessage(MessageVersion.None, this.replyAction);
            HttpResponseMessageProperty httpResponse = new HttpResponseMessageProperty();
            reply.Properties.Add(HttpResponseMessageProperty.Name, httpResponse);
            httpResponse.SuppressEntityBody = true;
            httpResponse.StatusCode = HttpStatusCode.OK;
            if (origin != null)
            {
                httpResponse.Headers.Add("Access-Control-Allow-Origin", origin);
            }
            if ((requestMethod == null ? false : this.allowedHttpMethods.Contains(requestMethod)))
            {
                httpResponse.Headers.Add("Access-Control-Allow-Methods", string.Join(",", this.allowedHttpMethods));
            }
            if (requestHeaders != null)
            {
                httpResponse.Headers.Add("Access-Control-Allow-Headers", requestHeaders);
            }
            return reply;
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            Message input = (Message)inputs[0];
            outputs = null;
            return this.HandlePreflight(input);
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            throw new NotSupportedException("Only synchronous invocation");
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            outputs = null;
            throw new NotSupportedException("Only synchronous invocation");
        }
    }
}
