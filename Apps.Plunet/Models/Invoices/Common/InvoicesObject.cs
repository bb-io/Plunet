using Newtonsoft.Json;

namespace Apps.Plunet.Models.Invoices.Common;

public class InvoicesObject
{
    public List<Invoice> Invoices { get; set; } = new();

    public Stream ToStream()
    {
        var stream = new MemoryStream();
        
        var json = JsonConvert.SerializeObject(this, Formatting.Indented);
        using var writer = new StreamWriter(stream);
        
        writer.Write(json);
        writer.Flush();
        stream.Position = 0;

        return stream;
    }
}