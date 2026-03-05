using Apps.Plunet.Actions;
using Apps.Plunet.Models.Document;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Newtonsoft.Json;
using Tests.Plunet.Base;

namespace Tests.Plunet;

[TestClass]
public class DocumentTests : TestBase
{
    [TestMethod]
    public async Task UploadFile_IsSuccess()
    {
        var action = new DocumentActions(InvocationContext, FileManager);
        var request = new UploadDocumentRequest
        {
            //FolderId = "q:104/vqi/qi:32:001/vqij/qij:1:INT/!_Out/test",
            FolderId = "c:136/!_In",
            File = new FileReference
            {
                Name = "upload.txt"
            },
            //Subfolder = "subfolder",
        };

        try
        {
            await action.UploadFile(request);
        }
        catch (PluginApplicationException ex)
        {
            Assert.Fail($"Method threw PluginApplicationException: {ex.Message}");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Method threw an unexpected exception: {ex.Message}");
        }
    }

    [TestMethod]
    public async Task DownloadFile_IsSuccess()
    {
        var action = new DocumentActions(InvocationContext, FileManager);
        var request = new DownloadDocumentRequest
        {
            //FileId = "q:104/vqi/qi:32:001/vqij/qij:1:INT/!_Out/test/subfolder/upload.txt",
            //FileId = "o:663/Prm/test-in-1.txt",
             FileId = "o:663/voi/oi:499:001/voij/oij:303:REV/!_Out/test-out-1.txt",
        };

        var response = await action.DownloadFile(request);

        Assert.IsNotNull(response);
        Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
    }   
    
    [TestMethod]
    public async Task ListFiles_IsSuccess()
    {
        var action = new DocumentActions(InvocationContext, FileManager);
        var request = new ListFilesRequest
        {
            FolderId = "o:663/voi/oi:499:001/voij/oij:303:REV/!_Out",
        };

        var response = await action.ListFiles(request);

        Assert.IsNotNull(response);
        Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
    } 
    
    [TestMethod]
    public async Task SearchFiles_IsSuccess()
    {
        var action = new DocumentActions(InvocationContext, FileManager);
        var request = new ListFilesRequest
        {
            FolderId = "o:663/voi/oi:499:001/voij/oij:303:REV/!_Out",
        };

        var response = await action.SearchFiles(request);

        Assert.IsNotNull(response);
        Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
    }
}