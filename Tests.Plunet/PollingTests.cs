using Apps.Plunet.Models.Payable.Response;
using Apps.Plunet.Webhooks.Polling;
using Apps.Plunet.Webhooks.Polling.Memories;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Polling;

namespace Tests.Plunet;

[TestClass]
public class PollingTests
{
    private static readonly System.Reflection.MethodInfo OnPayableCreatedMethod =
        typeof(PollingList).GetMethod(nameof(PollingList.OnPayableCreated))!;

    [TestMethod]
    public void OnPayableCreated_IsAMultipleEventWithSingularName()
    {
        var pollingEvent = OnPayableCreatedMethod
            .GetCustomAttributes(typeof(PollingEventAttribute), false)
            .Cast<PollingEventAttribute>()
            .Single();

        var multipleEvents = OnPayableCreatedMethod
            .GetCustomAttributes(typeof(MultipleEventsAttribute), false);

        Assert.AreEqual("On payable created", pollingEvent.Name);
        Assert.HasCount(1, multipleEvents);
    }

    [TestMethod]
    public void OnPayableCreated_ReturnsPayablesDirectly()
    {
        var expectedReturnType = typeof(Task<
            PollingEventResponse<PayableMemory, List<PayableResponse>>>);

        Assert.AreEqual(expectedReturnType, OnPayableCreatedMethod.ReturnType);
    }
}
