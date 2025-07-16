using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using Apps.Plunet.DataOutgoingInvoice30Service;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Plugins.Plunet.DataAdmin30Service;
using Blackbird.Plugins.Plunet.DataCustomer30Service;
using Blackbird.Plugins.Plunet.DataCustomerContact30Service;
using Blackbird.Plugins.Plunet.DataCustomFields30;
using Blackbird.Plugins.Plunet.DataDocument30Service;
using Blackbird.Plugins.Plunet.DataItem30Service;
using Blackbird.Plugins.Plunet.DataJob30Service;
using Blackbird.Plugins.Plunet.DataOrder30Service;
using Blackbird.Plugins.Plunet.DataPayable30Service;
using Blackbird.Plugins.Plunet.DataQuote30Service;
using Blackbird.Plugins.Plunet.DataRequest30Service;
using Blackbird.Plugins.Plunet.DataResource30Service;
using Blackbird.Plugins.Plunet.PlunetAPIService;
using DataCustomerAddress30Service;
using DataJobRound30Service;
using Blackbird.Plugins.Plunet.DataQualityManager30;
using DataResourceAddress30Service;

namespace Apps.Plunet.Api;

public static class Clients
{
    // 30 minutes timeout in milliseconds
    private const int ExtendedTimeoutMs = 1800000;

    public static PlunetAPIClient GetAuthClient(string url) => GetClient<PlunetAPIClient>(url, "PlunetAPI");

    public static DataCustomer30Client GetCustomerClient(string url) => GetClient<DataCustomer30Client>(url, "DataCustomer30");
    public static DataJobRound30Client GetJobRoundClient(string url) => GetClient<DataJobRound30Client>(url, "DataJobRound30");
    public static DataCustomerContact30Client GetContactClient(string url) => GetClient<DataCustomerContact30Client>(url, "DataCustomerContact30");
    public static DataAdmin30Client GetAdminClient(string url) => GetClient<DataAdmin30Client>(url, "DataAdmin30");
    public static DataDocument30Client GetDocumentClient(string url) => GetClient<DataDocument30Client>(url, "DataDocument30");
    public static DataItem30Client GetItemClient(string url) => GetClient<DataItem30Client>(url, "DataItem30");
    public static DataOrder30Client GetOrderClient(string url) => GetClient<DataOrder30Client>(url, "DataOrder30");
    public static DataPayable30Client GetPayableClient(string url) => GetClient<DataPayable30Client>(url, "DataPayable30");
    public static DataResource30Client GetResourceClient(string url) => GetClient<DataResource30Client>(url, "DataResource30");
    public static DataRequest30Client GetRequestClient(string url) => GetClient<DataRequest30Client>(url, "DataRequest30");
    public static DataQuote30Client GetQuoteClient(string url) => GetClient<DataQuote30Client>(url, "DataQuote30");
    public static DataJob30Client GetJobClient(string url) => GetClient<DataJob30Client>(url, "DataJob30");
    public static DataOutgoingInvoice30Client GetOutgoingInvoiceClient(string url) => GetClient<DataOutgoingInvoice30Client>(url, "DataOutgoingInvoice30");
    public static DataResourceAddress30Client GetResourceAddressClient(string url) => GetClient<DataResourceAddress30Client>(url, "DataResourceAddress30");
    public static DataCustomerAddress30Client GetCustomerAddressClient(string url) => GetClient<DataCustomerAddress30Client>(url, "DataCustomerAddress30");
    public static DataCustomFields30Client GetCustomFieldsClient(string url) => GetClient<DataCustomFields30Client>(url, "DataCustomFields30");
    public static DataQualityManager30Client GetQualityManagerClient(string url) => GetClient<DataQualityManager30Client>(url, "DataQualityManager30");

