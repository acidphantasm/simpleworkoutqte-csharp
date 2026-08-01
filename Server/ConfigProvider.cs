using SimpleWorkoutQTE.Models;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Web.Models.Configs;
using SPTarkov.Server.Web.Services;

namespace SimpleWorkoutQTE;

[Injectable(InjectionType.Singleton)]
public class ConfigProvider(ModConfig config) : IConfigEditorConfigProvider
{
    public IEnumerable<ConfigEditorConfigRegistration> GetConfigs()
    {
        yield return ConfigEditorConfigRegistration.Create(
            "com.acidphantasm.simpleworkoutqte",
            "SimpleWorkoutQTE",
            config,
            Path.Combine("user", "mods", "acidphantasm-simpleworkoutqte", "config.json")
        );
    }
}