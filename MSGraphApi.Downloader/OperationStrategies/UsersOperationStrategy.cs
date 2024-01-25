namespace MSGraphApi.Downloader.Operations;

using System.Text.Json;
using Microsoft.Graph.Models;
using MSGraphApi.Library.Services.GraphApi;

public class UsersOperationStrategy : BaseOperationStrategy<List<User>?>
{
    public UsersOperationStrategy(
        IGraphApi graphApi,
        GraphApiSettings settings,
        IDataStorage dataStorage,
        IFileSystemHelpers fileSystemHelpers
    )
        : base(graphApi, settings, dataStorage, fileSystemHelpers) { }

    public override string Operation => "Download Users";

    protected override string GetFormattedFilename(dynamic item)
    {
        return $"{item.DisplayName}-[{item.Id}";
    }

    protected override string GetStorageFolder()
    {
        return "Users";
    }

    protected override async Task<OperationResponse<List<User>?>> InvokeFetchDataAsync(
        string? nextPageToken
    )
    {
        var res = await _graphApi.GetUsersAsync(nextPageToken);
        return new OperationResponse<List<User>?>()
        {
            NextPageToken = res?.OdataNextLink,
            Data = res?.Value
        };
    }
}
