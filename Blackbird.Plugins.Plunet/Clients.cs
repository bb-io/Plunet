using Blackbird.Plugins.Plunet.DataCustomer30Service;
using Blackbird.Plugins.Plunet.DataCustomerContact30Service;
using Blackbird.Plugins.Plunet.PlunetAPIService;
using Blackbird.Plugins.Plunet.DataAdmin30Service;
using Blackbird.Plugins.Plunet.DataDocument30Service;
using Blackbird.Plugins.Plunet.DataItem30Service;
using Blackbird.Plugins.Plunet.DataOrder30Service;
using Blackbird.Plugins.Plunet.DataResource30Service;
using Blackbird.Plugins.Plunet.DataRequest30Service;
using Blackbird.Plugins.Plunet.DataQuote30Service;
using Blackbird.Plugins.Plunet.DataJob30Service;
using Blackbird.Plugins.Plunet.DataPayable30Service;
using DataResourceAddress30Service;
using DataCustomerAddress30Service;

namespace Blackbird.Plugins.Plunet
{
    public static class Clients
    {
        public static PlunetAPIClient GetAuthClient(string url) => new(PlunetAPIClient.EndpointConfiguration.PlunetAPIPort, url.TrimEnd('/') + "/PlunetAPI");
        public static DataCustomer30Client GetCustomerClient(string url) => new(DataCustomer30Client.EndpointConfiguration.DataCustomer30Port, url.TrimEnd('/') + "/DataCustomer30");
        public static DataCustomerContact30Client GetContactClient(string url) => new(DataCustomerContact30Client.EndpointConfiguration.DataCustomerContact30Port, url.TrimEnd('/') + "/DataCustomerContact30");
        public static DataAdmin30Client GetAdminClient(string url) => new(DataAdmin30Client.EndpointConfiguration.DataAdmin30Port, url.TrimEnd('/') + "/DataAdmin30");
        public static DataDocument30Client GetDocumentClient(string url) => new(DataDocument30Client.EndpointConfiguration.DataDocument30Port, url.TrimEnd('/') + "/DataDocument30");
        public static DataItem30Client GetItemClient(string url) => new(DataItem30Client.EndpointConfiguration.DataItem30Port, url.TrimEnd('/') + "/DataItem30");
        public static DataOrder30Client GetOrderClient(string url) => new(DataOrder30Client.EndpointConfiguration.DataOrder30Port, url.TrimEnd('/') + "/DataOrder30");
        public static DataPayable30Client GetPayableClient(string url) => new(DataPayable30Client.EndpointConfiguration.DataPayable30Port, url.TrimEnd('/') + "/DataPayable30");
        public static DataResource30Client GetResourceClient(string url) => new(DataResource30Client.EndpointConfiguration.DataResource30Port, url.TrimEnd('/') + "/DataResource30");
        public static DataRequest30Client GetRequestClient(string url) => new(DataRequest30Client.EndpointConfiguration.DataRequest30Port, url.TrimEnd('/') + "/DataRequest30");
        public static DataQuote30Client GetQuoteClient(string url) => new(DataQuote30Client.EndpointConfiguration.DataQuote30Port, url.TrimEnd('/') + "/DataQuote30");
        public static DataJob30Client GetJobClient(string url) => new(DataJob30Client.EndpointConfiguration.DataJob30Port, url.TrimEnd('/') + "/DataJob30");
        public static DataResourceAddress30Client GetResourceAddressClient(string url) => new(DataResourceAddress30Client.EndpointConfiguration.DataResourceAddress30Port, url.TrimEnd('/') + "/DataResourceAddress30");
        public static DataCustomerAddress30Client GetCustomerAddressClient(string url) => new(DataCustomerAddress30Client.EndpointConfiguration.DataCustomerAddress30Port, url.TrimEnd('/') + "/DataCustomerAddress30");

    }
}
