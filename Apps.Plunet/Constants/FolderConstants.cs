namespace Apps.Plunet.Constants;

public static class FolderConstants
{
    public static class VirtualRoots
    {
        public const string Home = "Root";
        public const string Request = "v:request";
        public const string Quote = "v:quote";
        public const string Order = "v:order";
        public const string Invoice = "v:invoice";
        public const string Resource = "v:resource";
        public const string Customer = "v:customer";
    }

    public static class DisplayNames
    {
        public const string Home = "Root";
        public const string Request = "Requests";
        public const string Quote = "Quotes";
        public const string Order = "Orders";
        public const string Invoice = "Invoices";
        public const string Resource = "Resources";
        public const string Customer = "Customers";

        public const string Id = "ID";

        public const string ItemFormat = "Pos{0:D3}";
        public const string ItemStringFormat = "Pos{0}";
        public const string JobFormat = "{0}-{1}";
    }

    public static class EntityPrefixes
    {
        public const string Request = "r:";
        public const string Quote = "q:";
        public const string Order = "o:";
        public const string Invoice = "i:";
        public const string Resource = "s:";
        public const string Customer = "c:";
    }

    public static class InvoiceDirections
    {
        public const string Receivable = "ir:";
        public const string Payable = "ip:";
    }

    public static class OrderRoute
    {
        public const string Items = "voi";
        public const string IndependentJobs = "voj";

        public static class Item
        {
            public const string Prefix = "oi:";
            public const string Jobs = "voij";
        }

        public static class Job
        {
            public const string IndependentPrefix = "oj:";
            public const string ItemPrefix = "oij:";
        }
    }

    public static class QuoteRoute
    {
        public const string Items = "vqi";
        public const string IndependentJobs = "vqj";

        public static class Item
        {
            public const string Prefix = "qi:";
            public const string Jobs = "vqij";
        }

        public static class Job
        {
            public const string IndependentPrefix = "qj:";
            public const string ItemPrefix = "qij:";
        }
    }

    public static readonly Dictionary<string, string> RootFolders = new()
    {
        { VirtualRoots.Request, DisplayNames.Request },
        { VirtualRoots.Quote, DisplayNames.Quote },
        { VirtualRoots.Order, DisplayNames.Order },
        { VirtualRoots.Invoice, DisplayNames.Invoice },
        { VirtualRoots.Resource, DisplayNames.Resource },
        { VirtualRoots.Customer, DisplayNames.Customer }
    };
}