using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;

//This should have default namespace so that it can be properly created by `CharacterActionPatcher`
// ReSharper disable once CheckNamespace
[UsedImplicitly]
#pragma warning disable CA1050
public class CharacterActionToggle(CharacterActionParams actionParams) : CharacterAction(actionParams)
#pragma warning restore CA1050
{
    public override IEnumerator ExecuteImpl()
    {
        var rulesetCharacter = ActingCharacter.RulesetCharacter;
        // var id = (ExtraActionId)ActionId;

        if (rulesetCharacter.IsToggleEnabled(ActionId))
        {
            rulesetCharacter.DisableToggle(ActionId);
        }
        else
        {
            rulesetCharacter.EnableToggle(ActionId);
        }

        yield break;
    }
}
