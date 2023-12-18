﻿using Apps.Plunet.Constants;
using Apps.Plunet.Invocables;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.DataSourceHandlers
{
    public class JobTypeDataHandler : PlunetInvocable, IAsyncDataSourceHandler
    {
        public JobTypeDataHandler(InvocationContext invocationContext) : base(invocationContext)
        {
        }

        public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
            CancellationToken cancellationToken)
        {
            var response = await AdminClient.getAvailableServicesAsync(Uuid, Language);

            if (response.statusMessage != ApiResponses.Ok)
                throw new(response.statusMessage);

            return response.data
                .Where(service => context.SearchString == null ||
                                   service.name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
                .ToDictionary(x => x.abbreviation.ToString(), x => x.name);
        }
    }
}