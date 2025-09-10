using Apps.Plunet.Actions;
using Apps.Plunet.Models.Invoices;
using Tests.Plunet.Base;

namespace Tests.Plunet
{
    [TestClass]
    public class InvoicesTests : TestBase
    {
        [TestMethod]
        public async Task Search_Invoices_IsSuccess()
        {
            var action = new InvoiceActions(InvocationContext, FileManager);
            var response = await action.SearchInvoices(new SearchInvoicesRequest
            {
                OnlyReturnIds = true,
                Limit = 10
            });
            Assert.IsNotNull(response);
            var options = new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            };
            string json = System.Text.Json.JsonSerializer.Serialize(response, options);
            Console.WriteLine(json);
        }
    }
}
