using System.Linq;
using HarmonyLib;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaCommunityExpansion.Multiclass.Patches.PowersAndPools
{
    internal static class RulesetCharacterPatcher
    {
        // ensures that original character power pools are in sync with substitute 
        [HarmonyPatch(typeof(RulesetCharacter), "UsePower")]
        internal static class RulesetCharacterUsePower
        {
            internal static void Postfix(RulesetCharacter __instance, RulesetUsablePower usablePower)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                if (__instance is RulesetCharacterMonster monster && monster.IsSubstitute && usablePower.PowerDefinition == PowerBarbarianRageStart)
                {
                    var rulesetHero = Gui.GameCampaign.Party.CharactersList.Find(x => x.RulesetCharacter.Name == __instance.Name)?.RulesetCharacter;

                    rulesetHero?.SpendRagePoint();
                }
            }
        }

        [HarmonyPatch(typeof(RulesetCharacter), "FindSpellRepertoireOfPower")]
        internal static class RulesetCharacterFindSpellRepertoireOfPower
        {
            internal static void Postfix(RulesetCharacter __instance, ref RulesetSpellRepertoire __result, RulesetUsablePower usablePower)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                if (__result == null && __instance is RulesetCharacterMonster rulesetCharacterMonster && rulesetCharacterMonster.IsSubstitute)
                {
                    var rulesetHero = Gui.GameCampaign.Party.CharactersList.Find(x => x.RulesetCharacter.Name == __instance.Name)?.RulesetCharacter;

                    __result = rulesetHero?.SpellRepertoires.FirstOrDefault(x => x.SpellCastingFeature == usablePower.PowerDefinition.SpellcastingFeature);
                }
            }
        }
    }
}
