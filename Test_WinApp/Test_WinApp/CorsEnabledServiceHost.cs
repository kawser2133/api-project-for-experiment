using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Web;

namespace Test_WinApp
{
    internal class CorsEnabledServiceHost : WebServiceHost
    {
        private Type contractType;

        public CorsEnabledServiceHost(Type serviceType, params Uri[] baseAddresses) : base(serviceType, baseAddresses)
        {
            this.contractType = this.GetContractType(serviceType);
        }

        private void AddPreflightOperations(ServiceEndpoint endpoint, List<OperationDescription> corsOperations)
        {
            string originalUriTemplate;
            Dictionary<string, PreflightOperationBehavior> uriTemplates = new Dictionary<string, PreflightOperationBehavior>(StringComparer.OrdinalIgnoreCase);
            foreach (OperationDescription operation in corsOperations)
            {
                if (operation.Behaviors.Find<WebGetAttribute>() == null)
                {
                    if (!operation.IsOneWay)
                    {
                        WebInvokeAttribute originalWia = operation.Behaviors.Find<WebInvokeAttribute>();
                        originalUriTemplate = ((originalWia == null ? true : originalWia.UriTemplate == null) ? operation.Name : this.NormalizeTemplate(originalWia.UriTemplate));
                        string originalMethod = (originalWia == null || originalWia.Method == null ? "POST" : originalWia.Method);
                        if (!uriTemplates.ContainsKey(originalUriTemplate))
                        {
                            ContractDescription contract = operation.DeclaringContract;
                            OperationDescription preflightOperation = new OperationDescription(string.Concat(operation.Name, "_preflight_"), contract);
                            MessageDescription inputMessage = new MessageDescription(string.Concat(operation.Messages[0].Action, "_preflight_"), MessageDirection.Input);
                            MessagePartDescriptionCollection parts = inputMessage.Body.Parts;
                            MessagePartDescription messagePartDescription = new MessagePartDescription("input", contract.Namespace)
                            {
                                Index = 0,
                                Type = typeof(Message)
                            };
                            parts.Add(messagePartDescription);
                            preflightOperation.Messages.Add(inputMessage);
                            MessageDescription outputMessage = new MessageDescription(string.Concat(operation.Messages[1].Action, "_preflight_"), MessageDirection.Output);
                            MessageBodyDescription body = outputMessage.Body;
                            messagePartDescription = new MessagePartDescription(string.Concat(preflightOperation.Name, "Return"), contract.Namespace)
                            {
                                Type = typeof(Message)
                            };
                            body.ReturnValue = messagePartDescription;
                            preflightOperation.Messages.Add(outputMessage);
                            WebInvokeAttribute wia = new WebInvokeAttribute()
                            {
                                UriTemplate = originalUriTemplate,
                                Method = "OPTIONS"
                            };
                            preflightOperation.Behaviors.Add(wia);
                            preflightOperation.Behaviors.Add(new DataContractSerializerOperationBehavior(preflightOperation));
                            PreflightOperationBehavior preflightOperationBehavior = new PreflightOperationBehavior(preflightOperation);
                            preflightOperationBehavior.AddAllowedMethod(originalMethod);
                            preflightOperation.Behaviors.Add(preflightOperationBehavior);
                            uriTemplates.Add(originalUriTemplate, preflightOperationBehavior);
                            contract.Operations.Add(preflightOperation);
                        }
                        else
                        {
                            uriTemplates[originalUriTemplate].AddAllowedMethod(originalMethod);
                        }
                    }
                }
            }
        }

        private Type GetContractType(Type serviceType)
        {
            Type type;
            if (!CorsEnabledServiceHost.HasServiceContract(serviceType))
            {
                Type[] possibleContractTypes = (
                    from i in (IEnumerable<Type>)serviceType.GetInterfaces()
                    where CorsEnabledServiceHost.HasServiceContract(i)
                    select i).ToArray<Type>();
                int length = (int)possibleContractTypes.Length;
                if (length == 0)
                {
                    throw new InvalidOperationException(string.Concat("Service type ", serviceType.FullName, " does not implement any interface decorated with the ServiceContractAttribute."));
                }
                if (length != 1)
                {
                    throw new InvalidOperationException(string.Concat("Service type ", serviceType.FullName, " implements multiple interfaces decorated with the ServiceContractAttribute, not supported by this factory."));
                }
                type = possibleContractTypes[0];
            }
            else
            {
                type = serviceType;
            }
            return type;
        }

        private static bool HasServiceContract(Type type)
        {
            return Attribute.IsDefined(type, typeof(ServiceContractAttribute), false);
        }

        private string NormalizeTemplate(string uriTemplate)
        {
            int queryIndex = uriTemplate.IndexOf('?');
            if (queryIndex >= 0)
            {
                uriTemplate = uriTemplate.Substring(0, queryIndex);
            }
            while (true)
            {
                int num = uriTemplate.IndexOf('{');
                int paramIndex = num;
                if (num < 0)
                {
                    break;
                }
                int endParamIndex = uriTemplate.IndexOf('}', paramIndex);
                if (endParamIndex >= 0)
                {
                    uriTemplate = string.Concat(uriTemplate.Substring(0, paramIndex), "*", uriTemplate.Substring(endParamIndex + 1));
                }
            }
            return uriTemplate;
        }

        protected override void OnOpening()
        {
            ServiceEndpoint endpoint = base.AddServiceEndpoint(this.contractType, new WebHttpBinding(), "");
            List<OperationDescription> corsEnabledOperations = (
                from o in endpoint.Contract.Operations
                where o.Behaviors.Find<CorsEnabledAttribute>() != null
                select o).ToList<OperationDescription>();
            this.AddPreflightOperations(endpoint, corsEnabledOperations);
            WebHttpBehavior webHttpBheavior = new WebHttpBehavior()
            {
                HelpEnabled = true
            };
            endpoint.Behaviors.Add(webHttpBheavior);
            endpoint.Behaviors.Add(new EnableCorsEndpointBehavior());
            base.OnOpening();
        }
    }
}
