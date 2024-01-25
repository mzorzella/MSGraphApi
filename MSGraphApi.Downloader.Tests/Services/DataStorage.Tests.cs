using Moq;

namespace MSGraphApi.Downloader.Services.Tests;

public class DataStorageTests
{
    [Fact]
    public async void StoreData_Should_Create_Destination_Folder_If_Not_Exists()
    {
        var ctx = new TestContext();

        const string filename = "path/to/my/filename.json";
        const string pathToFile = "path/to/my";

        ctx.FileSystemHelpersMock.Setup(h => h.GetDirectoryName(filename)).Returns(pathToFile);
        ctx.FileSystemHelpersMock.Setup(h => h.DirectoryExists(pathToFile)).Returns(false);
        await ctx.DataStorage.StoreData("path/to/my/filename.json", "content-here");
        ctx.FileSystemHelpersMock.Verify(h => h.GetDirectoryName(filename), Times.Once);
        ctx.FileSystemHelpersMock.Verify(h => h.DirectoryExists(pathToFile), Times.Once);
        ctx.FileSystemHelpersMock.Verify(h => h.CreateDirectory(pathToFile), Times.Once);
    }

    [Fact]
    public async void StoreData_Should_Not_Create_Destination_Folder_If_Already_Exists()
    {
        var ctx = new TestContext();

        const string filename = "path/to/my/filename.json";
        const string pathToFile = "path/to/my";

        ctx.FileSystemHelpersMock.Setup(h => h.GetDirectoryName(filename)).Returns(pathToFile);
        ctx.FileSystemHelpersMock.Setup(h => h.DirectoryExists(pathToFile)).Returns(true);
        await ctx.DataStorage.StoreData("path/to/my/filename.json", "content-here");
        ctx.FileSystemHelpersMock.Verify(h => h.GetDirectoryName(filename), Times.Once);
        ctx.FileSystemHelpersMock.Verify(h => h.DirectoryExists(pathToFile), Times.Once);
        ctx.FileSystemHelpersMock.Verify(h => h.CreateDirectory(pathToFile), Times.Never);
    }

    [Fact]
    public async void StoreData_Should_Write_Content_To_File()
    {
        var ctx = new TestContext();

        const string filename = "path/to/my/filename.json";
        const string pathToFile = "path/to/my";

        ctx.FileSystemHelpersMock.Setup(h => h.GetDirectoryName(filename)).Returns(pathToFile);
        ctx.FileSystemHelpersMock.Setup(h => h.DirectoryExists(pathToFile)).Returns(true);
        await ctx.DataStorage.StoreData("path/to/my/filename.json", "content-here");
        ctx.FileSystemHelpersMock.Verify(
            h => h.WriteAllTextAsync(filename, "content-here"),
            Times.Once
        );
    }

    private class TestContext
    {
        public DataStorage DataStorage { get; private set; }
        public Mock<IFileSystemHelpers> FileSystemHelpersMock { get; private set; }

        public TestContext()
        {
            FileSystemHelpersMock = new Mock<IFileSystemHelpers>();

            DataStorage = new DataStorage(FileSystemHelpersMock.Object);
        }
    }
}
