using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.FirstLevelCasterFeats
{
    [HarmonyPatch(typeof(GuiFeatDefinition), "IsFeatMacthingPrerequisites")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GuiFeatDefinition_IsFeatMacthingPrerequisites
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            var callSpellRepertoiresIndex = codes.FindIndex(c => c.Calls(typeof(RulesetCharacter).GetMethod("get_SpellRepertoires")));
            codes[callSpellRepertoiresIndex] = new CodeInstruction(System.Reflection.Emit.OpCodes.Call, 
                                                                   new Func<RulesetCharacterHero, int>(canCastSpells).Method
                                                                   );
            codes.RemoveAt(callSpellRepertoiresIndex + 1);
            return codes;
        }


        static private int canCastSpells(RulesetCharacterHero hero)
        {
            
            if (Main.Settings.EnableFirstLevelCasterFeats)
            {
                //Replace call to RulesetCharacterHero.SpellRepertores.Count with Count list of FeatureCastSpell 
                //which are registered before feat selection at lvl 1
                var list = new List<FeatureDefinition>();
                hero.EnumerateFeaturesToBrowse<FeatureDefinitionCastSpell>(list, null);
                return list.Count;
            }
            else
            {
                return hero.SpellRepertoires.Count;
            }
               

        }
    }
}
