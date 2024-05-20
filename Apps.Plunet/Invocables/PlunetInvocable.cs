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

    protected PlunetAPIClient AuthClient => new(PlunetAPIClient.EndpointConfiguration.PlunetAPIPort, Url.TrimEnd('/') + "/PlunetAPI");
    protected DataCustomer30Client CustomerClient => new(DataCustomer30Client.EndpointConfiguration.DataCustomer30Port, Url.TrimEnd('/') + "/DataCustomer30");
    protected DataCustomerContact30Client ContactClient => new(DataCustomerContact30Client.EndpointConfiguration.DataCustomerContact30Port, Url.TrimEnd('/') + "/DataCustomerContact30");
    protected DataAdmin30Client AdminClient => new(DataAdmin30Client.EndpointConfiguration.DataAdmin30Port, Url.TrimEnd('/') + "/DataAdmin30");
    protected DataDocument30Client DocumentClient => new(DataDocument30Client.EndpointConfiguration.DataDocument30Port, Url.TrimEnd('/') + "/DataDocument30");
    protected DataItem30Client ItemClient => new(DataItem30Client.EndpointConfiguration.DataItem30Port, Url.TrimEnd('/') + "/DataItem30");
    protected DataOrder30Client OrderClient => new(DataOrder30Client.EndpointConfiguration.DataOrder30Port, Url.TrimEnd('/') + "/DataOrder30");
    protected DataPayable30Client PayableClient => new(DataPayable30Client.EndpointConfiguration.DataPayable30Port, Url.TrimEnd('/') + "/DataPayable30");
    protected DataResource30Client ResourceClient => new(DataResource30Client.EndpointConfiguration.DataResource30Port, Url.TrimEnd('/') + "/DataResource30");
    protected DataRequest30Client RequestClient => new(DataRequest30Client.EndpointConfiguration.DataRequest30Port, Url.TrimEnd('/') + "/DataRequest30");
    protected DataQuote30Client QuoteClient => new(DataQuote30Client.EndpointConfiguration.DataQuote30Port, Url.TrimEnd('/') + "/DataQuote30");
    protected DataJob30Client JobClient => new(DataJob30Client.EndpointConfiguration.DataJob30Port, Url.TrimEnd('/') + "/DataJob30");
    protected DataOutgoingInvoice30Client OutgoingInvoiceClient => new(DataOutgoingInvoice30Client.EndpointConfiguration.DataOutgoingInvoice30Port, Url.TrimEnd('/') + "/DataOutgoingInvoice30");
    protected DataResourceAddress30Client ResourceAddressClient => new(DataResourceAddress30Client.EndpointConfiguration.DataResourceAddress30Port, Url.TrimEnd('/') + "/DataResourceAddress30");
    protected DataCustomerAddress30Client CustomerAddressClient => new(DataCustomerAddress30Client.EndpointConfiguration.DataCustomerAddress30Port, Url.TrimEnd('/') + "/DataCustomerAddress30");
    protected DataCustomFields30Client CustomFieldsClient => new(DataCustomFields30Client.EndpointConfiguration.DataCustomFields30Port, Url.TrimEnd('/') + "/DataCustomFields30");

    public PlunetInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Uuid = Creds.GetAuthToken();
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
        if (dashSeparatedStrings == null)
            return new List<LanguageCombination>();

        var languages = await GetSystemLanguages();
        return dashSeparatedStrings
            .Select(combination => new { source = combination.Split(" - ")[0], target = combination.Split(" - ")[1] })
            .Select(combination =>
                new LanguageCombination(languages.First(l => l.name == combination.source).folderName,
                    languages.First(l => l.name == combination.target).folderName));
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