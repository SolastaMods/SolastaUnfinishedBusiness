using HarmonyLib;
using SolastaCommunityExpansion;
using SolastaMulticlass.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaMulticlass.Patches.LevelUp
{
    internal static class CharacterStageSpellSelectionPanelPatcher
    {
        [HarmonyPatch(typeof(CharacterStageSpellSelectionPanel), "Refresh")]
        internal static class CharacterStageSpellSelectionPanelRefresh
        {
            internal static void Postfix(CharacterStageSpellSelectionPanel __instance, RulesetCharacterHero ___currentHero, GameObject ___levelButtonPrefab, RectTransform ___levelButtonsTable, RectTransform ___spellsByLevelTable)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                if (!LevelUpContext.IsLevelingUp(___currentHero))
                {
                    return;
                }

                // determines the display context
                var spellCastingClass = LevelUpContext.GetSelectedClass(___currentHero);
                var spellCastingSubclass = LevelUpContext.GetSelectedSubclass(___currentHero);
                var spellRepertoire = ___currentHero.SpellRepertoires
                    .Find(x => x.SpellCastingClass == spellCastingClass || (x.SpellCastingSubclass != null && x.SpellCastingSubclass == spellCastingSubclass));
                var accountForCantrips = spellRepertoire == null || spellRepertoire.KnownCantrips.Count > 0 ? 1 : 0;
                var classSpellLevel = SharedSpellsContext.GetClassSpellLevel(___currentHero, spellCastingClass, spellCastingSubclass);

                // patches the spell level buttons
                while(___levelButtonsTable.childCount < classSpellLevel + accountForCantrips)
                {
                    Gui.GetPrefabFromPool(___levelButtonPrefab, ___levelButtonsTable);

                    var index = ___levelButtonsTable.childCount - 1;
                    var child = ___levelButtonsTable.GetChild(index);

                    child.GetComponent<SpellLevelButton>().Bind(index, new SpellLevelButton.LevelSelectedHandler(__instance.LevelSelected));
                }

                while (___levelButtonsTable.childCount > classSpellLevel + accountForCantrips)
                {
                    Gui.ReleaseInstanceToPool(___levelButtonsTable.GetChild(___levelButtonsTable.childCount - 1).gameObject);
                }

                ___levelButtonsTable.GetChild(0).gameObject.SetActive(accountForCantrips > 0);

                LayoutRebuilder.ForceRebuildLayoutImmediate(___levelButtonsTable);

                // patches the spell by level table
                for (var i = 0; i < ___spellsByLevelTable.childCount; i++)
                {
                    var spellsByLevel = ___spellsByLevelTable.GetChild(i);

                    for (var j = 0; j < spellsByLevel.childCount; j++)
                    {
                        var transform = spellsByLevel.GetChild(j);

                        if (transform.TryGetComponent(typeof(SlotStatusTable), out var _))
                        {
                            transform.gameObject.SetActive(i < classSpellLevel + accountForCantrips); // table header
                        }
                        else
                        {
                            transform.gameObject.SetActive(i < classSpellLevel + accountForCantrips); // table content
                        }
                    }
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(___spellsByLevelTable);
            }
        }
    }
}
