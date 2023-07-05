namespace Blackbird.Plugins.Plunet.Models.Order;

public class UploadDocumentRequest
{
    public int OrderId { get; set; }

    public int FolderType { get; set; }

    public byte[] FileContentBytes { get; set; }

    public string FilePath { get; set; }
}