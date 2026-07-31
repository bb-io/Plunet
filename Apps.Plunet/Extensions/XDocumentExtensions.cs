using System.Xml.Linq;

namespace Apps.Plunet.Extensions;

public static class XDocumentExtensions
{
    public static string? GetElementValue(this XDocument doc, string xmlIdTagName)
    {
        return doc.Elements()
            .Descendants()
            .FirstOrDefault(x => x.Name.LocalName.Equals(xmlIdTagName, StringComparison.OrdinalIgnoreCase))?.Value;
    }
}