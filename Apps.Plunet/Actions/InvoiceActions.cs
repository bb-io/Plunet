using Apps.Plunet.Constants;
using Apps.Plunet.DataOutgoingInvoice30Service;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models;
using Apps.Plunet.Models.Customer;
using Apps.Plunet.Models.CustomProperties;
using Apps.Plunet.Models.Invoices;
using Apps.Plunet.Models.Invoices.Common;
using Apps.Plunet.Models.Invoices.Items;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Tax = Apps.Plunet.Models.Invoices.Common.Tax;

namespace Apps.Plunet.Actions;

[ActionList]
public class InvoiceActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : PlunetInvocable(invocationContext)
{
    [Action("Search invoices", Description = "Search invoices")]
    public async Task<SearchResponse<GetInvoiceResponse>> SearchInvoices([ActionParameter] SearchInvoicesRequest request)
    {
        var filter = new SearchFilter_Invoice
        {
            languageCode = request.LanguageCode ?? "EN",
            timeFrame = new SelectionEntry_TimeFrame
            {
                dateFrom = request.DateFrom ?? new DateTime(2021, 01, 01, 12, 10, 10),
                dateTo = request.DateTo ?? DateTime.Now,
            },
        };

        if (!string.IsNullOrEmpty(request.CustomerId))
        {
            filter.customerID = int.Parse(request.CustomerId);
        }

        var searchInvoices =
            await ExecuteWithRetry<IntegerArrayResult>(
                async () => await OutgoingInvoiceClient.searchAsync(Uuid, filter));
        var invoices = new List<GetInvoiceResponse>();
        foreach (var invoiceId in searchInvoices.data.Where(x => x.HasValue).Take(request.Limit ?? SystemConsts.SearchLimit))
        {
            var invoice = await GetInvoice(new InvoiceRequest
                { InvoiceId = invoiceId.ToString() ?? throw new InvalidOperationException("Invoice ID is null") });
            invoices.Add(invoice);
        }
        
        return new(invoices);
    }

    [Action("Find invoice", Description = "Find invoice based on specific parameters")]
    public async Task<FindResponse<GetInvoiceResponse>> FindInvoice([ActionParameter] FindInvoiceRequest request)
    {
        var invoices = await SearchInvoices(request);

        if (!string.IsNullOrEmpty(request.InvoiceNumber))
        {
            invoices.Items = invoices.Items.Where(x => x.InvoiceNumber == request.InvoiceNumber).ToList();
        }

        if (!string.IsNullOrEmpty(request.Flag) && !string.IsNullOrEmpty(request.TextModuleValue))
        {
            var customPropertyActions = new CustomPropertyActions(invocationContext);

            foreach (var invoice in invoices.Items)
            {
                var textModule = await customPropertyActions.GetTextmodule(new TextModuleRequest
                {
                    Flag = request.Flag,
                    MainId = invoice.InvoiceId,
                    UsageArea = "7"
                });

                if (textModule.Value == request.TextModuleValue)
                {
                    return new(invoice, invoices.TotalCount);
                }
            }
        }
        
        return new(invoices.Items.FirstOrDefault(), invoices.TotalCount);
    }

