using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.CustomUI;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

internal static class CharacterReactionSubitemExtension
{
    internal static void BindWarcaster(
        [NotNull] this CharacterReactionSubitem instance,
        [NotNull] ReactionRequestWarcaster reactionRequest,
        int slotLevel,
        bool interactable,
        CharacterReactionSubitem.SubitemSelectedHandler subitemSelected)
    {
        var spellRepertoire = reactionRequest.ReactionParams.SpellRepertoire;
        var label = instance.label;
        var toggle = instance.toggle;
        var tooltip = GetOrMakeBackgroundTooltip(toggle.transform);

        string title;

        if (slotLevel == 0)
        {
            title = "Reaction/&WarcasterAttackTitle";

            if (tooltip != null)
            {
                tooltip.Disabled = false;
                if (reactionRequest.ReactionParams.attackMode?.sourceObject is RulesetItem weapon)
                {
                    ServiceRepository.GetService<IGuiWrapperService>()
                        .GetGuiItemDefinition(weapon.Name)
                        .SetupTooltip(tooltip, reactionRequest.Character.RulesetActor);
                }
                else
                {
                    tooltip.Content = "Reaction/&WarcasterAttackDescription";
                }
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
        instance.canvasGroup.interactable = interactable;
        instance.SubitemSelected = subitemSelected;

        var rectTransform = toggle.GetComponent<RectTransform>();

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 250);

        // Hide all slots
        var slotStatusTable = instance.slotStatusTable;

        for (var index = 0; index < slotStatusTable.childCount; ++index)
        {
            slotStatusTable.GetChild(index).gameObject.SetActive(false);
        }
    }

    internal static void BindPowerBundle(
        [NotNull] this CharacterReactionSubitem instance,
        [NotNull] ReactionRequestSpendBundlePower reactionRequest,
        int slotLevel,
        bool interactable,
        CharacterReactionSubitem.SubitemSelectedHandler subitemSelected)
    {
        var spellRepertoire = reactionRequest.ReactionParams.SpellRepertoire;
        var label = instance.label;
        var toggle = instance.toggle;
        var tooltip = GetOrMakeBackgroundTooltip(toggle.transform);
        var spell = spellRepertoire.KnownSpells[slotLevel];
        var power = PowerBundle.GetPower(spell);

        if (power == null)
        {
            return;
        }

        if (tooltip != null)
        {
            tooltip.Disabled = false;
            ServiceRepository.GetService<IGuiWrapperService>()
                .GetGuiPowerDefinition(power.Name)
                .SetupTooltip(tooltip, reactionRequest.Character.RulesetActor);
        }

        label.Text = power.GuiPresentation.Title;
        toggle.interactable = interactable;
        instance.canvasGroup.interactable = interactable;
        instance.SubitemSelected = subitemSelected;

        var rectTransform = toggle.GetComponent<RectTransform>();

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 250);

        // Hide all slots
        var slotStatusTable = instance.slotStatusTable;

        for (var index = 0; index < slotStatusTable.childCount; ++index)
        {
            slotStatusTable.GetChild(index).gameObject.SetActive(false);
        }
    }

    private static GuiTooltip GetOrMakeBackgroundTooltip(Transform root)
    {
        var background = root.FindChildRecursive("Background");

        if (background == null)
        {
            return null;
        }

        if (background.TryGetComponent<GuiTooltip>(out var tooltip))
        {
            return tooltip;
        }

        tooltip = background.gameObject.AddComponent<GuiTooltip>();
        tooltip.AnchorMode = TooltipDefinitions.AnchorMode.LEFT_CENTER;

        return tooltip;
    }
}
