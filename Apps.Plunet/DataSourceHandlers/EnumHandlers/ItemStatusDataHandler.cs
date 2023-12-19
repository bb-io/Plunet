using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers
{
    public class ItemStatusDataHandler : EnumDataHandler
    {
        protected override Dictionary<string, string> EnumValues => new()
        {
            { "12", "Accepted" },
            { "3", "Approved" },
            { "5", "Canceled" },
            { "7", "Delivered" },
            { "8", "In preparation" },
            { "1", "In progress" },
            { "4", "Invoiced" },
            { "6", "Auto" },
            { "9", "Paid" },
            { "11", "Pending" },
            { "13", "Rejected" },
            { "14", "Sum" },
            { "10", "Without invoice" }
        };
    }
}
