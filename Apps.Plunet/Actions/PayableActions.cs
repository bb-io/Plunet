using System.Text;
using Apps.Plunet.Constants;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models;
using Apps.Plunet.Models.Invoices;
using Apps.Plunet.Models.Invoices.Common;
using Apps.Plunet.Models.Payable.Request;
using Apps.Plunet.Models.Payable.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Plugins.Plunet.DataPayable30Service;
using Newtonsoft.Json;

namespace Apps.Plunet.Actions;

[ActionList("Payables")]
public class PayableActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : PlunetInvocable(invocationContext)
{
    [Action("Search payables", Description = "Get a list of payables based on custom criteria")]
    public async Task<SearchResponse<PayableResponse>> SearchPayables([ActionParameter] SearchPayablesRequest input, [ActionParameter][Display("Only return IDs", Description = "If set to true, only the IDs of the payables will be returned. This will make larger queries not time-out.")] bool? simple = false)
    {
        var response = await ExecuteWithRetryAcceptNull(() => PayableClient.searchAsync(Uuid,
            new()
            {
                exported = ParseId(input.Exported),
                isoCodeCurrency = input.Currency,
                resourceID = ParseId(input.ResourceId),
                payableStatus = ParseId(input.Status),
                timeFrame = new()
                {
                    dateRelation = ParseId(input.TimeFrameRelation),
                    dateTo = input.DateTo,
                    dateFrom = input.DateFrom,
                }
            }));

        if (response is null)
        {
            return new();
        }

        var results = new List<PayableResponse>();
        foreach (var id in response.Where(x => x.HasValue))
        {
            if (simple.HasValue && simple.Value)
            {
                results.Add(new PayableResponse { Id = id!.Value.ToString() });
            }
            else
            {
                var payableResponse = await GetPayable(new PayableRequest { PayableId = id!.Value.ToString() });
                results.Add(payableResponse);
            }
        }

        return new(results);
    }

    [Action("Find payable", Description = "Find a payable based on search criteria")]
    public async Task<FindResponse<PayableResponse>> FindPayable([ActionParameter] SearchPayablesRequest input)
    {
        var result = await SearchPayables(input);
        return new(result.Items.FirstOrDefault(), result.TotalCount);
    }

    [Action("Get payable", Description = "Get details of a specific payable")]
    public async Task<PayableResponse> GetPayable([ActionParameter] PayableRequest request)
    {
        var id = ParseId(request.PayableId);
        var accountStatement = await ExecuteWithRetryAcceptNull(() => PayableClient.getAccountStatementAsync(Uuid, id));
        var companyCode = await ExecuteWithRetry(() => PayableClient.getCompanyCodeAsync(Uuid, id));
        var creditorAccount = await ExecuteWithRetryAcceptNull(() => PayableClient.getCreditorAccountAsync(Uuid, id));
        var currency = await ExecuteWithRetryAcceptNull(() => PayableClient.getCurrencyAsync(Uuid, id));
        var expenseAccount = await ExecuteWithRetryAcceptNull(() => PayableClient.getExpenseAccountAsync(Uuid, id));
        var externalInvoice = await ExecuteWithRetryAcceptNull(() => PayableClient.getExternalInvoiceNumberAsync(Uuid, id));
        var invoiceDate = await ExecuteWithRetry(() => PayableClient.getInvoiceDateAsync(Uuid, id));
        var taxTypes = await ExecuteWithRetryAcceptNull(() => PayableClient.getInvoiceTaxTypesAsync(Uuid, id));
        var isExported = await ExecuteWithRetry(() => PayableClient.getIsExportedAsync(Uuid, id));
        var memo = await ExecuteWithRetryAcceptNull(() => PayableClient.getMemoAsync(Uuid, id));
        var paidDate = await ExecuteWithRetry(() => PayableClient.getPaidDateAsync(Uuid, id));
        var creatorResourceId = await ExecuteWithRetry(() => PayableClient.getPaymentCreatorResourceIDAsync(Uuid, id));
        var dueDate = await ExecuteWithRetry(() => PayableClient.getPaymentDueDateAsync(Uuid, id));
        var method = await ExecuteWithRetry(() => PayableClient.getPaymentMethodAsync(Uuid, id));
        var resource = await ExecuteWithRetry(() => PayableClient.getResourceIDAsync(Uuid, id));
        var status = await ExecuteWithRetry(() => PayableClient.getStatusAsync(Uuid, id));
        var total = await ExecuteWithRetry(() => PayableClient.getTotalNetAmountAsync(Uuid, id, 1)); // PROJECT CURRENCY
        var taxes = taxTypes is null ? 0 :await ExecuteWithRetry(() => PayableClient.getTotalTaxAmountAsync(Uuid, id, 1, taxTypes.data.First().taxType));
        var valueDate = await ExecuteWithRetry(() => PayableClient.getValueDateAsync(Uuid, id));

        var itemResult = await ExecuteWithRetry(() => PayableClient.getPaymentItemListAsync(Uuid, id));

        var items = itemResult.Select(x => new PayableItemResponse
        {
            Id = x.payableItemID.ToString(),
            Description = x.briefDescription,
            JobDate = x.jobDate,
            JobNo = x.jobNo,
            JobStatus = x.jobStatus.ToString(),
            ProjectType = x.projectType.ToString(),
            TotalPrice = x.totalprice
        });

        return new PayableResponse
        {
            Id = request.PayableId,
            AccountStatement = accountStatement,
            CompanyCode = companyCode.ToString(),
            CreditorAccount = creditorAccount.ToString(),
            Currency = currency,
            ExpenseAccount = expenseAccount,
            ExternalInvoiceNumber = externalInvoice,
            InvoiceDate = invoiceDate,
            IsExported = isExported,
            Memo = memo,
            PaidDate = paidDate,
            CreatorResourceId = creatorResourceId.ToString(),
            DueDate = dueDate,
            PaymentMethod = method.ToString(),
            ResourceId = resource.ToString(),
            Status = status.ToString(),
            TotalNetAmount = total,
            TotalTaxAmount = taxes,
            ValueDate = valueDate,
            Items = items,
        };
    }

