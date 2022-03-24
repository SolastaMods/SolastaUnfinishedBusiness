using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Patches.Bugfix
{
    [HarmonyPatch(typeof(GuiFeatDefinition), "IsFeatMacthingPrerequisites")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GuiFeatDefinition_IsFeatMatchingPrerequisites
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            var callSpellRepertoiresIndex = codes.FindIndex(c => c.Calls(typeof(RulesetCharacter).GetMethod("get_SpellRepertoires")));

            if (callSpellRepertoiresIndex != -1)
            {
                codes[callSpellRepertoiresIndex] = new CodeInstruction(OpCodes.Call, new Func<RulesetCharacterHero, int>(CanCastSpells).Method);
                codes.RemoveAt(callSpellRepertoiresIndex + 1);
            }
            else
            {
                Main.Log("GuiFeatDefinition_IsFeatMatchingPrerequisites transpiler: Unable to find 'get_SpellRepertoires'");
            }

            return codes;
        }

        private static int CanCastSpells(RulesetCharacterHero hero)
        {
            return Main.Settings.EnableFirstLevelCasterFeats
                // Replace call to RulesetCharacterHero.SpellRepertores.Count with Count list of FeatureCastSpell 
                // which are registered before feat selection at lvl 1
                ? hero.EnumerateFeaturesToBrowse<FeatureDefinitionCastSpell>().Count
                : hero.SpellRepertoires.Count;
        }
    }
}
