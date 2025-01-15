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

[ActionList]
public class PayableActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : PlunetInvocable(invocationContext)
{
    [Action("Search payables", Description = "Get a list of payables based on custom criteria")]
    public async Task<SearchResponse<PayableResponse>> SearchPayables([ActionParameter] SearchPayablesRequest input, [ActionParameter][Display("Only IDs", Description = "If set to true, only the IDs of the payables will be returned. This will make larger queries not time-out.")] bool? simple = false)
    {
        var response = await ExecuteWithRetry<IntegerArrayResult>(async () => await PayableClient.searchAsync(Uuid,
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

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        if (response.data is null)
        {
            return new();
        }

        var results = new List<PayableResponse>();
        foreach (var id in response.data.Where(x => x.HasValue))
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
        var accountStatement =
            await GetString(ExecuteWithRetry<StringResult>(async () =>
                await PayableClient.getAccountStatementAsync(Uuid, id)));
        var companyCode =
            await Try(GetId(
                ExecuteWithRetry<IntegerResult>(async () => await PayableClient.getCompanyCodeAsync(Uuid, id))));
        var creditorAccount =
            await Try(GetString(ExecuteWithRetry<StringResult>(async () =>
                await PayableClient.getCreditorAccountAsync(Uuid, id))));
        var currency =
            await GetString(ExecuteWithRetry<StringResult>(async () => await PayableClient.getCurrencyAsync(Uuid, id)));
        var expenseAccount =
            await Try(GetString(ExecuteWithRetry<StringResult>(async () =>
                await PayableClient.getExpenseAccountAsync(Uuid, id))));
        var externalInvoice =
            await GetString(ExecuteWithRetry<StringResult>(async () =>
                await PayableClient.getExternalInvoiceNumberAsync(Uuid, id)));
        var invoiceDate =
            await GetDate(ExecuteWithRetry<DateResult>(async () => await PayableClient.getInvoiceDateAsync(Uuid, id)));
        var isExported =
            await GetBool(
                ExecuteWithRetry<BooleanResult>(async () => await PayableClient.getIsExportedAsync(Uuid, id)));
        var memo = await GetString(
            ExecuteWithRetry<StringResult>(async () => await PayableClient.getMemoAsync(Uuid, id)));
        var paidDate =
            await GetDate(ExecuteWithRetry<DateResult>(async () => await PayableClient.getPaidDateAsync(Uuid, id)));
        var creatorResourceId = await GetId(ExecuteWithRetry<IntegerResult>(async () =>
            await PayableClient.getPaymentCreatorResourceIDAsync(Uuid, id)));
        var dueDate =
            await GetDate(
                ExecuteWithRetry<DateResult>(async () => await PayableClient.getPaymentDueDateAsync(Uuid, id)));
        var method =
            await GetId(
                ExecuteWithRetry<IntegerResult>(async () => await PayableClient.getPaymentMethodAsync(Uuid, id)));
        var resource =
            await GetId(ExecuteWithRetry<IntegerResult>(async () => await PayableClient.getResourceIDAsync(Uuid, id)));
        var status =
            await GetId(ExecuteWithRetry<IntegerResult>(async () => await PayableClient.getStatusAsync(Uuid, id)));
        var total = await GetDouble(
            ExecuteWithRetry<DoubleResult>(async () =>
                await PayableClient.getTotalNetAmountAsync(Uuid, id, 2))); // PROJECT CURRENCY
        var valueDate =
            await GetDate(ExecuteWithRetry<DateResult>(async () => await PayableClient.getValueDateAsync(Uuid, id)));

        var itemRes =
            await ExecuteWithRetry<PayableItemResultList>(async () =>
                await PayableClient.getPaymentItemListAsync(Uuid, id));
        if (itemRes.statusMessage != ApiResponses.Ok)
            throw new(itemRes.statusMessage);

        var items = itemRes.data.Select(x => new PayableItemResponse
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
            CompanyCode = companyCode,
            CreditorAccount = creditorAccount,
            Currency = currency,
            ExpenseAccount = expenseAccount,
            ExternalInvoiceNumber = externalInvoice,
            InvoiceDate = invoiceDate,
            IsExported = isExported,
            Memo = memo,
            PaidDate = paidDate,
            CreatorResourceId = creatorResourceId,
            DueDate = dueDate,
            PaymentMethod = method,
            ResourceId = resource,
            Status = status,
            TotalNetAmount = total,
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
                    Taxes = new List<Apps.Plunet.Models.Invoices.Common.Tax>(),
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
            await ExecuteWithRetry<Result>(async () =>
                await PayableClient.setStatusAsync(Uuid, ParseId(input.Status), id));

        if (input.AccountStatement != null)
            await ExecuteWithRetry<Result>(async () =>
                await PayableClient.setAccountStatementAsync(Uuid, id, input.AccountStatement));

        if (input.CreditorAccount != null)
            await ExecuteWithRetry<Result>(async () =>
                await PayableClient.setCreditorAccountAsync(Uuid, input.CreditorAccount, id));

        if (input.ExternalInvoiceNumber != null)
            await ExecuteWithRetry<Result>(async () =>
                await PayableClient.setExternalInvoiceNumberAsync(Uuid, id, input.ExternalInvoiceNumber));

        if (input.IsExported.HasValue)
            await ExecuteWithRetry<Result>(async () =>
                await PayableClient.setIsExportedAsync(Uuid, id, input.IsExported.Value));

        if (input.Memo != null)
            await ExecuteWithRetry<Result>(async () => await PayableClient.setMemoAsync(Uuid, id, input.Memo));

        if (input.InvoiceDate.HasValue)
            await ExecuteWithRetry<Result>(async () =>
                await PayableClient.setInvoiceDateAsync(Uuid, input.InvoiceDate.Value, id));

        if (input.PaidDate.HasValue)
            await ExecuteWithRetry<Result>(async () =>
                await PayableClient.setPaidDateAsync(Uuid, id, input.PaidDate.Value));

        if (input.DueDate.HasValue)
            await ExecuteWithRetry<Result>(async () =>
                await PayableClient.setPaymentDueDateAsync(Uuid, id, input.DueDate.Value));

        if (input.ValueDate.HasValue)
            await ExecuteWithRetry<Result>(async () =>
                await PayableClient.setValueDateAsync(Uuid, input.ValueDate.Value, id));

        return await GetPayable(input);
    }


    // Create payable item
    // Delete payable item
    // Update payable item

    private async Task<T?> Try<T>(Task<T> task)
    {
        try
        {
            return await task;
        }
        catch (Exception ex)
        {
            return default(T);
        }
    }

    private async Task<string> GetString(Task<StringResult> task)
    {
        var response = await task;
        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);
        return response.data;
    }

    private async Task<double> GetDouble(Task<DoubleResult> task)
    {
        var response = await task;
        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);
        return response.data;
    }

    private async Task<string?> GetId(Task<IntegerResult> task)
    {
        var response = await task;
        if (response.data == 0) return null;
        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);
        return response.data.ToString();
    }

    private async Task<DateTime> GetDate(Task<DateResult> task)
    {
        var response = await task;
        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);
        return response.data;
    }

    private async Task<bool> GetBool(Task<BooleanResult> task)
    {
        var response = await task;
        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);
        return response.data;
    }

    private async Task<T> ExecuteWithRetry<T>(Func<Task<Result>> func, int maxRetries = 10, int delay = 1000)
        where T : Result
    {
        var attempts = 0;
        while (true)
        {
            Result? result;
            try
            {
                result = await func();
            }
            catch (Exception ex)
            {
                throw new PluginApplicationException($"Error while calling Plunet: {ex.Message}", ex);
            }

            if (result.statusMessage == ApiResponses.Ok)
            {
                return (T)result;
            }

            if (result.statusMessage.Contains("session-UUID used is invalid"))
            {
                if (attempts < maxRetries)
                {
                    await Task.Delay(delay);
                    await RefreshAuthToken();
                    attempts++;
                    continue;
                }

                throw new PluginApplicationException($"No more retries left. Last error: {result.statusMessage}, Session UUID used is invalid.");
            }

            return (T)result;
        }
    }

    // TODO: get/set invoice tax types
}