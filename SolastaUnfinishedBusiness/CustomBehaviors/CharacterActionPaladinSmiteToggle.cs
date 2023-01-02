using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;

//This should have default namespace so that it can be properly created by `CharacterActionPatcher`
// ReSharper disable once CheckNamespace
[UsedImplicitly]
public class CharacterActionPaladinSmiteToggle : CharacterAction
{
    private const ActionDefinitions.Id Action = (ActionDefinitions.Id)ExtraActionId.PaladinSmiteToggle;

    public CharacterActionPaladinSmiteToggle(CharacterActionParams actionParams) : base(actionParams)
    {
    }

    public override IEnumerator ExecuteImpl()
    {
        var rulesetCharacter = ActingCharacter.RulesetCharacter;

        if (rulesetCharacter.IsToggleEnabled(Action))
        {
            rulesetCharacter.DisableToggle(Action);
        }
        else
        {
            rulesetCharacter.EnableToggle(Action);
        }

        yield return null;
    }
}
