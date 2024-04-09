using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers
{
    public class DocumentStatusDataHandler : IStaticDataSourceHandler
    {
        public Dictionary<string, string> GetData()
        {
            return new()
            {
                { "7", "Documents approved" },
                { "1", "Documents available" },
                { "2", "Documents downloaded" },
                { "3", "Documents in review" },
                { "6", "Documents re-delivered" },
                { "0", "No documents available" },
                { "5", "Post processing in progress" },
                { "4", "Post processting requested" }
            };
        }
    }
}
