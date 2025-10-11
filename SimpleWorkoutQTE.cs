using System.Reflection;
using System.Runtime.CompilerServices;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Eft.Hideout;
using SPTarkov.Server.Core.Models.Enums.Hideout;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;

namespace _simpleWorkoutQTE;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "com.acidphantasm.simpleworkoutqte";
    public override string Name { get; init; } = "Simple Workout QTE";
    public override string Author { get; init; } = "acidphantasm";
    public override List<string>? Contributors { get; init; }
    public override SemanticVersioning.Version Version { get; init; } = new("2.0.0");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.0");
    public override List<string>? Incompatibilities { get; init; }
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public override string? Url { get; init; }
    public override bool? IsBundleMod { get; init; }
    public override string? License { get; init; } = "MIT";
}

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class SimpleWorkoutQTE(
    DatabaseService databaseService,
    ModHelper modHelper,
    ISptLogger<SimpleWorkoutQTE> logger)
    : IOnLoad
{
    private ModConfig _modConfig;
    
    public Task OnLoad()
    { 
        var pathToMod = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
        _modConfig = modHelper.GetJsonDataFromFile<ModConfig>(pathToMod, "config.json");
        
        EditWorkoutValues();
        return Task.CompletedTask;
    }
    
    private void EditWorkoutValues()
    {
        var quickTimeEvents = databaseService.GetHideout().Qte[0].QuickTimeEvents;
        var results = databaseService.GetHideout().Qte[0].Results;

        if (_modConfig.EasyMode)
        {
            foreach (var qteEvent in quickTimeEvents)
            {
                qteEvent.MovementSpeed = 0.5f;
                qteEvent.SuccessCoordinates.X = 0;
                qteEvent.SuccessCoordinates.Y = 0.75f;
            }
        }
        else
        {
            foreach (var qteEvent in quickTimeEvents)
            {
                qteEvent.MovementSpeed *= _modConfig.QteSpeed;
                qteEvent.SuccessCoordinates.Y *= _modConfig.QteSize;
                if (qteEvent.SuccessCoordinates.Y <= 0.07f && _modConfig.PreventVeryDifficultQTE)
                {
                    qteEvent.SuccessCoordinates.Y = 0.07f;
                }

                if (qteEvent.SuccessCoordinates.Y >= 1) qteEvent.SuccessCoordinates.Y = 1f;
                
                logger.Success($"{qteEvent.SuccessCoordinates.Y}");
            }
        }

        foreach (var (effectType, result) in results)
        {
            if (effectType == QteEffectType.singleSuccessEffect)
            {
                var originalEndurance = result.RewardEffects[0].LevelMultipliers;
                var originalStrength = result.RewardEffects[1].LevelMultipliers;

                originalEndurance[0].MultiplierValue *= _modConfig.Level0ExpMultiplier;
                originalEndurance[1].MultiplierValue *= _modConfig.Level10ExpMultiplier;
                originalEndurance[2].MultiplierValue *= _modConfig.Level25ExpMultiplier;
                
                originalStrength[0].MultiplierValue *= _modConfig.Level0ExpMultiplier;
                originalStrength[1].MultiplierValue *= _modConfig.Level10ExpMultiplier;
                originalStrength[2].MultiplierValue *= _modConfig.Level25ExpMultiplier;
            }

            if (effectType == QteEffectType.finishEffect)
            {
                result.RewardEffects[0].Time = _modConfig.MusclePainTime;
            }
        }
    }
}

public class ModConfig
{
    public long? MusclePainTime  { get; set; }
    public float Level0ExpMultiplier {  get; set; }
    public float Level10ExpMultiplier {  get; set; }
    public float Level25ExpMultiplier {  get; set; }
    public bool EasyMode  { get; set; }
    public float QteSpeed { get; set; }
    public float QteSize { get; set; }
    public bool PreventVeryDifficultQTE { get; set; }
}
