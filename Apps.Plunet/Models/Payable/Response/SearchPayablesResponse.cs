namespace Apps.Plunet.Models.Payable.Response;

public class SearchPayablesResponse
{
    public IEnumerable<PayableResponse> Payables { get; set; }
}