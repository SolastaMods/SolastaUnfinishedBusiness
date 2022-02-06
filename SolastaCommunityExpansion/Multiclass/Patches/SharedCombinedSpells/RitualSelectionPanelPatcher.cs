using System.Collections.Generic;
using HarmonyLib;
using SolastaModApi.Infrastructure;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Multiclass.Patches.SharedCombinedSpells
{
    internal static class RitualSelectionPanelPatcher
    {
        // ensures ritual spells from all spell repertoires are made available
        [HarmonyPatch(typeof(RitualSelectionPanel), "Bind")]
        internal static class RitualSelectionPanelBind
        {
            internal static bool Prefix(
                RitualSelectionPanel __instance,
                GameLocationCharacter caster,
                RitualSelectionPanel.RitualCastCancelledHandler ritualCastCancelled,
                RitualBox.RitualCastEngagedHandler ritualCastEngaged)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                var rulesetCharacter = caster.RulesetCharacter;
                var ritualType = RuleDefinitions.RitualCasting.None;

                __instance.Caster = caster;
                __instance.RitualCastCancelled = ritualCastCancelled;
                rulesetCharacter.EnumerateFeaturesToBrowse<FeatureDefinitionMagicAffinity>(rulesetCharacter.FeaturesToBrowse);

                // BEGIN PATCH
                var ritualSpellsCache = new List<SpellDefinition>();
                var ritualSpells = __instance.GetField<RitualSelectionPanel, List<SpellDefinition>>("ritualSpells");
                var ritualBoxesTable = __instance.GetField<RitualSelectionPanel, RectTransform>("ritualBoxesTable");
                var ritualBoxPrefab = __instance.GetField<RitualSelectionPanel, GameObject>("ritualBoxPrefab");
                var labelTransform = __instance.GetField<RitualSelectionPanel, RectTransform>("labelTransform");

                ritualSpells.Clear();

                foreach (var featureDefinition in rulesetCharacter.FeaturesToBrowse)
                {
                    if (featureDefinition is FeatureDefinitionMagicAffinity featureDefinitionMagicAffinity && featureDefinitionMagicAffinity.RitualCasting != RuleDefinitions.RitualCasting.None)
                    {
                        ritualType = featureDefinitionMagicAffinity.RitualCasting;
                        __instance.Caster.RulesetCharacter.EnumerateUsableRitualSpells(ritualType, ritualSpellsCache);

                        foreach (var ritualSpell in ritualSpellsCache)
                        {
                            if (!ritualSpells.Contains(ritualSpell))
                            {
                                ritualSpells.Add(ritualSpell);
                            }
                        }
                    }
                }
                // END PATCH

                while (ritualBoxesTable.childCount < ritualSpells.Count)
                {
                    Gui.GetPrefabFromPool(ritualBoxPrefab, ritualBoxesTable);
                }

                for (var index = 0; index < ritualBoxesTable.childCount; ++index)
                {
                    var child = ritualBoxesTable.GetChild(index);

                    if (index < ritualSpells.Count)
                    {
                        if (!child.gameObject.activeSelf)
                        {
                            child.gameObject.SetActive(true);
                        }

                        child.GetComponent<RitualBox>().Bind(rulesetCharacter, ritualSpells[index], ritualCastEngaged);
                    }
                    else if (child.gameObject.activeSelf)
                    {
                        child.GetComponent<RitualBox>().Unbind();
                        child.gameObject.SetActive(false);
                    }
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(labelTransform);

                var a = labelTransform.rect.width + 2f * labelTransform.anchoredPosition.x;

                LayoutRebuilder.ForceRebuildLayoutImmediate(ritualBoxesTable);

                __instance.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Max(a, ritualBoxesTable.rect.width));

                return false;
            }
        }
    }
}
