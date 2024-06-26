﻿using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;


namespace Apps.Plunet.Models.Document;

public class UploadDocumentRequest
{
    [Display("Entity ID")]
    public string MainId { get; set; }

    [Display("Folder type")]
    [StaticDataSource(typeof(FolderTypeDataHandler))]
    public string FolderType { get; set; }

    public FileReference File { get; set; }

    [Display("Subfolder")]
    public string? Subfolder { get; set; }

    [Display("Ignore if file already exists")]
    public bool? IgnoreIfFileAlreadyExists { get; set; }
}