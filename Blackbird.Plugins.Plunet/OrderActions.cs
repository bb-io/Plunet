using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Plugins.Plunet.DataAdmin30Service;
using Blackbird.Plugins.Plunet.DataDocument30Service;
using Blackbird.Plugins.Plunet.DataItem30Service;
using Blackbird.Plugins.Plunet.DataOrder30Service;
using Blackbird.Plugins.Plunet.Models;
using Blackbird.Plugins.Plunet.Models.Item;
using Blackbird.Plugins.Plunet.Models.Order;

namespace Blackbird.Plugins.Plunet;

[ActionList]
public class OrderActions
{
    [Action]
    public OrderResponse GetOrder(string userName, string password, AuthenticationCredentialsProvider authProvider,
        [ActionParameter] string token, [ActionParameter] int orderId)
    {
        using var orderClient = new DataOrder30Client();
        var orderResult = orderClient.getOrderObjectAsync(token, orderId).GetAwaiter().GetResult();
        var response = orderResult.data ?? null;
        return MapOrderResponse(response);
    }

    [Action]
    public CreateOrderResponse CreateOrder(string userName, string password,
        AuthenticationCredentialsProvider authProvider, [ActionParameter] CreateOrderRequest request)
    {
        using var orderClient = new DataOrder30Client();
        var orderIdResult = orderClient.insert2Async(request.UUID, new OrderIN
        {
            projectName = request.ProjectName,
            customerID = request.CustomerId,
            subject = request.ProjectName,
            projectManagerID = 7,
            orderDate = DateTime.Now,
            deliveryDeadline = request.Deadline,
        }).GetAwaiter().GetResult();
        return new CreateOrderResponse {OrderId = orderIdResult.data};
    }

    [Action]
    public CreateItemResponse AddItemToOrder(string userName, string password,
        AuthenticationCredentialsProvider authProvider, [ActionParameter] CreateItemRequest request)
    {
        using var itemClient = new DataItem30Client();
        var itemIdResult = itemClient.insert2Async(request.UUID, new ItemIN
        {
            briefDescription = request.ItemName,
            projectID = request.OrderId,
            totalPrice = request.TotalPrice,
            projectType = 3,
            deliveryDeadline = request.DeadlineDateTime,
        }).GetAwaiter().GetResult();
        return new CreateItemResponse {ItemId = itemIdResult.data};
    }


    [Action]
    public CreateOrderResponse CreateOrderBasedOnTemplate(string userName, string password,
        AuthenticationCredentialsProvider authProvider, [ActionParameter] CreateOrderRequest request,
        [ActionParameter] string templateName)
    {
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
        var templates = orderClient.getTemplateListAsync(request.UUID).GetAwaiter().GetResult();
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

        var orderIdResult = orderClient.insert_byTemplateAsync(request.UUID, order, template.templateID).GetAwaiter()
            .GetResult();
        return new CreateOrderResponse {OrderId = orderIdResult.data};
    }

    [Action]
    public AddLanguageCombinationResponse AddLanguageCombinationToOrder(string userName, string password,
        AuthenticationCredentialsProvider authProvider, [ActionParameter] AddLanguageCombinationRequest request)
    {
        using var orderClient = new DataOrder30Client();
        var langCombination =
            GetLanguageNamesCombinationByLanguageCodeIso(request.UUID, request.SourceLanguageCode,
                request.TargetLanguageCode);
        if (string.IsNullOrEmpty(langCombination.TargetLanguageName))
        {
            return new AddLanguageCombinationResponse();
        }

        var result = orderClient.addLanguageCombinationAsync(request.UUID, langCombination.SourceLanguageName,
            langCombination.TargetLanguageName, request.OrderId).GetAwaiter().GetResult();
        return new AddLanguageCombinationResponse {LanguageCombinationId = result.data};
    }

    [Action]
    public BaseResponse SetLanguageCombinationToItem(string userName, string password,
        AuthenticationCredentialsProvider authProvider, [ActionParameter] SetLanguageCombinationRequest request)
    {
        using var itemClient = new DataItem30Client();
        var response = itemClient
            .setLanguageCombinationIDAsync(request.UUID, request.LanguageCombinationId, 3, request.ItemId).GetAwaiter()
            .GetResult();
        return new BaseResponse
        {
            StatusCode = response.statusCode
        };
    }


    [Action]
    public PriceLineListResponse AddPriceLinesToItem(string userName, string password,
        AuthenticationCredentialsProvider authProvider, [ActionParameter] PriceLineRequest request)
    {
        using var itemClient = new DataItem30Client();
        var priceUnitListResult = itemClient.getPriceUnit_ListAsync(request.UUID, "en", "Translation").GetAwaiter()
            .GetResult();
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
            itemClient.insertPriceLineAsync(request.UUID, request.ItemId, 3, priceLine, priceUnit.description.Contains("New", StringComparison.OrdinalIgnoreCase)).GetAwaiter()
                .GetResult();
        }

        var priceListResult = itemClient.getPriceLine_ListAsync(request.UUID, request.ItemId, 3).GetAwaiter().GetResult();
        return new PriceLineListResponse {PriceLines = priceListResult.data.Select(MapPriceLineResponse)};
    }

    [Action]
    public BaseResponse UploadFile(string userName, string password,
        AuthenticationCredentialsProvider authProvider, [ActionParameter] UploadDocumentRequest request)
    {
        using var dataDocumentClient = new DataDocument30Client();
        var response = dataDocumentClient.upload_DocumentAsync(request.UUID, request.OrderId, request.FolderType,
            request.FileContentBytes, request.FilePath, request.FileSize).GetAwaiter().GetResult();
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