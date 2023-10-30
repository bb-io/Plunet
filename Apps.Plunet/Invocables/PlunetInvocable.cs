using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Blackbird.Plugins.Plunet.Invocables;

public class PlunetInvocable : BaseInvocable
{
    protected AuthenticationCredentialsProvider[] Creds =>
        InvocationContext.AuthenticationCredentialsProviders.ToArray();
    
    public PlunetInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
    }
}