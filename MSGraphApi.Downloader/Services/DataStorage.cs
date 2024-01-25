using Microsoft.Graph.Models;

public class DataStorage : IDataStorage
{
    private readonly IFileSystemHelpers _fileSystemHelpers;

    public DataStorage(IFileSystemHelpers fileSystemHelpers)
    {
        this._fileSystemHelpers = fileSystemHelpers;
    }

    public async Task Store(string filename, string content)
    {
        var workingDir = Path.GetDirectoryName(filename);
        if (!Directory.Exists(workingDir))
        {
            Directory.CreateDirectory(workingDir!);
        }
        await File.WriteAllTextAsync(filename, content);
    }
}

public interface IDataStorage
{
    Task Store(string fileName, string content);
}
