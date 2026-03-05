using Apps.Plunet.Constants;
using Apps.Plunet.Models.FFPicker;

namespace Apps.Plunet.Utils;

public static class PathParser
{
    public static FfPath Parse(string? folderId)
    {
        var id = string.IsNullOrEmpty(folderId) ? FolderConstants.VirtualRoots.Home : folderId;

        var segments = id.Split('/', StringSplitOptions.RemoveEmptyEntries);

        var root = segments.Length > 0 ? segments[0] : id;

        return new FfPath(id, root, segments.Skip(1).ToArray());
    }
}