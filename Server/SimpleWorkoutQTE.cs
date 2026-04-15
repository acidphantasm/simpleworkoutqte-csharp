using System.Reflection;
using _simpleWorkoutQTE.Models;
using _simpleWorkoutQTE.Models.Enums;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Eft.Hideout;
using SPTarkov.Server.Core.Models.Enums.Hideout;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;

namespace _simpleWorkoutQTE;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "com.acidphantasm.simpleworkoutqte";
    public override string Name { get; init; } = "Simple Workout QTE";
    public override string Author { get; init; } = "acidphantasm";
    public override List<string>? Contributors { get; init; }
    public override SemanticVersioning.Version Version { get; init; } = new("2.1.1");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.0");
    public override List<string>? Incompatibilities { get; init; }
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public override string? Url { get; init; }
    public override bool? IsBundleMod { get; init; }
    public override string License { get; init; } = "MIT";
}

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class SimpleWorkoutQTE(
    DatabaseService databaseService,
    ModHelper modHelper,
    ISptLogger<SimpleWorkoutQTE> logger)
    : IOnLoad
{
    private ModConfig _modConfig = null!;
    
    public Task OnLoad()
    { 
        var pathToMod = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
        _modConfig = modHelper.GetJsonDataFromFile<ModConfig>(pathToMod, "config.json");
        
        EditQte();
        EditWorkout();
        return Task.CompletedTask;
    }
    
    private void EditQte()
    {
        var hideout = databaseService.GetHideout();
        var qteData = hideout.Qte[0];
        var newQteData = new List<QuickTimeEvent>();
        
        if (_modConfig.EasyMode.Enable)
        {
            for (var i = 0; i < _modConfig.EasyMode.NumberOfReps; i++)
            {
                newQteData.Add(new QuickTimeEvent
                {
                    EventType = QteType.ShrinkingCircle,
                    Coordinates = new Position
                    {
                        X = 0.5f,
                        Y = 0.25f
                    },
                    MovementSpeed = 0.5f,
                    SuccessCoordinates = new Position
                    {
                        X = 0f,
                        Y = 0.75f
                    },
                    UniqueKey = "Mouse0",
                    StartDelay = 0.8,
                    EndDelay = 0.2
                });
            }
        }
        else
        {
            var startSpeed = _modConfig.NonEasyMode.StartSpeed;
            var endSpeed = _modConfig.NonEasyMode.EndSpeed;

            var startX = _modConfig.NonEasyMode.StartX;
            var endX = _modConfig.NonEasyMode.EndX;

            var startY = _modConfig.NonEasyMode.StartY;
            var endY = _modConfig.NonEasyMode.EndY;

            var count = _modConfig.NonEasyMode.NumberOfReps;
            var progressionType = _modConfig.NonEasyMode.ProgressionType;
            if (!Enum.IsDefined(typeof(QteProgressionType), progressionType))
            {
                logger.Warning($"[SimpleWorkoutQTE] ProgressionType is not valid. Must be Linear or Exponential. Defaulting to Linear.");
                progressionType = QteProgressionType.Linear;
            }

            for (var i = 0; i < count; i++)
            {
                var normalizedProgress = (count == 1) ? 0f : (float)i / (count - 1);
                var progress = progressionType switch
                {
                    QteProgressionType.Linear => normalizedProgress,
                    QteProgressionType.Exponential => MathF.Pow(normalizedProgress, 1.75f),
                    _ => normalizedProgress
                };

                var xProgress = progressionType == QteProgressionType.Exponential
                    ? MathF.Pow(MathF.Min(progress / 0.7f, 1f), 1.25f)
                    : progress;

                var newX = Math.Max(0f, MathF.Round(Lerp(startX, endX, xProgress), 2));
                var newY = Math.Max(0f, MathF.Round(Lerp(startY, endY, progress), 2));
                var speed = MathF.Round(Lerp(startSpeed, endSpeed, progress), 2);

                logger.Warning($"X{newX},Y{newY},S{speed}");
                newQteData.Add(new QuickTimeEvent
                {
                    EventType = QteType.ShrinkingCircle,
                    Coordinates = new Position
                    {
                        X = 0.5f,
                        Y = 0.25f
                    },
                    MovementSpeed = speed,
                    SuccessCoordinates = new Position
                    {
                        X = newX,
                        Y = newY
                    },
                    UniqueKey = "Mouse0",
                    StartDelay = 0.8,
                    EndDelay = 0.2
                });
            }
        }
        
        qteData.QuickTimeEvents = newQteData;
    }
    
    private static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    private void EditWorkout()
    {
        var results = databaseService.GetHideout().Qte[0].Results;
        if (results is null) 
            return;
        
        var finishEffect = results.First(x => x.Key == QteEffectType.finishEffect).Value;
        if (finishEffect.RewardEffects != null)
        {
            finishEffect.RewardEffects[0].Time = _modConfig.MusclePainTime;
        }
        
        var singleFailEffect = results.First(x => x.Key == QteEffectType.singleFailEffect).Value;
        if (singleFailEffect.RewardEffects != null)
        {
            singleFailEffect.Energy = _modConfig.FailureEnergyCost;
            singleFailEffect.Hydration = _modConfig.FailureHydrationCost;
        }
        
        var singleSuccessEffect = results.First(x => x.Key == QteEffectType.singleSuccessEffect).Value;
        if (singleSuccessEffect.RewardEffects != null)
        {
            singleSuccessEffect.Energy = _modConfig.SuccessEnergyCost;
            singleSuccessEffect.Hydration = _modConfig.SuccessHydrationCost;

            if (_modConfig.SkillRewardWeights.Count == 0)
            {
                logger.Error($"[SimpleWorkoutQTE] SkillReward config cannot be empty - ignoring config value for this section");
                return;
            }
            singleSuccessEffect.RewardEffects = new List<QteEffect>();
            foreach (var (skillName, weight) in _modConfig.SkillRewardWeights)
            {
                if (weight <= 0) 
                    continue;
                
                var effect = new QteEffect
                {
                    Weight = weight,
                    Result = QteResultType.None,
                    SkillId = skillName,
                    LevelMultipliers =
                    [
                        new SkillLevelMultiplier
                        {
                            Level = 0,
                            MultiplierValue = _modConfig.SkillLevel0ExpMultiplier
                        },
                        new SkillLevelMultiplier
                        {
                            Level = 10,
                            MultiplierValue = _modConfig.SkillLevel10ExpMultiplier
                        },
                        new SkillLevelMultiplier
                        {
                            Level = 20,
                            MultiplierValue = _modConfig.SkillLevel20ExpMultiplier
                        },
                        new SkillLevelMultiplier
                        {
                            Level = 30,
                            MultiplierValue = _modConfig.SkillLevel30ExpMultiplier
                        },
                        new SkillLevelMultiplier
                        {
                            Level = 40,
                            MultiplierValue = _modConfig.SkillLevel40ExpMultiplier
                        }
                    ],
                    Type = QteRewardType.Skill
                };

                singleSuccessEffect.RewardEffects.Add(effect);
            }
        }
    }
}
