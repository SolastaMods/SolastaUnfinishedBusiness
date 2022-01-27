using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUi.LevelUp
{
    [HarmonyPatch(typeof(SpellsByLevelGroup), "CommonBind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SpellsByLevelGroup_CommonBind
    {
        /*
         * So this patch seems like it should be unnecessary, however when collecting the autoprepared spells TA only looks at the FIRST
         * feature returned from enumerate features rather than iterate over all of them. This means only 1 feature worth of auto prepared
         * spells are shown to the user during level up. For the inspection/spell preparation binding a different method to collect the auto
         * prepared spells is used which works properly.
         */
        internal static void Prefix(SpellsByLevelGroup __instance, RulesetCharacter caster, ref List<SpellDefinition> auToPreparedSpells)
        {
            if (!Main.Settings.ShowAllAutoPreparedSpells)
            {
                return;
            }

            // Wait what? Yes, during level up no caster is bound. This is techncially fine, but we need one to collect the spells.
            if (caster == null)
            {
                var characterBuildingService = ServiceRepository.GetService<ICharacterBuildingService>();

                caster = characterBuildingService.HeroCharacter;
            }

            // Collect the auto prepared spells for all spell repertoirs (we also could have enumerated features and iterated through all)
            // either way works.
            // Also filter the prepped spells by level this group is displaying.
            foreach (RulesetSpellRepertoire rulesetSpellRepertoire in caster.SpellRepertoires)
            {
                foreach (SpellDefinition item in rulesetSpellRepertoire.AutoPreparedSpells)
                {
                    var flag = !auToPreparedSpells.Contains(item) && __instance.SpellLevel == item.SpellLevel;

                    if (flag)
                    {
                        auToPreparedSpells.Add(item);
                    }
                }
            }
        }
    }
}
