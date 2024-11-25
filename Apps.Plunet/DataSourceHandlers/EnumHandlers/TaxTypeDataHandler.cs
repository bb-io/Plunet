using Blackbird.Applications.Sdk.Common.Dictionaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.DataSourceHandlers.EnumHandlers
{
    public class TaxTypeDataHandler : IStaticDataSourceHandler
    {
        public Dictionary<string, string> GetData()
        {
            return new()
            {
                { "INFO", "3" },
                { "INFO_SUM", "6" },
                { "PRICE_BLOCK", "12" },
                { "SUM", "4" },
                { "TAX_1", "0" },
                { "TAX_1_2", "5" },
                { "TAX_1_2_3", "8" },
                { "TAX_1_2_3_4", "14" },
               //{ "TAX_1_2_3_4_5", "14" },   duplicateв value
                { "TAX_1_3", "10" },
                { "TAX_1_4", "17" },
                { "TAX_2", "1" },
                { "TAX_2_3", "11" },
                { "TAX_2_4_5", "16" },
                { "TAX_3", "7" },
                { "TAX_4", "9" },
                { "TAX_5", "13" },
                { "WITHOUT_TAX", "2" }
            };
        }
    }
}
