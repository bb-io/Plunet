using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Parsers;
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

    [Action("Search payables", Description = "Search payables by custom criterias")]
    public async Task<SearchPayablesResponse> SearchPayables([ActionParameter] SearchPayablesRequest input)
    {
        var uuid = Creds.GetAuthToken();

        await using var client = Clients.GetPayableClient(Creds.GetInstanceUrl());

        var response = await client.searchAsync(uuid, new()
        {
            exported = IntParser.Parse(input.Exported, nameof(input.Exported)) ?? 3,
            isoCodeCurrency = null,
            languageCode = input.LanguageCode ?? "EN",
            resourceID = IntParser.Parse(input.ResourceId, nameof(input.ResourceId)) ?? -1,
            payableStatus = IntParser.Parse(input.Status, nameof(input.Status)) ?? -1,
            timeFrame = new()
            {
                dateRelation = int.Parse(input.TimeFrameRelation),
                dateTo = input.DateTo ?? default,
                dateFrom = input.DateFrom ?? default
            }
        });

        await Creds.Logout();

        if (response.statusMessage != "OK")
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
    public async Task<PayableResponse> GetPayable(
        [ActionParameter] [Display("Payable ID")]
        string payableId)
    {
        var uuid = Creds.GetAuthToken();

        await using var client = Clients.GetPayableClient(Creds.GetInstanceUrl());

        var intId = IntParser.Parse(payableId, nameof(payableId))!.Value;
        var response = await client.getPaymentItemListAsync(uuid, intId);

        await Creds.Logout();

        if (response.statusMessage != "OK")
            throw new(response.statusMessage);

        return new(response.data.First());
    }
}