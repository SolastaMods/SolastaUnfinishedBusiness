namespace SolastaUnfinishedBusiness.Interfaces;

public interface IActionExecutionHandled
{
    /**
     * Called after action execution has been accounted for - actions spent, attack numbers updated, etc
     */
    void OnActionExecutionHandled(
        GameLocationCharacter character,
        CharacterActionParams actionParams,
        ActionDefinitions.ActionScope scope);
}
