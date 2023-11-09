using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers;

public class ProjectStatusDataHandler : EnumDataHandler
{
    protected override Dictionary<string, string> EnumValues => new()
    {
        { "1", "Active" },
        { "3", "Archived" },
        { "6", "Completed" },
        { "2", "Completed archivable" },
        { "5", "In preparation" },
        { "4", "Quote moved to order" }
    };
}