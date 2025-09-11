using Apps.Plunet.Invocables;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Plugins.Plunet.DataCustomFields30;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.DataSourceHandlers
{
    public class AllPropertyValueDataHandler : PlunetInvocable, IAsyncDataSourceHandler
    {
        public AllPropertyValueDataHandler(InvocationContext invocationContext) : base(invocationContext)
        {
        }

        public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
        {
            var client = new DataCustomFields30Client();
            var properties = await ExecuteWithRetryAcceptNull(() => client.getAvailablePropertiesAsync(Uuid, 2), cancellationToken);

            var result = new Dictionary<string, string>();

            if (properties == null || properties.Length == 0)
                return result;

            foreach (var prop in properties)
            {
                if (prop.avaliablePropertyValueIDList != null)
                {
                    foreach (var valueId in prop.avaliablePropertyValueIDList.Where(id => id.HasValue))
                    {
                        var id = valueId.Value.ToString();
                        var displayValue = $"{prop.propertyNameEnglish}: {id}";
                        if (context.SearchString == null || displayValue.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
                        {
                            result[id] = displayValue;
                        }
                    }
                }
            }

            return result;
        }
    }
}
