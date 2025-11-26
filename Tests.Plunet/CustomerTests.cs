using Apps.Plunet.Actions;
using Apps.Plunet.Models.Customer;
using Apps.Plunet.Models.CustomProperties;
using Newtonsoft.Json;
using Tests.Plunet.Base;

namespace Tests.Plunet;

[TestClass]
public class CustomerTests : TestBase
{
    [TestMethod]
    public async Task GetCustomer_IsSuccess()
    {
        var action = new CustomerActions(InvocationContext);
        var response = await action.GetCustomerById(new CustomerRequest
        {
            CustomerId = "1"
        });

        Assert.IsNotNull(response);
        Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
    }

    [TestMethod]
    public async Task SearchCustomers_IsSuccess()
    {
        var action = new CustomerActions(InvocationContext);
        var response = await action.SearchCustomers(new SearchCustomerRequest
        {
            //OnlyReturnIds = true,
            Limit = 10
        });

        Assert.IsNotNull(response);
        Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
    }
}
