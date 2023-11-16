using Apps.Plunet.Models;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Plugins.Plunet.DataAdmin30Service;

namespace Apps.Plunet.Extensions;

public static class LanguageExtensions
{
    public static async Task<LanguageCombination> GetLangNamesByLangIso(
        this LanguageCombination combination,
        AuthenticationCredentialsProvider[] creds)
    {
        await using var adminClient = new DataAdmin30Client(creds.GetInstanceUri());
        var availableLanguagesResult = await adminClient.getAvailableLanguagesAsync(creds.GetAuthToken(), "en");

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