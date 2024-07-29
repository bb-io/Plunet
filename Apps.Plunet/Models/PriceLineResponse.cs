using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models
{
    public class PricelineResponse
    {
        [Display("Priceline ID")]
        public string Id { get; set; }

        [Display("Amount")]
        public double Amount { get; set; }

        [Display("Amount per unit")]
        public double AmountPerUnit { get; set; }

        [Display("Memo")]
        public string Memo { get; set; }

        [Display("Sequence")]
        public int Sequence { get; set; }

        [Display("Tax type")]
        public string TaxType { get; set; }

        [Display("Time per unit")]
        public double TimePerUnit { get; set; }

        [Display("Unit price")]
        public double UnitPrice { get; set; }

        [Display("Price unit ID")]
        public string PriceUnitId { get; set; }

        [Display("Price unit descripion")]
        public string PriceUnitDescription { get; set; }

        [Display("Price unit service")]
        public string PriceUnitService { get; set; }

    }
}
