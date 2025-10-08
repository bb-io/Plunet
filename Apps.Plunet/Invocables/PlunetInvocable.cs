using Apps.Plunet.Api;
using Apps.Plunet.Constants;
using Apps.Plunet.DataOutgoingInvoice30Service;
using Apps.Plunet.Extensions;
using Apps.Plunet.Models;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Parsers;
using Blackbird.Plugins.Plunet.DataAdmin30Service;
using Blackbird.Plugins.Plunet.DataCustomer30Service;
using Blackbird.Plugins.Plunet.DataCustomerContact30Service;
using Blackbird.Plugins.Plunet.DataCustomFields30;
using Blackbird.Plugins.Plunet.DataDocument30Service;
using Blackbird.Plugins.Plunet.DataItem30Service;
using Blackbird.Plugins.Plunet.DataJob30Service;
using Blackbird.Plugins.Plunet.DataOrder30Service;
using Blackbird.Plugins.Plunet.DataPayable30Service;
using Blackbird.Plugins.Plunet.DataQuote30Service;
using Blackbird.Plugins.Plunet.DataRequest30Service;
using Blackbird.Plugins.Plunet.DataResource30Service;
using Blackbird.Plugins.Plunet.PlunetAPIService;
using DataCustomerAddress30Service;
using DataJobRound30Service;
using Blackbird.Plugins.Plunet.DataQualityManager30;
using DataResourceAddress30Service;
using System.ServiceModel;
using PropertyResult = Blackbird.Plugins.Plunet.DataCustomFields30.PropertyResult;
using Textmodule = Blackbird.Plugins.Plunet.DataCustomFields30.Textmodule;

namespace Apps.Plunet.Invocables;

public class PlunetInvocable : BaseInvocable
{
    protected AuthenticationCredentialsProvider[] Creds =>
        InvocationContext.AuthenticationCredentialsProviders.ToArray();

    protected string Uuid { get; set; }
    protected string Url => Creds.GetInstanceUrl();
    protected string Language => SystemConsts.Language;

    private Language[] _languages;

    protected async Task RefreshAuthToken()
    {
        Uuid = await AuthClient.loginAsync(Creds.GetUsername(), Creds.GetPassword());
    }

    protected PlunetAPIClient AuthClient => Clients.GetAuthClient(Creds.GetInstanceUrl());
    protected DataJobRound30Client JobRoundClient => Clients.GetJobRoundClient(Creds.GetInstanceUrl());
    protected DataCustomer30Client CustomerClient => Clients.GetCustomerClient(Creds.GetInstanceUrl());
    protected DataCustomerContact30Client ContactClient => Clients.GetContactClient(Creds.GetInstanceUrl());
    protected DataAdmin30Client AdminClient => Clients.GetAdminClient(Creds.GetInstanceUrl());
    protected DataDocument30Client DocumentClient => Clients.GetDocumentClient(Creds.GetInstanceUrl());
    protected DataItem30Client ItemClient => Clients.GetItemClient(Creds.GetInstanceUrl());
    protected DataOrder30Client OrderClient => Clients.GetOrderClient(Creds.GetInstanceUrl());
    protected DataPayable30Client PayableClient => Clients.GetPayableClient(Creds.GetInstanceUrl());
    protected DataResource30Client ResourceClient => Clients.GetResourceClient(Creds.GetInstanceUrl());
    protected DataRequest30Client RequestClient => Clients.GetRequestClient(Creds.GetInstanceUrl());
    protected DataQuote30Client QuoteClient =>ConfigureTimeout(Clients.GetQuoteClient(Creds.GetInstanceUrl()));
    protected DataJob30Client JobClient => Clients.GetJobClient(Creds.GetInstanceUrl());
    protected DataOutgoingInvoice30Client OutgoingInvoiceClient => Clients.GetOutgoingInvoiceClient(Creds.GetInstanceUrl());
    protected DataResourceAddress30Client ResourceAddressClient => Clients.GetResourceAddressClient(Creds.GetInstanceUrl());
    protected DataCustomerAddress30Client CustomerAddressClient => Clients.GetCustomerAddressClient(Creds.GetInstanceUrl());
    protected DataCustomFields30Client CustomFieldsClient => Clients.GetCustomFieldsClient(Creds.GetInstanceUrl());
    protected DataQualityManager30Client QualityManagerClient => Clients.GetQualityManagerClient(Creds.GetInstanceUrl());

