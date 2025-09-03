using System.Text;
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
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Newtonsoft.Json;
using Tax = Apps.Plunet.Models.Invoices.Common.Tax;

namespace Apps.Plunet.Actions;

[ActionList("Invoices")]
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
        
        if (!string.IsNullOrEmpty(request.InvoiceStatus))
        {
            filter.invoiceStatus = int.Parse(request.InvoiceStatus);
        }

        var searchInvoices =await ExecuteWithRetry(() => OutgoingInvoiceClient.searchAsync(Uuid, filter));
        var invoices = new List<GetInvoiceResponse>();
        foreach (var invoiceId in searchInvoices.Where(x => x.HasValue).Take(request.Limit ?? SystemConsts.SearchLimit))
        {
            var invoice = await GetInvoice(new InvoiceRequest
                { InvoiceId = invoiceId.ToString() ?? throw new PluginMisconfigurationException("Invoice ID is null") });
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
            
            return new(null, 0);
        }
        
        return new(invoices.Items.FirstOrDefault(), invoices.TotalCount);
    }

    [Action("Get invoice", Description = "Get invoice by ID")]
    public async Task<GetInvoiceResponse> GetInvoice([ActionParameter] InvoiceRequest request)
    {
        var invoiceObject = await ExecuteWithRetry(() => OutgoingInvoiceClient.getInvoiceObjectAsync(Uuid, int.Parse(request.InvoiceId)));
        var invoiceItems = await ExecuteWithRetryAcceptNull(() => OutgoingInvoiceClient.getInvoiceItemListAsync(Uuid, int.Parse(request.InvoiceId)));

        var items = new List<InvoiceItemResponse>();
        if (invoiceItems != null)
        {
            foreach (var item in invoiceItems)
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
                    { CustomerId = invoiceObject.customerID.ToString() });
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
            var pricelines = await ExecuteWithRetry(() => OutgoingInvoiceClient.getPriceLine_ListAsync(Uuid, int.Parse(item.InvoiceItemId)));
            foreach (var price in pricelines)
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
                throw new PluginMisconfigurationException("Custom field keys and values count must be equal");

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
                    CustomerName = invoice.Customer?.Name1 ?? string.Empty,
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

        var json = JsonConvert.SerializeObject(invoiceObject);
        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        memoryStream.Position = 0;
        
        var fileReference =
            await fileManagementClient.UploadAsync(memoryStream, "application/json", $"{invoice.InvoiceNumber}.json");

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
            await ExecuteWithRetry(() => OutgoingInvoiceClient.setSubjectAsync(Uuid, request.Subject, int.Parse(request.InvoiceId)));
        }

        if (!string.IsNullOrEmpty(request.BriefDescription))
        {
            await ExecuteWithRetry(() => OutgoingInvoiceClient.setBriefDescriptionAsync(Uuid, request.BriefDescription, int.Parse(request.InvoiceId)));
        }

        if (request.InvoiceDate.HasValue)
        {
            await ExecuteWithRetry(() => OutgoingInvoiceClient.setInvoiceDateAsync(Uuid, request.InvoiceDate.Value, int.Parse(request.InvoiceId)));
        }

        if (request.PaidDate.HasValue)
        {
            await ExecuteWithRetry(() => OutgoingInvoiceClient.setPaidDateAsync(Uuid, int.Parse(request.InvoiceId), request.PaidDate.Value));
        }

        if (!string.IsNullOrEmpty(request.InvoiceStatus))
        {
            await ExecuteWithRetry(() => OutgoingInvoiceClient.setStatusAsync(Uuid,int.Parse(request.InvoiceStatus), int.Parse(request.InvoiceId)));
        }

        return await GetInvoice(request);
    }
}