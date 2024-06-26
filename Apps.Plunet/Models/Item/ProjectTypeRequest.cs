﻿using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Item
{
    public class ProjectTypeRequest
    {
        [Display("Type")]
        [StaticDataSource(typeof(ItemProjectTypeDataHandler))]
        public string ProjectType { get; set; }
    }
}
