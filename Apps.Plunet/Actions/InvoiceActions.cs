using Apps.Plunet.DataOutgoingInvoice30Service;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models.Customer;
using Apps.Plunet.Models.Invoices;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Plunet.Actions;

[ActionList]
public class InvoiceActions(InvocationContext invocationContext) : PlunetInvocable(invocationContext)
{
    [Action("Search invoices", Description = "Search invoices")]
    public async Task<SearchInvoicesResponse> SearchInvoices([ActionParameter] SearchInvoicesRequest request)
    {
        var filter = new SearchFilter_Invoice
        {
            languageCode = request.LanguageCode ?? "EN",
            timeFrame = new SelectionEntry_TimeFrame
            {
                dateFrom = request.DateFrom ?? new DateTime(2021, 01, 01, 12, 10, 10),
                dateTo = request.DateTo ?? DateTime.Now
            }
        };

        if (!string.IsNullOrEmpty(request.CustomerId))
        {
            filter.customerID = int.Parse(request.CustomerId);
        }

        var searchInvoices = await OutgoingInvoiceClient.searchAsync(Uuid, filter);

        var invoices = new List<GetInvoiceResponse>();
        foreach (var invoiceId  in searchInvoices.data)
        {
            var invoice = await GetInvoice(new InvoiceRequest { InvoiceId = invoiceId.ToString() ?? throw new InvalidOperationException("Invoice ID us null") });
            invoices.Add(invoice);
        }

        return new SearchInvoicesResponse { Invoices = invoices };
    }

    [Action("Get invoice", Description = "Get invoice by ID")]
    public async Task<GetInvoiceResponse> GetInvoice([ActionParameter] InvoiceRequest request)
    {
        var invoiceObject = await OutgoingInvoiceClient.getInvoiceObjectAsync(Uuid, int.Parse(request.InvoiceId));

        var customerActions = new CustomerActions(invocationContext);
        if(request.GetCustomer.HasValue && request.GetCustomer.Value)
        {
            var customer = new GetCustomerResponse();
            try
            {
                customer = await customerActions.GetCustomerById(new CustomerRequest
                    { CustomerId = invoiceObject.data.customerID.ToString() });
            }
            catch (Exception e)
            {
                InvocationContext.Logger?.LogError("Error while getting customer: " + e.Message, new object[]{e});
            }

            return new GetInvoiceResponse(invoiceObject, customer);
        }
        
        return new GetInvoiceResponse(invoiceObject, null);
    }
}