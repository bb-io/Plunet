using Apps.Plunet.Constants;
using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models;
using Apps.Plunet.Models.Order;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.DataItem30Service;
using Blackbird.Plugins.Plunet.DataOrder30Service;
using IntegerArrayResult = Blackbird.Plugins.Plunet.DataOrder30Service.IntegerArrayResult;
using IntegerResult = Blackbird.Plugins.Plunet.DataOrder30Service.IntegerResult;
using Result = Blackbird.Plugins.Plunet.DataOrder30Service.Result;
using StringArrayResult = Blackbird.Plugins.Plunet.DataOrder30Service.StringArrayResult;
using StringResult = Blackbird.Plugins.Plunet.DataOrder30Service.StringResult;

namespace Apps.Plunet.Actions;

[ActionList]
public class OrderActions(InvocationContext invocationContext) : PlunetInvocable(invocationContext)
{
    [Action("Search orders", Description = "Search for specific orders based on specific criteria")]
    public async Task<ListOrderResponse> SearchOrders([ActionParameter] SearchOrderInput input)
    {
        var searchResult = await ExecuteWithRetry<IntegerArrayResult>(async () => await OrderClient.searchAsync(Uuid,
            new()
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
            }));

        if (searchResult.statusMessage != ApiResponses.Ok)
            throw new(searchResult.statusMessage);

        var results = new List<OrderResponse>();
        if (searchResult.data != null)
        {
            foreach (var id in searchResult.data.Where(x => x.HasValue))
            {
                var orderResponse = await GetOrder(new OrderRequest { OrderId = id!.Value.ToString() });
                results.Add(orderResponse);
            }
        }

