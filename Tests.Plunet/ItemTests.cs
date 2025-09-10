using Apps.Plunet.Actions;
using Apps.Plunet.Models;
using Apps.Plunet.Models.Item;
using Apps.Plunet.Models.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Plunet.Base;

namespace Tests.Plunet;
[TestClass]
public class ItemTests : TestBase
{
    [TestMethod]
    public async Task Get_item_works()
    {
        var actions = new ItemActions(InvocationContext);

        var result = await actions.GetItem(new ProjectTypeRequest { ProjectType = "3" }, new GetItemRequest { ItemId = "406" }, new OptionalCurrencyTypeRequest { });
        Assert.IsNotNull(result.OrderId);
    }

    [TestMethod]
    public async Task Search_items_works()
    {
        var actions = new ItemActions(InvocationContext);

        var result = await actions.SearchItems(new OptionalItemProjectRequest { ProjectId = "573", ProjectType = "3" }, 
            new SearchItemsRequest {}, new OptionalCurrencyTypeRequest { }, null);
       
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);
        Console.WriteLine(json);
        Assert.IsTrue(result.Items.Count() > 0);
    }

    [TestMethod]
    public async Task GetLanguageCatCodeAsync_WithValidInputs_ShouldNotThrowError()
    {
        var actions = new ItemActions(InvocationContext);

        var result = await actions.GetLanguageCatCodeAsync(new()
        {
            LanguageName = "English (USA)",
            CatType = "1"
        });
        
        Console.WriteLine(result);
        Assert.IsNotNull(result);
    }



    [TestMethod]
    public async Task Create_item_priceline_works()
    {
        var actions = new ItemActions(InvocationContext);

        var result = await actions.CreateItemPriceline(new ProjectTypeRequest { ProjectType = "3" }, new GetItemRequest {ItemId= "43" }, new ItemPriceUnitRequest { Service= "Translation" ,PriceUnit="62"  },
            new PricelineRequest { Amount=155, UnitPrice=155, TaxType="3" });

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);
        Console.WriteLine(json);
        Assert.IsTrue(true);
    }

    [TestMethod]
    public async Task Update_item_priceline_works()
    {
        var actions = new ItemActions(InvocationContext);

        var result = await actions.UpdateItemPriceline(new ProjectTypeRequest { ProjectType = "3" }, new GetItemRequest { ItemId = "43" }, new ItemPriceUnitRequest { Service = "Translation", PriceUnit = "62" },
            new PricelineIdRequest { Id= "1234" },new PricelineRequest { Amount = 155, UnitPrice = 155, TaxType = "8" });

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);
        Console.WriteLine(json);
        Assert.IsTrue(true);
    }


}
