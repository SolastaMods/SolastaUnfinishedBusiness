using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

public static class CharacterStageSpellSelectionPanelPatcher
{
    [HarmonyPatch(typeof(CharacterStageSpellSelectionPanel), "Refresh")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Refresh_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: Multiclass: Replaces calls to `BindLearning` with custom one, that processes spell visibility/tags for multiclass
            var bindMethod = typeof(SpellsByLevelGroup).GetMethod("BindLearning");
            var customBind = new Action<
                SpellsByLevelGroup, // group,
                ICharacterBuildingService, // characterBuildingService,
                FeatureDefinitionCastSpell, // spellFeature,
                SpellListDefinition, // spellListDefinition,
                List<string>, // restrictedSchools,
                bool, // ritualOnly,
                int, // spellLevel,
                SpellBox.SpellBoxChangedHandler, // spellBoxChanged,
                List<SpellDefinition>, // knownSpells,
                List<SpellDefinition>, // unlearnedSpells,
                string, // spellTag, 
                bool, // canAcquireSpells, 
                bool, // unlearn,
                RectTransform, // tooltipAnchor,
                TooltipDefinitions.AnchorMode, // anchorMode,
                CharacterStageSpellSelectionPanel // panel
            >(MulticlassGameUiContext.SpellsByLevelGroupBindLearning).Method;

            return instructions.ReplaceAllCalls(bindMethod,
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, customBind));
        }
    }
}
