using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Payable.Response
{
    public class PayableItemResponse
    {
        [Display("ID")]
        public string Id { get; set; }

        [Display("Brief description")]
        public string Description { get; set; }

        [Display("Job date")]
        public DateTime JobDate { get; set; }

        [Display("Job no.")]
        public string JobNo { get; set; }

        [Display("Job status")]
        public string JobStatus { get; set; }

        [Display("Project type")]
        public string ProjectType { get; set; }

        [Display("Total price")]
        public double TotalPrice { get; set; }

        //[Display("Price lines")]
        //public IEnumerable<PriceLineResponse> PriceLines { get; set; }
    }
}
