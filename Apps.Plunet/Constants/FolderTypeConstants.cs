namespace Apps.Plunet.Constants;

public static class FolderTypeConstants
{
    public static readonly Dictionary<string, int> Request = new()
    {
        { "ref", 1 },
        { "source", 2 }
    };

    public static readonly Dictionary<string, int> Quote = new()
    {
        { "ref", 3 },
        { "source", 4 },
        { "!_Out", 9 },
        { "Final", 11 },
        { "Prm", 24 }
    };

    public static readonly Dictionary<string, int> QuoteItem = new()
    {
        { "Cat", 7 },
        { "source", 14 },
        { "ref", 15 }
    };

    public static readonly Dictionary<string, int> QuoteJob = new()
    {
        { "!_In", 20 },
        { "!_Out", 21 }
    };

    public static readonly Dictionary<string, int> Order = new()
    {
        { "ref", 5 },
        { "source", 6 },  
        { "!_Out", 10 },
        { "Final", 12 },
        { "Prm", 25 },
    };

    public static readonly Dictionary<string, int> OrderItem = new()
    {
        { "Cat", 8 },
        { "source", 16 },
        { "ref", 17 },
    };

    public static readonly Dictionary<string, int> OrderJob = new()
    {
        { "!_In", 22 },
        { "!_Out", 23 },
    };

    public const string ReceivableInvoiceKey = "Receivable";

    public const string PayableInvoiceKey = "Payable"; // incoming

    public static readonly Dictionary<string, int> Invoice = new()
    {
        { ReceivableInvoiceKey, 13 },
        { PayableInvoiceKey, 26 }
    };
    
    public const int Resource = 18;

    public const int Customer = 19;
}