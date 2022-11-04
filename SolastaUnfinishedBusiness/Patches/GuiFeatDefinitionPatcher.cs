using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

public static class GuiFeatDefinitionPatcher
{
    [HarmonyPatch(typeof(GuiFeatDefinition), "IsFeatMatchingPrerequisites")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class IsFeatMatchingPrerequisites_Patch
    {
        public static void Postfix(
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
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();

            //PATCH: Removes annoying error message on DEBUG builds
            var assertCheckIndex = codes.FindIndex(c => c.opcode == OpCodes.Ldc_I4_1);

            if (assertCheckIndex != -1)
            {
                codes[assertCheckIndex] = new CodeInstruction(OpCodes.Ldc_I4_0);
            }

            //PATCH: Replace call to RulesetCharacterHero.SpellRepertoires.Count with Count list of FeatureCastSpell
            //which are registered before feat selection at lvl 1
            return codes.ReplaceCode(c =>
                    c.Calls(typeof(RulesetCharacter).GetMethod("get_SpellRepertoires")),
                1,
                1,
                new CodeInstruction(OpCodes.Call,
                    new Func<RulesetCharacterHero, int>(CanCastSpells).Method));
        }

        private static int CanCastSpells([NotNull] RulesetCharacterHero hero)
        {
            return hero.EnumerateFeaturesToBrowse<FeatureDefinitionCastSpell>().Count;
        }
    }
}
