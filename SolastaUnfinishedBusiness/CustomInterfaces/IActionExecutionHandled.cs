namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IActionExecutionHandled
{
    /**
     * Called after action execution has been accounted for - actions spent, attack numbers upodated, etc
     */
    void OnActionExecutionHandled(
        GameLocationCharacter character, 
        CharacterActionParams actionParams,
        ActionDefinitions.ActionScope scope);
}
