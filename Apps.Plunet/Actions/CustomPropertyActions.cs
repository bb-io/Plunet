using Apps.Plunet.Invocables;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Apps.Plunet.Models.CustomProperties;
using Apps.Plunet.Constants;
using Blackbird.Plugins.Plunet.DataCustomFields30;

namespace Apps.Plunet.Actions
{
    [ActionList]
    public class CustomPropertyActions(InvocationContext invocationContext) : PlunetInvocable(invocationContext)
    {
        [Action("Get property", Description = "Get the selected value from any entity")]
        public async Task<string> GetProperty([ActionParameter] PropertyRequest input)
        {
            var response = await ExecuteWithRetry<PropertyResult>(async () =>
                await CustomFieldsClient.getPropertyAsync(Uuid, input.Name, ParseId(input.UsageArea),
                    ParseId(input.MainId)));

            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);

            var selectedValue = response.data?.selectedPropertyValueID;
            if (selectedValue != null)
            {
                var result = await ExecuteWithRetry<StringResult>(async () =>
                    await CustomFieldsClient.getPropertyValueTextAsync(Uuid, input.Name, (int)selectedValue, Language));
                
                return result.data;
            }

            return string.Empty;
        }

        [Action("Set property", Description = "Set the selected proeprty values for any entity")]
        public async Task SetProperty([ActionParameter] SetPropertyRequest input)
        {
            var response = await ExecuteWithRetry(async () => await CustomFieldsClient.setPropertyValueListAsync(Uuid, input.Name,
                ParseId(input.UsageArea), input.Values.Select(x => (int?)ParseId(x)).ToArray(), ParseId(input.MainId)));
            if (response.Result.statusMessage != ApiResponses.Ok)
                throw new(response.Result.statusMessage);
        }

        [Action("Get text module value", Description = "Get a text module value from any entity")]
        public async Task<TextModuleResponse> GetTextmodule([ActionParameter] TextModuleRequest input)
        {
            var response = await ExecuteWithRetry<TextmoduleResult>(async () => await CustomFieldsClient.getTextmoduleAsync(Uuid, input.Flag, ParseId(input.UsageArea),
                ParseId(input.MainId), Language));
            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);

            string value = string.Empty;
            if (string.IsNullOrEmpty(response.data.stringValue) && response.data.selectedValues is null)
            {
            }
            else if (string.IsNullOrEmpty(response.data.stringValue) && response.data.selectedValues.Any())
            {
                value = response.data.selectedValues.First();
            }
            else
            {
                value = response.data.stringValue;
            }

            return new()
            {
                Value = value
            };
        }

        [Action("Get multiple text module values", Description = "Get the text module values from any entity for modules where multiple values can be selected")]
        public async Task<MultipleTextModuleResponse> GetTextmoduleValues([ActionParameter] TextModuleRequest input)
        {
            var response = await ExecuteWithRetry<TextmoduleResult>(async () => await CustomFieldsClient.getTextmoduleAsync(Uuid, input.Flag, ParseId(input.UsageArea),
                ParseId(input.MainId), Language));
            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);

            var values = new List<string>();
            if (string.IsNullOrEmpty(response.data.stringValue) && response.data.selectedValues is null)
            {
            }
            else if (string.IsNullOrEmpty(response.data.stringValue) && response.data.selectedValues.Any())
            {
                values = response.data.selectedValues.ToList();
            }
            else
            {
                values = new List<string>() { response.data.stringValue };
            }

            return new()
            {
                Values = values
            };
        }

        [Action("Set text module value", Description = "Set a text module value for any entity")]
        public async Task SetTextmodule([ActionParameter] TextModuleRequest input,
            [ActionParameter] [Display("Value")] string value)
        {
            var response = await ExecuteWithRetry<Result>(async () => await CustomFieldsClient.setTextmoduleAsync(
                Uuid,
                new TextmoduleIN
                {
                    flag = input.Flag,
                    stringValue = value,
                    textModuleUsageArea = ParseId(input.UsageArea)
                },
                ParseId(input.MainId), Language));
            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);
        }

        [Action("Set multiple text module values", Description = "Set a text module value for any entity where multiple values can be selected")]
        public async Task SetTextmoduleValues([ActionParameter] TextModuleRequest input,
            [ActionParameter][Display("Values")] IEnumerable<string> values)
        {
            var response = await ExecuteWithRetry<Result>(async () => await CustomFieldsClient.setTextmoduleAsync(
                Uuid,
                new TextmoduleIN
                {
                    flag = input.Flag,
                    selectedValues = values.ToArray(),
                    textModuleUsageArea = ParseId(input.UsageArea)
                },
                ParseId(input.MainId), Language));
            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);
        }

        private async Task<T> ExecuteWithRetry<T>(Func<Task<Result>> func, int maxRetries = 10, int delay = 1000)
            where T : Result
        {
            var attempts = 0;
            while (true)
            {
                var result = await func();

                if (result.statusMessage == ApiResponses.Ok)
                {
                    return (T)result;
                }

                if(result.statusMessage.Contains("session-UUID used is invalid"))
                {
                    if (attempts < maxRetries)
                    {
                        await Task.Delay(delay);
                        await RefreshAuthToken();
                        attempts++;
                        continue;
                    }

                    throw new($"No more retries left. Last error: {result.statusMessage}, Session UUID used is invalid.");
                }

                return (T)result;
            }
        }
        
        private async Task<setPropertyValueListResponse> ExecuteWithRetry(Func<Task<setPropertyValueListResponse>> func, int maxRetries = 10, int delay = 1000)
        {
            var attempts = 0;
            while (true)
            {
                var result = await func();

                if (result.Result.statusMessage == ApiResponses.Ok)
                {
                    return result;
                }

                if(result.Result.statusMessage.Contains("session-UUID used is invalid"))
                {
                    if (attempts < maxRetries)
                    {
                        await Task.Delay(delay);
                        await RefreshAuthToken();
                        attempts++;
                        continue;
                    }

                    throw new($"No more retries left. Last error: {result.Result.statusMessage}, Session UUID used is invalid.");
                }

                return result;
            }
        }
    }
}