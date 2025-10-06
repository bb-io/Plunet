using Newtonsoft.Json;
using Tests.Plunet.Base;
using Apps.Plunet.Actions;
using Apps.Plunet.Models.Contacts;

namespace Tests.Plunet;

[TestClass]
public class ContactTests : TestBase
{
    [TestMethod]
    public async Task GetContact_WithValidInputs_IsSuccess()
    {
        var actions = new ContactActions(InvocationContext);

        var result = await actions.GetContactById(new ContactRequest { ContactId = "8" });
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task UpdateContact_WithValidInputs_IsSuccess()
    {
        // Arrange
        var actions = new ContactActions(InvocationContext);
        var contact = new ContactRequest { ContactId = "165" };
        var request = new CreateContactRequest 
        {
            CustomerId = "3",
            Email = "saul.goodman@bcs.com"
        };

        // Act
        var result = await actions.UpdateContact(contact, request);

        // Assert
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        Assert.IsNotNull(result);
    }
}
