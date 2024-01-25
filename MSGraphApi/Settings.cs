using System.Reflection;
using Microsoft.Extensions.Configuration;
using MSGraphApi.Library.Services.GraphApi;

public class Settings
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? TenantId { get; set; }

    public static GraphApiSettings LoadSettings()
    {
        // Load settings
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.Development.json", optional: true)
            .AddUserSecrets(Assembly.GetExecutingAssembly())
            .Build();

        return config.GetRequiredSection("Settings").Get<GraphApiSettings>()
            ?? throw new Exception(
                "Could not load app settings. See README for configuration instructions."
            );
    }
}
