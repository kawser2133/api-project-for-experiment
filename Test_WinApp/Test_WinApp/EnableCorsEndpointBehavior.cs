using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace Test_WinApp
{
    internal class EnableCorsEndpointBehavior : IEndpointBehavior
    {
        public EnableCorsEndpointBehavior()
        {
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            List<OperationDescription> corsEnabledOperations = (
                from o in endpoint.Contract.Operations
                where o.Behaviors.Find<CorsEnabledAttribute>() != null
                select o).ToList<OperationDescription>();
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new CorsEnabledMessageInspector(corsEnabledOperations));
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }
}
