using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers;

public class FolderTypeDataHandler : EnumDataHandler
{
    protected override Dictionary<string, string> EnumValues => new()
    {
        { "1", "Request reference" },
        { "2", "Request source" },
        { "3", "Quote reference" },
        { "4", "Quote source" },
        { "5", "Order reference" },
        { "6", "Order source" },
        { "7", "Quote item cat" },
        { "8", "Order item cat" },
        { "9", "Quote out" },
        { "10", "Order out" },
        { "11", "Quote final" },
        { "12", "Order final" },
        { "13", "Receivable" },
        { "14", "Quote item source" },
        { "15", "Quote item reference" },
        { "16", "Order item source" },
        { "17", "Order item reference" },
        { "18", "Resource" },
        { "19", "Customer" },
        { "20", "Quote job in" },
        { "21", "Quote job out" },
        { "22", "Order job in" },
        { "23", "Order job out" },
        { "24", "Quote prm" },
        { "25", "Order prm" },
        { "26", "Payable" }
    };
}