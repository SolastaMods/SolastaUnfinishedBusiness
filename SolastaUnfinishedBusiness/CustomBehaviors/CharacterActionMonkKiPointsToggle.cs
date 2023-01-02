using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;

//This should have default namespace so that it can be properly created by `CharacterActionPatcher`
// ReSharper disable once CheckNamespace
[UsedImplicitly]
public class CharacterActionMonkKiPointsToggle : CharacterAction
{
    public CharacterActionMonkKiPointsToggle(CharacterActionParams actionParams) : base(actionParams)
    {
    }

    public override IEnumerator ExecuteImpl()
    {
        var rulesetCharacter = ActingCharacter.RulesetCharacter;

        if (rulesetCharacter.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.MonkKiPointsToggle))
        {
            rulesetCharacter.DisableToggle((ActionDefinitions.Id)ExtraActionId.MonkKiPointsToggle);
        }
        else
        {
            rulesetCharacter.EnableToggle((ActionDefinitions.Id)ExtraActionId.MonkKiPointsToggle);
        }

        rulesetCharacter.KiPointsAltered?.Invoke(rulesetCharacter, rulesetCharacter.RemainingKiPoints);

        yield return null;
    }
}
