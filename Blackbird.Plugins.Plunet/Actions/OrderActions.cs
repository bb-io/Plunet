using System.Net.Mime;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Parsers;
using Blackbird.Plugins.Plunet.Api;
using Blackbird.Plugins.Plunet.DataItem30Service;
using Blackbird.Plugins.Plunet.DataOrder30Service;
using Blackbird.Plugins.Plunet.DataSourceHandlers;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Invocables;
using Blackbird.Plugins.Plunet.Models;
using Blackbird.Plugins.Plunet.Models.Item;
using Blackbird.Plugins.Plunet.Models.Order;

namespace Blackbird.Plugins.Plunet.Actions;

[ActionList]
public class OrderActions : PlunetInvocable
{
    public OrderActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Action("Get order", Description = "Get details for a Plunet order")]
    public async Task<OrderResponse> GetOrder([ActionParameter] [Display("Order ID")] [DataSource(typeof(OrderIdDataHandler))] string orderId)
    {
        var intOrderId = IntParser.Parse(orderId, nameof(orderId))!.Value;
        var uuid = Creds.GetAuthToken();

        await using var orderClient = Clients.GetOrderClient(Creds.GetInstanceUrl());
        var orderResult = await orderClient.getOrderObjectAsync(uuid, intOrderId);

        await Creds.Logout();

        if (orderResult.data is null)
            throw new(orderResult.statusMessage);

        return new(orderResult.data);
    }

    [Action("Create order", Description = "Create a new order in Plunet")]
    public async Task<CreateOrderResponse> CreateOrder([ActionParameter] CreateOrderRequest request)
    {
        var uuid = Creds.GetAuthToken();

        await using var orderClient = Clients.GetOrderClient(Creds.GetInstanceUrl());
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

        await Creds.Logout();

        return new()
        {
            OrderId = orderIdResult.data.ToString()
        };
    }

    [Action("Add item to order", Description = "Add a new item to an order")]
    public async Task<CreateItemResponse> AddItemToOrder([ActionParameter] CreateItemRequest request)
    {
        var uuid = Creds.GetAuthToken();

        await using var itemClient = Clients.GetItemClient(Creds.GetInstanceUrl());
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

        await Creds.Logout();

        return new()
        {
            ItemId = itemIdResult.data.ToString()
        };
    }

    [Action("List templates", Description = "List all templates")]
    public async Task<ListTemplatesResponse> ListTemplates()
    {
        var uuid = Creds.GetAuthToken();

        await using var orderClient = Clients.GetOrderClient(Creds.GetInstanceUrl());
        var response = await orderClient.getTemplateListAsync(uuid);

        await Creds.Logout();

        var templates = response.data.Select(x => new TemplateResponse(x)).ToArray();
        return new(templates);
    }

    [Action("Create order based on template", Description = "Create a new order based on a template")]
    public async Task<CreateOrderResponse> CreateOrderBasedOnTemplate([ActionParameter] CreateOrderRequest request,
        [ActionParameter] [DataSource(typeof(TemplateDataHandler))] [Display("Template")]
        string templateId)
    {
        var uuid = Creds.GetAuthToken();

        await using var orderClient = Clients.GetOrderClient(Creds.GetInstanceUrl());
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

        var orderIdResult = await orderClient.insert_byTemplateAsync(uuid, order, int.Parse(templateId));
        await Creds.Logout();

        return new()
        {
            OrderId = orderIdResult.data.ToString()
        };
    }

    [Action("Add language combination to order", Description = "Add a new language combination to an existing order")]
    public async Task<AddLanguageCombinationResponse> AddLanguageCombinationToOrder(
        [ActionParameter] AddLanguageCombinationRequest request)
    {
        try
        {
            var intOrderId = IntParser.Parse(request.OrderId, nameof(request.OrderId))!.Value;
            var uuid = Creds.GetAuthToken();

            await using var orderClient = Clients.GetOrderClient(Creds.GetInstanceUrl());
            var langCombination = await new LanguageCombination(request.SourceLanguageCode, request.TargetLanguageCode)
                .GetLangNamesByLangIso(Creds);

            var result = await orderClient.addLanguageCombinationAsync(uuid, langCombination.Source,
                langCombination.Target, intOrderId);

            return new()
            {
                LanguageCombinationId = result.data.ToString()
            };
        }
        finally
        {
            await Creds.Logout();
        }
    }

    [Action("Set language combination to item", Description = "Set the language combination to an item")]
    public async Task SetLanguageCombinationToItem([ActionParameter] SetLanguageCombinationRequest request)
    {
        var intLangCombId = IntParser.Parse(request.LanguageCombinationId, nameof(request.LanguageCombinationId))!
            .Value;
        var intItemId = IntParser.Parse(request.ItemId, nameof(request.ItemId))!.Value;

        var uuid = Creds.GetAuthToken();

        await using var itemClient = Clients.GetItemClient(Creds.GetInstanceUrl());
        await itemClient.setLanguageCombinationIDAsync(uuid, intLangCombId, 3, intItemId);

        await Creds.Logout();
    }

    [Action("Add priceline to item", Description = "Adds a new priceline")]
    public async Task<PriceLineListResponse> AddPriceLinesToItem([ActionParameter] PriceLineRequest request)
    {
        try
        {
            var intItemId = IntParser.Parse(request.ItemId, nameof(request.ItemId))!.Value;
            var uuid = Creds.GetAuthToken();

            await using var itemClient = Clients.GetItemClient(Creds.GetInstanceUrl());

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

            return new()
            {
                PriceLines = priceListResult.data.Select(x => new PriceLineResponse(x))
            };
        }
        finally
        {
            await Creds.Logout();
        }
    }

    [Action("Upload file", Description = "Upload a file to Plunet")]
    public async Task UploadFile([ActionParameter] UploadDocumentRequest request)
    {
        var intOrderId = IntParser.Parse(request.OrderId, nameof(request.OrderId))!.Value;
        var intFoldType = IntParser.Parse(request.FolderType, nameof(request.FolderType))!.Value;

        var uuid = Creds.GetAuthToken();

        await using var dataDocumentClient = Clients.GetDocumentClient(Creds.GetInstanceUrl());
        await dataDocumentClient.upload_DocumentAsync(uuid, intOrderId, intFoldType,
            request.File.Bytes, request.FilePath, request.File.Bytes.Length);

        await Creds.Logout();
    }

    [Action("Download file", Description = "Download a file from Plunet")]
    public async Task<FileResponse> DownloadFile([ActionParameter] DownloadDocumentRequest request)
    {
        var intOrderId = IntParser.Parse(request.OrderId, nameof(request.OrderId))!.Value;
        var intFoldType = IntParser.Parse(request.FolderType, nameof(request.FolderType))!.Value;

        var uuid = Creds.GetAuthToken();

        await using var dataDocumentClient = Clients.GetDocumentClient(Creds.GetInstanceUrl());
        var response =
            await dataDocumentClient.download_DocumentAsync(uuid, intOrderId, intFoldType,
                request.FilePathName);

        await Creds.Logout();

        return new(new(response.fileContent)
        {
            Name = response.filename,
            ContentType = MediaTypeNames.Application.Octet
        });
    }

    [Action("List files", Description = "List files from Plunet")]
    public async Task<ListFilesResponse> ListFiles([ActionParameter] ListFilesRequest request)
    {
        var intOrderId = IntParser.Parse(request.OrderId, nameof(request.OrderId))!.Value;
        var intFoldType = IntParser.Parse(request.FolderType, nameof(request.FolderType))!.Value;

        var uuid = Creds.GetAuthToken();

        await using var dataDocumentClient = Clients.GetDocumentClient(Creds.GetInstanceUrl());
        var response = await dataDocumentClient.getFileListAsync(uuid, intOrderId, intFoldType);

        await Creds.Logout();

        return new(response.data);
    }

    [Action("Delete order", Description = "Delete a Plunet order")]
    public async Task DeleteOrder([ActionParameter] [Display("Order ID")] string orderId)
    {
        var intOrderId = IntParser.Parse(orderId, nameof(orderId))!.Value;
        var uuid = Creds.GetAuthToken();

        await using var orderClient = Clients.GetOrderClient(Creds.GetInstanceUrl());
        await orderClient.deleteAsync(uuid, intOrderId);

        await Creds.Logout();
    }

    [Action("Update order", Description = "Update Plunet order")]
    public async Task UpdateOrder([ActionParameter] UpdateOrderRequest request)
    {
        var intOrderId = IntParser.Parse(request.OrderId, nameof(request.OrderId))!.Value;
        var uuid = Creds.GetAuthToken();

        var orderClient = Clients.GetOrderClient(Creds.GetInstanceUrl());
        await orderClient.updateAsync(uuid, new OrderIN
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

        await Creds.Logout();
    }

    [Action("Get language combinations for order", Description = "Get language combinations (source language - target " +
                                                                 "language) for order.")]
    public async Task<LanguageCombinationsResponse> GetLanguageCombinationsForOrder(
        [ActionParameter] [Display("Order ID")] [DataSource(typeof(OrderIdDataHandler))] string orderId)
    {
        var intOrderId = IntParser.Parse(orderId, nameof(orderId))!.Value;
        var uuid = Creds.GetAuthToken();

        await using var orderClient = Clients.GetOrderClient(Creds.GetInstanceUrl());
        var languageCombinations = await orderClient.getLanguageCombinationAsync(uuid, intOrderId);
        
        if (languageCombinations.data is null)
            throw new(languageCombinations.statusMessage);
        
        await using var client = Clients.GetAdminClient(Creds.GetInstanceUrl());
        var languages = await client.getAvailableLanguagesAsync(uuid, "en");

        var orderLanguageCombinations = languageCombinations.data
            .Select(combination => new { source = combination.Split(" - ")[0], target = combination.Split(" - ")[1] })
            .Select(combination =>
                new LanguageCombination(languages.data.First(l => l.name == combination.source).folderName,
                    languages.data.First(l => l.name == combination.target).folderName));
        
        await Creds.Logout();

        return new() { LanguageCombinations = orderLanguageCombinations };
    }
}