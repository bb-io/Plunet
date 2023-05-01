using System.Xml.Serialization;

namespace Blackbird.Plugins.Plunet.Webhooks.Models;

[XmlRoot(Namespace = "http://API.Integration/")]
public class JobCallback : ITriggerableCallback
{
    [XmlElement(Namespace = "")]
    public long JobID { get; set; }

    [XmlElement(Namespace = "")]
    public long ProjectType { get; set; } // 1 = Quote, 3 = Order

    [XmlElement(Namespace = "")]
    public long EventType { get; set; }

    [XmlElement(Namespace = "")]
    public string Authenticationcode { get; set; }

    public TriggerContent GetTriggerContent() { return new TriggerContent() { ID = JobID }; }
}