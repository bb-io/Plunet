using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Plugins.Plunet.DataItem30Service;
using Blackbird.Plugins.Plunet.DataOrder30Service;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Models;
using Blackbird.Plugins.Plunet.Models.Item;
using Blackbird.Plugins.Plunet.Models.Order;
using Blackbird.Plugins.Plunet.Utils;

namespace Blackbird.Plugins.Plunet.Actions;

[ActionList]
public class OrderActions
{
    [Action("Get order", Description = "Get details for a Plunet order")]
    public async Task<OrderResponse> GetOrder(
        List<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] [Display("Order ID")]
        string orderId)
    {
        var intOrderId = IntParser.Parse(orderId, nameof(orderId))!.Value;
        var uuid = authProviders.GetAuthToken();

        await using var orderClient = Clients.GetOrderClient(authProviders.GetInstanceUrl());
        var orderResult = await orderClient.getOrderObjectAsync(uuid, intOrderId);
        await authProviders.Logout();

        if (orderResult.data is null)
            throw new(orderResult.statusMessage);

        return new(orderResult.data);
    }

    [Action("Create order", Description = "Create a new order in Plunet")]
    public async Task<CreateOrderResponse> CreateOrder(List<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] CreateOrderRequest request)
    {
        var uuid = authProviders.GetAuthToken();

        await using var orderClient = Clients.GetOrderClient(authProviders.GetInstanceUrl());
        var orderIdResult = await orderClient.insert2Async(uuid, new()
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

        await authProviders.Logout();

        return new CreateOrderResponse { OrderId = orderIdResult.data.ToString() };
    }

    [Action("Add item to order", Description = "Add a new item to an order")]
    public async Task<CreateItemResponse> AddItemToOrder(List<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] CreateItemRequest request)
    {
        var uuid = authProviders.GetAuthToken();

        await using var itemClient = Clients.GetItemClient(authProviders.GetInstanceUrl());
        var itemIdResult = await itemClient.insert2Async(uuid, new()
        {
            briefDescription = request.ItemName,
            projectID = IntParser.Parse(request.ProjectId, nameof(request.ProjectId)) ?? default,
            totalPrice = request.TotalPrice ?? default,
            projectType = request.ProjectType,
            deliveryDeadline = request.DeadlineDateTime ?? default,
            reference = request.Reference,
            status = request.Status ?? default,
        });

        await authProviders.Logout();

        return new CreateItemResponse { ItemId = itemIdResult.data.ToString() };
    }

    [Action("Create order based on template", Description = "Create a new order based on a template")]
    public async Task<CreateOrderResponse> CreateOrderBasedOnTemplate(
        List<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] CreateOrderRequest request,
        [ActionParameter] [Display("Template name")]
        string templateName)
    {
        try
        {
            var uuid = authProviders.GetAuthToken();

            await using var orderClient = Clients.GetOrderClient(authProviders.GetInstanceUrl());
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
            var templates = await orderClient.getTemplateListAsync(uuid);

            if (templates == null || !templates.data.Any())
                throw new("Not templates found");

            var template = templates.data.FirstOrDefault(t =>
                t.templateName.Contains(templateName, StringComparison.OrdinalIgnoreCase));

            if (template == null)
                throw new("No template found with the provided name");

            var orderIdResult = await orderClient.insert_byTemplateAsync(uuid, order, template.templateID);
            return new CreateOrderResponse { OrderId = orderIdResult.data.ToString() };
        }
        finally
        {
            await authProviders.Logout();
        }
    }

    [Action("Add language combination to order", Description = "Add a new language combination to an existing order")]
    public async Task<AddLanguageCombinationResponse> AddLanguageCombinationToOrder(
        List<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] AddLanguageCombinationRequest request)
    {
        try
        {
            var intOrderId = IntParser.Parse(request.OrderId, nameof(request.OrderId))!.Value;
            var uuid = authProviders.GetAuthToken();

            await using var orderClient = Clients.GetOrderClient(authProviders.GetInstanceUrl());
            var langCombination =
                await ClientHelper.GetLanguageNamesCombinationByLanguageCodeIso(uuid,
                    new(request.SourceLanguageCode, request.TargetLanguageCode));

            var result = await orderClient.addLanguageCombinationAsync(uuid, langCombination.Source,
                langCombination.Target, intOrderId);

            return new AddLanguageCombinationResponse { LanguageCombinationId = result.data.ToString() };
        }
        finally
        {
            await authProviders.Logout();
        }
    }

    [Action("Set language combination to item", Description = "Set the language combination to an item")]
    public async Task<BaseResponse> SetLanguageCombinationToItem(List<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] SetLanguageCombinationRequest request)
    {
        var intLangCombId = IntParser.Parse(request.LanguageCombinationId, nameof(request.LanguageCombinationId))!
            .Value;
        var intItemId = IntParser.Parse(request.ItemId, nameof(request.ItemId))!.Value;
        var uuid = authProviders.GetAuthToken();

        await using var itemClient = Clients.GetItemClient(authProviders.GetInstanceUrl());
        var response = await itemClient
            .setLanguageCombinationIDAsync(uuid, intLangCombId, 3, intItemId);
        await authProviders.Logout();

        return new BaseResponse
        {
            StatusCode = response.statusCode
        };
    }

    [Action("Add priceline to item", Description = "Adds a new priceline")]
    public async Task<PriceLineListResponse> AddPriceLinesToItem(
        List<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] PriceLineRequest request)
    {
        try
        {
            var intItemId = IntParser.Parse(request.ItemId, nameof(request.ItemId))!.Value;
            var uuid = authProviders.GetAuthToken();

            await using var itemClient = Clients.GetItemClient(authProviders.GetInstanceUrl());
            var priceUnitListResult = await itemClient.getPriceUnit_ListAsync(uuid, "en", "Translation");
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
                await itemClient.insertPriceLineAsync(uuid, intItemId, 3, priceLine,
                    priceUnit.description.Contains("New", StringComparison.OrdinalIgnoreCase));
            }

            var priceListResult = await itemClient.getPriceLine_ListAsync(uuid, intItemId, 3);

            return new PriceLineListResponse
            {
                PriceLines = priceListResult.data.Select(x => new PriceLineResponse(x))
            };
        }
        finally
        {
            await authProviders.Logout();
        }
    }

    [Action("Upload file", Description = "Upload a file to Plunet")]
    public async Task<BaseResponse> UploadFile(List<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] UploadDocumentRequest request)
    {
        var intOrderId = IntParser.Parse(request.OrderId, nameof(request.OrderId))!.Value;
        var intFoldType = IntParser.Parse(request.FolderType, nameof(request.FolderType))!.Value;
        var uuid = authProviders.GetAuthToken();

        await using var dataDocumentClient = Clients.GetDocumentClient(authProviders.GetInstanceUrl());
        var response = await dataDocumentClient.upload_DocumentAsync(uuid, intOrderId, intFoldType,
            request.FileContentBytes, request.FilePath, request.FileContentBytes.Length);
        await authProviders.Logout();

        return new BaseResponse { StatusCode = response.Result.statusCode };
    }

    [Action("Download file", Description = "Download a file from Plunet")]
    public async Task<FileResponse> DownloadFile(List<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] DownloadDocumentRequest request)
    {
        var intOrderId = IntParser.Parse(request.OrderId, nameof(request.OrderId))!.Value;
        var intFoldType = IntParser.Parse(request.FolderType, nameof(request.FolderType))!.Value;
        var uuid = authProviders.GetAuthToken();

        await using var dataDocumentClient = Clients.GetDocumentClient(authProviders.GetInstanceUrl());
        var response =
            await dataDocumentClient.download_DocumentAsync(uuid, intOrderId, intFoldType,
                request.FilePathName);

        await authProviders.Logout();

        return new(response.fileContent);
    }

    [Action("List files", Description = "List files from Plunet")]
    public async Task<ListFilesResponse> ListFiles(List<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] ListFilesRequest request)
    {
        var intOrderId = IntParser.Parse(request.OrderId, nameof(request.OrderId))!.Value;
        var intFoldType = IntParser.Parse(request.FolderType, nameof(request.FolderType))!.Value;
        var uuid = authProviders.GetAuthToken();

        await using var dataDocumentClient = Clients.GetDocumentClient(authProviders.GetInstanceUrl());
        var response = await dataDocumentClient.getFileListAsync(uuid, intOrderId, intFoldType);

        await authProviders.Logout();

        return new(response.data);
    }

    [Action("Delete order", Description = "Delete a Plunet order")]
    public async Task<BaseResponse> DeleteOrder(List<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] [Display("Order ID")]
        string orderId)
    {
        var intOrderId = IntParser.Parse(orderId, nameof(orderId))!.Value;
        var uuid = authProviders.GetAuthToken();

        await using var orderClient = Clients.GetOrderClient(authProviders.GetInstanceUrl());
        var response = await orderClient.deleteAsync(uuid, intOrderId);
        await authProviders.Logout();

        return new BaseResponse { StatusCode = response.statusCode };
    }

    [Action("Update order", Description = "Update Plunet order")]
    public async Task<BaseResponse> UpdateOrder(List<AuthenticationCredentialsProvider> authProviders,
        [ActionParameter] UpdateOrderRequest request)
    {
        var intOrderId = IntParser.Parse(request.OrderId, nameof(request.OrderId))!.Value;
        var uuid = authProviders.GetAuthToken();

        var orderClient = Clients.GetOrderClient(authProviders.GetInstanceUrl());
        var response = await orderClient.updateAsync(uuid, new OrderIN
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

        await authProviders.Logout();

        return new BaseResponse { StatusCode = response.statusCode };
    }
}