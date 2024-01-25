namespace MSGraphApi.Downloader.Operations;

public interface IOperationStrategy
{
    string Operation { get; }
    Task<string?> FetchDataAsync(string? nextPageToken = null);
    Task<GraphDownloaderSummary> StoreDataAsync();
}
