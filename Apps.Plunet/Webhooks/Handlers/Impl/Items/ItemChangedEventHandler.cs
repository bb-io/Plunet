﻿using Blackbird.Plugins.Plunet.Webhooks.CallbackClients;
using Blackbird.Plugins.Plunet.Webhooks.CallbackClients.Base;
using Blackbird.Plugins.Plunet.Webhooks.Handlers.Base;
using Blackbird.Plugins.Plunet.Webhooks.Models;

namespace Blackbird.Plugins.Plunet.Webhooks.Handlers.Impl.Items;

public class ItemChangedEventHandler : PlunetWebhookHandler
{
    protected override IPlunetWebhookClient Client => new ItemClient();
    protected override EventType EventType => EventType.StatusChanged;
}