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

            var label = instance.GetField<GuiLabel>("label");
            var toggle = instance.GetField<Toggle>("toggle");
            var background = toggle.transform.FindChildRecursive("Background");
            GuiTooltip tooltip = null;
            if (background != null)
            {
                tooltip = background.gameObject.AddComponent<GuiTooltip>();
                tooltip.Anchor = null; //toggle.transform as RectTransform;
                tooltip.AnchorMode = TooltipDefinitions.AnchorMode.LEFT_CENTER;
                tooltip.Disabled = false;
                tooltip.TooltipClass = "";
            }
            
            string title;
            if (slotLevel == 0)
            {
                title = "Attack";
                if (tooltip != null)
                {
                    tooltip.Content = "ATTACK!!!";
                }
            }
            else
            {
                var spell = spellRepertoire.KnownSpells[slotLevel - 1];
                title = spell.GuiPresentation.Title;
                if (tooltip != null)
                {
                    ServiceRepository.GetService<IGuiWrapperService>()
                        .GetGuiSpellDefinition(spell.Name)
                        .SetupTooltip(tooltip, null);
                }
            }

            label.Text = title;
            toggle.interactable = interactable;
            instance.GetField<CanvasGroup>("canvasGroup").interactable = interactable;
            
            instance.SubitemSelected = subitemSelected;

            var toggleScale = 8.0f;
            var labelScale = 0.75f;
            var toggleTransform = toggle.transform;
            var labelTransform = label.transform;
            
            var oldScale = new Vector3(1, 1, 1);
            oldScale.x *= toggleScale;
            toggleTransform.localScale = oldScale;

            oldScale = new Vector3(labelScale, labelScale, labelScale);
            oldScale.x /= toggleScale;
            labelTransform.localScale = oldScale;
            
            for (int index = 0; index < slotStatusTable.childCount; ++index)
                slotStatusTable.GetChild(index).gameObject.SetActive(false);
        }
    }
}
