using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Document;

public class WordCountResponse
{
    [Display("Total word count")]
    public double TotalWordCount { get; set; }

    [Display("Documents word count")]
    public List<WordCountItem> DocumentWordCountItems { get; set; } = new();
}

public class WordCountItem
{
    [Display("Document name")]
    public string DocumentName { get; set; } = string.Empty;

    [Display("Word count")]
    public double WordCount { get; set; }
}