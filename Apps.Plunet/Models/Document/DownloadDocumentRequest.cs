using Apps.Plunet.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.Plunet.Models.Document;

public class DownloadDocumentRequest
{

    [Display("File ID")]
    [FileDataSource(typeof(FilePickerDataSourceHandler))]
    public string FileId { get; set; }

}