using Blackbird.Plugins.Plunet.Extensions;

namespace Blackbird.Plugins.Plunet.Utils;

public static class IntParser
{
    public static int? Parse(string? input, string errorName)
    {
        if (input is null)
            return null;
        
        if (!int.TryParse(input, out var intValue))
            throw new Exception($"{errorName.ToPascalCase()} should be an integer number (from -2147483648 to 2147483647)");

        return intValue;
    } 
}