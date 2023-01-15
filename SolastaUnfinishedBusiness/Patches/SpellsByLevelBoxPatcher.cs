using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class SpellsByLevelBoxPatcher
{
    [HarmonyPatch(typeof(SpellsByLevelBox), "OnActivateStandardBox")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnActivateStandardBox_Patch
    {
        private static bool UniqueLevelSlots(SpellsByLevelBox spellsByLevelBox)
        {
            var hero = spellsByLevelBox.spellRepertoire.GetCasterHero();

            return !SharedSpellsContext.IsMulticaster(hero) &&
                   spellsByLevelBox.spellRepertoire.SpellCastingFeature.UniqueLevelSlots;
        }

        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: ensures multiclass warlock will use the correct slot level when casting spells using spell slots (MULTICLASS)
            var uniqueLevelSlotsMethod = typeof(FeatureDefinitionCastSpell).GetMethod("get_UniqueLevelSlots");
            var myUniqueLevelSlotsMethod = new Func<SpellsByLevelBox, bool>(UniqueLevelSlots).Method;

            return instructions.ReplaceCalls(uniqueLevelSlotsMethod, "SpellsByLevelBox.OnActivateStandardBox",
                new CodeInstruction(OpCodes.Pop),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, myUniqueLevelSlotsMethod));
        }
    }
}
