using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class CharacterActionCrystalDefenseOff : CharacterAction
{
    public CharacterActionCrystalDefenseOff(CharacterActionParams actionParams) : base(actionParams)
    {
    }

    public override IEnumerator ExecuteImpl()
    {
        var RulesetCharacter = ActingCharacter.RulesetCharacter;
        RulesetCharacter.RemoveAllConditionsOfCategoryAndType("12Status", "ConditionCrystalDefense", true);
        yield return CharacterActionStandUp.StandUp(ActingCharacter, false, true);
    }
}
