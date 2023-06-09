﻿using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Plugins.Plunet.DataAdmin30Service;
using Blackbird.Plugins.Plunet.DataItem30Service;
using Blackbird.Plugins.Plunet.DataOrder30Service;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Models;
using Blackbird.Plugins.Plunet.Models.Item;
using Blackbird.Plugins.Plunet.Models.Order;
using Blackbird.Plugins.Plunet.Models.Quote;

namespace Blackbird.Plugins.Plunet.Actions;

[ActionList]
public class OrderActions
{
    [Action("Get order", Description = "Get details for a Plunet order")]
    public async Task<OrderResponse> GetOrder(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int orderId)
    {
        var uuid = authProviders.GetAuthToken();
        using var orderClient = Clients.GetOrderClient(authProviders.GetInstanceUrl());
        var orderResult = await orderClient.getOrderObjectAsync(uuid, orderId);
        var response = orderResult.data ?? null;
        await authProviders.Logout();
        return MapOrderResponse(response);
    }

    [Action("Create order", Description = "Create a new order in Plunet")]
    public async Task<CreateOrderResponse> CreateOrder(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] CreateOrderRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        using var orderClient = Clients.GetOrderClient(authProviders.GetInstanceUrl());
        var orderIdResult = await orderClient.insert2Async(uuid, new OrderIN
        {
            projectName = request.ProjectName,
            customerID = request.CustomerId,
            subject = request.ProjectName,
            projectManagerID = 7,
            orderDate = DateTime.Now,
            deliveryDeadline = request.Deadline,
        });
        await authProviders.Logout();
        return new CreateOrderResponse {OrderId = orderIdResult.data};
    }

    [Action("Add item to order", Description = "Add a new item to an order")]
    public async Task<CreateItemResponse> AddItemToOrder(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] CreateItemRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        using var itemClient = Clients.GetItemClient(authProviders.GetInstanceUrl());
        var itemIdResult = await itemClient.insert2Async(uuid, new ItemIN
        {
            briefDescription = request.ItemName,
            projectID = request.OrderId,
            totalPrice = request.TotalPrice,
            projectType = 3,
            deliveryDeadline = request.DeadlineDateTime,
        });
        await authProviders.Logout();
        return new CreateItemResponse {ItemId = itemIdResult.data};
    }

    [Action("Create order based on template", Description = "Create a new order based on a template")]
    public async Task<CreateOrderResponse> CreateOrderBasedOnTemplate(List<AuthenticationCredentialsProvider> authProviders,  [ActionParameter] CreateOrderRequest request,
        [ActionParameter] string templateName)
    {
        var uuid = authProviders.GetAuthToken();
        using var orderClient = Clients.GetOrderClient(authProviders.GetInstanceUrl());
        var order = new OrderIN
        {
            projectName = request.ProjectName,
            customerID = request.CustomerId,
            subject = request.ProjectName,
            projectManagerID = 7,
            orderDate = DateTime.Now,
            deliveryDeadline = request.Deadline
        };
        var templates = await orderClient.getTemplateListAsync(uuid);
        if (templates == null || !templates.data.Any())
        {
            await authProviders.Logout();
            return new CreateOrderResponse();
        }

        var template = templates.data.FirstOrDefault(t =>
            t.templateName.Contains(templateName, StringComparison.OrdinalIgnoreCase));
        if (template == null)
        {
            await authProviders.Logout();
            return new CreateOrderResponse();
        }

        var orderIdResult = await orderClient.insert_byTemplateAsync(uuid, order, template.templateID);
        await authProviders.Logout();
        return new CreateOrderResponse {OrderId = orderIdResult.data};
    }

    [Action("Add language combination to order", Description = "Add a new language combination to an existing order")]
    public async Task<AddLanguageCombinationResponse> AddLanguageCombinationToOrder(List<AuthenticationCredentialsProvider> authProviders,  [ActionParameter] AddLanguageCombinationRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        using var orderClient = Clients.GetOrderClient(authProviders.GetInstanceUrl());
        var langCombination =
            GetLanguageNamesCombinationByLanguageCodeIso(uuid, request.SourceLanguageCode,
                request.TargetLanguageCode);
        if (string.IsNullOrEmpty(langCombination.TargetLanguageName))
        {
            await authProviders.Logout();
            return new AddLanguageCombinationResponse();
        }

        var result = await orderClient.addLanguageCombinationAsync(uuid, langCombination.SourceLanguageName,
            langCombination.TargetLanguageName, request.OrderId);
        await authProviders.Logout();
        return new AddLanguageCombinationResponse {LanguageCombinationId = result.data};
    }

    [Action("Set language combination to item", Description = "Set the language combination to an item")]
    public async Task<BaseResponse> SetLanguageCombinationToItem(List<AuthenticationCredentialsProvider> authProviders,  [ActionParameter] SetLanguageCombinationRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        using var itemClient = Clients.GetItemClient(authProviders.GetInstanceUrl());
        var response = await itemClient
            .setLanguageCombinationIDAsync(uuid, request.LanguageCombinationId, 3, request.ItemId);
        await authProviders.Logout();
        return new BaseResponse
        {
            StatusCode = response.statusCode
        };
    }

    [Action("Add priceline to item", Description = "Adds a new priceline")]
    public async Task<PriceLineListResponse> AddPriceLinesToItem(List<AuthenticationCredentialsProvider> authProviders,  [ActionParameter] PriceLineRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        using var itemClient = Clients.GetItemClient(authProviders.GetInstanceUrl());
        var priceUnitListResult = await itemClient.getPriceUnit_ListAsync(uuid, "en", "Translation");
        var priceUnits = priceUnitListResult.data.Where(x =>
            x.description.Contains("Words Translation", StringComparison.OrdinalIgnoreCase));
        if (priceUnits == null || !priceUnits.Any())
        {
            await authProviders.Logout();
            return new PriceLineListResponse();
        }

        foreach (var priceUnit in priceUnits)
        {
            var priceLine = new PriceLineIN
            {
                amount = request.Amount,
                priceUnitID = priceUnit.priceUnitID,
                taxType = 0,
                unit_price = request.UnitPrice
            };
            await itemClient.insertPriceLineAsync(uuid, request.ItemId, 3, priceLine,
                priceUnit.description.Contains("New", StringComparison.OrdinalIgnoreCase));
        }

        var priceListResult = await itemClient.getPriceLine_ListAsync(uuid, request.ItemId, 3);
        await authProviders.Logout();
        return new PriceLineListResponse {PriceLines = priceListResult.data.Select(MapPriceLineResponse)};
    }

    [Action("Upload file", Description = "Upload a file to Plunet")]
    public async Task<BaseResponse> UploadFile(List<AuthenticationCredentialsProvider> authProviders,  [ActionParameter] UploadDocumentRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        using var dataDocumentClient = Clients.GetDocumentClient(authProviders.GetInstanceUrl());
        var response = await dataDocumentClient.upload_DocumentAsync(uuid, request.OrderId, request.FolderType,
            request.FileContentBytes, request.FilePath, request.FileContentBytes.Length);
        await authProviders.Logout();
        return new BaseResponse {StatusCode = response.Result.statusCode};
    }    
    
    [Action("Download file", Description = "Download a file from Plunet")]
    public async Task<FileResponse> DownloadFile(List<AuthenticationCredentialsProvider> authProviders, 
        [ActionParameter] DownloadDocumentRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        
        using var dataDocumentClient = Clients.GetDocumentClient(authProviders.GetInstanceUrl());
        var response = await dataDocumentClient.download_DocumentAsync(uuid, request.OrderId, request.FolderType, request.FilePathName);
        
        await authProviders.Logout();
        
        return new(response.fileContent);
    }    
    
    [Action("List files", Description = "List files from Plunet")]
    public async Task<ListFilesResponse> ListFiles(List<AuthenticationCredentialsProvider> authProviders, 
        [ActionParameter] ListFilesRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        
        using var dataDocumentClient = Clients.GetDocumentClient(authProviders.GetInstanceUrl());
        var response = await dataDocumentClient.getFileListAsync(uuid, request.OrderId, request.FolderType);
        
        await authProviders.Logout();
        
        return new(response.data);
    }

    [Action("Delete order", Description = "Delete a Plunet order")]
    public async Task<BaseResponse> DeleteOrder(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int orderId)
    {
        var uuid = authProviders.GetAuthToken();
        using var orderClient = Clients.GetOrderClient(authProviders.GetInstanceUrl());
        var response = await orderClient.deleteAsync(uuid, orderId);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    [Action("Update order", Description = "Update Plunet order")]
    public async Task<BaseResponse> UpdateOrder(List<AuthenticationCredentialsProvider> authProviders, [ActionParameter] UpdateOrderRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        var orderClient = Clients.GetOrderClient(authProviders.GetInstanceUrl());
        var response = await orderClient.updateAsync(uuid, new OrderIN
        {
            orderID = request.OrderId,
            currency = request.Currency,
            customerID = request.CustomerId,
            projectManagerMemo = request.ProjectManagerMemo,
            projectName = request.ProjectName,
            referenceNumber = request.ReferenceNumber,
            subject = request.Subject,
            projectManagerID = 7,
            deliveryDeadline = request.DeliveryDeadline
        }, true);
        await authProviders.Logout();
        return new BaseResponse { StatusCode = response.statusCode };
    }

    private (string SourceLanguageName, string TargetLanguageName)
        GetLanguageNamesCombinationByLanguageCodeIso(string token, string sourceLanguageCode, string targetLanguageCode)
    {
        using var adminClient = new DataAdmin30Client();
        var availableLanguagesResult = adminClient.getAvailableLanguagesAsync(token, "en").GetAwaiter().GetResult();
        if (availableLanguagesResult.data == null || availableLanguagesResult.data.Length == 0)
        {
            return new ValueTuple<string, string>();
        }

        var sourceLanguage = availableLanguagesResult.data.FirstOrDefault(x =>
                                 x.isoCode.Equals(sourceLanguageCode, StringComparison.OrdinalIgnoreCase) ||
                                 x.folderName.Equals(sourceLanguageCode, StringComparison.OrdinalIgnoreCase)) ??
                             availableLanguagesResult.data.FirstOrDefault(x => x.isoCode.ToUpper() == "ENG");
        var targetLanguage = availableLanguagesResult.data.FirstOrDefault(x =>
            x.isoCode.Equals(targetLanguageCode, StringComparison.OrdinalIgnoreCase) ||
            x.folderName.Equals(targetLanguageCode, StringComparison.OrdinalIgnoreCase));
        return targetLanguage == null
            ? new ValueTuple<string, string>()
            : new ValueTuple<string, string>(sourceLanguage?.name, targetLanguage.name);
    }

    private OrderResponse MapOrderResponse(Order? order)
    {
        return order == null
            ? new OrderResponse()
            : new OrderResponse
            {
                Currency = order.currency,
                CustomerId = order.customerID,
                DeliveryDeadline = order.deliveryDeadline,
                OrderClosingDate = order.orderClosingDate,
                OrderDate = order.orderDate,
                OrderId = order.orderID,
                OrderName = order.orderDisplayName,
                ProjectManagerId = order.projectManagerID,
                ProjectName = order.projectName,
                Rate = order.rate
            };
    }

    private PriceLineResponse MapPriceLineResponse(PriceLine? priceLine)
    {
        return priceLine == null
            ? new PriceLineResponse()
            : new PriceLineResponse
            {
                PriceAmount = priceLine.amount,
                PriceAmountPerUnit = priceLine.amount_perUnit,
                UnitPrice = priceLine.unit_price,
                PriceLineId = priceLine.priceLineID,
                PriceUnitId = priceLine.priceUnitID,
                Sequence = priceLine.sequence,
                TaxType = priceLine.taxType
            };
    }
}