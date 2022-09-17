using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Patches;

internal static class SpellSelectionPanelPatcher
{
    private static readonly List<RectTransform> SpellLineTables = new();

    [HarmonyPatch(typeof(SpellSelectionPanel), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Bind_Patch
    {
        internal static void Prefix(
            GuiCharacter caster,
            ref bool cantripOnly,
            ActionDefinitions.ActionType actionType)
        {
            //PATCH: support for `IReplaceAttackWithCantrip`
            var gameLocationCaster = caster.GameLocationCharacter;
            if (gameLocationCaster.RulesetCharacter.HasSubFeatureOfType<IReplaceAttackWithCantrip>() &&
                gameLocationCaster.UsedMainAttacks > 0 && actionType == ActionDefinitions.ActionType.Main)
            {
                cantripOnly = true;
            }
        }

        internal static void Postfix(SpellSelectionPanel __instance, GuiCharacter caster,
            SpellsByLevelBox.SpellCastEngagedHandler spellCastEngaged, ActionDefinitions.ActionType actionType,
            bool cantripOnly)
        {
            //PATCH: show spell selection on multiple rows
            if (!Main.Settings.EnableMultiLineSpellPanel)
            {
                return;
            }

            var spellRepertoireLines = __instance.spellRepertoireLines;
            var spellRepertoireSecondaryLine = __instance.spellRepertoireSecondaryLine;
            var spellRepertoireLinesTable = __instance.spellRepertoireLinesTable;
            var slotAdvancementPanel = __instance.SlotAdvancementPanel;

            foreach (var spellRepertoireLine in spellRepertoireLines)
            {
                spellRepertoireLine.Unbind();
            }

            spellRepertoireLines.Clear();
            Gui.ReleaseChildrenToPool(spellRepertoireLinesTable);
            spellRepertoireSecondaryLine.Unbind();
            spellRepertoireSecondaryLine.gameObject.SetActive(false);

            if (spellRepertoireLinesTable.parent.GetComponent<VerticalLayoutGroup>() == null)
            {
                GameObject spellLineHolder = new();

                var vertGroup = spellLineHolder.AddComponent<VerticalLayoutGroup>();

                vertGroup.spacing = 10;
                spellLineHolder.AddComponent<ContentSizeFitter>();
                spellLineHolder.transform.SetParent(spellRepertoireLinesTable.parent, true);
                spellLineHolder.transform.SetAsFirstSibling();
                spellLineHolder.transform.localScale = Vector3.one;
                spellRepertoireLinesTable.SetParent(spellLineHolder.transform);
            }

            var spellRepertoires = __instance.Caster.RulesetCharacter.SpellRepertoires;
            var needNewLine = true;
            var lineIndex = 0;
            var indexOfLine = 0;
            var spellLevelsOnLine = 0;
            var curTable = spellRepertoireLinesTable;

            foreach (var rulesetSpellRepertoire in spellRepertoires)
            {
                var startLevel = 0;
                var maxLevel = rulesetSpellRepertoire.MaxSpellLevelOfSpellCastingLevel;

                for (var level = startLevel; level <= maxLevel; level++)
                {
                    if (!IsLevelActive(rulesetSpellRepertoire, level, actionType))
                    {
                        continue;
                    }

                    spellLevelsOnLine++;

                    if (spellLevelsOnLine < Main.Settings.MaxSpellLevelsPerLine)
                    {
                        continue;
                    }

                    curTable = AddActiveSpellsToLine(
                        __instance,
                        spellCastEngaged,
                        actionType,
                        cantripOnly,
                        spellRepertoireLines,
                        curTable,
                        slotAdvancementPanel,
                        spellRepertoires,
                        needNewLine,
                        lineIndex,
                        indexOfLine,
                        rulesetSpellRepertoire,
                        startLevel,
                        level);

                    startLevel = level + 1;
                    lineIndex++;
                    spellLevelsOnLine = 0;
                    needNewLine = true;
                    indexOfLine = 0;
                }

                if (spellLevelsOnLine == 0)
                {
                    continue;
                }

                curTable = AddActiveSpellsToLine(
                    __instance,
                    spellCastEngaged,
                    actionType,
                    cantripOnly,
                    spellRepertoireLines,
                    curTable,
                    slotAdvancementPanel,
                    spellRepertoires,
                    needNewLine,
                    lineIndex,
                    indexOfLine,
                    rulesetSpellRepertoire,
                    startLevel,
                    rulesetSpellRepertoire.MaxSpellLevelOfSpellCastingLevel);

                needNewLine = false;
                indexOfLine++;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(curTable);
            __instance.RectTransform.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal,
                spellRepertoireLinesTable.rect.width);
        }

        private static RectTransform AddActiveSpellsToLine(
            SpellSelectionPanel __instance,
            SpellsByLevelBox.SpellCastEngagedHandler spellCastEngaged,
            ActionDefinitions.ActionType actionType,
            bool cantripOnly,
            ICollection<SpellRepertoireLine> spellRepertoireLines,
            RectTransform spellRepertoireLinesTable,
            SlotAdvancementPanel slotAdvancementPanel,
            IReadOnlyCollection<RulesetSpellRepertoire> spellRepertoires,
            bool needNewLine,
            int lineIndex,
            int indexOfLine,
            RulesetSpellRepertoire rulesetSpellRepertoire,
            int startLevel,
            int level)
        {
            if (needNewLine)
            {
                var previousTable = spellRepertoireLinesTable;
                LayoutRebuilder.ForceRebuildLayoutImmediate(previousTable);

                if (lineIndex > 0)
                {
                    // instantiate new table
                    spellRepertoireLinesTable =
                        Object.Instantiate(spellRepertoireLinesTable, previousTable.parent.transform);
                    // clear it of children
                    spellRepertoireLinesTable.DetachChildren();
                    //spellRepertoireLinesTable.SetParent(previousTable.parent.transform, true);
                    spellRepertoireLinesTable.localScale = previousTable.localScale;
                    spellRepertoireLinesTable.transform.SetAsFirstSibling();
                    SpellLineTables.Add(spellRepertoireLinesTable);
                }
            }

            var curLine = SetUpNewLine(indexOfLine, spellRepertoireLinesTable, spellRepertoireLines, __instance);

            curLine.Bind(
                __instance.Caster,
                rulesetSpellRepertoire,
                spellRepertoires.Count > 1,
                spellCastEngaged,
                slotAdvancementPanel,
                actionType,
                cantripOnly,
                startLevel,
                level,
                false);

            return spellRepertoireLinesTable;
        }

        private static SpellRepertoireLine SetUpNewLine(
            int index,
            Transform spellRepertoireLinesTable,
            ICollection<SpellRepertoireLine> spellRepertoireLines,
            SpellSelectionPanel __instance)
        {
            GameObject newLine;

            if (spellRepertoireLinesTable.childCount <= index)
            {
                newLine = Gui.GetPrefabFromPool(__instance.spellRepertoireLinePrefab,
                    spellRepertoireLinesTable);
            }
            else
            {
                newLine = spellRepertoireLinesTable.GetChild(index).gameObject;
            }

            newLine.SetActive(true);

            var component = newLine.GetComponent<SpellRepertoireLine>();

            spellRepertoireLines.Add(component);

            return component;
        }

        private static bool IsLevelActive(
            RulesetSpellRepertoire spellRepertoire, int level,
            ActionDefinitions.ActionType actionType)
        {
            var spellActivationTime = actionType switch
            {
                ActionDefinitions.ActionType.Bonus => RuleDefinitions.ActivationTime.BonusAction,
                ActionDefinitions.ActionType.Main => RuleDefinitions.ActivationTime.Action,
                ActionDefinitions.ActionType.Reaction => RuleDefinitions.ActivationTime.Reaction,
                ActionDefinitions.ActionType.NoCost => RuleDefinitions.ActivationTime.NoCost,
                _ => RuleDefinitions.ActivationTime.Action
            };

            if (level == 0)
            {
                // changed to support game v1.3.44 and allow ancestry cantrips to display off battle
                return actionType == ActionDefinitions.ActionType.None ||
                       spellRepertoire.KnownCantrips.Any(cantrip => cantrip.ActivationTime == spellActivationTime);
            }

            switch (spellRepertoire.SpellCastingFeature.SpellReadyness)
            {
                case RuleDefinitions.SpellReadyness.Prepared when spellRepertoire.PreparedSpells
                    .Any(spellDefinition =>
                        spellDefinition.SpellLevel == level
                        && spellDefinition.ActivationTime == spellActivationTime):
                case RuleDefinitions.SpellReadyness.AllKnown
                    when spellRepertoire.KnownSpells.Any(spellDefinition => spellDefinition.SpellLevel == level):

                    return true;

                default:
                    return false;
            }
        }
    }

    [HarmonyPatch(typeof(SpellSelectionPanel), "Unbind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Unbind_Patch
    {
        internal static void Postfix()
        {
            //PATCH: show spell selection on multiple rows
            if (!Main.Settings.EnableMultiLineSpellPanel)
            {
                return;
            }

            foreach (var spellTable in SpellLineTables
                         .Where(spellTable =>
                             spellTable != null && spellTable.gameObject.activeSelf && spellTable.childCount > 0))
            {
                Gui.ReleaseChildrenToPool(spellTable);
                spellTable.SetParent(null);
                Object.Destroy(spellTable.gameObject);
            }

            SpellLineTables.Clear();
        }
    }
}
