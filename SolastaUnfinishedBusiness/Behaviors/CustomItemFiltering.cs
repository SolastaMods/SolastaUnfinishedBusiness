using SolastaUnfinishedBusiness.Api.GameExtensions;

namespace SolastaUnfinishedBusiness.Behaviors;

internal interface ICustomItemFilter
{
    public bool IsValid(RulesetCharacter character, RulesetItem rulesetItem, RulesetEffect rulesetEffect);
}

internal delegate bool IsValidItemHandler(RulesetCharacter character, RulesetItem rulesetItem);

internal class CustomItemFilter : ICustomItemFilter
{
    private readonly IsValidItemHandler _handler;

    internal CustomItemFilter(IsValidItemHandler handler)
    {
        _handler = handler;
    }

    public virtual bool IsValid(RulesetCharacter character, RulesetItem rulesetItem, RulesetEffect rulesetEffect)
    {
        return _handler(character, rulesetItem);
    }

    internal static void FilterItems(InventoryPanel panel)
    {
        if (panel.InventoryManagementMode != ActionDefinitions.InventoryManagementMode.SelectItem ||
            panel.ItemSelectionType != ActionDefinitions.ItemSelectionType.Carried)
        {
            return;
        }

        var actionParams = panel.CharacterActionParams;
        var filter = actionParams.RulesetEffect.SourceDefinition.GetFirstSubFeatureOfType<ICustomItemFilter>();

        if (filter == null)
        {
            return;
        }

        foreach (var box in panel.allSlotBoxes)
        {
            var item = box.InventorySlot.EquipedItem;

            if (item == null)
            {
                box.ValidForItemSelection = false;

                continue;
            }

            box.ValidForItemSelection = filter.IsValid(
                actionParams.ActingCharacter.RulesetCharacter, item, actionParams.RulesetEffect);
        }
    }
}
