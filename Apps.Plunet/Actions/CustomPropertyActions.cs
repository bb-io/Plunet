using Apps.Plunet.Invocables;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Apps.Plunet.Models.CustomProperties;
using Blackbird.Plugins.Plunet.DataCustomFields30;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Plunet.Actions
{
    [ActionList("Custom properties")]
    public class CustomPropertyActions(InvocationContext invocationContext) : PlunetInvocable(invocationContext)
    {
        [Action("Get property", Description = "Get the selected value from any entity")]
        public async Task<GetPropertyResponse> GetProperty([ActionParameter] PropertyRequest input)
        {
            var response = await ExecuteWithRetryAcceptNull(() => CustomFieldsClient.getPropertyAsync(Uuid, input.Name, ParseId(input.UsageArea), ParseId(input.MainId)));

            var selectedValue = response?.selectedPropertyValueID;
            if (selectedValue != null)
            {
                var result = await ExecuteWithRetryAcceptNull(() => CustomFieldsClient.getPropertyValueTextAsync(Uuid, input.Name, (int)selectedValue, Language));
                
                return new GetPropertyResponse { Id = (int)selectedValue, Value = result};
            }

            return new GetPropertyResponse();
        }

        [Action("Set property", Description = "Set the selected proeprty values for any entity")]
        public async Task SetProperty([ActionParameter] SetPropertyRequest input)
        {
            await ExecuteWithRetry(() => CustomFieldsClient.setPropertyValueListAsync(Uuid, input.Name, ParseId(input.UsageArea), input.Values.Select(x => (int?)ParseId(x)).ToArray(), ParseId(input.MainId)));
        }

        [Action("Get text module value", Description = "Get a text module value from any entity")]
        public async Task<TextModuleResponse> GetTextmodule([ActionParameter] TextModuleRequest input)
        {
            if (input == null || input.Flag == null || string.IsNullOrEmpty(input.UsageArea) || string.IsNullOrEmpty(input.MainId) || string.IsNullOrEmpty(Language))
            {
                throw new PluginMisconfigurationException("The inputs can not be null. Please check your input and try again");
            }

            var response = await ExecuteWithRetryAcceptNull(() => CustomFieldsClient.getTextmoduleAsync(Uuid, input.Flag, ParseId(input.UsageArea), ParseId(input.MainId), Language));

            string value = string.Empty;
            if (response == null)
            {
                return new TextModuleResponse { Value = value };
            }

            if (!string.IsNullOrEmpty(response.stringValue))
            {
                value = response.stringValue;
            }
            else if (response.selectedValues != null && response.selectedValues.Any())
            {
                value = response.selectedValues.First();
            }

            return new TextModuleResponse
            {
                Value = value
            };
        }

        [Action("Get multiple text module values", Description = "Get the text module values from any entity for modules where multiple values can be selected")]
        public async Task<MultipleTextModuleResponse> GetTextmoduleValues([ActionParameter] TextModuleRequest input)
        {
            var response = await ExecuteWithRetryAcceptNull(() => CustomFieldsClient.getTextmoduleAsync(Uuid, input.Flag, ParseId(input.UsageArea), ParseId(input.MainId), Language));

            var values = new List<string>();
            if (!string.IsNullOrEmpty(response?.stringValue))
                values = [response.stringValue];
            else if (response?.selectedValues.Any() == true)
                values = response.selectedValues.ToList();

            return new()
            {
                Values = values
            };
        }

        [Action("Set text module value", Description = "Set a text module value for any entity")]
        public async Task SetTextmodule([ActionParameter] TextModuleRequest input,
            [ActionParameter] [Display("Value")] string value)
        {
            await ExecuteWithRetry(() => CustomFieldsClient.setTextmoduleAsync(Uuid,
                new TextmoduleIN
                {
                    flag = input.Flag,
                    stringValue = value,
                    selectedValues = [value],
                    textModuleUsageArea = ParseId(input.UsageArea)
                },
                ParseId(input.MainId), Language));
        }

        [Action("Set multiple text module values", Description = "Set a text module value for any entity where multiple values can be selected")]
        public async Task SetTextmoduleValues([ActionParameter] TextModuleRequest input,
            [ActionParameter][Display("Values")] IEnumerable<string> values)
        {
            await ExecuteWithRetry(() => CustomFieldsClient.setTextmoduleAsync(Uuid,
                new TextmoduleIN
                {
                    flag = input.Flag,
                    selectedValues = values.ToArray(),
                    textModuleUsageArea = ParseId(input.UsageArea)
                },
                ParseId(input.MainId), Language));
        }


        [Action("Get multiselect property", Description = "Get selected values from a multiselect custom property")]
        public async Task<GetMultiplePropertyValuesResponse> GetMultiselectProperty([ActionParameter] PropertyRequest input)
        {
            var property = await ExecuteWithRetryAcceptNull(() =>
                CustomFieldsClient.getPropertyAsync(Uuid, input.Name, ParseId(input.UsageArea), ParseId(input.MainId)));

            var ids = property?.selectedPropertyValueList?
                .Where(v => v.HasValue)
                .Select(v => v!.Value)
                .Distinct()
                .ToArray();

            if (ids == null || ids.Length == 0)
                return new GetMultiplePropertyValuesResponse();

            var texts = await Task.WhenAll(ids.Select(id =>
                ExecuteWithRetryAcceptNull(() =>
                    CustomFieldsClient.getPropertyValueTextAsync(Uuid, input.Name, id, Language))));

            var pairs = ids
               .Select((id, idx) => new PropertyIdName
               {
                   Id = id,
                   Name = texts.ElementAtOrDefault(idx) ?? string.Empty
               }).ToArray();

            var values = pairs.Select(p => p.Name).Where(n => !string.IsNullOrWhiteSpace(n)).Distinct().ToArray();

            return new GetMultiplePropertyValuesResponse
            {
                Values = values,
                Ids = pairs.Select(p => p.Id),
                Pairs = pairs
            };
        }
    }
}