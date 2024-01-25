namespace MSGraphApi.Downloader.Operations;

using System.Threading.Tasks;
using MSGraphApi.Library.Services.GraphApi;

public abstract class BaseOperationStrategy : IOperationStrategy
{
    private readonly GraphApiSettings _settings;
    protected readonly IGraphApi _graphApi;
    protected readonly IDataStorage _dataStorage;

    public BaseOperationStrategy(
        IGraphApi graphApi,
        GraphApiSettings settings,
        IDataStorage dataStorage
    )
    {
        _graphApi = graphApi;
        _settings = settings;
        _dataStorage = dataStorage;
        graphApi.Init(settings);
    }

    public abstract string Operation { get; }

    public abstract Task<string?> FetchDataAsync(string? nextPageToken = null);
    public abstract Task<GraphDownloaderSummary> StoreDataAsync();
}
