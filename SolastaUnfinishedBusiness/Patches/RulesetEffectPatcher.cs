using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Subclasses;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RulesetEffectPatcher
{
    //PATCH: supports Oath of Ancients / Oath of Dread level 20 powers
    [HarmonyPatch(typeof(RulesetEffect), nameof(RulesetEffect.ConditionSaveRerollRequested))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    // ReSharper disable once InconsistentNaming
    public static class EffectDescription_Getter_Patch
    {
        [UsedImplicitly]
        public static void Prefix(
            RulesetEffect __instance,
            RulesetActor character)
        {
            switch (__instance)
            {
                case RulesetEffectSpell rulesetEffectSpell:
                {
                    var caster = rulesetEffectSpell.Caster;
                    var sourceDefinition = rulesetEffectSpell.SourceDefinition;

                    OathOfAncients.OnRollSavingThrowElderChampion(caster, character, sourceDefinition);
                    OathOfDread.OnRollSavingThrowAspectOfDread(caster, character, sourceDefinition);
                    break;
                }
                case RulesetEffectPower rulesetEffectPower:
                {
                    var caster = rulesetEffectPower.User;
                    var sourceDefinition = rulesetEffectPower.SourceDefinition;

                    OathOfAncients.OnRollSavingThrowElderChampion(caster, character, sourceDefinition);
                    OathOfDread.OnRollSavingThrowAspectOfDread(caster, character, sourceDefinition);
                    break;
                }
            }
        }
    }
}
