using Apps.Plunet.Models.Job;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Plugins.Plunet.DataQualityManager30;

namespace Apps.Plunet.Utils;

internal static class JobFeedbackMapper
{
    public static JobQualityRating[]? BuildRatings(SetJobFeedbackRequest input, Func<string, int> parseId)
    {
        var hasSimpleInput = !string.IsNullOrWhiteSpace(input.CriterionId) || input.Rating.HasValue;
        var criterionIds = input.CriterionIds?.ToList() ?? [];
        var criticalAmounts = input.CriticalAmounts?.ToList() ?? [];
        var hardAmounts = input.HardAmounts?.ToList() ?? [];
        var minorAmounts = input.MinorAmounts?.ToList() ?? [];
        var ratings = input.Ratings?.ToList() ?? [];
        var hasAdvancedInput = criterionIds.Count != 0 || criticalAmounts.Count != 0 || hardAmounts.Count != 0 ||
            minorAmounts.Count != 0 || ratings.Count != 0;

        if (hasSimpleInput && hasAdvancedInput)
        {
            throw new PluginMisconfigurationException(
                "Use either the simple job feedback fields or the advanced criterion lists, not both.");
        }

        if (hasSimpleInput)
        {
            if (string.IsNullOrWhiteSpace(input.CriterionId) || !input.Rating.HasValue)
            {
                throw new PluginMisconfigurationException(
                    "Both Criterion ID and Rating are required for the simple job feedback input.");
            }

            var normalizedSimpleRating = NormalizeRatingValue(input.Rating.Value);
            ValidateRatingRange([normalizedSimpleRating]);

            return
            [
                new JobQualityRating
                {
                    criterionID = parseId(input.CriterionId),
                    rating = normalizedSimpleRating
                }
            ];
        }

        if (criterionIds.Count == 0)
        {
            return null;
        }

        ValidateFeedbackInputLengths(criterionIds.Count, criticalAmounts.Count, hardAmounts.Count,
            minorAmounts.Count, ratings.Count);
        var normalizedRatings = ratings.Select(NormalizeRatingValue).ToList();
        ValidateRatingRange(normalizedRatings);

        return criterionIds.Select((criterionId, index) => new JobQualityRating
        {
            criterionID = parseId(criterionId),
            lisaRating = new LISARating
            {
                amount_critical = criticalAmounts.ElementAtOrDefault(index),
                amount_hard = hardAmounts.ElementAtOrDefault(index),
                amount_minor = minorAmounts.ElementAtOrDefault(index)
            },
            rating = normalizedRatings.ElementAtOrDefault(index)
        }).ToArray();
    }

    private static void ValidateFeedbackInputLengths(int criterionCount, int criticalCount, int hardCount,
        int minorCount, int ratingCount)
    {
        var lengths = new Dictionary<string, int>
        {
            { "criterion IDs", criterionCount },
            { "critical amounts", criticalCount },
            { "hard amounts", hardCount },
            { "minor amounts", minorCount },
            { "ratings", ratingCount }
        };

        var invalidLengths = lengths.Where(x => x.Value != 0 && x.Value != criterionCount).ToList();
        if (invalidLengths.Any())
        {
            throw new PluginMisconfigurationException(
                "Feedback rating inputs must have the same number of entries as criterion IDs.");
        }
    }

    private static void ValidateRatingRange(IEnumerable<int> ratings)
    {
        if (ratings.Any(x => x is < -1 or > 100))
        {
            throw new PluginMisconfigurationException(
                "Job feedback ratings must be between -1 and 100, or between 1 and 5 for star shorthand.");
        }
    }

    private static int NormalizeRatingValue(int rating)
        => rating is >= 1 and <= 5 ? rating * 20 : rating;
}
