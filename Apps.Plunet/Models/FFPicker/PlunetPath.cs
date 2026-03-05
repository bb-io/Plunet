namespace Apps.Plunet.Models.FFPicker;

public class PlunetPath(string raw, string rootSegment, string[] segments)
{
    public string Raw { get; } = raw;
    public string RootSegment { get; } = rootSegment;
    public string[] Segments { get; } = segments;
}