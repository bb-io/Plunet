using Apps.Plunet.Actions;
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
