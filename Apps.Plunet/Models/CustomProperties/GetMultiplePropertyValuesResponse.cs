using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.CustomProperties
{
    public class GetMultiplePropertyValuesResponse
    {
        public IEnumerable<string> Values { get; set; } = Array.Empty<string>();

        [Display("Property ID`s")]
        public IEnumerable<int> Ids { get; set; } = Array.Empty<int>();


        public IEnumerable<PropertyIdName> Pairs { get; set; } = Array.Empty<PropertyIdName>();
    }
    public class PropertyIdName
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