    public PlunetInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        try
        {
            Uuid = Creds.GetAuthToken();
        }
        catch (Exception ex)
        {
            throw new PluginApplicationException(ex.Message);
        }
    }

    private static DataQuote30Client ConfigureTimeout(DataQuote30Client client)
    {
        if (client.Endpoint.Binding is BasicHttpBinding binding)
        {
            binding.SendTimeout = binding.ReceiveTimeout = TimeSpan.FromMinutes(5);
        }
        return client;
    }

    public async Task Logout()
    {
        await using var plunetApiClient = Clients.GetAuthClient(Url);
        await plunetApiClient.logoutAsync(Uuid);
    }

    public async Task<Callback[]> GetSubscribedWebhooks()
    {
        var res = await AdminClient.getListOfRegisteredCallbacksAsync(Uuid);
        return res.data;
    }

    protected int ParseId(string? value, int defaultValue = -1)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return defaultValue;
        }

        try
        {
            return IntParser.Parse(value, nameof(value)) ?? defaultValue;
        } catch(Exception e)
        {
            if (e.Message.Contains("should be an integer"))
                throw new PluginMisconfigurationException($"The given ID is not correct. It should be a number while we received: '{value}'");
            throw;
        }
        
    }

    protected async Task<Language[]> GetSystemLanguages()
    {
        if (_languages == null)
        {
            _languages = await ExecuteWithRetry(() => AdminClient.getAvailableLanguagesAsync(Uuid, Language));
        }

        return _languages;
    }

    protected async Task<IEnumerable<LanguageCombination>> ParseLanguageCombinations(IEnumerable<string> dashSeparatedStrings)
    {
        if (dashSeparatedStrings == null || !dashSeparatedStrings.Any())
            return new List<LanguageCombination>();

        try
        {
            var languages = await GetSystemLanguages();
            return dashSeparatedStrings
            .Select(combination =>
            {
                int separatorIndex = combination.IndexOf(" - ");
                if (separatorIndex == -1) return null;

                var source = combination.Substring(0, separatorIndex);
                var target = combination.Substring(separatorIndex + 3);

                return new { source, target };
            })
            .Where(x => x != null)
            .Select(combination =>
                new LanguageCombination(
                    languages.FirstOrDefault(l => l.name == combination.source)?.folderName,
                    languages.FirstOrDefault(l => l.name == combination.target)?.folderName
                ));
        }
        catch
        {
            return Enumerable.Empty<LanguageCombination>();
        }

    }

    protected async Task<List<string>> GetLanguageCodes(IEnumerable<string> languageName)
    {
        var languages = new List<string>();
        foreach (var name in languageName)
        {
            var language = await GetLanguageCode(name);
            languages.Add(language);
        }

        return languages;
    }

    protected async Task<string> GetLanguageCode(string languageName)
    {
        var languages = await GetSystemLanguages();
        var language = languages.FirstOrDefault(x => x.name.Equals(languageName, StringComparison.OrdinalIgnoreCase));

        if (language == null)
            throw new PluginApplicationException($"Language {languageName} could not be found.");

        return language.folderName;
    }

    protected async Task<Language> GetLanguageFromIsoOrFolderOrName(string isOrFolderOrName)
    {
        var languages = await GetSystemLanguages();

        var language = languages.FirstOrDefault(x =>
                                 x.isoCode.Equals(isOrFolderOrName, StringComparison.OrdinalIgnoreCase) ||
                                 x.folderName.Equals(isOrFolderOrName, StringComparison.OrdinalIgnoreCase) ||
                                 x.name.Equals(isOrFolderOrName, StringComparison.OrdinalIgnoreCase));

        if (language == null)
            throw new PluginApplicationException($"Language {isOrFolderOrName} could not be found.");

        return language;
    }

    private async Task<T?> ThrowOrHandleRetries<T>(Func<Task<T>> func, Func<T, string> getStatusMessageFunc, Func<T, bool> hasDataFunc, bool acceptNull = false, int maxRetries = 10, int delay = 1000)
    {
        var attempts = 0;
        while (true)
        {
            try
            {
                var result = await func();
                var statusMessage = getStatusMessageFunc(result);
                if (statusMessage == ApiResponses.Ok)
                {
                    return result;
                }

                if ((statusMessage.Contains("session-UUID used is invalid") || statusMessage.Contains("Connection timed out")) && attempts < maxRetries)
                {
                    await Task.Delay(delay);
                    await RefreshAuthToken();
                    attempts++;
                    continue;
                }

                if (!hasDataFunc(result) && acceptNull)
                {
                    return default;
                }

                throw new PluginApplicationException(statusMessage);
            }
            catch (Exception ex) when (IsTimeout(ex))
            {
                throw new PluginApplicationException(
                    $"Connection to Plunet timed out. Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new PluginApplicationException(ex.Message);
            }
        }
    }

    private static bool IsTimeout(Exception ex)
    {
        for (var e = ex; e != null; e = e.InnerException)
        {
            if (e is TimeoutException ||
                e is TaskCanceledException ||
                e.Message.Contains("timed out", StringComparison.OrdinalIgnoreCase) ||
                e.Message.Contains("Connection timed out", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    // Custom fields service
    protected async Task ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataCustomFields30.Result>> func, int maxRetries = 10, int delay = 1000)
        => await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => false, false, maxRetries, delay);
    protected async Task<Textmodule?> ExecuteWithRetryAcceptNull(Func<Task<TextmoduleResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;
    protected async Task ExecuteWithRetry(Func<Task<setPropertyValueListResponse>> func, int maxRetries = 10, int delay = 1000)
        => await ThrowOrHandleRetries(func, (x) => x.Result.statusMessage, (x) => false, false, maxRetries, delay);
    protected async Task<Blackbird.Plugins.Plunet.DataCustomFields30.Property?> ExecuteWithRetryAcceptNull(Func<Task<PropertyResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;
    protected async Task<string?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataCustomFields30.StringResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;


    // Job service
    protected async Task ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataJob30Service.Result>> func, int maxRetries = 10, int delay = 1000)
        => await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => false, false, maxRetries, delay);
    protected async Task<Job> ExecuteWithRetry(Func<Task<JobResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<JobMetric> ExecuteWithRetry(Func<Task<JobMetricResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<TrackingTimeList> ExecuteWithRetry(Func<Task<JobTrackingTimeResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<Blackbird.Plugins.Plunet.DataJob30Service.PriceLine[]?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataJob30Service.PriceLineListResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;
    protected async Task<Blackbird.Plugins.Plunet.DataJob30Service.PriceUnit> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataJob30Service.PriceUnitResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<Blackbird.Plugins.Plunet.DataJob30Service.PriceLine?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataJob30Service.PriceLineResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;
    protected async Task<Job[]> ExecuteWithRetry(Func<Task<JobListResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<int> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataJob30Service.IntegerResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != 0, false, maxRetries, delay))!.data;
    protected async Task<int?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataJob30Service.IntegerResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != 0, true, maxRetries, delay))?.data;
    protected async Task<string?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataJob30Service.StringResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;
    protected async Task<string> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataJob30Service.StringResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<DateTime> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataJob30Service.DateResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => false, false, maxRetries, delay))!.data;


    // Job round service
    protected async Task ExecuteWithRetry(Func<Task<DataJobRound30Service.Result>> func, int maxRetries = 10, int delay = 1000)
        => await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => false, false, maxRetries, delay);
    protected async Task<JobRound> ExecuteWithRetry(Func<Task<JobRoundResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<int?[]> ExecuteWithRetry(Func<Task<DataJobRound30Service.IntegerArrayResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<int?[]?> ExecuteWithRetryAcceptNull(Func<Task<DataJobRound30Service.IntegerArrayResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;
    protected async Task<int> ExecuteWithRetry(Func<Task<DataJobRound30Service.IntegerResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != 0, false, maxRetries, delay))!.data;
    protected async Task<int?> ExecuteWithRetryAcceptNull(Func<Task<DataJobRound30Service.IntegerResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != 0, true, maxRetries, delay))?.data;


    // Quote service
    protected async Task ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataQuote30Service.Result>> func, int maxRetries = 10, int delay = 1000)
        => await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => false, false, maxRetries, delay);
    protected async Task<Quote> ExecuteWithRetry(Func<Task<QuoteResult>> func, int maxRetries = 10, int delay = 1000)
     => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<int?[]> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataQuote30Service.IntegerArrayResult>> func, int maxRetries = 10, int delay = 1000)
    => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<int?[]?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataQuote30Service.IntegerArrayResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;
    protected async Task<int> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataQuote30Service.IntegerResult>> func, int maxRetries = 10, int delay = 1000)
    => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != 0, false, maxRetries, delay))!.data;
    protected async Task<int?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataQuote30Service.IntegerResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != 0, true, maxRetries, delay))?.data;
    protected async Task<string?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataQuote30Service.StringResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;


    // Request service
    protected async Task ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataRequest30Service.Result>> func, int maxRetries = 10, int delay = 1000)
        => await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => false, false, maxRetries, delay);
    protected async Task<Request> ExecuteWithRetry(Func<Task<RequestResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<int?[]> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataRequest30Service.IntegerArrayResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<int?[]?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataRequest30Service.IntegerArrayResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;
    protected async Task<int> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataRequest30Service.IntegerResult>> func, int maxRetries = 10, int delay = 1000)
    => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != 0, false, maxRetries, delay))!.data;
    protected async Task<int?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataRequest30Service.IntegerResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != 0, true, maxRetries, delay))?.data;
    protected async Task<string> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataRequest30Service.StringResult>> func, int maxRetries = 10, int delay = 1000)
    => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;


    // Resource service
    protected async Task ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataResource30Service.Result>> func, int maxRetries = 10, int delay = 1000)
    => await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => false, false, maxRetries, delay);
    protected async Task<Resource> ExecuteWithRetry(Func<Task<ResourceResult>> func, int maxRetries = 10, int delay = 1000)
    => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<Blackbird.Plugins.Plunet.DataResource30Service.PaymentInfo> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataResource30Service.PaymentInfoResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<int?[]> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataResource30Service.IntegerArrayResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<int?[]?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataResource30Service.IntegerArrayResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;
    protected async Task<int> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataResource30Service.IntegerResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != 0, false, maxRetries, delay))!.data;
    protected async Task<int?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataResource30Service.IntegerResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != 0, true, maxRetries, delay))?.data;


    // Resource address service
    protected async Task ExecuteWithRetry(Func<Task<DataResourceAddress30Service.Result>> func, int maxRetries = 10, int delay = 1000)
        => await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => false, false, maxRetries, delay);
    protected async Task<int> ExecuteWithRetry(Func<Task<DataResourceAddress30Service.IntegerResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != 0, false, maxRetries, delay))!.data;
    protected async Task<int?[]> ExecuteWithRetry(Func<Task<DataResourceAddress30Service.IntegerArrayResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<string?> ExecuteWithRetryAcceptNull(Func<Task<DataResourceAddress30Service.StringResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;


    // Admin service
    protected async Task<Blackbird.Plugins.Plunet.DataAdmin30Service.StringResult?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataAdmin30Service.StringResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay));
    protected async Task<Callback[]?> ExecuteWithRetryAcceptNull(Func<Task<CallbackListResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;
    protected async Task<Language[]> ExecuteWithRetry(Func<Task<LanguageListResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;


    // Customer service
    protected async Task ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataCustomer30Service.Result>> func, int maxRetries = 10, int delay = 1000)
        => await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => false, false, maxRetries, delay);
    protected async Task<Customer> ExecuteWithRetry(Func<Task<CustomerResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<Blackbird.Plugins.Plunet.DataCustomer30Service.PaymentInfo> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataCustomer30Service.PaymentInfoResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<int?[]?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataCustomer30Service.IntegerArrayResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;
    protected async Task<int> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataCustomer30Service.IntegerResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != 0, false, maxRetries, delay))!.data;
    protected async Task<int?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataCustomer30Service.IntegerResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != 0, true, maxRetries, delay))?.data;
    protected async Task<string?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataCustomer30Service.StringResult>> func, int maxRetries = 10, int delay = 1000)
    => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;


    // Customer address service
    protected async Task<int> ExecuteWithRetry(Func<Task<DataCustomerAddress30Service.IntegerResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != 0, false, maxRetries, delay))!.data;
    protected async Task<int?[]?> ExecuteWithRetryAcceptNull(Func<Task<DataCustomerAddress30Service.IntegerArrayResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;
    protected async Task<string?> ExecuteWithRetryAcceptNull(Func<Task<DataCustomerAddress30Service.StringResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;
    protected async Task ExecuteWithRetry(Func<Task<DataCustomerAddress30Service.Result>> func, int maxRetries = 10, int delay = 1000)
        => await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => false, false, maxRetries, delay);

    // Customer contact service
    protected async Task ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataCustomerContact30Service.Result>> func, int maxRetries = 10, int delay = 1000)
        => await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => false, false, maxRetries, delay);
    protected async Task<CustomerContact> ExecuteWithRetry(Func<Task<CustomerContactResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<CustomerContact[]?> ExecuteWithRetryAcceptNull(Func<Task<CustomerContactListResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;
    protected async Task<int?[]?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataCustomerContact30Service.IntegerArrayResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;
    protected async Task<int?[]> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataCustomerContact30Service.IntegerArrayResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<int> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataCustomerContact30Service.IntegerResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != 0, false, maxRetries, delay))!.data;
    protected async Task<int?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataCustomerContact30Service.IntegerResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != 0, true, maxRetries, delay))?.data;


    // Document service
    protected async Task ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataDocument30Service.Result>> func, int maxRetries = 10, int delay = 1000)
        => await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => false, false, maxRetries, delay);
    protected async Task ExecuteWithRetry(Func<Task<upload_DocumentResponse>> func, int maxRetries = 10, int delay = 1000)
        => await ThrowOrHandleRetries(func, (x) => x.Result.statusMessage, (x) => false, false, maxRetries, delay);
    protected async Task<FileResult> ExecuteWithRetry(Func<Task<FileResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.fileContent != null, false, maxRetries, delay))!;
    protected async Task<string[]?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataDocument30Service.StringArrayResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;


    // Item service
    protected async Task ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataItem30Service.Result>> func, int maxRetries = 10, int delay = 1000)
        => await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => false, false, maxRetries, delay);
    protected async Task<Item> ExecuteWithRetry(Func<Task<ItemResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<Item[]?> ExecuteWithRetryAcceptNull(Func<Task<ItemListResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;
    protected async Task<Blackbird.Plugins.Plunet.DataItem30Service.PriceLine[]?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataItem30Service.PriceLineListResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;
    protected async Task<Blackbird.Plugins.Plunet.DataItem30Service.PriceUnit> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataItem30Service.PriceUnitResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<Blackbird.Plugins.Plunet.DataItem30Service.PriceLine?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataItem30Service.PriceLineResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;
    protected async Task<int?[]> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataItem30Service.IntegerArrayResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<int> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataItem30Service.IntegerResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != 0, false, maxRetries, delay))!.data;
    protected async Task<int?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataItem30Service.IntegerResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != 0, true, maxRetries, delay))?.data;
    protected async Task<string?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataItem30Service.StringResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;
    protected async Task<string[]> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataItem30Service.StringArrayResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<string[]?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataItem30Service.StringArrayResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;
    protected async Task<Blackbird.Plugins.Plunet.DataItem30Service.setCatReport2Response> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataItem30Service.setCatReport2Response>> func, int maxRetries = 10, int delay = 1000)
        => await ThrowOrHandleRetries(func, (x) => x.Result.statusMessage, (x) => false, false, maxRetries, delay);


    // Order service

    protected async Task ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataOrder30Service.Result>> func, int maxRetries = 10, int delay = 1000)
        => await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => false, false, maxRetries, delay);
    protected async Task<Order> ExecuteWithRetry(Func<Task<OrderResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<int?[]> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataOrder30Service.IntegerArrayResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<int?[]?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataOrder30Service.IntegerArrayResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;
    protected async Task<string[]> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataOrder30Service.StringArrayResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<string[]?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataOrder30Service.StringArrayResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;
    protected async Task<int> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataOrder30Service.IntegerResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != 0, false, maxRetries, delay))!.data;
    protected async Task<int?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataOrder30Service.IntegerResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != 0, true, maxRetries, delay))?.data;
    protected async Task<string?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataOrder30Service.StringResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;


    // Invoice service
    protected async Task<Invoice> ExecuteWithRetry(Func<Task<InvoiceResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<DataOutgoingInvoice30Service.PriceLine[]> ExecuteWithRetry(Func<Task<DataOutgoingInvoice30Service.PriceLineListResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<InvoiceItem[]?> ExecuteWithRetryAcceptNull(Func<Task<InvoiceItemResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;
    protected async Task ExecuteWithRetry(Func<Task<DataOutgoingInvoice30Service.Result>> func, int maxRetries = 10, int delay = 1000)
        => await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => false, false, maxRetries, delay);
    protected async Task<int?[]> ExecuteWithRetry(Func<Task<DataOutgoingInvoice30Service.IntegerArrayResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<int?[]?> ExecuteWithRetryAcceptNull(Func<Task<DataOutgoingInvoice30Service.IntegerArrayResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;
    protected async Task<int> ExecuteWithRetry(Func<Task<DataOutgoingInvoice30Service.IntegerResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != 0, false, maxRetries, delay))!.data;
    protected async Task<int?> ExecuteWithRetryAcceptNull(Func<Task<DataOutgoingInvoice30Service.IntegerResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != 0, true, maxRetries, delay))?.data;
    protected async Task<string?> ExecuteWithRetryAcceptNull(Func<Task<DataOutgoingInvoice30Service.StringResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;


    // Payable service
    protected async Task<PayableItem[]> ExecuteWithRetry(Func<Task<PayableItemResultList>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataPayable30Service.Result>> func, int maxRetries = 10, int delay = 1000)
        => await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => false, false, maxRetries, delay);
    protected async Task<int?[]> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataPayable30Service.IntegerArrayResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<int?[]?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataPayable30Service.IntegerArrayResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;
    protected async Task<int> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataPayable30Service.IntegerResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != 0, false, maxRetries, delay))!.data;
    protected async Task<int?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataPayable30Service.IntegerResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != 0, true, maxRetries, delay))?.data;
    protected async Task<string?> ExecuteWithRetryAcceptNull(Func<Task<Blackbird.Plugins.Plunet.DataPayable30Service.StringResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;
    protected async Task<double> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataPayable30Service.DoubleResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != 0, false, maxRetries, delay))!.data;
    protected async Task<DateTime> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataPayable30Service.DateResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => false, false, maxRetries, delay))!.data;
    protected async Task<bool> ExecuteWithRetry(Func<Task<Blackbird.Plugins.Plunet.DataPayable30Service.BooleanResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => false, false, maxRetries, delay))!.data;

    // Quality Manager service
    protected async Task<JobQuality> ExecuteWithRetry(Func<Task<JobQualityResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, false, maxRetries, delay))!.data;
    protected async Task<JobQuality?> ExecuteWithRetryAcceptNull(Func<Task<JobQualityResult>> func, int maxRetries = 10, int delay = 1000)
        => (await ThrowOrHandleRetries(func, (x) => x.statusMessage, (x) => x.data != null, true, maxRetries, delay))?.data;

}