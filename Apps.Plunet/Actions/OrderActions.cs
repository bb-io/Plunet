using Apps.Plunet.Constants;
using Apps.Plunet.DataSourceHandlers;
using Apps.Plunet.Invocables;
using Apps.Plunet.Models;
using Apps.Plunet.Models.Order;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
using Blackbird.Plugins.Plunet.DataOrder30Service;

namespace Apps.Plunet.Actions;

[ActionList("Orders")]
public class OrderActions(InvocationContext invocationContext) : PlunetInvocable(invocationContext)
{
    [Action("Search orders", Description = "Search for specific orders based on specific criteria")]
    public async Task<SearchResponse<OrderResponse>> SearchOrders([ActionParameter] SearchOrderInput input)
    {
        var searchResult = await ExecuteWithRetry(() => OrderClient.searchAsync(Uuid,
            new()
            {
                sourceLanguage = input.SourceLanguage ?? string.Empty,
                targetLanguage = input.TargetLanguage ?? string.Empty,
                orderStatus = ParseId(input.OrderStatus),
                timeFrame = input.DateFrom is not null || input.DateTo is not null
                    ? new()
                    {
                        dateFrom = input.DateFrom ?? default,
                        dateTo = input.DateTo ?? default,
                        dateRelation = ParseId(input.DateRelation, 1)
                    }
                    : default,
                customerID = ParseId(input.CustomerId),
                projectName = input.ProjectName ?? string.Empty,
                projectType = ParseId(input.ProjectType),
                projectDescription = input.ProjectDescription ?? string.Empty,
                itemStatus = input.ItemStatus?.Select(ParseId).ToArray(),
            }));

        var ids = searchResult?
           .Where(x => x.HasValue)
           .Select(x => x!.Value)
           .ToArray() ?? Array.Empty<int>();

        if (ids.Length == 0)
            return new SearchResponse<OrderResponse>();

        var limitedIds = ids.Take(input.Limit ?? SystemConsts.SearchLimit).ToArray();

        if (input.OnlyReturnIds == true)
        {
            var idOnly = limitedIds
                .Select(id => new OrderResponse(
                    new Order
                    {
                        orderID = id,
                        projectName = string.Empty,
                        orderDisplayName = string.Empty,
                        currency = string.Empty,
                        subject = string.Empty
                    },
                    Array.Empty<LanguageCombination>()
                ))
                .ToList();

            return new SearchResponse<OrderResponse>(idOnly);
        }
        var results = new List<OrderResponse>(limitedIds.Length);
        foreach (var id in limitedIds)
        {
            var orderResponse = await GetOrder(new OrderRequest { OrderId = id.ToString() });
            results.Add(orderResponse);
        }

        return new SearchResponse<OrderResponse>(results);
    }

    [Action("Get order", Description = "Get the Plunet order")]
    public async Task<OrderResponse> GetOrder([ActionParameter] OrderRequest request)
    {
        var order = await ExecuteWithRetry(() => OrderClient.getOrderObjectAsync(Uuid, ParseId(request.OrderId)));

        var languageCombinations = await ExecuteWithRetry(() => OrderClient.getLanguageCombinationAsync(Uuid, ParseId(request.OrderId)));

        var orderLanguageCombinations = await ParseLanguageCombinations(languageCombinations);

        var items = await ExecuteWithRetryAcceptNull(() => ItemClient.getAllItemObjectsAsync(Uuid, ParseId(request.OrderId), 3));

        var totalOrderPrice = items?.Sum(x => x.totalPrice) ?? 0;

        var contact = await ExecuteWithRetryAcceptNull(() => OrderClient.getCustomerContactIDAsync(Uuid, ParseId(request.OrderId)));

        var status = await ExecuteWithRetry(() => OrderClient.getProjectStatusAsync(Uuid, ParseId(request.OrderId)));

        var projectCategory = await ExecuteWithRetryAcceptNull(() => OrderClient.getProjectCategoryAsync(Uuid, Language, ParseId(request.OrderId)));

        var projectStatus = await ExecuteWithRetry(() => OrderClient.getProjectStatusAsync(Uuid, ParseId(request.OrderId)));

        return new(order, orderLanguageCombinations)
        {
            TotalPrice = totalOrderPrice,
            ContactId = contact?.ToString(),
            Status = status.ToString(),
            ProjectCategory = projectCategory ?? string.Empty,
            ProjectStatus = projectStatus.ToString()
        };
    }

    [Action("Find order from order number (not ID)",
        Description = "Get order details given an order number")]
    public async Task<OrderResponse> GetOrderFromOrderNumber([ActionParameter] OrderNumberRequest input) 
    {
        var order = await ExecuteWithRetry(() => OrderClient.getOrderObject2Async(Uuid, input.OrderNumber));

        var languageCombinations = await ExecuteWithRetry(() => OrderClient.getLanguageCombinationAsync(Uuid, order.orderID));

        var orderLanguageCombinations = await ParseLanguageCombinations(languageCombinations);

        var items = await ExecuteWithRetryAcceptNull(() => ItemClient.getAllItemObjectsAsync(Uuid, order.orderID, 3));

        var totalOrderPrice = items?.Sum(x => x.totalPrice) ?? 0;

        var contact = await ExecuteWithRetryAcceptNull(() => OrderClient.getCustomerContactIDAsync(Uuid, order.orderID));

        var status = await ExecuteWithRetry(() => OrderClient.getProjectStatusAsync(Uuid, order.orderID));

        var projectCategory = await ExecuteWithRetryAcceptNull(() => OrderClient.getProjectCategoryAsync(Uuid, Language, order.orderID));

        var projectStatus = await ExecuteWithRetry(() => OrderClient.getProjectStatusAsync(Uuid, order.orderID));

        return new(order, orderLanguageCombinations)
        {
            TotalPrice = totalOrderPrice,
            ContactId = contact?.ToString(),
            Status = status.ToString(),
            ProjectCategory = projectCategory ?? string.Empty,
            ProjectStatus = projectStatus.ToString()
        };

    }

