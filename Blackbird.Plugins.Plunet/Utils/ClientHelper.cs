using Blackbird.Plugins.Plunet.DataAdmin30Service;
using Blackbird.Plugins.Plunet.Models;

namespace Blackbird.Plugins.Plunet.Utils;

public static class ClientHelper
{
    public static async Task<LanguageCombination> GetLanguageNamesCombinationByLanguageCodeIso(
        string token,
        LanguageCombination combination)
    {
        await using var adminClient = new DataAdmin30Client();
        var availableLanguagesResult = await adminClient.getAvailableLanguagesAsync(token, "en");

        if (availableLanguagesResult.data == null || availableLanguagesResult.data.Length == 0)
            throw new("No available languages found");

        var sourceLanguage = availableLanguagesResult.data.FirstOrDefault(x =>
                                 x.isoCode.Equals(combination.Source, StringComparison.OrdinalIgnoreCase) ||
                                 x.folderName.Equals(combination.Source, StringComparison.OrdinalIgnoreCase)) ??
                             availableLanguagesResult.data.FirstOrDefault(x => x.isoCode.ToUpper() == "ENG");

        var targetLanguage = availableLanguagesResult.data.FirstOrDefault(x =>
            x.isoCode.Equals(combination.Target, StringComparison.OrdinalIgnoreCase) ||
            x.folderName.Equals(combination.Target, StringComparison.OrdinalIgnoreCase));

        return targetLanguage != null
            ? new(sourceLanguage?.name, targetLanguage.name)
            : throw new("Target language did not match");
    }
}