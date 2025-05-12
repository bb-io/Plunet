using Apps.Plunet.Actions;
using Apps.Plunet.Models.Customer;
using Apps.Plunet.Models.CustomProperties;
using System.Text.Json;
using Tests.Plunet.Base;

namespace Tests.Plunet
{
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
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string json = JsonSerializer.Serialize(response, options);
            Console.WriteLine(json);
        }

        [TestMethod]
        public async Task GetTextModule_IsSuccess()
        {
            var action = new CustomPropertyActions(InvocationContext);
            var response = await action.GetTextmodule(new TextModuleRequest 
            {
                Flag = "[ETW]",
                UsageArea = "11",
                MainId = "74"
            });
            Assert.IsNotNull(response);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string json = JsonSerializer.Serialize(response, options);
            Console.WriteLine(json);
        }
    }
}
