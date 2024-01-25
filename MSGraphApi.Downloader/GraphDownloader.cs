namespace MSGraphApi.Downloader;

using MSGraphApi.Downloader.Operations;
using MSGraphApi.Library.Services.GraphApi;

public class GraphDownloaderSummary
{
    public required string LocationPath { get; set; }
    public required int ObjectsCount { get; set; }
}

public class GraphDownloader
{
    private readonly IGraphApi _graphApi;
    private readonly GraphApiSettings _settings;
    private readonly IEnumerable<IOperationStrategy> _strategies;

    public GraphDownloader(
        IGraphApi graphApi,
        GraphApiSettings settings,
        IEnumerable<IOperationStrategy> strategies
    )
    {
        _graphApi = graphApi;
        _settings = settings;
        _strategies = strategies;
    }

    public async Task<GraphDownloaderSummary> Download(string operation)
    {
        var operationStrategy = findOperationStrategy(operation);
        _graphApi.Init(_settings);

        var resultSummary = new GraphDownloaderSummary() { LocationPath = "", ObjectsCount = 0 };

        await Paginate(
            async (nextPageToken) =>
            {
                var nextToken = await operationStrategy.FetchDataAsync(nextPageToken);
                var partialSummary = await operationStrategy.StoreDataAsync();
                if (partialSummary != null)
                {
                    resultSummary.LocationPath = partialSummary.LocationPath;
                    resultSummary.ObjectsCount =
                        resultSummary.ObjectsCount + partialSummary.ObjectsCount;
                }
                return nextToken;
            }
        );

        return resultSummary;
    }

    private async Task Paginate(Func<string?, Task<string?>> action)
    {
        string? nextPageToken = null;
        do
        {
            nextPageToken = await action(nextPageToken);
        } while (nextPageToken != null);
    }

    private IOperationStrategy findOperationStrategy(string operation)
    {
        var operationStrategy = _strategies.FirstOrDefault(s => s.Operation == operation);
        if (operationStrategy == null)
        {
            throw new ArgumentException($"Operation \"{operation}\" is not supported");
        }

        return operationStrategy;
    }
}
