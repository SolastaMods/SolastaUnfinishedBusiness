using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Api.Extensions;

public static class InvocationSelectionPanelExtensions
{
    //Custom bind that acknowledges bonus action invocations
    public static void CustomBind(
        this InvocationSelectionPanel invocationPanel,
        GameLocationCharacter caster,
        InvocationSelectionPanel.InvocationSelectedHandler selected,
        InvocationSelectionPanel.InvocationCancelledHandler canceled,
        CharacterActionPanel actionPanel)
    {
        invocationPanel.Bind(caster, selected, canceled);
        var table = invocationPanel.invocationsTable;
        var invocations = caster.RulesetCharacter.Invocations;

        for (var i = 0; i < table.childCount; i++)
        {
            var box = table.GetChild(i).GetComponent<InvocationActivationBox>();
            var active = i < invocations.Count;

            if (active)
            {
                //hide invocations that are not bonus action
                if (actionPanel.actionId == (ActionDefinitions.Id)ExtraActionId.CastInvocationBonus)
                {
                    active = box.Invocation.invocationDefinition.IsBonusAction();
                }
                //filter main action invocations only during battle
                else if (actionPanel.ActionScope == ActionDefinitions.ActionScope.Battle)
                {
                    active = !box.Invocation.invocationDefinition.IsBonusAction();
                }
            }

            box.gameObject.SetActive(active);
        }
            
        LayoutRebuilder.ForceRebuildLayoutImmediate(table);
        Gui.InputService.RecomputeSelectableNavigation(true);
    }
}
