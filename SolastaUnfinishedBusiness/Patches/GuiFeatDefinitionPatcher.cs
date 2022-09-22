using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[HarmonyPatch(typeof(GuiFeatDefinition), "IsFeatMatchingPrerequisites")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GuiFeatDefinition_IsFeatMatchingPrerequisites
{
    internal static void Postfix(
        ref bool __result,
        FeatDefinition feat,
        RulesetCharacterHero hero,
        ref string prerequisiteOutput)
    {
        //PATCH: Enforces Feats With PreRequisites
        if (feat is not FeatDefinitionWithPrerequisites featDefinitionWithPrerequisites
            || featDefinitionWithPrerequisites.Validators.Count == 0)
        {
            return;
        }

        var (result, output) = featDefinitionWithPrerequisites.Validate(featDefinitionWithPrerequisites, hero);

        __result = __result && result;
        prerequisiteOutput += '\n' + output;
    }

    [NotNull]
    internal static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
    {
        //PATCH: Replace call to RulesetCharacterHero.SpellRepertoires.Count with Count list of FeatureCastSpell
        //which are registered before feat selection at lvl 1
        var codes = instructions.ToList();
        var callSpellRepertoiresIndex =
            codes.FindIndex(c => c.Calls(typeof(RulesetCharacter).GetMethod("get_SpellRepertoires")));

        codes[callSpellRepertoiresIndex] = new CodeInstruction(OpCodes.Call,
            new Func<RulesetCharacterHero, int>(CanCastSpells).Method);
        codes.RemoveAt(callSpellRepertoiresIndex + 1);

        //PATCH: fix in DEBUG build to avoid the annoying assert statement about Feats acquired at level 1
        //it replaces the Trace comparision ClassesHistory.Count > 1 with ClassesHistory.Count > 0
        if (!Main.IsDebugBuild)
        {
            return codes;
        }

        var assertCheckIndex = codes.FindIndex(c => c.opcode == OpCodes.Ldc_I4_1);

        if (assertCheckIndex != -1)
        {
            codes[assertCheckIndex] = new CodeInstruction(OpCodes.Ldc_I4_0);
        }

        return codes;
    }

    private static int CanCastSpells([NotNull] RulesetCharacterHero hero)
    {
        return hero.EnumerateFeaturesToBrowse<FeatureDefinitionCastSpell>().Count;
    }
}
