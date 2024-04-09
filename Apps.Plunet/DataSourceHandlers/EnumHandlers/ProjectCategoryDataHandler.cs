using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers;

public class ProjectCategoryDataHandler : IStaticDataSourceHandler
{
    private Dictionary<string, string> EnumValues { get; } = new()
    {
        { "No project category", "No project category" },
        { "Translation", "Translation" },
        { "Interpreting", "Interpreting" },
        { "Proofreading", "Proofreading" },
        { "Translation and Revision", "Translation and Revision" },
    };

    public Dictionary<string, string> GetData()
    {
        return EnumValues;
    }
}