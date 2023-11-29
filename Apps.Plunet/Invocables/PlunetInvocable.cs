using Apps.Plunet.Constants;
using Apps.Plunet.Extensions;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Parsers;
using Blackbird.Plugins.Plunet.DataAdmin30Service;
using Blackbird.Plugins.Plunet.DataCustomer30Service;
using Blackbird.Plugins.Plunet.DataCustomerContact30Service;
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

    protected string Uuid => Creds.GetAuthToken();
    protected string Url => Creds.GetInstanceUrl();
    protected string Language => SystemConsts.Language;

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
    protected DataResourceAddress30Client ResourceAddressClient => new(DataResourceAddress30Client.EndpointConfiguration.DataResourceAddress30Port, Url.TrimEnd('/') + "/DataResourceAddress30");
    protected DataCustomerAddress30Client CustomerAddressClient => new(DataCustomerAddress30Client.EndpointConfiguration.DataCustomerAddress30Port, Url.TrimEnd('/') + "/DataCustomerAddress30");

    public PlunetInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    protected int ParseId(string? value)
    {
        return IntParser.Parse(value, nameof(value)) ?? -1;
    }
}