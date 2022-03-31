using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaMulticlass.Patches.RitualCasting
{
    internal static class RitualSelectionPanelPatcher
    {
        // ensures ritual spells from all spell repertoires are made available
        [HarmonyPatch(typeof(RitualSelectionPanel), "Bind")]
        internal static class RitualSelectionPanelBind
        {
            internal static bool Prefix(
                RitualSelectionPanel __instance,
                List<SpellDefinition> ___ritualSpells,
                RectTransform ___ritualBoxesTable,
                GameObject ___ritualBoxPrefab,
                RectTransform ___labelTransform,
                GameLocationCharacter caster,
                RitualSelectionPanel.RitualCastCancelledHandler ritualCastCancelled,
                RitualBox.RitualCastEngagedHandler ritualCastEngaged)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                var rulesetCharacter = caster.RulesetCharacter;

                __instance.Caster = caster;
                __instance.RitualCastCancelled = ritualCastCancelled;
                rulesetCharacter.EnumerateFeaturesToBrowse<FeatureDefinitionMagicAffinity>(rulesetCharacter.FeaturesToBrowse);

                // BEGIN PATCH
                var ritualSpellsCache = new List<SpellDefinition>();

                ___ritualSpells.Clear();

                foreach (var featureDefinitionMagicAffinity in rulesetCharacter
                    .FeaturesToBrowse.OfType<FeatureDefinitionMagicAffinity>()
                    .Where(f => f.RitualCasting != RuleDefinitions.RitualCasting.None))
                {
                    __instance.Caster.RulesetCharacter.EnumerateUsableRitualSpells(featureDefinitionMagicAffinity.RitualCasting, ritualSpellsCache);

                    foreach (var ritualSpell in ritualSpellsCache)
                    {
                        if (!___ritualSpells.Contains(ritualSpell))
                        {
                            ___ritualSpells.Add(ritualSpell);
                        }
                    }
                }
                // END PATCH

                while (___ritualBoxesTable.childCount < ___ritualSpells.Count)
                {
                    Gui.GetPrefabFromPool(___ritualBoxPrefab, ___ritualBoxesTable);
                }

                for (var index = 0; index < ___ritualBoxesTable.childCount; ++index)
                {
                    var child = ___ritualBoxesTable.GetChild(index);

                    if (index < ___ritualSpells.Count)
                    {
                        if (!child.gameObject.activeSelf)
                        {
                            child.gameObject.SetActive(true);
                        }

                        child.GetComponent<RitualBox>().Bind(rulesetCharacter, ___ritualSpells[index], ritualCastEngaged);
                    }
                    else if (child.gameObject.activeSelf)
                    {
                        child.GetComponent<RitualBox>().Unbind();
                        child.gameObject.SetActive(false);
                    }
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(___labelTransform);

                var a = ___labelTransform.rect.width + (2f * ___labelTransform.anchoredPosition.x);

                LayoutRebuilder.ForceRebuildLayoutImmediate(___ritualBoxesTable);

                __instance.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Max(a, ___ritualBoxesTable.rect.width));

                return false;
            }
        }
    }
}
