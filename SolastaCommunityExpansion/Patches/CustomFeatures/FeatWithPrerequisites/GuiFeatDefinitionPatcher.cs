using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Builders;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.FeatWithPrerequisites
{
    [HarmonyPatch(typeof(GuiFeatDefinition), "IsFeatMacthingPrerequisites")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal  class GuiFeatDefinition_IsFeatMatchingPrerequisites
    {
        internal static void Postfix(
            ref bool __result,
            FeatDefinition feat,
            RulesetCharacterHero hero,
            ref string prerequisiteOutput)
        {
            if (feat is not FeatDefinitionWithPrerequisites featDefinitionWithPrerequisites || featDefinitionWithPrerequisites.Validators.Count == 0)
            {
                return;
            }

            var (result, output) = featDefinitionWithPrerequisites.Validate(feat, hero);

            __result &= result;
            prerequisiteOutput += "\n" + output;
        }

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
                ? 1
                : hero.SpellRepertoires.Count;
        }
    }
}
