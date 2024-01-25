namespace MSGraphApi.Downloader.Operations;

using System.Collections;
using System.Text.Json;
using System.Threading.Tasks;
using MSGraphApi.Library.Services.GraphApi;

public class OperationResponse<TDataRes>
{
    public string? NextPageToken { get; set; }
    public TDataRes? Data { get; set; }
}

public abstract class BaseOperationStrategy<TDataRes> : IOperationStrategy
    where TDataRes : class?
{
    private readonly GraphApiSettings _settings;
    protected readonly IGraphApi _graphApi;
    protected readonly IDataStorage _dataStorage;
    private readonly IFileSystemHelpers _fileSystemHelpers;
    private OperationResponse<TDataRes?>? _fetchResponse;

    public BaseOperationStrategy(
        IGraphApi graphApi,
        GraphApiSettings settings,
        IDataStorage dataStorage,
        IFileSystemHelpers fileSystemHelpers
    )
    {
        _graphApi = graphApi;
        _settings = settings;
        _dataStorage = dataStorage;
        this._fileSystemHelpers = fileSystemHelpers;
        graphApi.Init(settings);
    }

    public abstract string Operation { get; }

    public async Task<string?> FetchDataAsync(string? nextPageToken = null)
    {
        try
        {
            _fetchResponse = await InvokeFetchDataAsync(nextPageToken);
            return _fetchResponse?.NextPageToken;
        }
        catch (Exception ex)
        {
            throw new GraphApiException("Cannot fetch Graph API data", ex);
        }
    }

    public async Task<GraphDownloaderSummary> StoreDataAsync()
    {
        var workingDir = $"{_fileSystemHelpers.GetCurrentDirectory()}/MSGraph/{GetStorageFolder()}";
        var itemsCount = 0;

        var data = _fetchResponse?.Data;
        if (data != null && data is IList)
        {
            var collection = data as IList;
            itemsCount = collection!.Count;

            var tasks = new List<Task>();
            foreach (var item in collection!)
            {
                tasks.Add(
                    _dataStorage.StoreData(
                        $"{workingDir}/{GetFormattedFilename(item)}.json",
                        JsonSerializer.Serialize(
                            item,
                            new JsonSerializerOptions() { WriteIndented = true }
                        )
                    )
                );
            }
            try
            {
                await Task.WhenAll(tasks.ToArray());
            }
            catch (Exception ex)
            {
                throw new GraphApiDataStorageException("Cannot store Graph API data", ex);
            }
        }
        return new GraphDownloaderSummary()
        {
            LocationPath = workingDir,
            ObjectsCount = itemsCount
        };
    }

    protected abstract Task<OperationResponse<TDataRes?>> InvokeFetchDataAsync(
        string? nextPageToken
    );

    protected abstract string GetFormattedFilename(dynamic item);

    protected abstract string GetStorageFolder();
}
