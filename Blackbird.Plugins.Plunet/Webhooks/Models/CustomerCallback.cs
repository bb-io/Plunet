using System.Xml.Serialization;

namespace Blackbird.Plugins.Plunet.Webhooks.Models;

[XmlRoot(Namespace = "http://API.Integration/")]
public class CustomerCallback : ITriggerableCallback
{
    [XmlElement(Namespace = "")]
    public long CustomerID { get; set; }
    
    [XmlElement(Namespace = "")]
    public long EventType { get; set; }

    [XmlElement(Namespace = "")]
    public string Authenticationcode { get; set; }

    public TriggerContent GetTriggerContent() { return new TriggerContent() { ID = CustomerID }; }
}