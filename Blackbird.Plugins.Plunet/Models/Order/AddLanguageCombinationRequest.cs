namespace Blackbird.Plugins.Plunet.Models.Order;

public class AddLanguageCombinationRequest
{
    public int OrderId { get; set; }

    public string SourceLanguageCode { get; set; }

    public string TargetLanguageCode { get; set; }
}