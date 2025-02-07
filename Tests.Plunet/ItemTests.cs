﻿using Apps.Plunet.Actions;
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

        var result = await actions.SearchItems(new OptionalItemProjectRequest { ProjectId = "573", ProjectType = "3" }, new SearchItemsRequest { }, new OptionalCurrencyTypeRequest { });
        foreach(var item in result.Items)
        {
            Console.WriteLine($"{item.ItemId}: {item.BriefDescription}");
        }
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
}