    [Action("Get invoice", Description = "Get invoice by ID")]
    public async Task<GetInvoiceResponse> GetInvoice([ActionParameter] InvoiceRequest request)
    {
        var invoiceObject = await ExecuteWithRetry<InvoiceResult>(async () =>
            await OutgoingInvoiceClient.getInvoiceObjectAsync(Uuid, int.Parse(request.InvoiceId)));
        var invoiceItems = await ExecuteWithRetry<InvoiceItemResult>(async () =>
            await OutgoingInvoiceClient.getInvoiceItemListAsync(Uuid, int.Parse(request.InvoiceId)));

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

    [Action("Export invoice", Description = "Get invoice by ID as JSON")]
    public async Task<ExportInvoiceResponse> GetInvoiceAsJson([ActionParameter] InvoiceRequest request)
    {
        var invoice = await GetInvoice(new InvoiceRequest { InvoiceId = request.InvoiceId, GetCustomer = true });

        var lineItems = new List<LineItem>();
        foreach (var item in invoice.InvoiceItems)
        {
            var priceline = await ExecuteWithRetry<PriceLineListResult>(async () =>
                await OutgoingInvoiceClient.getPriceLine_ListAsync(Uuid, int.Parse(item.InvoiceItemId)));
            foreach (var price in priceline.data)
            {
                lineItems.Add(new LineItem
                {
                    Description = item.BriefDescription,
                    Quantity = (int)price.amount,
                    UnitPrice = (decimal)price.unit_price,
                    Amount = (decimal)price.amount * (decimal)price.unit_price
                });
            }
        }

        var customFields = new Dictionary<string, string>();
        if (request.CustomFieldKeys != null && request.CustomFieldValues != null)
        {
            if (request.CustomFieldKeys.Count() != request.CustomFieldValues.Count())
                throw new InvalidOperationException("Custom field keys and values count must be equal");

            for (var i = 0; i < request.CustomFieldKeys.Count(); i++)
            {
                customFields.Add(request.CustomFieldKeys.ElementAt(i), request.CustomFieldValues.ElementAt(i));
            }
        }

        var invoiceObject = new InvoicesObject
        {
            Invoices =
            [
                new()
                {
                    CustomerName = invoice.Customer?.Name ?? string.Empty,
                    InvoiceNumber = invoice.InvoiceNumber,
                    InvoiceDate = invoice.InvoiceDate,
                    Currency = invoice.CurrencyCode,
                    Taxes =
                    [
                        new Tax
                        {
                            Description = "Sales Tax",
                            Amount = (decimal)invoice.Tax
                        }
                    ],
                    Lines = lineItems,
                    Total = lineItems.Sum(x => x.Amount) + (decimal)invoice.Tax,
                    SubTotal = lineItems.Sum(x => x.Amount),
                    CustomFields = customFields
                }
            ]
        };

        var stream = invoiceObject.ToStream();
        var fileReference =
            await fileManagementClient.UploadAsync(stream, "application/json", $"{invoice.InvoiceNumber}.json");

        return new ExportInvoiceResponse
        {
            File = fileReference
        };
    }

    [Action("Update invoice", Description = "Update invoice")]
    public async Task<GetInvoiceResponse> UpdateInvoice([ActionParameter] UpdateInvoiceRequest request)
    {
        if (!string.IsNullOrEmpty(request.Subject))
        {
            var result =
                await ExecuteWithRetry<Result>(async () =>
                    await OutgoingInvoiceClient.setSubjectAsync(Uuid, request.Subject, int.Parse(request.InvoiceId)));

            if (result.statusMessage != "OK")
                throw new InvalidOperationException(
                    $"Error while updating invoice subject, message: {result.statusMessage}");
        }

        if (!string.IsNullOrEmpty(request.BriefDescription))
        {
            var result =
                await ExecuteWithRetry<Result>(async () => await OutgoingInvoiceClient.setBriefDescriptionAsync(Uuid,
                    request.BriefDescription,
                    int.Parse(request.InvoiceId)));

            if (result.statusMessage != "OK")
                throw new InvalidOperationException(
                    $"Error while updating invoice brief description, message: {result.statusMessage}");
        }

        if (request.InvoiceDate.HasValue)
        {
            var result =
                await ExecuteWithRetry<Result>(async () => await OutgoingInvoiceClient.setInvoiceDateAsync(Uuid,
                    request.InvoiceDate.Value,
                    int.Parse(request.InvoiceId)));

            if (result.statusMessage != "OK")
                throw new InvalidOperationException(
                    $"Error while updating invoice date, message: {result.statusMessage}");
        }

        if (request.PaidDate.HasValue)
        {
            var result = await ExecuteWithRetry<Result>(async () =>
                await OutgoingInvoiceClient.setPaidDateAsync(Uuid, int.Parse(request.InvoiceId),
                    request.PaidDate.Value));

            if (result.statusMessage != "OK")
                throw new InvalidOperationException(
                    $"Error while updating invoice paid date, message: {result.statusMessage}");
        }

        if (!string.IsNullOrEmpty(request.InvoiceStatus))
        {
            var result = await ExecuteWithRetry<Result>(async () => await OutgoingInvoiceClient.setStatusAsync(Uuid,
                int.Parse(request.InvoiceStatus),
                int.Parse(request.InvoiceId)));

            if (result.statusMessage != "OK")
                throw new InvalidOperationException(
                    $"Error while updating invoice status, message: {result.statusMessage}");
        }

        return await GetInvoice(request);
    }

    private async Task<T> ExecuteWithRetry<T>(Func<Task<Result>> func, int maxRetries = 10, int delay = 1000)
        where T : Result
    {
        var attempts = 0;
        while (true)
        {
            var result = await func();

            if (result.statusMessage == ApiResponses.Ok)
            {
                return (T)result;
            }

            if(result.statusMessage.Contains("session-UUID used is invalid"))
            {
                if (attempts < maxRetries)
                {
                    await Task.Delay(delay);
                    await RefreshAuthToken();
                    attempts++;
                    continue;
                }

                throw new($"No more retries left. Last error: {result.statusMessage}, Session UUID used is invalid.");
            }

            return (T)result;
        }
    }
}