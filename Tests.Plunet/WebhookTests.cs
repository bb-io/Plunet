using Apps.Plunet.Actions;
using Apps.Plunet.Webhooks.Handlers.Impl.Customers;
using Tests.Plunet.Base;

namespace Tests.Plunet;

[TestClass]
public class WebhookTests : TestBase
{
    [TestMethod]
    public async Task Sure()
    {
        var handler = new CustomerCreatedEventHandler(InvocationContext);

        await handler.UnsubscribeAsync(InvocationContext.AuthenticationCredentialsProviders, new Dictionary<string, string> { });
    }
}
