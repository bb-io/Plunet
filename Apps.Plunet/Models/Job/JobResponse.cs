using Blackbird.Applications.Sdk.Common;

namespace Apps.Plunet.Models.Job
{
    public class JobResponse
    {
        [Display("Comment")]
        public string Comment { get; set; }

        [Display("Contact person ID")]
        public string ContactPersonId { get; set; }

        [Display("Date created")]
        public DateTime CreationDate { get; set; }

        [Display("Currency")]
        public string Currency { get; set; }

        [Display("Delivery date")]
        public DateTime DeliveryDate { get; set; }

        [Display("Delivery note")]
        public string DeliveryNote { get; set; }

        [Display("Description")]
        public string Description { get; set; }

        [Display("Payable ID")]
        public string PayableId { get; set; }

        [Display("Number of source files")]
        public int NumberOfSourceFiles { get; set; }

        [Display("Due date")]
        public DateTime? DueDate { get; set; }

        [Display("Project ID")]
        public string ProjectId { get; set; }

        [Display("Project type")]
        public string ProjectType { get; set; }
        
        [Display("Item ID")]
        public string ItemId { get; set; }

        [Display("Job ID")]
        public string JobId { get; set; }

        [Display("Job type")]
        public string JobType { get; set; }

        [Display("Resource ID")]
        public string ResourceId {  get; set; }

        [Display("Start date")]
        public DateTime? StartDate { get; set; }

        [Display("Status")]
        public string Status { get; set; }

        [Display("Total price")]
        public double TotalPrice { get; set; }

        [Display("Percentage completed")]
        public double PercentageComplated { get; set; }
    }
}
