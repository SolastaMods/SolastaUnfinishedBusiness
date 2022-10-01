using SolastaUnfinishedBusiness.Api.Extensions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

public interface ICustomItemFilter
{
    public bool IsValid(RulesetCharacter character, RulesetItem rulesetItem);
}

public delegate bool IsValidItemHandler(RulesetCharacter character, RulesetItem rulesetItem);

public class CustomItemFilter : ICustomItemFilter
{
    private readonly IsValidItemHandler _handler;

    public CustomItemFilter(IsValidItemHandler handler)
    {
        _handler = handler;
    }

    public bool IsValid(RulesetCharacter character, RulesetItem rulesetItem)
    {
        return _handler(character, rulesetItem);
    }

    public static void FilterItems(InventoryPanel panel)
    {
        if (panel.InventoryManagementMode != ActionDefinitions.InventoryManagementMode.SelectItem
            || panel.ItemSelectionType != ActionDefinitions.ItemSelectionType.Carried)
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

            box.ValidForItemSelection = filter.IsValid(actionParams.ActingCharacter.RulesetCharacter, item);
        }
    }
}
