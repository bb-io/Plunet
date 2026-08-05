namespace Apps.Plunet.Extensions;

public static class ExceptionExtensions
{
    public static bool IsCantFindError(this Exception ex)
    {
        var message = ex.Message.Replace('\u2019', '\'');
        return message.Contains("can't find the requested", StringComparison.OrdinalIgnoreCase) || 
               message.Contains("cannot find the requested", StringComparison.OrdinalIgnoreCase);
    }
}