using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers;

public class CatTypeDataHandler : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
    {
        return new()
        {
            { "1", "Trados" },
            { "3", "Transit" },
            { "4", "memoQ" },
            { "5", "Across" },
            { "6", "PractiCount" },
            { "7", "Passolo" },
            { "8", "Logoport" },
            { "9", "Idiom" },
            { "10", "XTM" },
            { "11", "Fusion" },
            { "12", "DejaVu" },
            { "13", "Wordfast" },
            { "14", "Catalyst" },
            { "15", "Helium" },
            { "16", "Phrase" },
            { "17", "Falcon" }
        };
    }
}