    [Action("Export payable", Description = "Export a payable")]
    public async Task<ExportInvoiceResponse> ExportPayable([ActionParameter] PayableRequest request)
    {
        var payable = await GetPayable(request);

        var lineItems = new List<LineItem>();
        foreach (var item in payable.Items)
        {
            lineItems.Add(new LineItem
            {
                Description = item.Description,
                Quantity = 1,
                UnitPrice = (decimal)item
                    .TotalPrice, 
                Amount = (decimal)item.TotalPrice
            });
        }

        var customFields = new Dictionary<string, string>();
        if (!string.IsNullOrEmpty(payable.CompanyCode))
        {
            customFields.Add("CompanyCode", payable.CompanyCode);
        }

        if (!string.IsNullOrEmpty(payable.CreditorAccount))
        {
            customFields.Add("CreditorAccount", payable.CreditorAccount);
        }

        string resourceName;
        try
        {
            var resourceActions = new ResourceActions(invocationContext);
            var resource = await resourceActions.GetResource(payable.CreatorResourceId);
            resourceName = resource.FullName;
        }
        catch (Exception)
        {
            resourceName = $"Resource ID: {payable.CreatorResourceId}";
        }
        
        var payableObject = new InvoicesObject
        {
            Invoices =
            [
                new()
                {
                    CustomerName = resourceName,
                    InvoiceNumber = payable.ExternalInvoiceNumber,
                    InvoiceDate = payable.InvoiceDate,
                    Currency = payable.Currency,
                    Taxes = new List<Models.Invoices.Common.Tax>(),
                    Lines = lineItems,
                    Total = (decimal)payable.TotalNetAmount,
                    SubTotal = lineItems.Sum(x => x.Amount),
                    CustomFields = customFields
                }
            ]
        };

        var json = JsonConvert.SerializeObject(payableObject, Formatting.Indented);
        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        memoryStream.Position = 0;

        var fileReference =
            await fileManagementClient.UploadAsync(memoryStream, "application/json",
                $"{payable.ExternalInvoiceNumber}.json");

        return new ExportInvoiceResponse
        {
            File = fileReference
        };
    }


    [Action("Update payable", Description = "Edit the fields of a payable")]
    public async Task<PayableResponse> UpdatePayable([ActionParameter] UpdatePayableRequest input)
    {
        var id = ParseId(input.PayableId);

        if (input.Status != null)
            await ExecuteWithRetry(() => PayableClient.setStatusAsync(Uuid, ParseId(input.Status), id));

        if (input.AccountStatement != null)
            await ExecuteWithRetry(() => PayableClient.setAccountStatementAsync(Uuid, id, input.AccountStatement));

        if (input.CreditorAccount != null)
            await ExecuteWithRetry(() => PayableClient.setCreditorAccountAsync(Uuid, input.CreditorAccount, id));

        if (input.ExternalInvoiceNumber != null)
            await ExecuteWithRetry(() => PayableClient.setExternalInvoiceNumberAsync(Uuid, id, input.ExternalInvoiceNumber));

        if (input.IsExported.HasValue)
            await ExecuteWithRetry(() => PayableClient.setIsExportedAsync(Uuid, id, input.IsExported.Value));

        if (input.Memo != null)
            await ExecuteWithRetry(() => PayableClient.setMemoAsync(Uuid, id, input.Memo));

        if (input.InvoiceDate.HasValue)
            await ExecuteWithRetry(() => PayableClient.setInvoiceDateAsync(Uuid, input.InvoiceDate.Value, id));

        if (input.PaidDate.HasValue)
            await ExecuteWithRetry(() => PayableClient.setPaidDateAsync(Uuid, id, input.PaidDate.Value));

        if (input.DueDate.HasValue)
            await ExecuteWithRetry(() => PayableClient.setPaymentDueDateAsync(Uuid, id, input.DueDate.Value));

        if (input.ValueDate.HasValue)
            await ExecuteWithRetry(() => PayableClient.setValueDateAsync(Uuid, input.ValueDate.Value, id));

        return await GetPayable(input);
    }


    // Create payable item
    // Delete payable item
    // Update payable item

    // TODO: get/set invoice tax types
}