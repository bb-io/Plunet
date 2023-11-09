﻿using Apps.Plunet.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.Plunet.Models.Document;

public class UploadDocumentRequest
{
    [Display("Entity ID")]
    public string MainId { get; set; }

    [Display("Folder type")]
    [DataSource(typeof(FolderTypeDataHandler))]
    public string FolderType { get; set; }

    public File File { get; set; }

    [Display("Subfolder")]
    public string? Subfolder { get; set; }
}