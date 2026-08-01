using SPTarkov.Server.Core.Models.Spt.Mod;

namespace SimpleWorkoutQTE;

public record ModMetadata : IModMetadata
{
    public string ModGuid { get; init; } = "com.acidphantasm.simpleworkoutqte";
    public string Name { get; init; } = "Simple Workout QTE";
    public string Author { get; init; } = "acidphantasm";
    public List<string>? Contributors { get; init; }
    public SemanticVersioning.Version Version { get; init; } = new("2.2.0");
    public SemanticVersioning.Range SptVersion { get; init; } = new("~4.1.0");
    public List<string>? Incompatibilities { get; init; }
    public Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public string? Url { get; init; }
    public bool HasPrepatcher { get; init; } = false;
    public string License { get; init; } = "MIT";
}