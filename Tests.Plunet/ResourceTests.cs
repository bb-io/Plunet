using Apps.Plunet.Actions;
using Apps.Plunet.Models.Order;
using Apps.Plunet.Models.Resource.Request;
using Blackbird.Applications.Sdk.Common.Invocation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Plunet.Base;

namespace Tests.Plunet;

[TestClass]
public class ResourceTests : TestBase
{
    public const string resourceName = "Henk";
    public const string updatedResourceName = "Piet";

    public const string deliveryCountry = "Albania";
    public const string updatedDeliveryCountry = "Germany";

    public const string invoiceStreet1 = "Dorpstraat";
    public const string updatedInvoiceStreet1 = "Kerkstraat";

    public const string contractNumber = "01249";
    public const string updatedContractNumber = "124910";

    [TestMethod]
    public async Task Get_resource_works()
    {
        var actions = new ResourceActions(InvocationContext);

        var result = await actions.GetResource("80");
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        Assert.AreEqual(resourceName, result.Name2);
    }

    [TestMethod]
    public async Task Create_resource_works()
    {
        var actions = new ResourceActions(InvocationContext);

        var result = await actions.CreateResource(new ResourceParameters 
        { 
            Name2 = resourceName, 
            DeliveryCountry = deliveryCountry, 
            InvoiceStreet = invoiceStreet1, 
            ContractNumber = contractNumber 
        });
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        Assert.AreEqual(resourceName, result.Name2);
        Assert.AreEqual(deliveryCountry, result.DeliveryAddress.Country);
        Assert.AreEqual(invoiceStreet1, result.InvoiceAddress.Street);
        Assert.AreEqual(contractNumber, result.Payment.ContractNumber);
    }

    [TestMethod]
    public async Task Update_resource_works()
    {
        var actions = new ResourceActions(InvocationContext);

        var result = await actions.CreateResource(new ResourceParameters 
        { 
            Name2 = resourceName, 
            DeliveryCountry = deliveryCountry, 
            InvoiceStreet = invoiceStreet1, 
            ContractNumber = contractNumber 
        });
        var updated = await actions.UpdateResource(new ResourceRequest { ResourceId = result.ResourceID }, new ResourceParameters 
        { 
            Name2 = updatedResourceName, 
            DeliveryCountry = updatedDeliveryCountry, 
            InvoiceStreet = updatedInvoiceStreet1, 
            ContractNumber = updatedContractNumber 
        });
        Console.WriteLine(JsonConvert.SerializeObject(updated, Formatting.Indented));
        Assert.AreEqual(updatedResourceName, updated.Name2);
        Assert.AreEqual(updatedDeliveryCountry, updated.DeliveryAddress.Country);
        Assert.AreEqual(updatedInvoiceStreet1, updated.InvoiceAddress.Street);
        Assert.AreEqual(updatedContractNumber, updated.Payment.ContractNumber);
    }
}
