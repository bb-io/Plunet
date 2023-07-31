using System.Text.RegularExpressions;

namespace Blackbird.Plugins.Plunet.Extensions;

public static class StringExtensions
{
    public static string ToPascalCase(this string input)
        => Regex.Replace(input, @"\b\p{Ll}", match => match.Value.ToUpper());
}