    public static TClient GetClient<TClient>(string url, string endpointSuffix) where TClient : class
    {
        if (url.StartsWith("https://"))
        {
            try
            {
                var endpointAddress = url.TrimEnd('/') + "/" + endpointSuffix;

                var endpointConfigurationType = typeof(TClient).GetNestedType("EndpointConfiguration");

                if (endpointConfigurationType != null)
                {
                    string enumName = endpointSuffix + "Port";

                    var endpointConfigValue = Enum.Parse(endpointConfigurationType, enumName);
                    var constructor = typeof(TClient).GetConstructor(new Type[] { endpointConfigurationType, typeof(string) });

                    if (constructor != null)
                    {
                        var client = (TClient)constructor.Invoke(new object[] { endpointConfigValue, endpointAddress });
                        // Set timeouts on the client

                        var clientType = client.GetType();
                        bool isClientBase = clientType.BaseType != null && clientType.BaseType.IsGenericType && clientType.BaseType.GetGenericTypeDefinition() == typeof(ClientBase<>);
                        if (isClientBase)
                        {
                            dynamic clientBase = client;
                            clientBase.Endpoint.Binding.SendTimeout = TimeSpan.FromMilliseconds(ExtendedTimeoutMs);
                            clientBase.Endpoint.Binding.ReceiveTimeout = TimeSpan.FromMilliseconds(ExtendedTimeoutMs);
                            clientBase.Endpoint.Binding.OpenTimeout = TimeSpan.FromMilliseconds(ExtendedTimeoutMs);
                            clientBase.Endpoint.Binding.CloseTimeout = TimeSpan.FromMilliseconds(ExtendedTimeoutMs);
                        }

                        return client;
                    }

                    throw new InvalidOperationException($"Cannot find constructor with (EndpointConfiguration, string) in {typeof(TClient).Name}");
                }

                throw new InvalidOperationException($"Cannot find EndpointConfiguration enum in {typeof(TClient).Name}");
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("SSL"))
                {
                    throw new PluginApplicationException($"Error establishing SSL connection: {ex.InnerException.Message}. Please try again");
                }
                throw;
            }
        }
        else
        {
            var endpointAddress = new EndpointAddress(url.TrimEnd('/') + "/" + endpointSuffix);

            var envelopeVersion = EnvelopeVersion.Soap12;
            var addressingVersion = AddressingVersion.None;

            var messageVersion = MessageVersion.CreateVersion(envelopeVersion, addressingVersion);

            var textBindingElement = new TextMessageEncodingBindingElement
            {
                MessageVersion = messageVersion,
                WriteEncoding = System.Text.Encoding.UTF8
            };

            var httpBindingElement = new HttpTransportBindingElement
            {
                // Common HTTP transport settings
                AllowCookies = true,
                MaxBufferSize = int.MaxValue,
                MaxReceivedMessageSize = int.MaxValue
            };

            var binding = new CustomBinding(textBindingElement, httpBindingElement);
            // Set timeouts on the main binding as well
            binding.SendTimeout = TimeSpan.FromMilliseconds(ExtendedTimeoutMs);
            binding.ReceiveTimeout = TimeSpan.FromMilliseconds(ExtendedTimeoutMs);
            binding.OpenTimeout = TimeSpan.FromMilliseconds(ExtendedTimeoutMs);
            binding.CloseTimeout = TimeSpan.FromMilliseconds(ExtendedTimeoutMs);



            var constructor = typeof(TClient).GetConstructor(new Type[] { typeof(Binding), typeof(EndpointAddress) });
            if (constructor != null)
            {
                var client = (TClient)constructor.Invoke(new object[] { binding, endpointAddress });

                if (client is ClientBase<object> clientBase && clientBase.InnerChannel is IClientChannel channel)
                {
                    channel.OperationTimeout = TimeSpan.FromMilliseconds(ExtendedTimeoutMs);

                    Console.WriteLine($"SendTimeout: {clientBase.Endpoint.Binding.SendTimeout}");
                    Console.WriteLine($"ReceiveTimeout: {clientBase.Endpoint.Binding.ReceiveTimeout}");
                    Console.WriteLine($"OperationTimeout: {clientBase.InnerChannel?.OperationTimeout}");
                }

                return client;
            }

            throw new InvalidOperationException($"Cannot find constructor with (Binding, EndpointAddress) in {typeof(TClient).Name}");
        }
    }
}
