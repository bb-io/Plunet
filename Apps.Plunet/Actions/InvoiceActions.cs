using Apps.Plunet.DataOutgoingInvoice30Service;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models.Customer;
using Apps.Plunet.Models.Invoices;
using Apps.Plunet.Models.Invoices.Items;
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
        foreach (var invoiceId in searchInvoices.data)
        {
            var invoice = await GetInvoice(new InvoiceRequest
                { InvoiceId = invoiceId.ToString() ?? throw new InvalidOperationException("Invoice ID is null") });
            invoices.Add(invoice);
        }

        return new SearchInvoicesResponse { Invoices = invoices };
    }

    [Action("Get invoice", Description = "Get invoice by ID")]
    public async Task<GetInvoiceResponse> GetInvoice([ActionParameter] InvoiceRequest request)
    {
        var invoiceObject = await OutgoingInvoiceClient.getInvoiceObjectAsync(Uuid, int.Parse(request.InvoiceId));
        var invoiceItems = await OutgoingInvoiceClient.getInvoiceItemListAsync(Uuid, int.Parse(request.InvoiceId));

        var items = new List<InvoiceItemResponse>();
        if (invoiceItems.data != null)
        {
            foreach (var item in invoiceItems.data)
            {
                if (item != null)
                {
                    items.Add(new InvoiceItemResponse
                    {
                        InvoiceId = item.invoiceID.ToString(),
                        InvoiceItemId = item.invoiceItemID.ToString(),
                        ItemNumber = item.itemNumber,
                        LanguageCombinationId = item.languageCombinationID.ToString(),
                        OrderId = item.orderID.ToString(),
                        OrderItemId = item.orderItemId.ToString(),
                        TotalPrice = item.totalPrice,
                        BriefDescription = item.briefDescription,
                        Comment = item.comment
                    });
                }
            }
        }

        var invoiceResponse = new GetInvoiceResponse(invoiceObject, null)
        {
            InvoiceItems = items
        };

        if (request.GetCustomer.HasValue && request.GetCustomer.Value)
        {
            var customerActions = new CustomerActions(invocationContext);
            var customer = new GetCustomerResponse();

            try
            {
                customer = await customerActions.GetCustomerById(new CustomerRequest
                    { CustomerId = invoiceObject.data.customerID.ToString() });
            }
            catch (Exception e)
            {
                InvocationContext.Logger?.LogError("Error while getting customer: " + e.Message,
                    new object[] { e });
            }

            invoiceResponse.Customer = customer;
            return invoiceResponse;
        }

        return invoiceResponse;
    }

    [Action("Update invoice", Description = "Update invoice")]
    public async Task<GetInvoiceResponse> UpdateInvoice([ActionParameter] UpdateInvoiceRequest request)
    {
        if (!string.IsNullOrEmpty(request.Subject))
        {
            var result =
                await OutgoingInvoiceClient.setSubjectAsync(Uuid, request.Subject, int.Parse(request.InvoiceId));

            if (result.statusMessage != "OK")
                throw new InvalidOperationException("Error while updating invoice subject");
        }

        if (!string.IsNullOrEmpty(request.BriefDescription))
        {
            var result =
                await OutgoingInvoiceClient.setBriefDescriptionAsync(Uuid, request.BriefDescription,
                    int.Parse(request.InvoiceId));

            if (result.statusMessage != "OK")
                throw new InvalidOperationException("Error while updating invoice brief description");
        }

        if (request.InvoiceDate.HasValue)
        {
            var result =
                await OutgoingInvoiceClient.setInvoiceDateAsync(Uuid, request.InvoiceDate.Value,
                    int.Parse(request.InvoiceId));

            if (result.statusMessage != "OK")
                throw new InvalidOperationException("Error while updating invoice date");
        }

        if (request.PaidDate.HasValue)
        {
            var result =
                await OutgoingInvoiceClient.setPaidDateAsync(Uuid, int.Parse(request.InvoiceId),
                    request.PaidDate.Value);

            if (result.statusMessage != "OK")
                throw new InvalidOperationException("Error while updating invoice paid date");
        }

        if (!string.IsNullOrEmpty(request.InvoiceStatus))
        {
            var result = await OutgoingInvoiceClient.setStatusAsync(Uuid, int.Parse(request.InvoiceStatus),
                int.Parse(request.InvoiceId));

            if (result.statusMessage != "OK")
                throw new InvalidOperationException("Error while updating invoice status");
        }

        return await GetInvoice(request);
    }
}