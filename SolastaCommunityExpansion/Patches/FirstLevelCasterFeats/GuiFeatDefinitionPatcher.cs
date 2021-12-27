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
            //Replace call to RulesetCharacterHero.SpellRepertores with list of FeatureCastSpell 
            //which are registered before feat selection at lvl 1
            var codes = instructions.ToList();
            var callSpellRepertoiresIndex = codes.FindIndex(c => c.Calls(typeof(RulesetCharacter).GetMethod("get_SpellRepertoires")));
            codes[callSpellRepertoiresIndex] = new CodeInstruction(System.Reflection.Emit.OpCodes.Call, 
                                                                   new Func<RulesetCharacterHero, List<FeatureDefinition>>(getFeaturesCastSpell).Method
                                                                   );
            return codes;
        }


        static private List<FeatureDefinition> getFeaturesCastSpell(RulesetCharacterHero hero)
        {
            var list = new List<FeatureDefinition>();
            hero.EnumerateFeaturesToBrowse<FeatureDefinitionCastSpell>(list, null);
            return list;
        }
    }
}
