using Apps.Plunet.Constants;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models;
using Apps.Plunet.Models.Order;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.DataOrder30Service;

namespace Apps.Plunet.Actions;

[ActionList]
public class OrderActions : PlunetInvocable
{
    public OrderActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Action("Search orders", Description = "Search for specific orders based on specific criteria")]
    public async Task<ListOrderResponse> SearchOrders([ActionParameter] SearchOrderInput input)
    {
        var searchResult = await OrderClient.searchAsync(Uuid, new()
        {
            sourceLanguage = input.SourceLanguage ?? string.Empty,
            targetLanguage = input.TargetLanguage ?? string.Empty,
            orderStatus = ParseId(input.OrderStatus),
            timeFrame = input.DateFrom is not null || input.DateTo is not null
                ? new()
                {
                    dateFrom = input.DateFrom ?? default,
                    dateTo = input.DateTo ?? default
                }
                : default,
            customerID = ParseId(input.CustomerId),
            projectName = input.ProjectName ?? string.Empty,
            projectType = ParseId(input.ProjectType),
            projectDescription = input.ProjectDescription ?? string.Empty,
        });

        if (searchResult.statusMessage != ApiResponses.Ok)
            throw new(searchResult.statusMessage);

        var getOrderTasks = searchResult.data is null ? Enumerable.Empty<Task<OrderResponse>>() : searchResult.data
            .Where(x => x.HasValue)
            .Select(x => GetOrder(new OrderRequest { OrderId = x!.Value.ToString() }));

        return new ListOrderResponse { Orders = await Task.WhenAll(getOrderTasks) };
    }

    [Action("Get order", Description = "Get the Plunet order")]
    public async Task<OrderResponse> GetOrder([ActionParameter] OrderRequest request)
    {
        var orderResult = await OrderClient.getOrderObjectAsync(Uuid, ParseId(request.OrderId));

        if (orderResult.statusMessage != ApiResponses.Ok)
            throw new(orderResult.statusMessage);

        var languageCombinations = await OrderClient.getLanguageCombinationAsync(Uuid, ParseId(request.OrderId));

        if (languageCombinations.statusMessage != ApiResponses.Ok)
            throw new(languageCombinations.statusMessage);

        var orderLanguageCombinations = await ParseLanguageCombinations(languageCombinations.data);

        var itemsResult = await ItemClient.getAllItemObjectsAsync(Uuid, ParseId(request.OrderId), 3);

        if (itemsResult.statusMessage != ApiResponses.Ok)
            throw new(itemsResult.statusMessage);

        var totalOrderPrice = itemsResult.data.Sum(x => x.totalPrice);

        return new(orderResult.data, orderLanguageCombinations)
        {
            TotalPrice = totalOrderPrice
        };
    }

    [Action("Create order", Description = "Create a new order in Plunet")]
    public async Task<OrderResponse> CreateOrder([ActionParameter] OrderTemplateRequest templateRequest, [ActionParameter] CreateOrderRequest request)
    {
        var orderIn = new OrderIN()
        {
            projectName = request.ProjectName,
            customerContactID = ParseId(request.ContactId),
            customerID = ParseId(request.CustomerId),
            subject = request.Subject,
            projectManagerID = ParseId(request.ProjectManagerId),
            orderDate = DateTime.Now,
            deliveryDeadline = request.Deadline ?? default,
            currency = request.Currency,
            projectManagerMemo = request.ProjectManagerMemo,
            rate = request.Rate ?? default,
            referenceNumber = request.ReferenceNumber
        };

        var response = templateRequest.TemplateId == null ?
            await OrderClient.insert2Async(Uuid, orderIn) :
            await OrderClient.insert_byTemplateAsync(Uuid, orderIn, ParseId(templateRequest.TemplateId));

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        return await GetOrder(new OrderRequest { OrderId = response.data.ToString() });
    }

    [Action("Delete order", Description = "Delete a Plunet order")]
    public async Task DeleteOrder([ActionParameter] OrderRequest request)
    {
        await OrderClient.deleteAsync(Uuid, ParseId(request.OrderId));
    }

    [Action("Update order", Description = "Update Plunet order")]
    public async Task<OrderResponse> UpdateOrder([ActionParameter] OrderRequest order, [ActionParameter] CreateOrderRequest request)
    {
        var response = await OrderClient.updateAsync(Uuid, new OrderIN
        {
            orderID = ParseId(order.OrderId),
            projectName = request.ProjectName,
            customerContactID = ParseId(request.ContactId),
            customerID = ParseId(request.CustomerId),
            subject = request.Subject,
            projectManagerID = ParseId(request.ProjectManagerId),
            orderDate = DateTime.Now,
            deliveryDeadline = request.Deadline ?? default,
            currency = request.Currency,
            projectManagerMemo = request.ProjectManagerMemo,
            rate = request.Rate ?? default,
            referenceNumber = request.ReferenceNumber
        }, false);

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        return await GetOrder(order);
    }


    //[Action("Add item to order", Description = "Add a new item to an order")]
    //public async Task<CreateItemResponse> AddItemToOrder([ActionParameter] CreateItemRequest request)
    //{
    //    var itemIdResult = await ItemClient.insert2Async(Uuid, new()
    //    {
    //        briefDescription = request.ItemName,
    //        projectID = ParseId(request.ProjectId),
    //        totalPrice = request.TotalPrice ?? default,
    //        projectType = request.ProjectType,
    //        deliveryDeadline = request.DeadlineDateTime ?? default,
    //        reference = request.Reference,
    //        status = request.Status ?? default,
    //    });
    //    return new()
    //    {
    //        ItemId = itemIdResult.data.ToString()
    //    };
    //}   

    [Action("Add language combination to order", Description = "Add a new language combination to an existing order")]
    public async Task<AddLanguageCombinationResponse> AddLanguageCombinationToOrder([ActionParameter] OrderRequest order, [ActionParameter] LanguageCombinationRequest request)
    {
        var sourceLanguage = await GetLanguageFromIsoOrFolderOrName(request.SourceLanguageCode);
        var targetLanguage = await GetLanguageFromIsoOrFolderOrName(request.TargetLanguageCode);

        var result = await OrderClient.addLanguageCombinationAsync(Uuid, sourceLanguage.name, targetLanguage.name, ParseId(order.OrderId));

        return new()
        {
            LanguageCombinationId = result.data.ToString()
        };
    }

    //[Action("Set language combination to item", Description = "Set the language combination to an item")]
    //public async Task SetLanguageCombinationToItem([ActionParameter] SetLanguageCombinationRequest request)
    //{
    //    var intLangCombId = ParseId(request.LanguageCombinationId)
    //    var intItemId = ParseId(request.ItemId);
    //    await ItemClient.setLanguageCombinationIDAsync(Uuid, intLangCombId, 3, intItemId);
    //}

    //[Action("Add priceline to item", Description = "Adds a new priceline")]
    //public async Task<PriceLineListResponse> AddPriceLinesToItem([ActionParameter] PriceLineRequest request)
    //{
    //    var intItemId = ParseId(request.ItemId);

    //    var priceUnitListResult = await ItemClient.getPriceUnit_ListAsync(Uuid, "en", "Translation");
    //    var priceUnits = priceUnitListResult.data?.Where(x =>
    //        x.description.Contains("Words Translation", StringComparison.OrdinalIgnoreCase)).ToArray();

    //    if (priceUnits == null || !priceUnits.Any())
    //        throw new("No price units found");

    //    foreach (var priceUnit in priceUnits)
    //    {
    //        var priceLine = new PriceLineIN
    //        {
    //            amount = request.Amount,
    //            priceUnitID = priceUnit.priceUnitID,
    //            taxType = 0,
    //            unit_price = request.UnitPrice
    //        };
    //        await ItemClient.insertPriceLineAsync(Uuid, intItemId, 3, priceLine,
    //            priceUnit.description.Contains("New", StringComparison.OrdinalIgnoreCase));
    //    }
    //    var priceListResult = await ItemClient.getPriceLine_ListAsync(Uuid, intItemId, 3);
    //    return new()
    //    {
    //        PriceLines = priceListResult.data.Select(x => new PriceLineResponse(x))
    //    };
    //}
    
}