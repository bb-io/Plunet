using Apps.Plunet.Constants;
using Apps.Plunet.Models;
using Apps.Plunet.Models.Payable.Request;
using Apps.Plunet.Models.Payable.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Parsers;
using Blackbird.Plugins.Plunet.Api;
using Blackbird.Plugins.Plunet.Constants;
using Blackbird.Plugins.Plunet.DataPayable30Service;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Invocables;
using Blackbird.Plugins.Plunet.Models.Payable.Request;
using Blackbird.Plugins.Plunet.Models.Payable.Response;

namespace Blackbird.Plugins.Plunet.Actions;

[ActionList]
public class PayableActions : PlunetInvocable
{
    public PayableActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Action("Search payables", Description = "Get a list of payables based on custom criterias")]
    public async Task<SearchPayablesResponse> SearchPayables([ActionParameter] SearchPayablesRequest input)
    {
        var response = await PayableClient.searchAsync(Uuid, new()
        {
            exported = IntParser.Parse(input.Exported, nameof(input.Exported)) ?? 3,
            isoCodeCurrency = input.Currency,
            languageCode = Language,
            resourceID = IntParser.Parse(input.ResourceId, nameof(input.ResourceId)) ?? -1,
            payableStatus = IntParser.Parse(input.Status, nameof(input.Status)) ?? -1,
            timeFrame = new()
            {
                dateRelation = int.Parse(input.TimeFrameRelation),
                dateTo = input.DateTo,
                dateFrom = input.DateFrom,
            }
        });

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        if (response.data is null)
            return new()
            {
                Payables = Enumerable.Empty<PayableResponse>()
            };

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
        var id = IntParser.Parse(payableId, nameof(payableId))!.Value;
        var accountStatement = await GetString(PayableClient.getAccountStatementAsync(Uuid, id));
        var companyCode = await Try(GetId(PayableClient.getCompanyCodeAsync(Uuid, id)));
        var creditorAccount = await Try(GetString(PayableClient.getCreditorAccountAsync(Uuid, id)));
        var currency = await GetString(PayableClient.getCurrencyAsync(Uuid, id));
        var expenseAccount = await Try(GetString(PayableClient.getExpenseAccountAsync(Uuid, id)));
        var externalInvoice = await GetString(PayableClient.getExternalInvoiceNumberAsync(Uuid, id));
        var invoiceDate = await GetDate(PayableClient.getInvoiceDateAsync(Uuid, id));
        var isExported = await GetBool(PayableClient.getIsExportedAsync(Uuid, id));
        var memo = await GetString(PayableClient.getMemoAsync(Uuid, id));
        var paidDate = await GetDate(PayableClient.getPaidDateAsync(Uuid, id));
        var creatorResourceId = await GetId(PayableClient.getPaymentCreatorResourceIDAsync(Uuid, id));
        var dueDate = await GetDate(PayableClient.getPaymentDueDateAsync(Uuid, id));
        var method = await GetId(PayableClient.getPaymentMethodAsync(Uuid, id));
        var resource = await GetId(PayableClient.getResourceIDAsync(Uuid, id));
        var status = await GetId(PayableClient.getStatusAsync(Uuid, id));
        var total = await GetDouble(PayableClient.getTotalNetAmountAsync(Uuid, id, 2)); // PROJECT CURRENCY
        var valueDate = await GetDate(PayableClient.getValueDateAsync(Uuid, id));

        var itemRes = await PayableClient.getPaymentItemListAsync(Uuid, id);
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
        var id = IntParser.Parse(input.Id, nameof(input.Id))!.Value;

        if (input.Status != null)
            await PayableClient.setStatusAsync(Uuid, IntParser.Parse(input.Status, nameof(input.Status))!.Value, id);

        if (input.AccountStatement != null)
            await PayableClient.setAccountStatementAsync(Uuid, id, input.AccountStatement);

        if (input.CreditorAccount != null)
            await PayableClient.setCreditorAccountAsync(Uuid, input.CreditorAccount, id);

        if (input.ExternalInvoiceNumber != null)
            await PayableClient.setExternalInvoiceNumberAsync(Uuid, id, input.ExternalInvoiceNumber);

        if (input.IsExported.HasValue)
            await PayableClient.setIsExportedAsync(Uuid, id, input.IsExported.Value);

        if (input.Memo != null)
            await PayableClient.setMemoAsync(Uuid, id, input.Memo);

        if (input.InvoiceDate.HasValue)
            await PayableClient.setInvoiceDateAsync(Uuid, input.InvoiceDate.Value, id);

        if (input.PaidDate.HasValue)
            await PayableClient.setPaidDateAsync(Uuid, id, input.PaidDate.Value);

        if (input.DueDate.HasValue)
            await PayableClient.setPaymentDueDateAsync(Uuid, id, input.DueDate.Value);

        if (input.ValueDate.HasValue)
            await PayableClient.setValueDateAsync(Uuid, input.ValueDate.Value, id);

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

    private async Task<string> GetId(Task<IntegerResult> task)
    {
        var response = await task;
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



    // TODO: get/set invoice tax types



}