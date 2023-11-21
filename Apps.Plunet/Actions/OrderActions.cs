using Apps.Plunet.Api;
using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.Extensions;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models;
using Apps.Plunet.Models.Document;
using Apps.Plunet.Models.Item;
using Apps.Plunet.Models.Order;
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

    [Action("Get order", Description = "Get the Plunet order")]
    public async Task<OrderResponse> GetOrder([ActionParameter] OrderRequest request)
    {
        var intOrderId = IntParser.Parse(request.OrderId, nameof(request.OrderId))!.Value;
        var orderResult = await OrderClient.getOrderObjectAsync(Uuid, intOrderId);

        if (orderResult.data is null)
            throw new(orderResult.statusMessage);

        return new(orderResult.data);
    }

    [Action("Create order", Description = "Create a new order in Plunet")]
    public async Task<CreateOrderResponse> CreateOrder([ActionParameter] CreateOrderRequest request)
    {
        var orderIdResult = await OrderClient.insert2Async(Uuid, new()
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
        });

        return new()
        {
            OrderId = orderIdResult.data.ToString()
        };
    }

    [Action("Delete order", Description = "Delete a Plunet order")]
    public async Task DeleteOrder([ActionParameter] OrderRequest request)
    {
        var intOrderId = IntParser.Parse(request.OrderId, nameof(request.OrderId))!.Value;
        await OrderClient.deleteAsync(Uuid, intOrderId);
    }

    [Action("Update order", Description = "Update Plunet order")]
    public async Task UpdateOrder([ActionParameter] UpdateOrderRequest request)
    {
        var intOrderId = IntParser.Parse(request.OrderId, nameof(request.OrderId))!.Value;

        await OrderClient.updateAsync(Uuid, new OrderIN
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
    }


    [Action("Add item to order", Description = "Add a new item to an order")]
    public async Task<CreateItemResponse> AddItemToOrder([ActionParameter] CreateItemRequest request)
    {
        var itemIdResult = await ItemClient.insert2Async(Uuid, new()
        {
            briefDescription = request.ItemName,
            projectID = IntParser.Parse(request.ProjectId, nameof(request.ProjectId)) ?? default,
            totalPrice = request.TotalPrice ?? default,
            projectType = request.ProjectType,
            deliveryDeadline = request.DeadlineDateTime ?? default,
            reference = request.Reference,
            status = request.Status ?? default,
        });
        return new()
        {
            ItemId = itemIdResult.data.ToString()
        };
    }

    [Action("List templates", Description = "List all templates")]
    public async Task<ListTemplatesResponse> ListTemplates()
    {
        var response = await OrderClient.getTemplateListAsync(Uuid);
        var templates = response.data.Select(x => new TemplateResponse(x)).ToArray();
        return new(templates);
    }

    [Action("Create order based on template", Description = "Create a new order based on a template")]
    public async Task<CreateOrderResponse> CreateOrderBasedOnTemplate([ActionParameter] CreateOrderRequest request,
        [ActionParameter] OrderTemplateRequest templateRequest)
    {
        var order = new OrderIN
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

        var orderIdResult = await OrderClient.insert_byTemplateAsync(Uuid, order, int.Parse(templateRequest.TemplateId));
        return new()
        {
            OrderId = orderIdResult.data.ToString()
        };
    }

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

    [Action("Set language combination to item", Description = "Set the language combination to an item")]
    public async Task SetLanguageCombinationToItem([ActionParameter] SetLanguageCombinationRequest request)
    {
        var intLangCombId = IntParser.Parse(request.LanguageCombinationId, nameof(request.LanguageCombinationId))!
            .Value;
        var intItemId = IntParser.Parse(request.ItemId, nameof(request.ItemId))!.Value;
        await ItemClient.setLanguageCombinationIDAsync(Uuid, intLangCombId, 3, intItemId);
    }

    [Action("Add priceline to item", Description = "Adds a new priceline")]
    public async Task<PriceLineListResponse> AddPriceLinesToItem([ActionParameter] PriceLineRequest request)
    {
        var intItemId = IntParser.Parse(request.ItemId, nameof(request.ItemId))!.Value;

        var priceUnitListResult = await ItemClient.getPriceUnit_ListAsync(Uuid, "en", "Translation");
        var priceUnits = priceUnitListResult.data?.Where(x =>
            x.description.Contains("Words Translation", StringComparison.OrdinalIgnoreCase)).ToArray();

        if (priceUnits == null || !priceUnits.Any())
            throw new("No price units found");

        foreach (var priceUnit in priceUnits)
        {
            var priceLine = new PriceLineIN
            {
                amount = request.Amount,
                priceUnitID = priceUnit.priceUnitID,
                taxType = 0,
                unit_price = request.UnitPrice
            };
            await ItemClient.insertPriceLineAsync(Uuid, intItemId, 3, priceLine,
                priceUnit.description.Contains("New", StringComparison.OrdinalIgnoreCase));
        }
        var priceListResult = await ItemClient.getPriceLine_ListAsync(Uuid, intItemId, 3);
        return new()
        {
            PriceLines = priceListResult.data.Select(x => new PriceLineResponse(x))
        };
    }

    [Action("Get language combinations for order", Description = "Get language combinations (source language - target " +
                                                                 "language) for order.")]
    public async Task<LanguageCombinationsResponse> GetLanguageCombinationsForOrder(
        [ActionParameter] OrderRequest request)
    {
        var intOrderId = IntParser.Parse(request.OrderId, nameof(request.OrderId))!.Value;

        var languageCombinations = await OrderClient.getLanguageCombinationAsync(Uuid, intOrderId);

        if (languageCombinations.data is null)
            throw new(languageCombinations.statusMessage);

        var languages = await AdminClient.getAvailableLanguagesAsync(Uuid, "en");

        var orderLanguageCombinations = languageCombinations.data
            .Select(combination => new { source = combination.Split(" - ")[0], target = combination.Split(" - ")[1] })
            .Select(combination =>
                new LanguageCombination(languages.data.First(l => l.name == combination.source).folderName,
                    languages.data.First(l => l.name == combination.target).folderName));

        return new() { LanguageCombinations = orderLanguageCombinations };
    }
}