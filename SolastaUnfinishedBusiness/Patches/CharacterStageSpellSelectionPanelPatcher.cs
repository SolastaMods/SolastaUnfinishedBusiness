using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterStageSpellSelectionPanelPatcher
{
    [HarmonyPatch(typeof(CharacterStageSpellSelectionPanel), nameof(CharacterStageSpellSelectionPanel.Refresh))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Refresh_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
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

            return instructions.ReplaceCalls(bindMethod, "CharacterStageSpellSelectionPanel.Refresh",
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, customBind));
        }

        [UsedImplicitly]
        private static void Postfix(CharacterStageSpellSelectionPanel __instance)
        {
            var levelTable = __instance.spellsByLevelTable;
            var viewWidth = __instance.spellsScrollRect.GetComponent<RectTransform>().rect.width;
            var spacing = levelTable.GetComponent<HorizontalLayoutGroup>().spacing;
            var totalWidth = 0f;
            var lastWidth = 0f;
            for (var i = 0; i < levelTable.childCount; ++i)
            {
                var child = levelTable.GetChild(i);
                if (!child.gameObject.activeSelf) { continue; }

                lastWidth = child.GetComponent<RectTransform>().rect.width + spacing;
                totalWidth += lastWidth;
            }

            levelTable.sizeDelta = new Vector2(
                //totalWidth + (viewWidth - lastWidth), //original, adds just enough space to last level to make it fit
                //but there's problem if there's more spells that can fit - do not add space then, as it is not needed
                totalWidth + Math.Max(viewWidth - lastWidth, 0),
                levelTable.sizeDelta.y);
        }
    }
}
