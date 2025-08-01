﻿using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Plunet.Models
{
    public class OptionalTargetLanguageRequest
    {
        [Display("Target language")]
        [DataSource(typeof(LanguageNameDataHandler))]
        public string? TargetLanguageName { get; set; }
    }
}
