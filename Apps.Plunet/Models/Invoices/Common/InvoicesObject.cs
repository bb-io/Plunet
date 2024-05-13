using System.Text;
using Newtonsoft.Json;

namespace Apps.Plunet.Models.Invoices.Common;

public class InvoicesObject
{
    public List<Invoice> Invoices { get; set; } = new();

    public Stream ToStream()
    {
        var stream = new MemoryStream();
    
        var json = JsonConvert.SerializeObject(this, Formatting.Indented);
        using (var writer = new StreamWriter(stream, encoding: Encoding.UTF8, bufferSize: 1024, leaveOpen: true))
        {
            writer.Write(json);
            writer.Flush();
        }
    
        stream.Position = 0;
        return stream;
    }
}