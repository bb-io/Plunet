using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers;

public class FormOfAddressDataHandler : IStaticDataSourceHandler
{
    private static Dictionary<string, string> EnumValues => new()
        {
            { "1", "Sir" },
            { "2", "Madam" },
            { "3", "Company" }
        };

    public Dictionary<string, string> GetData()
    {
        return EnumValues;
    }
}

