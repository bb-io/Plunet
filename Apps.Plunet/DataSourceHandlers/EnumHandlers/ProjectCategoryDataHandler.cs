using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers;

public class ProjectCategoryDataHandler : EnumDataHandler
{
    protected override Dictionary<string, string> EnumValues { get; } = new()
    {
        { "No project category", "No project category" },
        { "Translation", "Translation" },
        { "Interpreting", "Interpreting" },
        { "Proofreading", "Proofreading" },
        { "Translation and Revision", "Translation and Revision" },
    };
}