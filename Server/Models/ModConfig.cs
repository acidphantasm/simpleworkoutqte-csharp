using SPTarkov.Server.Core.Models.Enums;

namespace SimpleWorkoutQTE.Models;

public record ModConfig
{
    public EasyMode EasyMode { get; set; } = new EasyMode();
    public NonEasyMode NonEasyMode { get; set; } = new NonEasyMode();
    public long MusclePainTime { get; set; } = 43200;
    public int SuccessEnergyCost { get; set; } = -2;
    public int SuccessHydrationCost { get; set; } = -2;
    public int FailureEnergyCost { get; set; } = -4;
    public int FailureHydrationCost { get; set; } = -4;
    public float SkillLevel0ExpMultiplier { get; set; } = 6;
    public float SkillLevel10ExpMultiplier { get; set; } = 8;
    public float SkillLevel20ExpMultiplier { get; set; } = 10;
    public float SkillLevel30ExpMultiplier { get; set; } = 12;
    public float SkillLevel40ExpMultiplier { get; set; } = 14;

    public Dictionary<string, int> SkillRewardWeights { get; set; } = new()
    {
        [nameof(SkillTypes.Strength)] = 25,
        [nameof(SkillTypes.Endurance)] = 25,
        [nameof(SkillTypes.Vitality)] = 0,
        [nameof(SkillTypes.Health)] = 0,
        [nameof(SkillTypes.StressResistance)] = 0,
        [nameof(SkillTypes.Metabolism)] = 0,
        [nameof(SkillTypes.Immunity)] = 0,
        [nameof(SkillTypes.Perception)] = 0,
        [nameof(SkillTypes.Intellect)] = 0,
        [nameof(SkillTypes.Attention)] = 0,
        [nameof(SkillTypes.Charisma)] = 0,
        [nameof(SkillTypes.MagDrills)] = 0,
        [nameof(SkillTypes.CovertMovement)] = 0,
        [nameof(SkillTypes.Surgery)] = 0,
        [nameof(SkillTypes.Search)] = 0,
        [nameof(SkillTypes.AimDrills)] = 0,
    };

    public string SkillReferenceSheet { get; set; } =
        "You can add any enum value from this reference sheet to the SkillRewardWeights, I only put the ones that seemed relevant though -> https://wiki.sp-tarkov.com/en/modding/references/skills-reference";
}