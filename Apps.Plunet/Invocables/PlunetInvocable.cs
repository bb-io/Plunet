using Apps.Plunet.Api;
using Apps.Plunet.Constants;
using Apps.Plunet.DataOutgoingInvoice30Service;
using Apps.Plunet.Extensions;
using Apps.Plunet.Models;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
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
using DataResourceAddress30Service;

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
    protected DataQuote30Client QuoteClient => Clients.GetQuoteClient(Creds.GetInstanceUrl());
    protected DataJob30Client JobClient => Clients.GetJobClient(Creds.GetInstanceUrl());
    protected DataOutgoingInvoice30Client OutgoingInvoiceClient => Clients.GetOutgoingInvoiceClient(Creds.GetInstanceUrl());
    protected DataResourceAddress30Client ResourceAddressClient => Clients.GetResourceAddressClient(Creds.GetInstanceUrl());
    protected DataCustomerAddress30Client CustomerAddressClient => Clients.GetCustomerAddressClient(Creds.GetInstanceUrl());
    protected DataCustomFields30Client CustomFieldsClient => Clients.GetCustomFieldsClient(Creds.GetInstanceUrl());

    public PlunetInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Uuid = Creds.GetAuthToken();
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

    protected int ParseId(string? value)
    {
        return IntParser.Parse(value, nameof(value)) ?? -1;
    }

    protected async Task<Language[]> GetSystemLanguages()
    {
        if (_languages == null)
        {
            var response = await ExecuteWithRetry<LanguageListResult>(async () => await AdminClient.getAvailableLanguagesAsync(Uuid, Language));

            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);

            _languages = response.data;
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
            .Select(combination => new { source = combination.Split(" - ")[0], target = combination.Split(" - ")[1] })
            .Select(combination =>
                new LanguageCombination(languages.First(l => l.name == combination.source).folderName,
                    languages.First(l => l.name == combination.target).folderName));
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
            throw new($"Language {languageName} could not be found in your Plunet instance");

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
            throw new($"Language {isOrFolderOrName} could not be found in your Plunet instance");

        return language;
    }
    
    private async Task<T> ExecuteWithRetry<T>(Func<Task<Blackbird.Plugins.Plunet.DataAdmin30Service.Result>> func, int maxRetries = 10, int delay = 1000)
        where T : Blackbird.Plugins.Plunet.DataAdmin30Service.Result
    {
        var attempts = 0;
        while (true)
        {
            var result = await func();
            
            if(result.statusMessage == ApiResponses.Ok)
            {
                return (T)result;
            }
            
            if(result.statusMessage.Contains("session-UUID used is invalid") && attempts < maxRetries)
            {
                await Task.Delay(delay);
                await RefreshAuthToken();
                attempts++;
                continue;
            }

            return (T)result;
        }
    }
}