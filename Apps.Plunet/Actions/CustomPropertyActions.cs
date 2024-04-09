using Apps.Plunet.Invocables;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Apps.Plunet.Models.CustomProperties;
using Apps.Plunet.Constants;

namespace Apps.Plunet.Actions
{
    [ActionList]
    public class CustomPropertyActions : PlunetInvocable
    {
        public CustomPropertyActions(InvocationContext invocationContext) : base(invocationContext)
        {
        }

        [Action("Get property", Description = "Get a list of selected property values from any entity")]
        public async Task<PropertyList> GetProperty([ActionParameter] PropertyRequest input)
        {
            var response = await CustomFieldsClient.getPropertyAsync(Uuid, input.Name, ParseId(input.UsageArea), ParseId(input.MainId));
            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);

            return new()
            {
                Values = response.data?.selectedPropertyValueList?.Select(x => x.ToString()),
            };
        }

        [Action("Set property", Description = "Set the selected proeprty values for any entity")]
        public async Task SetProperty([ActionParameter] SetPropertyRequest input)
        {
            var response = await CustomFieldsClient.setPropertyValueListAsync(Uuid, input.Name, ParseId(input.UsageArea), input.Values.Select(x => (int?)ParseId(x)).ToArray(), ParseId(input.MainId));
            if (response.Result.statusMessage != ApiResponses.Ok)
                throw new(response.Result.statusMessage);
        }

        [Action("Get text module", Description = "Get a text module value from any entity")]
        public async Task<TextModuleResponse> GetTextmodule([ActionParameter] TextModuleRequest input)
        {
            var response = await CustomFieldsClient.getTextmoduleAsync(Uuid, input.Flag, ParseId(input.UsageArea), ParseId(input.MainId), Language);
            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);

            string value = string.Empty;
            if (string.IsNullOrEmpty(response.data.stringValue) && response.data.selectedValues.Any())
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

        [Action("Set text module", Description = "Set a text module value for any entity")]
        public async Task SetTextmodule([ActionParameter] TextModuleRequest input, [ActionParameter][Display("Value")] string value)
        {
            var response = await CustomFieldsClient.setTextmoduleAsync(
                Uuid, 
                new Blackbird.Plugins.Plunet.DataCustomFields30.TextmoduleIN 
                { 
                    flag = input.Flag, 
                    stringValue = value, 
                    textModuleUsageArea = ParseId(input.UsageArea) 
                }, 
                ParseId(input.MainId), Language);
            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);
        }

    }
}
