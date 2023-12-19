using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers
{
    public class CustomerStatusDataHandler : EnumDataHandler
    {
        protected override Dictionary<string, string> EnumValues => new()
        {
            ["1"] = "Active",
            ["6"] = "Aquistition address",
            ["5"] = "Blocked",
            ["3"] = "Contacted",
            ["8"] = "Deletion requested",
            ["4"] = "New",
            ["7"] = "New auto",
            ["2"] = "Not active",
        };
    }
}