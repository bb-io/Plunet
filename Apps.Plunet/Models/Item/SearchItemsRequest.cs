﻿using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.Models.Item
{
    public class SearchItemsRequest
    {

        [Display("Item status")]
        [DataSource(typeof(ItemStatusDataHandler))]
        public string Status { get; set; }

        [Display("Document status")]
        [DataSource(typeof(DocumentStatusDataHandler))]
        public string? DocumentStatus { get; set; }

        [Display("Currency type")]
        [DataSource(typeof(CurrencyTypeDataHandler))]
        public string? CurrencyType { get; set; }
    }
}
