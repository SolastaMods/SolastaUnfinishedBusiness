using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.Subclasses;

//This should have default namespace so that it can be properly created by `CharacterActionPatcher`
// ReSharper disable once CheckNamespace
[UsedImplicitly]
#pragma warning disable CA1050
public class CharacterActionCombatRageStart : CharacterAction
#pragma warning restore CA1050
{
    public CharacterActionCombatRageStart(CharacterActionParams actionParams)
        : base(actionParams)
    {
    }

    public override IEnumerator ExecuteImpl()
    {
        var characterActionRageStart = this;

        yield return null;

        var actingCharacter = characterActionRageStart.ActingCharacter;

        if (actingCharacter.Stealthy)
        {
            actingCharacter.SetStealthy(false);
        }

        var actionDefinition = PathOfTheSavagery.CombatRageStart;
        var powerDefinition = actionDefinition.ActivatedPower;

        characterActionRageStart.ActionParams.ActionDefinition = actionDefinition;

        var rulesetCharacter = actingCharacter.RulesetCharacter;
        var usablePower = UsablePowersProvider.Get(powerDefinition, rulesetCharacter);
        var effectPower = new RulesetEffectPower(rulesetCharacter, usablePower);

        rulesetCharacter.SpendRagePoint();
        effectPower.ApplyEffectOnCharacter(rulesetCharacter, true, actingCharacter.LocationPosition);

        yield return ServiceRepository.GetService<IGameLocationBattleService>()?
            .HandleReactionToRageStart(actingCharacter);
    }
}
