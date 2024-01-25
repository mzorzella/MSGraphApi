public class DataStorage : IDataStorage
{
    private readonly IFileSystemHelpers _fileSystemHelpers;

    public DataStorage(IFileSystemHelpers fileSystemHelpers)
    {
        _fileSystemHelpers = fileSystemHelpers;
    }

    public async Task StoreData(string filename, string content)
    {
        var workingDir = _fileSystemHelpers.GetDirectoryName(filename);
        if (!_fileSystemHelpers.DirectoryExists(workingDir!))
        {
            _fileSystemHelpers.CreateDirectory(workingDir!);
        }
        await _fileSystemHelpers.WriteAllTextAsync(filename, content);
    }
}