    [Action("Get order target languages for source",
        Description = "Given a source language and an order, get all the target languages that this order represents")]
    public async Task<LanguagesResponse> GetOrderTargetLanguage([ActionParameter] OrderRequest request,
        [ActionParameter] SourceLanguageRequest language)
    {
        var languageCombinations = await ExecuteWithRetry(() => OrderClient.getLanguageCombinationAsync(Uuid, ParseId(request.OrderId)));

        var orderLanguageCombinations = await ParseLanguageCombinations(languageCombinations);
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
            referenceNumber = request.ReferenceNumber           
        };

        OrderClient.Endpoint.Binding.SendTimeout = TimeSpan.FromMinutes(5);
        OrderClient.Endpoint.Binding.ReceiveTimeout = TimeSpan.FromMinutes(5);
        var orderId = templateRequest.TemplateId == null
            ? await ExecuteWithRetry(() => OrderClient.insert2Async(Uuid, orderIn))
            : await ExecuteWithRetry(() => OrderClient.insert_byTemplateAsync(Uuid, orderIn, ParseId(templateRequest.TemplateId)));

        //if (request.Status != null)
        //{
        //    await ExecuteWithRetry(() => OrderClient.setProjectStatusAsync(Uuid, orderId, ParseId(request.Status)));
        //}

        if (request.ProjectCategory != null)
            await ExecuteWithRetry(() => OrderClient.setProjectCategoryAsync(Uuid, request.ProjectCategory, Language, orderId));

        if (request.MasterProjectID != null)
            await ExecuteWithRetry(() => OrderClient.setMasterProjectIDAsync(Uuid, orderId, ParseId(request.MasterProjectID)));

        if (request.Deadline is not null)
            await ExecuteWithRetry(() => OrderClient.setDeliveryDeadlineAsync(Uuid, request.Deadline.Value, orderId ));

        return await GetOrder(new OrderRequest { OrderId = orderId.ToString() });
    }

    [Action("Create order by template", Description = "Create a new order in Plunet by template")]
    public async Task<OrderResponse> CreateOrderByTemplate(
        [ActionParameter, Display("Template ID"), DataSource(typeof(TemplateDataHandler))]
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
         //   Status = request.Status,
            Subject = request.Subject,
            ProjectCategory = request.ProjectCategory,
            MasterProjectID = request.MasterProjectID
        };

        return await CreateOrder(createOrderRequest, new OrderTemplateRequest { TemplateId = templateId });
    }

    [Action("Delete order", Description = "Delete a Plunet order")]
    public async Task DeleteOrder([ActionParameter] OrderRequest request)
    {
        await ExecuteWithRetry(() => OrderClient.deleteAsync(Uuid, ParseId(request.OrderId)));
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
            currency = request.Currency,
            projectManagerMemo = request.ProjectManagerMemo,
            rate = request.Rate ?? default,
            referenceNumber = request.ReferenceNumber
        };

        await ExecuteWithRetry(() => OrderClient.updateAsync(Uuid, orderUpdate, false));

        //if (request.Status != null)
        //{
        //    await ExecuteWithRetry(() => OrderClient.setProjectStatusAsync(Uuid, ParseId(order.OrderId), ParseId(request.Status)));
        //}

        if (request.Deadline is not null)
        { await ExecuteWithRetry(() => OrderClient.setDeliveryDeadlineAsync(Uuid, request.Deadline.Value, ParseId(order.OrderId))); }

        if (request.ProjectCategory != null)
        {
            await ExecuteWithRetry(() => OrderClient.setProjectCategoryAsync(Uuid, request.ProjectCategory, Language, ParseId(order.OrderId)));
        }

        if (request.MasterProjectID != null)
        {
            await ExecuteWithRetry(() => OrderClient.setMasterProjectIDAsync(Uuid, ParseId(order.OrderId), ParseId(request.MasterProjectID)));
        }

        return await GetOrder(order);
    }

    [Action("Add language combination to order", Description = "Add a new language combination to an existing order")]
    public async Task<AddLanguageCombinationResponse> AddLanguageCombinationToOrder(
        [ActionParameter] OrderRequest order, [ActionParameter] LanguageCombinationRequest request)
    {
        var sourceLanguage = await GetLanguageFromIsoOrFolderOrName(request.SourceLanguageCode);
        var targetLanguage = await GetLanguageFromIsoOrFolderOrName(request.TargetLanguageCode);

        var languageCombinations = await ExecuteWithRetry(() => OrderClient.getLanguageCombinationAsync(Uuid, ParseId(order.OrderId)));
        var orderLanguageCombinations = await ParseLanguageCombinations(languageCombinations);
        if (orderLanguageCombinations.Any(x => x.Source == sourceLanguage.folderName && x.Target == targetLanguage.folderName))
            { return null; }

        var result = await ExecuteWithRetry(() => OrderClient.addLanguageCombinationAsync(Uuid, sourceLanguage.name, targetLanguage.name, ParseId(order.OrderId)));

        return new()
        {
            LanguageCombinationId = result.ToString()
        };
    }

    [Action("Link orders", Description = "Create a link between two orders")]
    public async Task LinkOrders(
    [ActionParameter] LinkOrdersRequest request)
    {
        await ExecuteWithRetry(() =>
            OrderClient.createLinkAsync(
                Uuid,
                ParseId(request.SourceOrderId),ParseId(request.TargetId),3, request.IsBidirectional, request.Memo
            )
        );
    }
}