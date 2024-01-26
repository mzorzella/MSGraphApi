using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace MSGraphApi.Library.Services.GraphApi;

public class GraphApi : IGraphApi
{
    private GraphServiceClient? _client;

    public void Init(GraphApiSettings settings)
    {
        var creds = new ClientSecretCredential(
            settings.TenantId,
            settings.ClientId,
            settings.ClientSecret
        );
        _client = new GraphServiceClient(creds, ["https://graph.microsoft.com/.default"]);
    }

    public Task<GroupCollectionResponse?> GetGroupsAsync(string? odataNextLink = null)
    {
        _ =
            _client
            ?? throw new ArgumentNullException("Graph has not been initialized for app-only auth");

        var api = _client.Groups;

        if (odataNextLink != null)
        {
            return api.WithUrl(odataNextLink).GetAsync();
        }
        return api.GetAsync(requestConfiguration =>
        {
            requestConfiguration.QueryParameters.Top = 10;
        });
    }

    public Task<UserCollectionResponse?> GetUsersAsync(string? odataNextLink = null)
    {
        _ =
            _client
            ?? throw new ArgumentNullException("Graph has not been initialized for app-only auth");

        var api = _client.Users;
        if (odataNextLink != null)
        {
            api.WithUrl(odataNextLink);
        }
        return _client.Users.GetAsync(requestConfiguration =>
        {
            requestConfiguration.QueryParameters.Top = 2;
        });
    }
}
