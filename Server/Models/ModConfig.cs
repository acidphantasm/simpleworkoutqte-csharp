using SPTarkov.Server.Core.Models.Enums;

namespace _simpleWorkoutQTE.Models;

public record ModConfig
{
    public required EasyMode EasyMode  { get; set; }
    public required NonEasyMode NonEasyMode { get; set; }
    public long MusclePainTime { get; set; }
    public int SuccessEnergyCost { get; set; }
    public int SuccessHydrationCost { get; set; }
    public int FailureEnergyCost { get; set; }
    public int FailureHydrationCost { get; set; }
    public float SkillLevel0ExpMultiplier {  get; set; }
    public float SkillLevel10ExpMultiplier {  get; set; }
    public float SkillLevel20ExpMultiplier {  get; set; }
    public float SkillLevel30ExpMultiplier {  get; set; }
    public float SkillLevel40ExpMultiplier {  get; set; }
    public required Dictionary<SkillTypes, int> SkillRewardWeights { get; set; }
    public required string SkillReferenceSheet { get; set; }
}