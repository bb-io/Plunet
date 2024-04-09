using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers
{
    public class JobStatusDataHandler : IStaticDataSourceHandler
    {
        public Dictionary<string, string> GetData()
        {
            return new()
            {
                { "3", "Approved" },
                { "7", "Assigned waiting" },
                { "4", "Canceled" },
                { "2", "Delivered" },
                { "0", "In preparation" },
                { "1", "In progress" },
                { "5", "Invoice accepted" },
                { "9", "Invoice checked" },
                { "10", "Invoice created" },
                { "6", "Payed" },
                { "8", "Requested" },
                { "12", "Transferred to order" },
                { "11", "Without invoice" }
            };
        }
    }
}
