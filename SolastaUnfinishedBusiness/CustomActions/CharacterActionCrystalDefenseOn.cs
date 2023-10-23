using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class CharacterActionCrystalDefenseOn : CharacterAction
{
    public CharacterActionCrystalDefenseOn(CharacterActionParams actionParams) : base(actionParams)
    {
    }

    public override IEnumerator ExecuteImpl()
    {
        var RulesetCharacter = ActingCharacter.RulesetCharacter;
        if (!RulesetCharacter.HasConditionOfCategoryAndType("12Status", "ConditionCrystalDefense"))
        {
            ConditionDefinition element = DatabaseRepository.GetDatabase<ConditionDefinition>().GetElement("ConditionCrystalDefense");
            RulesetCondition newCondition = RulesetCondition.CreateActiveCondition(RulesetCharacter.Guid, element, RuleDefinitions.DurationType.Irrelevant, 0, RuleDefinitions.TurnOccurenceType.StartOfTurn, RulesetCharacter.Guid, RulesetCharacter.CurrentFaction.Name);
            RulesetCharacter.AddConditionOfCategory("12Status", newCondition);
            if (ActingCharacter.SetProne(value: true))
            {
                yield return ActingCharacter.EventSystem.WaitForEvent(GameLocationCharacterEventSystem.Event.ProneInAnimationEnd);
            }
        }
        yield return null;
    }
}
