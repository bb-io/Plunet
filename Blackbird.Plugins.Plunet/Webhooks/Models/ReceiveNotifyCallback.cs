using System.Xml.Serialization;

namespace Blackbird.Plugins.Plunet.Webhooks.Models;

[XmlRoot(Namespace = "http://API.Integration/")]
public class ReceiveNotifyCallback
{
    [XmlElement(Namespace = "")]
    public long OrderID { get; set; }
    
    [XmlElement(Namespace = "")]
    public long EventType { get; set; }

    [XmlElement(Namespace = "")]
    public string Authenticationcode { get; set; }
}