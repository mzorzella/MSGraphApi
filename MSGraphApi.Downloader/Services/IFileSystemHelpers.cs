public interface IFileSystemHelpers
{
    public string? GetDirectoryName(string? filename);
    public bool DirectoryExists(string directory);
    public DirectoryInfo CreateDirectory(string directory);
    public string GetCurrentDirectory();
    public Task WriteAllTextAsync(string path, string content);
}
