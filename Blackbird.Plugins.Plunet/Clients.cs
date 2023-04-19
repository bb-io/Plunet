﻿using Blackbird.Plugins.Plunet.DataCustomer30Service;
using Blackbird.Plugins.Plunet.DataCustomerContact30Service;
using Blackbird.Plugins.Plunet.PlunetAPIService;
using Blackbird.Plugins.Plunet.DataAdmin30Service;
using Blackbird.Plugins.Plunet.DataDocument30Service;
using Blackbird.Plugins.Plunet.DataItem30Service;
using Blackbird.Plugins.Plunet.DataOrder30Service;

namespace Blackbird.Plugins.Plunet
{
    public static class Clients
    {
        public static PlunetAPIClient GetAuthClient(string url) => new PlunetAPIClient(PlunetAPIClient.EndpointConfiguration.PlunetAPIPort, url + "/PlunetAPI");
        public static DataCustomer30Client GetCustomerClient(string url) => new DataCustomer30Client(DataCustomer30Client.EndpointConfiguration.DataCustomer30Port, url + "/DataCustomer30");
        public static DataCustomerContact30Client GetContactClient(string url) => new DataCustomerContact30Client(DataCustomerContact30Client.EndpointConfiguration.DataCustomerContact30Port, url + "/DataCustomerContact30");
        public static DataAdmin30Client GetAdminClient(string url) => new DataAdmin30Client(DataAdmin30Client.EndpointConfiguration.DataAdmin30Port, url + "/DataAdmin30");
        public static DataDocument30Client GetDocumentClient(string url) => new DataDocument30Client(DataDocument30Client.EndpointConfiguration.DataDocument30Port, url + "/DataDocument30");
        public static DataItem30Client GetItemClient(string url) => new DataItem30Client(DataItem30Client.EndpointConfiguration.DataItem30Port, url + "/DataItem30");
        public static DataOrder30Client GetOrderClient(string url) => new DataOrder30Client(DataOrder30Client.EndpointConfiguration.DataOrder30Port, url + "/DataOrder30");
    }
}
