using System.Reflection;
using System.Text.Json;
using SimpleWorkoutQTE.Models;
using SPTarkov.Server.Core.DI;

namespace SimpleWorkoutQTE;

public class ConfigRegistration : IOnDIConstruct
{
    public static async Task OnDIConstructAsync(IServiceCollection serviceCollection, CancellationToken ct)
    {
        ModConfig config = await LoadConfigFromDiskAsync(ct);
        serviceCollection.AddSingleton(config);
    }

    private static async Task<ModConfig> LoadConfigFromDiskAsync(CancellationToken ct)
    {
        string configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config.json");

        if (!File.Exists(configPath))
        {
            var defaultConfig = new ModConfig();
            await SaveConfigToDiskAsync(defaultConfig, configPath, ct);
            return defaultConfig;
        }

        await using FileStream stream = File.OpenRead(configPath);
        ModConfig? config = await JsonSerializer.DeserializeAsync<ModConfig>(
            stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            ct);

        return config ?? new ModConfig();
    }

    private static async Task SaveConfigToDiskAsync(ModConfig config, string path, CancellationToken ct)
    {
        await using FileStream stream = File.Create(path);
        await JsonSerializer.SerializeAsync(
            stream,
            config,
            new JsonSerializerOptions { WriteIndented = true },
            ct);
    }
}