        return new ListOrderResponse { Orders = results };
    }

    [Action("Get order", Description = "Get the Plunet order")]
    public async Task<OrderResponse> GetOrder([ActionParameter] OrderRequest request)
    {
        var orderResult = await ExecuteWithRetry<OrderResult>(async () =>
            await OrderClient.getOrderObjectAsync(Uuid, ParseId(request.OrderId)));
        if (orderResult.statusMessage != ApiResponses.Ok)
            throw new(orderResult.statusMessage);

        var languageCombinations = await ExecuteWithRetry<StringArrayResult>(async () =>
            await OrderClient.getLanguageCombinationAsync(Uuid, ParseId(request.OrderId)));
        if (languageCombinations.statusMessage != ApiResponses.Ok)
            throw new(languageCombinations.statusMessage);

        var orderLanguageCombinations = await ParseLanguageCombinations(languageCombinations.data);

        var itemsResult = await ExecuteWithRetry<ItemListResult>(async () =>
            await ItemClient.getAllItemObjectsAsync(Uuid, ParseId(request.OrderId), 3));
        if (itemsResult.statusMessage != ApiResponses.Ok)
            throw new(itemsResult.statusMessage);

        var totalOrderPrice = itemsResult.data?.Sum(x => x.totalPrice) ?? 0;

        var contactResult = await ExecuteWithRetry<IntegerResult>(async () =>
            await OrderClient.getCustomerContactIDAsync(Uuid, ParseId(request.OrderId)));

        var statusResult = await ExecuteWithRetry<IntegerResult>(async () =>
            await OrderClient.getProjectStatusAsync(Uuid, ParseId(request.OrderId)));
        if (statusResult.statusMessage != ApiResponses.Ok)
            throw new(statusResult.statusMessage);

        var categoryResult = await ExecuteWithRetry<StringResult>(async () =>
            await OrderClient.getProjectCategoryAsync(Uuid, Language, ParseId(request.OrderId)));
        if (categoryResult.statusMessage != ApiResponses.Ok)
        {
            if (categoryResult.statusMessage.Contains(ApiResponses.ProjectCategoryIsNotSet))
            {
                categoryResult = new StringResult() { data = string.Empty };
            }
            else
            {
                throw new(categoryResult.statusMessage);
            }
        }

        var projectStatusResult = await ExecuteWithRetry<IntegerResult>(async () =>
            await OrderClient.getProjectStatusAsync(Uuid, ParseId(request.OrderId)));
        if (projectStatusResult.statusMessage != ApiResponses.Ok)
            throw new(projectStatusResult.statusMessage);

        return new(orderResult.data, orderLanguageCombinations)
        {
            TotalPrice = totalOrderPrice,
            ContactId = contactResult.data == 0 ? null : contactResult.data.ToString(),
            Status = statusResult.data.ToString(),
            ProjectCategory = categoryResult.data,
            ProjectStatus = projectStatusResult.data.ToString()
        };
    }

    [Action("Get order target languages for source",
        Description = "Given a source language and an order, get all the target languages that this order represents")]
    public async Task<LanguagesResponse> GetOrderTargetLanguage([ActionParameter] OrderRequest request,
        [ActionParameter] SourceLanguageRequest language)
    {
        var languageCombinations = await ExecuteWithRetry<StringArrayResult>(async () =>
            await OrderClient.getLanguageCombinationAsync(Uuid, ParseId(request.OrderId)));
        if (languageCombinations.statusMessage != ApiResponses.Ok)
            throw new(languageCombinations.statusMessage);

        var orderLanguageCombinations = await ParseLanguageCombinations(languageCombinations.data);
        var response = orderLanguageCombinations.Where(x => x.Source == language.SourceLanguageCode)
            .Select(x => x.Target).Distinct();

        return new LanguagesResponse
        {
            Languages = response,
        };
    }

    [Action("Create order", Description = "Create a new order in Plunet")]
    public async Task<OrderResponse> CreateOrder([ActionParameter] CreateOrderRequest request,
        OrderTemplateRequest templateRequest)
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
            referenceNumber = request.ReferenceNumber,
        };

        var response = templateRequest.TemplateId == null
            ? await ExecuteWithRetry<IntegerResult>(async () => await OrderClient.insert2Async(Uuid, orderIn))
            : await ExecuteWithRetry<IntegerResult>(async () =>
                await OrderClient.insert_byTemplateAsync(Uuid, orderIn, ParseId(templateRequest.TemplateId)));

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        if (request.Status != null)
        {
            var statusResponse = await ExecuteWithRetry<Result>(async () =>
                await OrderClient.setProjectStatusAsync(Uuid, response.data, ParseId(request.Status)));
            if (statusResponse.statusMessage != ApiResponses.Ok)
                throw new(statusResponse.statusMessage);
        }

        if (request.ProjectCategory != null)
        {
            var categoryResponse =
                await ExecuteWithRetry<Result>(async () =>
                    await OrderClient.setProjectCategoryAsync(Uuid, request.ProjectCategory, Language, response.data));
            
            if (categoryResponse.statusMessage != ApiResponses.Ok)
                throw new(categoryResponse.statusMessage);
        }

        string orderId = response.data.ToString();
        return await GetOrder(new OrderRequest { OrderId = orderId });
    }

    [Action("Create order by template", Description = "Create a new order in Plunet by template")]
    public async Task<OrderResponse> CreateOrderByTemplate(
        [ActionParameter, Display("Template"), DataSource(typeof(TemplateDataHandler))]
        string templateId,
        [ActionParameter] CreateOrderByTemplateRequest request)
    {
        var createOrderRequest = new CreateOrderRequest
        {
            ContactId = request.ContactId,
            CustomerId = request.CustomerId,
            Currency = request.Currency,
            Deadline = request.Deadline,
            ProjectManagerId = request.ProjectManagerId,
            ProjectManagerMemo = request.ProjectManagerMemo,
            ProjectName = request.ProjectName,
            Rate = request.Rate,
            ReferenceNumber = request.ReferenceNumber,
            Status = request.Status,
            Subject = request.Subject,
            ProjectCategory = request.ProjectCategory
        };
        
        return await CreateOrder(createOrderRequest, new OrderTemplateRequest { TemplateId = templateId });
    }

    [Action("Delete order", Description = "Delete a Plunet order")]
    public async Task DeleteOrder([ActionParameter] OrderRequest request)
    {
        await ExecuteWithRetry<Result>(async () => await OrderClient.deleteAsync(Uuid, ParseId(request.OrderId)));
    }

    [Action("Update order", Description = "Update Plunet order")]
    public async Task<OrderResponse> UpdateOrder([ActionParameter] OrderRequest order,
        [ActionParameter] CreateOrderRequest request)
    {
        var orderResopnse = await GetOrder(order);

        var orderUpdate = new OrderIN
        {
            orderID = ParseId(order.OrderId),
            projectName = request.ProjectName,
            customerContactID = ParseId(request.ContactId),
            customerID = ParseId(request.CustomerId) == -1
                ? ParseId(orderResopnse.CustomerId)
                : ParseId(request.CustomerId),
            subject = request.Subject,
            projectManagerID = ParseId(request.ProjectManagerId) == -1
                ? ParseId(orderResopnse.ProjectManagerId)
                : ParseId(request.ProjectManagerId),
            orderDate = DateTime.Now,
            deliveryDeadline = request.Deadline ?? default,
            currency = request.Currency,
            projectManagerMemo = request.ProjectManagerMemo,
            rate = request.Rate ?? default,
            referenceNumber = request.ReferenceNumber
        };

        var response = await ExecuteWithRetry<Result>(async () => await OrderClient.updateAsync(Uuid, orderUpdate, false));

        if (response.statusMessage != ApiResponses.Ok)
            throw new(response.statusMessage);

        if (request.Status != null)
        {
            var statusResponse =
                await ExecuteWithRetry<Result>(async () => await OrderClient.setProjectStatusAsync(Uuid, ParseId(order.OrderId), ParseId(request.Status)));
            if (statusResponse.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);
        }

        if (request.ProjectCategory != null)
        {
            var categoryResponse =
                await ExecuteWithRetry<Result>(async () => await OrderClient.setProjectCategoryAsync(Uuid, request.ProjectCategory, Language,
                    ParseId(order.OrderId)));
            
            if (categoryResponse.statusMessage != ApiResponses.Ok)
                throw new(categoryResponse.statusMessage);
        }

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
    public async Task<AddLanguageCombinationResponse> AddLanguageCombinationToOrder(
        [ActionParameter] OrderRequest order, [ActionParameter] LanguageCombinationRequest request)
    {
        var sourceLanguage = await GetLanguageFromIsoOrFolderOrName(request.SourceLanguageCode);
        var targetLanguage = await GetLanguageFromIsoOrFolderOrName(request.TargetLanguageCode);

        var result = await ExecuteWithRetry<IntegerResult>(async () => await OrderClient.addLanguageCombinationAsync(Uuid, sourceLanguage.name, targetLanguage.name,
            ParseId(order.OrderId)));

        return new()
        {
            LanguageCombinationId = result.data.ToString()
        };
    }

    private async Task<T> ExecuteWithRetry<T>(Func<Task<Result>> func, int maxRetries = 10, int initialDelay = 1000)
        where T : Result
    {
        var attempts = 0;
        var delay = initialDelay;

        while (true)
        {
            var result = await func();

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

                    delay = Math.Min(delay * 2, 20000);
                    continue;
                }

                throw new($"No more retries left. Last error: {result.statusMessage}, Session UUID used is invalid.");
            }

            return (T)result;
        }
    }

    private async Task<T> ExecuteWithRetry<T>(Func<Task<T>> func, int maxRetries = 10, int initialDelay = 1000)
        where T : Blackbird.Plugins.Plunet.DataItem30Service.Result
    {
        var attempts = 0;
        var delay = initialDelay;

        while (true)
        {
            var result = await func();

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

                    delay = Math.Min(delay * 2, 20000);
                    continue;
                }

                throw new($"No more retries left. Last error: {result.statusMessage}, Session UUID used is invalid.");
            }

            return (T)result;
        }
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