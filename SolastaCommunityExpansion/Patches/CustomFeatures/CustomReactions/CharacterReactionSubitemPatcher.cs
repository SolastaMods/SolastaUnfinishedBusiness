using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.CustomUI;
using SolastaModApi.Infrastructure;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomReactions
{
    internal static class CharacterReactionSubitemPatcher
    {
        [HarmonyPatch(typeof(CharacterReactionSubitem), "Bind")]
        internal static class CharacterReactionSubitem_Bind
        {
            internal static bool Prefix(CharacterReactionSubitem __instance, 
                RulesetSpellRepertoire spellRepertoire,
                int slotLevel,
                string text,
                bool interactable,
                CharacterReactionSubitem.SubitemSelectedHandler subitemSelected)
            {
                Main.Log2($"CharacterReactionSubitem Bind '{text}' slot: {slotLevel}", true);
                if (text == ReactionRequestWarcaster.Name)
                {
                    BindWarcaster(__instance, spellRepertoire, slotLevel, interactable, subitemSelected);
                    return false;
                }
                
                // reset sizes
                __instance.GetField<GuiLabel>("label").transform.localScale = new Vector3(1, 1, 1);
                __instance.GetField<Toggle>("toggle").transform.localScale = new Vector3(1, 1, 1);

                return true;
            }
        }

        private static void BindWarcaster(CharacterReactionSubitem instance, 
            RulesetSpellRepertoire spellRepertoire,
            int slotLevel,
            bool interactable,
            CharacterReactionSubitem.SubitemSelectedHandler subitemSelected)
        {
            Main.Log2($"CharacterReactionSubitem BindWarcaster slot: {slotLevel}, spells: [{string.Join(", ", spellRepertoire.KnownSpells.Select(s=>s.Name))}]", true);
            var slotStatusTable = instance.GetField<RectTransform>("slotStatusTable");

            string title, description;
            if (slotLevel == 0)
            {
                title = "Attack";
                description = string.Empty;
            }
            else
            {
                var spell = spellRepertoire.KnownSpells[slotLevel - 1];
                title = spell.GuiPresentation.Title;
                description = "QEQEQE EERRR";//spell.GuiPresentation.Description;
            }

            var label = instance.GetField<GuiLabel>("label");
            var toggle = instance.GetField<Toggle>("toggle");
            var slotStatusPrefab = instance.GetField<GameObject>("slotStatusPrefab");
            var tooltip = toggle.gameObject.AddComponent<GuiTooltip>();
            

            label.Text = title;
            var guiTooltip = slotStatusTable.GetComponent<GuiTooltip>();
            guiTooltip.Content = description;
            tooltip.Anchor = null;//toggle.transform as RectTransform;
            tooltip.AnchorMode = guiTooltip.AnchorMode;
            tooltip.Disabled = false;
            tooltip.TooltipClass = "";
            // tooltip.transform.SetParent(guiTooltip.transform.parent);
            tooltip.Content = description;
            tooltip.gameObject.SetActive(true);

            toggle.interactable = interactable;
            instance.GetField<CanvasGroup>("canvasGroup").interactable = interactable;
            
            instance.SubitemSelected = subitemSelected;

            var tooltipSlots = 1;
            var toggleScale = 8.0f;
            var labelScale = 0.8f;
            var toggleTransform = toggle.transform;
            var labelTransform = label.transform;
            
            var oldScale = new Vector3(1, 1, 1);
            oldScale.x *= toggleScale;
            toggleTransform.localScale = oldScale;

            oldScale = new Vector3(labelScale, labelScale, labelScale);
            oldScale.x /= toggleScale;
            labelTransform.localScale = oldScale;
            
            while (slotStatusTable.childCount < tooltipSlots)
                Gui.GetPrefabFromPool(slotStatusPrefab, slotStatusTable);

            for (int index = 0; index < slotStatusTable.childCount; ++index)
            {
                var child = slotStatusTable.GetChild(index);
                var component = child.GetComponent<SlotStatus>();
                component.Used.gameObject.SetActive(false);
                component.Available.gameObject.SetActive(false);
                
                child.gameObject.SetActive(index < tooltipSlots);
            }
        }

        //tooltip tests
        // [HarmonyPatch(typeof(GuiManager), "UpdateTooltip")]
        // internal static class GuiManager_UpdateTooltip
        // {
        //     internal static void Prefic(GuiManager __instance)
        //     {
        //         var results = __instance.GetField<List<RaycastResult>>("results");
        //         for (int index = 0; index < results.Count; ++index)
        //         {
        //             var item = results[index];
        //             if (item.gameObject.name.Contains("Toggle"))
        //             {
        //                 var tt = item.gameObject.GetComponent<GuiTooltip>();
        //                 Main.Log2($"{item.gameObject.name} - tt: '{tt?.Content}'");
        //             }
        //         }
        //     }
        // }
    }
}
