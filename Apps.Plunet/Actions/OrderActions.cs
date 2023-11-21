using Apps.Plunet.Api;
using Apps.Plunet.Constants;
using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.Extensions;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models;
using Apps.Plunet.Models.Document;
using Apps.Plunet.Models.Item;
using Apps.Plunet.Models.Order;
using Apps.Plunet.Models.Quote.Request;
using Apps.Plunet.Models.Quote.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Parsers;
using Blackbird.Plugins.Plunet.DataItem30Service;
using Blackbird.Plugins.Plunet.DataOrder30Service;
using PriceLineResponse = Apps.Plunet.Models.Item.PriceLineResponse;

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
            languageCode = input.LanguageCode ?? string.Empty,
            sourceLanguage = input.SourceLanguage ?? string.Empty,
            targetLanguage = input.TargetLanguage ?? string.Empty,
            orderStatus = IntParser.Parse(input.OrderStatus, nameof(input.OrderStatus)) ?? -1,
            timeFrame = input.DateFrom is not null || input.DateTo is not null
                ? new()
                {
                    dateFrom = input.DateFrom ?? default,
                    dateTo = input.DateTo ?? default
                }
                : default,
            customerID = IntParser.Parse(input.CustomerId, nameof(input.CustomerId)) ?? -1,
            projectName = input.ProjectName ?? string.Empty,
            projectType = IntParser.Parse(input.ProjectType, nameof(input.ProjectType)) ?? -1,
            projectDescription = input.ProjectDescription ?? string.Empty,
        });

        if (searchResult.data is null)
            throw new(searchResult.statusMessage);

        var getOrderTasks = searchResult.data
            .Where(x => x.HasValue)
            .Select(x => GetOrder(new OrderRequest { OrderId = x.Value.ToString() }));

        return new ListOrderResponse { Orders = await Task.WhenAll(getOrderTasks) };
    }

    [Action("Get order", Description = "Get the Plunet order")]
    public async Task<OrderResponse> GetOrder([ActionParameter] OrderRequest request)
    {
        var intOrderId = IntParser.Parse(request.OrderId, nameof(request.OrderId))!.Value;
        var orderResult = await OrderClient.getOrderObjectAsync(Uuid, intOrderId);

        if (orderResult.statusMessage != ApiResponses.Ok)
            throw new(orderResult.statusMessage);

        var languageCombinations = await OrderClient.getLanguageCombinationAsync(Uuid, intOrderId);

        if (languageCombinations.statusMessage != ApiResponses.Ok)
            throw new(languageCombinations.statusMessage);

        var languages = await AdminClient.getAvailableLanguagesAsync(Uuid, Language);

        var orderLanguageCombinations = languageCombinations.data
            .Select(combination => new { source = combination.Split(" - ")[0], target = combination.Split(" - ")[1] })
            .Select(combination =>
                new LanguageCombination(languages.data.First(l => l.name == combination.source).folderName,
                    languages.data.First(l => l.name == combination.target).folderName));

        return new(orderResult.data, orderLanguageCombinations);
    }

    [Action("Create order", Description = "Create a new order in Plunet")]
    public async Task<OrderResponse> CreateOrder([ActionParameter] OrderTemplateRequest templateRequest, [ActionParameter] CreateOrderRequest request)
    {
        var orderIn = new OrderIN()
        {
            projectName = request.ProjectName,
            customerContactID = IntParser.Parse(request.ContactId, nameof(request.ContactId)) ?? default,
            customerID = IntParser.Parse(request.CustomerId, nameof(request.CustomerId)) ?? default,
            subject = request.Subject,
            projectManagerID = IntParser.Parse(request.ProjectManagerId, nameof(request.ProjectManagerId))!.Value,
            orderDate = DateTime.Now,
            deliveryDeadline = request.Deadline ?? default,
            currency = request.Currency,
            projectManagerMemo = request.ProjectManagerMemo,
            rate = request.Rate ?? default,
            referenceNumber = request.ReferenceNumber
        };

        var response = templateRequest.TemplateId == null ?
            await OrderClient.insert2Async(Uuid, orderIn) :
            await OrderClient.insert_byTemplateAsync(Uuid, orderIn, IntParser.Parse(templateRequest.TemplateId, nameof(templateRequest.TemplateId)) ?? default);

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        return await GetOrder(new OrderRequest { OrderId = response.data.ToString() });
    }

    [Action("Delete order", Description = "Delete a Plunet order")]
    public async Task DeleteOrder([ActionParameter] OrderRequest request)
    {
        var intOrderId = IntParser.Parse(request.OrderId, nameof(request.OrderId))!.Value;
        await OrderClient.deleteAsync(Uuid, intOrderId);
    }

    [Action("Update order", Description = "Update Plunet order")]
    public async Task<OrderResponse> UpdateOrder([ActionParameter] OrderRequest order, [ActionParameter] CreateOrderRequest request)
    {
        var intOrderId = IntParser.Parse(order.OrderId, nameof(order.OrderId))!.Value;

        var response = await OrderClient.updateAsync(Uuid, new OrderIN
        {
            orderID = intOrderId,
            projectName = request.ProjectName,
            customerContactID = IntParser.Parse(request.ContactId, nameof(request.ContactId)) ?? default,
            customerID = IntParser.Parse(request.CustomerId, nameof(request.CustomerId)) ?? default,
            subject = request.Subject,
            projectManagerID = IntParser.Parse(request.ProjectManagerId, nameof(request.ProjectManagerId))!.Value,
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
    //        projectID = IntParser.Parse(request.ProjectId, nameof(request.ProjectId)) ?? default,
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
    public async Task<AddLanguageCombinationResponse> AddLanguageCombinationToOrder(
        [ActionParameter] AddLanguageCombinationRequest request)
    {
        var intOrderId = IntParser.Parse(request.OrderId, nameof(request.OrderId))!.Value;
        var langCombination = await new LanguageCombination(request.SourceLanguageCode, request.TargetLanguageCode)
            .GetLangNamesByLangIso(Creds);

        var result = await OrderClient.addLanguageCombinationAsync(Uuid, langCombination.Source,
            langCombination.Target, intOrderId);

        return new()
        {
            LanguageCombinationId = result.data.ToString()
        };
    }

    //[Action("Set language combination to item", Description = "Set the language combination to an item")]
    //public async Task SetLanguageCombinationToItem([ActionParameter] SetLanguageCombinationRequest request)
    //{
    //    var intLangCombId = IntParser.Parse(request.LanguageCombinationId, nameof(request.LanguageCombinationId))!
    //        .Value;
    //    var intItemId = IntParser.Parse(request.ItemId, nameof(request.ItemId))!.Value;
    //    await ItemClient.setLanguageCombinationIDAsync(Uuid, intLangCombId, 3, intItemId);
    //}

    //[Action("Add priceline to item", Description = "Adds a new priceline")]
    //public async Task<PriceLineListResponse> AddPriceLinesToItem([ActionParameter] PriceLineRequest request)
    //{
    //    var intItemId = IntParser.Parse(request.ItemId, nameof(request.ItemId))!.Value;

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