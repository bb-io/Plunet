using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers
{
    public class PreselectedTaxDataHandler : IStaticDataSourceHandler
    {
        public Dictionary<string, string> GetData()
        {
            return new()
            {
                ["3"] = "INFO",
                ["6"] = "INFO_SUM",
                ["12"] = "PRICE_BLOCK",
                ["4"] = "SUM",
                ["0"] = "TAX_1",
                ["5"] = "TAX_1_2",
                ["8"] = "TAX_1_2_3",
                ["14"] = "TAX_1_2_3_4",
                ["14"] = "TAX_1_2_3_4_5",
                ["10"] = "TAX_1_3",
                ["17"] = "TAX_1_4",
                ["1"] = "TAX_2",
                ["11"] = "TAX_2_3",
                ["16"] = "TAX_2_4_5",
                ["7"] = "TAX_3",
                ["9"] = "TAX_4",
                ["13"] = "TAX_5",
                ["2"] = "WITHOUT_TAX"
            };
        }
    }
}