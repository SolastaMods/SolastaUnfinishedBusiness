using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.GameUi.AdventureLog
{
    //
    // this patch shouldn't be protected to avoid game breaking on load
    //
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
                ___sectionHeaderGroup.gameObject.SetActive(gameAdventureEntryLore.Title != string.Empty);
                ___sectionHeaderTitle.Text = gameAdventureEntryLore.Title;

                ___conversationTitle.transform.parent.gameObject.SetActive(false);
                ___conversationGroup.gameObject.SetActive(true);
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
                    // Commented out for 1.3.27
                    //child.GetComponent<GuiTextFragment>().Bind(___totalFragments[i]);
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
