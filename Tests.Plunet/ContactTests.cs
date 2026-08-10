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

        var result = await actions.GetContactById(new ContactRequest { ContactId = "22849" });
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task CreateContact_WithValidInputs_IsSuccess()
    {
        var actions = new ContactActions(InvocationContext);

        var result = await actions.CreateContact(new CreateContactRequest 
        { 
            CustomerId= "9",
            FirstName="Artem 4",
            LastName = "Testing 4",
            Email = "art@gmail.com",
            Phone = "+4112341111123411",
            MobilePhone = "<!& @6828fce71367962e &>",
            Status="0"
        });
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
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

    [TestMethod]
    public async Task GetCustomerContacts_IsSuccess()
    {
        // Arrange
        var actions = new ContactActions(InvocationContext);

        // Act
        var result = await actions.GetCustomerContacts(new Apps.Plunet.Models.Customer.CustomerRequest { CustomerId= "5434" });

        // Assert
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        Assert.IsNotNull(result);
    }
}
