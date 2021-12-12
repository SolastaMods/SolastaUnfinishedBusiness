using HarmonyLib;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches
{
    [HarmonyPatch(typeof(GuiAdventureLine), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GuiAdventureLine_Bind
    {
        internal static void Postfix(
            RectTransform ___sectionHeaderGroup,
            GuiLabel ___sectionHeaderTitle,
            RectTransform ___conversationGroup, 
            GuiLabel ___conversationTitle,
            RectTransform ___conversationFragmentsContainer,
            GameObject ___fragmentPrefab,
            List<TextBreaker.FragmentInfo> ___totalFragments,
            GameRecordEntry entry)
        {
            if (entry is Models.AdventureLogContext.GameAdventureEntryDungeonMaker gameAdventureEntryLore)
            {
                ___sectionHeaderGroup.gameObject.SetActive(true);
                ___sectionHeaderTitle.Text = gameAdventureEntryLore.Title;
                ___conversationGroup.gameObject.SetActive(true);
                ___conversationTitle.transform.parent.gameObject.SetActive(false);
                ___totalFragments.Clear();

                foreach (TextBreaker textBreaker in gameAdventureEntryLore.TextBreakers)
                {
                    ___totalFragments.AddRange(textBreaker.Fragments);
                }

                while (___conversationFragmentsContainer.childCount < ___totalFragments.Count)
                {
                    Gui.GetPrefabFromPool(___fragmentPrefab, ___conversationFragmentsContainer);
                }

                for (int i = 0; i < ___totalFragments.Count; ++i)
                {
                    var child = ___conversationFragmentsContainer.GetChild(i);

                    child.gameObject.SetActive(true);
                    child.GetComponent<GuiTextFragment>().Bind(___totalFragments[i]);
                }

                for (int i = ___totalFragments.Count; i < ___conversationFragmentsContainer.childCount; ++i)
                {
                    var child = ___conversationFragmentsContainer.GetChild(i);

                    if (child.gameObject.activeSelf)
                    {
                        child.GetComponent<GuiTextFragment>().Unbind();
                        child.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
