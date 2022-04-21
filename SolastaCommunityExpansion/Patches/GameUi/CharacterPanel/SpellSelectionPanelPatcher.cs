using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaModApi.Infrastructure;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterPanel
{
    internal static class SpellSelectionPanelPatcher
    {
        private static readonly List<RectTransform> spellLineTables = new();

        // second line bind
        [HarmonyPatch(typeof(SpellSelectionPanel), "Bind")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class SpellSelectionPanel_Bind
        {
            internal static void Postfix(SpellSelectionPanel __instance, GameLocationCharacter caster, SpellsByLevelBox.SpellCastEngagedHandler spellCastEngaged, ActionDefinitions.ActionType actionType, bool cantripOnly)
            {
                if (!Main.Settings.EnableMultiLineSpellPanel)
                {
                    return;
                }

                List<SpellRepertoireLine> spellRepertoireLines = __instance.GetField<List<SpellRepertoireLine>>("spellRepertoireLines");
                SpellRepertoireLine spellRepertoireSecondaryLine = __instance.GetField<SpellRepertoireLine>("spellRepertoireSecondaryLine");
                RectTransform spellRepertoireLinesTable = __instance.GetField<RectTransform>("spellRepertoireLinesTable");
                SlotAdvancementPanel slotAdvancementPanel = __instance.GetField<SlotAdvancementPanel>("slotAdvancementPanel");

                foreach (SpellRepertoireLine spellRepertoireLine in spellRepertoireLines)
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
                    VerticalLayoutGroup vertGroup = spellLineHolder.AddComponent<VerticalLayoutGroup>();

                    vertGroup.spacing = 10;
                    spellLineHolder.AddComponent<ContentSizeFitter>();
                    spellLineHolder.transform.SetParent(spellRepertoireLinesTable.parent, true);
                    spellLineHolder.transform.SetAsFirstSibling();
                    spellLineHolder.transform.localScale = Vector3.one;
                    spellRepertoireLinesTable.SetParent(spellLineHolder.transform);
                }

                var spellRepertoires = __instance.Caster.RulesetCharacter.SpellRepertoires;

                bool needNewLine = true;
                int lineIndex = 0;
                int indexOfLine = 0;
                int spellLevelsOnLine = 0;
                RectTransform curTable = spellRepertoireLinesTable;

                for (int repertoireIndex = 0; repertoireIndex < spellRepertoires.Count; repertoireIndex++)
                {
                    RulesetSpellRepertoire rulesetSpellRepertoire = spellRepertoires[repertoireIndex];
                    int startLevel = 0;

                    for (int level = startLevel; level <= rulesetSpellRepertoire.MaxSpellLevelOfSpellCastingLevel; level++)
                    {
                        if (IsLevelActive(rulesetSpellRepertoire, level, actionType))
                        {
                            spellLevelsOnLine++;

                            if (spellLevelsOnLine >= Main.Settings.MaxSpellLevelsPerLine)
                            {
                                curTable = AddActiveSpellsToLine(__instance, caster, spellCastEngaged, actionType, cantripOnly, spellRepertoireLines, curTable, slotAdvancementPanel, spellRepertoires, needNewLine, lineIndex, indexOfLine, rulesetSpellRepertoire, startLevel, level);
                                startLevel = level + 1;
                                lineIndex++;
                                spellLevelsOnLine = 0;
                                needNewLine = true;
                                indexOfLine = 0;
                            }
                        }
                    }

                    if (spellLevelsOnLine != 0)
                    {
                        curTable = AddActiveSpellsToLine(__instance, caster, spellCastEngaged, actionType, cantripOnly, spellRepertoireLines, curTable, slotAdvancementPanel, spellRepertoires, needNewLine, lineIndex, indexOfLine, rulesetSpellRepertoire, startLevel, rulesetSpellRepertoire.MaxSpellLevelOfSpellCastingLevel);
                        needNewLine = false;
                        indexOfLine++;
                    }
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(curTable);
                __instance.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, spellRepertoireLinesTable.rect.width);
            }

            private static RectTransform AddActiveSpellsToLine(SpellSelectionPanel __instance, GameLocationCharacter caster,
                SpellsByLevelBox.SpellCastEngagedHandler spellCastEngaged, ActionDefinitions.ActionType actionType, bool cantripOnly,
                List<SpellRepertoireLine> spellRepertoireLines, RectTransform spellRepertoireLinesTable,
                SlotAdvancementPanel slotAdvancementPanel, List<RulesetSpellRepertoire> spellRepertoires, bool needNewLine, int lineIndex,
                int indexOfLine, RulesetSpellRepertoire rulesetSpellRepertoire, int startLevel, int level)
            {
                if (needNewLine)
                {
                    RectTransform previousTable = spellRepertoireLinesTable;
                    LayoutRebuilder.ForceRebuildLayoutImmediate(previousTable);

                    if (lineIndex > 0)
                    {
                        // instantiate new table
                        spellRepertoireLinesTable = Object.Instantiate(spellRepertoireLinesTable);
                        // clear it of children
                        spellRepertoireLinesTable.DetachChildren();
                        spellRepertoireLinesTable.SetParent(previousTable.parent.transform, true);
                        spellRepertoireLinesTable.localScale = previousTable.localScale;
                        spellRepertoireLinesTable.transform.SetAsFirstSibling();
                        spellLineTables.Add(spellRepertoireLinesTable);
                    }
                }

                SpellRepertoireLine curLine = SetUpNewLine(indexOfLine, spellRepertoireLinesTable, spellRepertoireLines, __instance);
                curLine.Bind(caster.RulesetCharacter, rulesetSpellRepertoire, spellRepertoires.Count > 1, spellCastEngaged, slotAdvancementPanel, actionType, cantripOnly, startLevel, level);
                return spellRepertoireLinesTable;
            }

            private static SpellRepertoireLine SetUpNewLine(int index, RectTransform spellRepertoireLinesTable, List<SpellRepertoireLine> spellRepertoireLines, SpellSelectionPanel __instance)
            {
                GameObject newLine;

                if (spellRepertoireLinesTable.childCount <= index)
                {
                    newLine = Gui.GetPrefabFromPool(__instance.GetField<GameObject>("spellRepertoireLinePrefab"),
                        spellRepertoireLinesTable);
                }
                else
                {
                    newLine = spellRepertoireLinesTable.GetChild(index).gameObject;
                }

                newLine.SetActive(true);
                SpellRepertoireLine component = newLine.GetComponent<SpellRepertoireLine>();
                spellRepertoireLines.Add(component);
                return component;
            }

            private static bool IsLevelActive(RulesetSpellRepertoire spellRepertoire, int level, ActionDefinitions.ActionType actionType)
            {
                RuleDefinitions.ActivationTime spellActivationTime = RuleDefinitions.ActivationTime.Action;
                switch (actionType)
                {
                    case ActionDefinitions.ActionType.Bonus:
                        spellActivationTime = RuleDefinitions.ActivationTime.BonusAction;
                        break;

                    case ActionDefinitions.ActionType.Main:
                        spellActivationTime = RuleDefinitions.ActivationTime.Action;
                        break;

                    case ActionDefinitions.ActionType.Reaction:
                        spellActivationTime = RuleDefinitions.ActivationTime.Reaction;
                        break;

                    case ActionDefinitions.ActionType.NoCost:
                        spellActivationTime = RuleDefinitions.ActivationTime.NoCost;
                        break;
                }

                if (level == 0)
                {
                    // changed to support game v1.3.44 and allow ancestry cantrips to display off battle
                    if (actionType == ActionDefinitions.ActionType.None)
                    {
                        return true;
                    }

                    foreach (SpellDefinition cantrip in spellRepertoire.KnownCantrips)
                    {
                        if (cantrip.ActivationTime == spellActivationTime)
                        {
                            return true;
                        }
                    }

                    return false;
                }

                if (spellRepertoire.SpellCastingFeature.SpellReadyness == RuleDefinitions.SpellReadyness.Prepared &&
                    spellRepertoire.PreparedSpells
                        .Any(spellDefinition =>
                            spellDefinition.SpellLevel == level
                            && spellDefinition.ActivationTime == spellActivationTime))
                {
                    return true;
                }

                if (spellRepertoire.SpellCastingFeature.SpellReadyness == RuleDefinitions.SpellReadyness.AllKnown &&
                    spellRepertoire.KnownSpells.Any(spellDefinition => spellDefinition.SpellLevel == level))
                {
                    return true;
                }

                return false;
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

                foreach (RectTransform spellTable in spellLineTables)
                {
                    if (spellTable != null && spellTable.gameObject.activeSelf && spellTable.childCount > 0)
                    {
                        Gui.ReleaseChildrenToPool(spellTable);
                        spellTable.SetParent(null);
                        Object.Destroy(spellTable.gameObject);
                    }
                }

                spellLineTables.Clear();
            }
        }
    }
}
