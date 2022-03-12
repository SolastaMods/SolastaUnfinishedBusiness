using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Multiclass.Models;

namespace SolastaCommunityExpansion.Multiclass.Patches.HeroInspection
{
    internal static class CharacterInformationPanelPatcher
    {
        // overrides the selected class search term with the one determined by the hotkeys / enumerate class badges logic
        [HarmonyPatch(typeof(CharacterInformationPanel), "Refresh")]
        internal static class CharacterInformationPanelRefresh
        {
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    foreach (var instruction in instructions)
                    {
                        yield return instruction;
                    }

                    yield break;
                }

                var containsMethod = typeof(string).GetMethod("Contains");
                var getSelectedClassSearchTermMethod = typeof(InspectionPanelContext).GetMethod("GetSelectedClassSearchTerm");
                var enumerateClassBadgesMethod = typeof(CharacterInformationPanel).GetMethod("EnumerateClassBadges", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                var myEnumerateClassBadgesMethod = typeof(InspectionPanelContext).GetMethod("EnumerateClassBadges");
                var found = 0;

                foreach (var instruction in instructions)
                {
                    if (instruction.Calls(containsMethod))
                    {
                        found++;

                        if (found == 2 || found == 3)
                        {
                            yield return new CodeInstruction(OpCodes.Call, getSelectedClassSearchTermMethod);
                        }

                        yield return instruction;
                    }
                    else if (instruction.Calls(enumerateClassBadgesMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Call, myEnumerateClassBadgesMethod);
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }
        }

        // filter active features
        [HarmonyPatch(typeof(CharacterInformationPanel), "TryFindChoiceFeature")]
        internal static class CharacterInformationPanelTryFindChoiceFeature
        {
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    foreach (var instruction in instructions)
                    {
                        yield return instruction;
                    }

                    yield break;
                }

                var classFeatureUnlocksMethod = typeof(CharacterClassDefinition).GetMethod("get_FeatureUnlocks");
                var classFilteredFeatureUnlocksMethod = typeof(LevelUpContext).GetMethod("ClassFilteredFeatureUnlocks");

                var subclassFeatureUnlocksMethod = typeof(CharacterSubclassDefinition).GetMethod("get_FeatureUnlocks");
                var subclassFilteredFeatureUnlocksMethod = typeof(LevelUpContext).GetMethod("SubclassFilteredFeatureUnlocks");

                var inspectedCharacterMethod = typeof(CharacterInformationPanel).GetMethod("get_InspectedCharacter");
                var rulesetCharacterHeroMethod = typeof(GuiCharacter).GetMethod("get_RulesetCharacterHero");

                foreach (var instruction in instructions)
                {
                    if (instruction.Calls(classFeatureUnlocksMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Call, inspectedCharacterMethod);
                        yield return new CodeInstruction(OpCodes.Call, rulesetCharacterHeroMethod);
                        yield return new CodeInstruction(OpCodes.Call, classFilteredFeatureUnlocksMethod);
                    }
                    else if (instruction.Calls(subclassFeatureUnlocksMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Call, inspectedCharacterMethod);
                        yield return new CodeInstruction(OpCodes.Call, rulesetCharacterHeroMethod);
                        yield return new CodeInstruction(OpCodes.Call, subclassFilteredFeatureUnlocksMethod);
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }
        }
    }
}
