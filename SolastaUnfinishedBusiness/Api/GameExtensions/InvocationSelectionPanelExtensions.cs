using UnityEngine.UI;
using static ActionDefinitions;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

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
        var actionId = actionPanel.actionId;
        var action = ServiceRepository.GetService<IGameLocationActionService>().AllActionDefinitions[actionId];

        for (var i = 0; i < table.childCount; i++)
        {
            var box = table.GetChild(i).GetComponent<InvocationActivationBox>();
            var active = i < invocations.Count && box.gameObject.activeSelf;

            if (active)
            {
                var invocationDefinition = box.Invocation.invocationDefinition;

                //strict id checks when in battle
                if (actionPanel.ActionScope == ActionScope.Battle)
                {
                    active = actionId == invocationDefinition.GetActionId();
                }
                //allow all invocations that match main action id
                else
                {
                    active = actionId == invocationDefinition.GetMainActionId();
                }
            }

            box.gameObject.SetActive(active);
        }

        var child = invocationPanel.transform.Find("Header/InvocationLabel");

        if (child)
        {
            var label = child.GetComponent<GuiLabel>();

            if (label)
            {
                label.Text = actionId is Id.CastInvocation or (Id)ExtraActionId.CastInvocationBonus
                    ? "Feature/&PointPoolWarlockInvocationInitialTitle"
                    : action.GuiPresentation.Title;
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(table);
        Gui.InputService.RecomputeSelectableNavigation(true);
    }
}
