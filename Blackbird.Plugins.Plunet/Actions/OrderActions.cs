using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Plugins.Plunet.DataAdmin30Service;
using Blackbird.Plugins.Plunet.DataDocument30Service;
using Blackbird.Plugins.Plunet.DataItem30Service;
using Blackbird.Plugins.Plunet.DataOrder30Service;
using Blackbird.Plugins.Plunet.Extensions;
using Blackbird.Plugins.Plunet.Models;
using Blackbird.Plugins.Plunet.Models.Item;
using Blackbird.Plugins.Plunet.Models.Order;

namespace Blackbird.Plugins.Plunet.Actions;

[ActionList]
public class OrderActions
{
    [Action]
    public async Task<OrderResponse> GetOrder(IEnumerable<AuthenticationCredentialsProvider> authProviders, [ActionParameter] int orderId)
    {
        var uuid = authProviders.GetAuthToken();
        using var orderClient = new DataOrder30Client();
        var orderResult = await orderClient.getOrderObjectAsync(uuid, orderId);
        var response = orderResult.data ?? null;
        return MapOrderResponse(response);
    }

    [Action]
    public async Task<CreateOrderResponse> CreateOrder(IEnumerable<AuthenticationCredentialsProvider> authProviders, [ActionParameter] CreateOrderRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        using var orderClient = new DataOrder30Client();
        var orderIdResult = await orderClient.insert2Async(uuid, new OrderIN
        {
            projectName = request.ProjectName,
            customerID = request.CustomerId,
            subject = request.ProjectName,
            projectManagerID = 7,
            orderDate = DateTime.Now,
            deliveryDeadline = request.Deadline,
        });
        return new CreateOrderResponse {OrderId = orderIdResult.data};
    }

    [Action]
    public async Task<CreateItemResponse> AddItemToOrder(IEnumerable<AuthenticationCredentialsProvider> authProviders, [ActionParameter] CreateItemRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        using var itemClient = new DataItem30Client();
        var itemIdResult = await itemClient.insert2Async(uuid, new ItemIN
        {
            briefDescription = request.ItemName,
            projectID = request.OrderId,
            totalPrice = request.TotalPrice,
            projectType = 3,
            deliveryDeadline = request.DeadlineDateTime,
        });
        return new CreateItemResponse {ItemId = itemIdResult.data};
    }


    [Action]
    public async Task<CreateOrderResponse> CreateOrderBasedOnTemplate(IEnumerable<AuthenticationCredentialsProvider> authProviders,  [ActionParameter] CreateOrderRequest request,
        [ActionParameter] string templateName)
    {
        var uuid = authProviders.GetAuthToken();
        using var orderClient = new DataOrder30Client();
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
            return new CreateOrderResponse();
        }

        var template = templates.data.FirstOrDefault(t =>
            t.templateName.Contains(templateName, StringComparison.OrdinalIgnoreCase));
        if (template == null)
        {
            return new CreateOrderResponse();
        }

        var orderIdResult = await orderClient.insert_byTemplateAsync(uuid, order, template.templateID);
        return new CreateOrderResponse {OrderId = orderIdResult.data};
    }

    [Action]
    public async Task<AddLanguageCombinationResponse> AddLanguageCombinationToOrder(IEnumerable<AuthenticationCredentialsProvider> authProviders,  [ActionParameter] AddLanguageCombinationRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        using var orderClient = new DataOrder30Client();
        var langCombination =
            GetLanguageNamesCombinationByLanguageCodeIso(uuid, request.SourceLanguageCode,
                request.TargetLanguageCode);
        if (string.IsNullOrEmpty(langCombination.TargetLanguageName))
        {
            return new AddLanguageCombinationResponse();
        }

        var result = await orderClient.addLanguageCombinationAsync(uuid, langCombination.SourceLanguageName,
            langCombination.TargetLanguageName, request.OrderId);
        return new AddLanguageCombinationResponse {LanguageCombinationId = result.data};
    }

    [Action]
    public async Task<BaseResponse> SetLanguageCombinationToItem(IEnumerable<AuthenticationCredentialsProvider> authProviders,  [ActionParameter] SetLanguageCombinationRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        using var itemClient = new DataItem30Client();
        var response = await itemClient
            .setLanguageCombinationIDAsync(uuid, request.LanguageCombinationId, 3, request.ItemId);
        return new BaseResponse
        {
            StatusCode = response.statusCode
        };
    }


    [Action]
    public async Task<PriceLineListResponse> AddPriceLinesToItem(IEnumerable<AuthenticationCredentialsProvider> authProviders,  [ActionParameter] PriceLineRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        using var itemClient = new DataItem30Client();
        var priceUnitListResult = await itemClient.getPriceUnit_ListAsync(uuid, "en", "Translation");
        var priceUnits = priceUnitListResult.data.Where(x =>
            x.description.Contains("Words Translation", StringComparison.OrdinalIgnoreCase));
        if (priceUnits == null || !priceUnits.Any())
        {
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
        return new PriceLineListResponse {PriceLines = priceListResult.data.Select(MapPriceLineResponse)};
    }

    [Action]
    public async Task<BaseResponse> UploadFile(IEnumerable<AuthenticationCredentialsProvider> authProviders,  [ActionParameter] UploadDocumentRequest request)
    {
        var uuid = authProviders.GetAuthToken();
        using var dataDocumentClient = new DataDocument30Client();
        var response = await dataDocumentClient.upload_DocumentAsync(uuid, request.OrderId, request.FolderType,
            request.FileContentBytes, request.FilePath, request.FileSize);
        return new BaseResponse {StatusCode = response.Result.statusCode};
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