public class FileSystemHelpers : IFileSystemHelpers
{
    public string? GetDirectoryName(string? filename)
    {
        return Path.GetDirectoryName(filename);
    }

    public bool DirectoryExists(string directory)
    {
        return Directory.Exists(directory);
    }

    public DirectoryInfo CreateDirectory(string directory)
    {
        return Directory.CreateDirectory(directory);
    }

    public string GetCurrentDirectory()
    {
        return Directory.GetCurrentDirectory();
    }

    public Task WriteAllTextAsync(string path, string content)
    {
        return File.WriteAllTextAsync(path, content);
    }
}
