using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;

namespace SolastaUnfinishedBusiness.Patches;

public class SpellsByLevelBoxPatcher
{
    [HarmonyPatch(typeof(SpellsByLevelBox), "OnActivateStandardBox")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class OnActivateStandardBox_Patch
    {
        private static bool UniqueLevelSlots()
        {
            return false;
        }

        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: ensures multiclass warlock will use the correct slot level when casting spells using spell slots (MULTICLASS)
            var uniqueLevelSlotsMethod = typeof(FeatureDefinitionCastSpell).GetMethod("get_UniqueLevelSlots");
            var myUniqueLevelSlotsMethod = new Func<bool>(UniqueLevelSlots).Method;

            return instructions.ReplaceCalls(uniqueLevelSlotsMethod, "SpellsByLevelBox.OnActivateStandardBox",
                new CodeInstruction(OpCodes.Pop),
                new CodeInstruction(OpCodes.Call, myUniqueLevelSlotsMethod));
        }
    }
}
