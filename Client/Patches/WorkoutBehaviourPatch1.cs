using System;
using System.Linq;
using System.Reflection;
using EFT.Communications;
using EFT.Hideout;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace _simpleWorkoutQTE.Patches
{
    internal class WorkoutBehaviourPatch1 : ModulePatch
    {
        private static readonly Random Random = new Random();
        
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(WorkoutBehaviour), nameof(WorkoutBehaviour.method_18));
        }

        [PatchPrefix]
        private static bool PatchPrefix(WorkoutBehaviour __instance)
        {
            var array = __instance.QteHandleData_0.Results[QteData.EQteEffectType.SingleSuccessEffect].Effects.Where(new Func<QteEffect, bool>(__instance.method_21)).ToArray<QteEffect>();
            if (array.Length == 0)
            {
                return true;
            }
            
            QteEffect selectedEffect = null;
            
            var totalWeight = array.Sum(effect => effect.Weight);
            if (totalWeight <= 0f)
            {
                var fallbackIndex = (array.Length == 1) ? 0 : Random.Next(0, array.Length);
                selectedEffect = array[fallbackIndex];
            }
            else
            {
                var roll = (float)(Random.NextDouble() * totalWeight);
                var cumulative = 0f;

                foreach (var effect in array)
                {
                    cumulative += effect.Weight;
                    if (roll < cumulative)
                    {
                        selectedEffect = effect;
                        break;
                    }
                }
            }
            
            selectedEffect ??= array[^1];

            var skill = __instance.HideoutPlayerOwner_0.HideoutPlayer.Skills.GetSkill(selectedEffect.Skill);
            var gymEffectivity = (__instance.HealthControllerClass.HasSevereMusclePainEffect() ? __instance.HealthControllerClass.GetSevereMusclePainSettings().GymEffectivity : (__instance.HealthControllerClass.HasMildMusclePainEffect() ? __instance.HealthControllerClass.GetMildMusclePainSettings().GymEffectivity : 0f));
            var skillExpMultiplier = 0f;
            foreach (QteEffect.SkillExperienceMultiplierData skillExperienceMultiplierData in selectedEffect.SkillExpMultiplierData)
            {
                if (skill.Level >= skillExperienceMultiplierData.level)
                {
                    skillExpMultiplier = skillExperienceMultiplierData.value;
                }
            }
            var factor = skillExpMultiplier - skillExpMultiplier * gymEffectivity;
            var factorValue = __instance.HideoutPlayerOwner_0.HideoutPlayer.Skills.SkillProgress.Factor(factor, true).FactorValue;
            var skillProgress = ((skill.Level >= 9) ? factorValue : skill.CalculateExpOnFirstLevels(factorValue));
            skill.SetCurrent(skill.Current + skillProgress, true);
            skill.AddPointsEarnedForWorkout(skillProgress);
            NotificationManagerClass.DisplayNotification(new GClass2551(string.Format("Skill '{0}' increased by {1}".Localized(null), skill.Id.ToString().Localized(null), Math.Round((double)factorValue, 2)), ENotificationDurationType.Default, ENotificationIconType.Default, null));

            Plugin.LogSource.LogDebug($"Skill: {selectedEffect.Skill} - Weight: {selectedEffect.Weight} - Points Gained: {skillProgress}");
            return false;
        }
    }
}