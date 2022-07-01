using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterPanel;

internal static class SpellSelectionPanelPatcher
{
    private static readonly List<RectTransform> spellLineTables = new();

    // second line bind
    [HarmonyPatch(typeof(SpellSelectionPanel), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SpellSelectionPanel_Bind
    {
        internal static void Postfix(SpellSelectionPanel __instance, GameLocationCharacter caster,
            SpellsByLevelBox.SpellCastEngagedHandler spellCastEngaged, ActionDefinitions.ActionType actionType,
            bool cantripOnly)
        {
            if (!Main.Settings.EnableMultiLineSpellPanel)
            {
                return;
            }

            var spellRepertoireLines = __instance.spellRepertoireLines;
            var spellRepertoireSecondaryLine =
                __instance.spellRepertoireSecondaryLine;
            var spellRepertoireLinesTable = __instance.spellRepertoireLinesTable;
            var slotAdvancementPanel = __instance.slotAdvancementPanel;

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

                for (var level = startLevel;
                     level <= rulesetSpellRepertoire.MaxSpellLevelOfSpellCastingLevel;
                     level++)
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

                    curTable = AddActiveSpellsToLine(__instance, caster, spellCastEngaged, actionType,
                        cantripOnly, spellRepertoireLines, curTable, slotAdvancementPanel, spellRepertoires,
                        needNewLine, lineIndex, indexOfLine, rulesetSpellRepertoire, startLevel, level);
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

                curTable = AddActiveSpellsToLine(__instance, caster, spellCastEngaged, actionType, cantripOnly,
                    spellRepertoireLines, curTable, slotAdvancementPanel, spellRepertoires, needNewLine,
                    lineIndex, indexOfLine, rulesetSpellRepertoire, startLevel,
                    rulesetSpellRepertoire.MaxSpellLevelOfSpellCastingLevel);
                needNewLine = false;
                indexOfLine++;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(curTable);
            __instance.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                spellRepertoireLinesTable.rect.width);
        }

        private static RectTransform AddActiveSpellsToLine(SpellSelectionPanel __instance,
            GameLocationCharacter caster,
            SpellsByLevelBox.SpellCastEngagedHandler spellCastEngaged, ActionDefinitions.ActionType actionType,
            bool cantripOnly,
            List<SpellRepertoireLine> spellRepertoireLines, RectTransform spellRepertoireLinesTable,
            SlotAdvancementPanel slotAdvancementPanel, List<RulesetSpellRepertoire> spellRepertoires,
            bool needNewLine, int lineIndex,
            int indexOfLine, RulesetSpellRepertoire rulesetSpellRepertoire, int startLevel, int level)
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
                    spellLineTables.Add(spellRepertoireLinesTable);
                }
            }

            var curLine = SetUpNewLine(indexOfLine, spellRepertoireLinesTable, spellRepertoireLines, __instance);
            curLine.Bind(caster.RulesetCharacter, rulesetSpellRepertoire, spellRepertoires.Count > 1,
                spellCastEngaged, slotAdvancementPanel, actionType, cantripOnly, startLevel, level);
            return spellRepertoireLinesTable;
        }

        private static SpellRepertoireLine SetUpNewLine(int index, RectTransform spellRepertoireLinesTable,
            List<SpellRepertoireLine> spellRepertoireLines, SpellSelectionPanel __instance)
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

        private static bool IsLevelActive(RulesetSpellRepertoire spellRepertoire, int level,
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
                if (actionType == ActionDefinitions.ActionType.None)
                {
                    return true;
                }

                return spellRepertoire.KnownCantrips.Any(cantrip => cantrip.ActivationTime == spellActivationTime);
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

    // second line unbind
    [HarmonyPatch(typeof(SpellSelectionPanel), "Unbind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SpellSelectionPanel_Unbind
    {
        internal static void Postfix()
        {
            if (!Main.Settings.EnableMultiLineSpellPanel)
            {
                return;
            }

            foreach (var spellTable in spellLineTables
                         .Where(spellTable =>
                             spellTable != null && spellTable.gameObject.activeSelf && spellTable.childCount > 0))
            {
                Gui.ReleaseChildrenToPool(spellTable);
                spellTable.SetParent(null);
                Object.Destroy(spellTable.gameObject);
            }

            spellLineTables.Clear();
        }
    }
}
