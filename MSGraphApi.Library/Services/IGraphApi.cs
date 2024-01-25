using Microsoft.Graph.Models;

namespace MSGraphApi.Library.Services.GraphApi;

public interface IGraphApi
{
    public void Init(GraphApiSettings settings);
    public Task<GroupCollectionResponse?> GetGroupsAsync(string? odataNextLink = null);
    public Task<UserCollectionResponse?> GetUsersAsync(string? odataNextLink = null);
}
