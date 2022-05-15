using SolastaCommunityExpansion.CustomUI;
using SolastaModApi.Infrastructure;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Api.AdditionalExtensions
{
    public static class CharacterReactionSubitemExtension
    {
        public static void BindWarcaster(this CharacterReactionSubitem instance,
            ReactionRequestWarcaster reactionRequest,
            int slotLevel,
            bool interactable,
            CharacterReactionSubitem.SubitemSelectedHandler subitemSelected)
        {
            var spellRepertoire = reactionRequest.ReactionParams.SpellRepertoire;

            var label = instance.GetField<GuiLabel>("label");
            var toggle = instance.GetField<Toggle>("toggle");
            var tooltip = GetOrMakeBackgroundTooltip(toggle.transform);

            string title;
            if (slotLevel == 0)
            {
                title = "Reaction/&WarcasterAttackTitle";
                if (tooltip != null)
                {
                    tooltip.Disabled = false;
                    tooltip.Content = "Reaction/&WarcasterAttackDescription";
                }
            }
            else
            {
                var spell = spellRepertoire.KnownSpells[slotLevel - 1];
                title = spell.GuiPresentation.Title;
                if (tooltip != null)
                {
                    tooltip.Disabled = false;
                    ServiceRepository.GetService<IGuiWrapperService>()
                        .GetGuiSpellDefinition(spell.Name)
                        .SetupTooltip(tooltip, reactionRequest.Character.RulesetActor);
                }
            }

            label.Text = title;
            toggle.interactable = interactable;
            instance.GetField<CanvasGroup>("canvasGroup").interactable = interactable;

            instance.SubitemSelected = subitemSelected;

            var rectTransform = toggle.GetComponent<RectTransform>();
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);

            // Hide all slots
            var slotStatusTable = instance.GetField<RectTransform>("slotStatusTable");
            for (var index = 0; index < slotStatusTable.childCount; ++index)
            {
                slotStatusTable.GetChild(index).gameObject.SetActive(false);
            }
        }

        private static GuiTooltip GetOrMakeBackgroundTooltip(Transform root)
        {
            var background = root.FindChildRecursive("Background");
            if (background != null)
            {
                if (!background.TryGetComponent<GuiTooltip>(out var tooltip))
                {
                    tooltip = background.gameObject.AddComponent<GuiTooltip>();
                    tooltip.AnchorMode = TooltipDefinitions.AnchorMode.LEFT_CENTER;
                }

                return tooltip;
            }

            return null;
        }
    }
}
