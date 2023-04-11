using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class SpellSelectionPanelPatcher
{
    [HarmonyPatch(typeof(SpellSelectionPanel), nameof(SpellSelectionPanel.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static void Prefix(
            GuiCharacter caster,
            ref bool cantripOnly,
            ActionDefinitions.ActionType actionType)
        {
            //PATCH: supports `IReplaceAttackWithCantrip`
            var gameLocationCaster = caster.GameLocationCharacter;

            if (gameLocationCaster.RulesetCharacter.HasSubFeatureOfType<IReplaceAttackWithCantrip>()
                && gameLocationCaster.UsedMainAttacks > 0 && actionType == ActionDefinitions.ActionType.Main)
            {
                cantripOnly = true;
            }
        }

        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: hide spell panels for repertoires that have hidden spell casting feature
            var getRepertoires = typeof(RulesetCharacter).GetMethod("get_SpellRepertoires");
            var getVisibleRepertoires = new Func<RulesetCharacter, List<RulesetSpellRepertoire>>(GetRepertoires).Method;

            return instructions.ReplaceCalls(getRepertoires, "SpellSelectionPanel.Bind",
                new CodeInstruction(OpCodes.Call, getVisibleRepertoires));
        }

        private static List<RulesetSpellRepertoire> GetRepertoires(RulesetCharacter character)
        {
            return character.SpellRepertoires
                .Where(r => !r.SpellCastingFeature.GuiPresentation.Hidden)
                .ToList();
        }
    }
}
