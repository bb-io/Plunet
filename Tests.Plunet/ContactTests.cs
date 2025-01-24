using Apps.Plunet.Actions;
using Apps.Plunet.Connections;
using Apps.Plunet.Models.Order;
using Blackbird.Applications.Sdk.Common.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Plunet.Base;

namespace Tests.Plunet;

[TestClass]
public class ContactTests : TestBase
{
    [TestMethod]
    public async Task Get_contact_works()
    {
        var actions = new ContactActions(InvocationContext);

        var result = await actions.GetContactById(new Apps.Plunet.Models.Contacts.ContactRequest { ContactId = "8" });
        Assert.IsNotNull(result);
    }
}
