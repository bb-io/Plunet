﻿using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Plunet.Models.Job
{
    public class JobTypeRequest
    {
        [Display("Job type")]
        [DataSource(typeof(JobTypeDataHandler))]
        public string JobType { get; set; }
    }
}
