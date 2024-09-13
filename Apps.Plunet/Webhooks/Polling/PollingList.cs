using Apps.Plunet.Invocables;
using Apps.Plunet.Models.Payable.Response;
using Apps.Plunet.Models;
using Apps.Plunet.Webhooks.Polling.Memories;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using Apps.Plunet.Actions;
namespace Apps.Plunet.Webhooks.Polling
{
    [PollingEventList]
    public class PollingList(InvocationContext invocationContext) : PlunetInvocable(invocationContext)
    {
        [PollingEvent("On payables created", "Triggered when payable is createdd")]
        public async Task<PollingEventResponse<PayableMemory, SearchResponse<PayableResponse>>> OnPayableCreated(
            PollingEventRequest<PayableMemory> request,
            [PollingEventParameter] PayableCreatedInput payableCreatedInput)
        {
            var payableActions = new PayableActions(InvocationContext, null!);
            var allPayables = await payableActions.SearchPayables(new Plunet.Models.Payable.Request.SearchPayablesRequest()
            {
                DateFrom = DateTime.MinValue,
                DateTo = DateTime.MinValue,
                TimeFrameRelation = "1",
                Status = payableCreatedInput.Status
            });
            var newPayablesState = allPayables.Items.Select(x => x.Id).ToList();
            if (request.Memory == null)
            {
                return new()
                {
                    FlyBird = false,
                    Memory = new PayableMemory() { PayablesIds = newPayablesState }
                };
            }
            var newItemsIds = newPayablesState.Except(request.Memory.PayablesIds).ToList();
            if (newItemsIds.Count == 0)
                return new()
                {
                    FlyBird = false,
                    Memory = new PayableMemory() { PayablesIds = newPayablesState }
                };
            var addedPayables = allPayables.Items.Where(x => newItemsIds.Contains(x.Id)).ToList();
            return new()
            {
                FlyBird = true,
                Memory = new PayableMemory() { PayablesIds = newPayablesState },
                Result = new SearchResponse<PayableResponse>() { Items = addedPayables, TotalCount = addedPayables.Count }
            };
        }
    }
}
