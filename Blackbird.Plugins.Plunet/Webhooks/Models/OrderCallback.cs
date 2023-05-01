using System.Xml.Serialization;

namespace Blackbird.Plugins.Plunet.Webhooks.Models;

[XmlRoot(Namespace = "http://API.Integration/")]
public class ItemCallback : ITriggerableCallback
{
    [XmlElement(Namespace = "")]
    public long ItemID { get; set; }
    
    [XmlElement(Namespace = "")]
    public long EventType { get; set; }

    [XmlElement(Namespace = "")]
    public string Authenticationcode { get; set; }

    public TriggerContent GetTriggerContent() { return new TriggerContent() { ID = ItemID }; }
}