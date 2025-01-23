using Apps.Plunet.Actions;
using Apps.Plunet.Models.Order;
using Apps.Plunet.Models.Quote.Request;
using Apps.Plunet.Models.Resource.Request;
using Blackbird.Applications.Sdk.Common.Invocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Plunet.Base;

namespace Tests.Plunet;

[TestClass]
public class QuoteTests : TestBase
{
    public const string quoteName = "Test quote #1";
    public const string updatedQuoteName = "Updated test quote #1";

    [TestMethod]
    public async Task Update_quote_works()
    {
        var actions = new QuoteActions(InvocationContext);

        var result = await actions.CreateQuote(new CreateQuoteRequest { ProjectName = quoteName, Status = "8" }, new QuoteTemplateRequest { });
        var updated = await actions.UpdateQuote(new GetQuoteRequest { QuoteId = result.QuoteId }, new CreateQuoteRequest { ProjectName = updatedQuoteName });
        Assert.AreEqual(updatedQuoteName, updated.ProjectName);
    }
}
