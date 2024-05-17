using Apps.Plunet.Constants;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models.Payable.Request;
using Apps.Plunet.Models.Payable.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.DataPayable30Service;

namespace Apps.Plunet.Actions;

[ActionList]
public class PayableActions(InvocationContext invocationContext) : PlunetInvocable(invocationContext)
{
    [Action("Search payables", Description = "Get a list of payables based on custom criterias")]
    public async Task<SearchPayablesResponse> SearchPayables([ActionParameter] SearchPayablesRequest input)
    {
        var response = await ExecuteWithRetry<IntegerArrayResult>(async () => await PayableClient.searchAsync(Uuid, new()
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
            return new()
            {
                Payables = Enumerable.Empty<PayableResponse>()
            };
        }

        var ids = response.data.Where(x => x.HasValue)
            .Select(x => GetPayable(x!.Value.ToString()))
            .ToArray();

        var result = await Task.WhenAll(ids);
        return new()
        {
            Payables = result
        };
    }

    [Action("Get payable", Description = "Get details of a specific payable")]
    public async Task<PayableResponse> GetPayable([ActionParameter] [Display("Payable ID")] string payableId)
    {
        var id = ParseId(payableId);
        var accountStatement = await GetString(ExecuteWithRetry<StringResult>(async () => await PayableClient.getAccountStatementAsync(Uuid, id)));
        var companyCode = await Try(GetId(ExecuteWithRetry<IntegerResult>(async () => await PayableClient.getCompanyCodeAsync(Uuid, id))));
        var creditorAccount = await Try(GetString(ExecuteWithRetry<StringResult>(async () => await PayableClient.getCreditorAccountAsync(Uuid, id))));
        var currency = await GetString(ExecuteWithRetry<StringResult>(async () => await PayableClient.getCurrencyAsync(Uuid, id)));
        var expenseAccount = await Try(GetString(ExecuteWithRetry<StringResult>(async () => await PayableClient.getExpenseAccountAsync(Uuid, id))));
        var externalInvoice = await GetString(ExecuteWithRetry<StringResult>(async () => await PayableClient.getExternalInvoiceNumberAsync(Uuid, id)));
        var invoiceDate = await GetDate(ExecuteWithRetry<DateResult>(async () => await PayableClient.getInvoiceDateAsync(Uuid, id)));
        var isExported = await GetBool(ExecuteWithRetry<BooleanResult>(async () => await PayableClient.getIsExportedAsync(Uuid, id)));
        var memo = await GetString(ExecuteWithRetry<StringResult>(async () => await PayableClient.getMemoAsync(Uuid, id)));
        var paidDate = await GetDate(ExecuteWithRetry<DateResult>(async () => await PayableClient.getPaidDateAsync(Uuid, id)));
        var creatorResourceId = await GetId(ExecuteWithRetry<IntegerResult>(async () => await PayableClient.getPaymentCreatorResourceIDAsync(Uuid, id)));
        var dueDate = await GetDate(ExecuteWithRetry<DateResult>(async () => await PayableClient.getPaymentDueDateAsync(Uuid, id)));
        var method = await GetId(ExecuteWithRetry<IntegerResult>(async () => await PayableClient.getPaymentMethodAsync(Uuid, id)));
        var resource = await GetId(ExecuteWithRetry<IntegerResult>(async () => await PayableClient.getResourceIDAsync(Uuid, id)));
        var status = await GetId(ExecuteWithRetry<IntegerResult>(async () => await PayableClient.getStatusAsync(Uuid, id)));
        var total = await GetDouble(ExecuteWithRetry<DoubleResult>(async () => await PayableClient.getTotalNetAmountAsync(Uuid, id, 2))); // PROJECT CURRENCY
        var valueDate = await GetDate(ExecuteWithRetry<DateResult>(async () => await PayableClient.getValueDateAsync(Uuid, id)));

        var itemRes = await ExecuteWithRetry<PayableItemResultList>(async () => await PayableClient.getPaymentItemListAsync(Uuid, id));
        if (itemRes.statusMessage != ApiResponses.Ok)
            throw new(itemRes.statusMessage);

        var items = itemRes.data.Select( x => new PayableItemResponse
        {
            Id = x.payableItemID.ToString(),
            Description = x.briefDescription,
            JobDate = x.jobDate,
            JobNo = x.jobNo,
            JobStatus = x.jobStatus.ToString(),
            ProjectType = x.projectType.ToString(),
            TotalPrice = x.totalprice,
        });

        return new PayableResponse
        {
            Id = payableId,
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

    [Action("Update payable", Description = "Edit the fields of a payable")]
    public async Task<PayableResponse> UpdatePayable([ActionParameter] UpdatePayableRequest input)
    {
        var id = ParseId(input.Id);

        if (input.Status != null)
            await ExecuteWithRetry<Result>(async () => await PayableClient.setStatusAsync(Uuid, ParseId(input.Status), id));

        if (input.AccountStatement != null)
            await ExecuteWithRetry<Result>(async () => await PayableClient.setAccountStatementAsync(Uuid, id, input.AccountStatement));

        if (input.CreditorAccount != null)
            await ExecuteWithRetry<Result>(async () => await PayableClient.setCreditorAccountAsync(Uuid, input.CreditorAccount, id));

        if (input.ExternalInvoiceNumber != null)
            await ExecuteWithRetry<Result>(async () => await PayableClient.setExternalInvoiceNumberAsync(Uuid, id, input.ExternalInvoiceNumber));

        if (input.IsExported.HasValue)
            await ExecuteWithRetry<Result>(async () => await PayableClient.setIsExportedAsync(Uuid, id, input.IsExported.Value));

        if (input.Memo != null)
            await ExecuteWithRetry<Result>(async () => await PayableClient.setMemoAsync(Uuid, id, input.Memo));

        if (input.InvoiceDate.HasValue)
            await ExecuteWithRetry<Result>(async () => await PayableClient.setInvoiceDateAsync(Uuid, input.InvoiceDate.Value, id));

        if (input.PaidDate.HasValue)
            await ExecuteWithRetry<Result>(async () => await PayableClient.setPaidDateAsync(Uuid, id, input.PaidDate.Value));

        if (input.DueDate.HasValue)
            await ExecuteWithRetry<Result>(async () => await PayableClient.setPaymentDueDateAsync(Uuid, id, input.DueDate.Value));

        if (input.ValueDate.HasValue)
            await ExecuteWithRetry<Result>(async () => await PayableClient.setValueDateAsync(Uuid, input.ValueDate.Value, id));

        return await GetPayable(input.Id);
    }


    // Create payable item
    // Delete payable item
    // Update payable item

    private async Task<T?> Try<T>(Task<T> task)
    {
        try
        {
            return await task;
        } catch(Exception ex)
        {
            return default (T);
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
            var result = await func();

            if (result.statusMessage == ApiResponses.Ok)
            {
                return (T)result;
            }

            if (result.statusMessage.Contains("session-UUID used is invalid") && attempts < maxRetries)
            {
                await Task.Delay(delay);
                await RefreshAuthToken();
                attempts++;
                continue;
            }

            return (T)result;
        }
    }
    
    // TODO: get/set invoice tax types
}