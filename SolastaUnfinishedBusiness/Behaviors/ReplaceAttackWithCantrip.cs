using SolastaUnfinishedBusiness.Api.GameExtensions;
using static ActionDefinitions;

namespace SolastaUnfinishedBusiness.Behaviors;

internal interface IAttackReplaceWithCantrip;

internal static class ReplaceAttackWithCantrip
{
    internal static void AllowCastDuringMainAttack(
        GameLocationCharacter character,
        Id actionId,
        ActionScope scope,
        ref ActionStatus result)
    {
        if (scope != ActionScope.Battle)
        {
            return;
        }

        if (actionId != Id.CastMain)
        {
            return;
        }

        if (!character.RulesetCharacter.HasSubFeatureOfType<IAttackReplaceWithCantrip>())
        {
            return;
        }

        if (character.UsedMainAttacks == 0)
        {
            return;
        }

        if (character.UsedMainCantrip)
        {
            return;
        }

        if (result == ActionStatus.NoLongerAvailable)
        {
            result = ActionStatus.Available;
        }
    }

    internal static void AllowAttacksAfterCantrip(
        GameLocationCharacter character,
        CharacterActionParams actionParams,
        ActionScope scope)
    {
        if (scope != ActionScope.Battle)
        {
            return;
        }

        var rulesetCharacter = character.RulesetCharacter;

        if (!rulesetCharacter.HasSubFeatureOfType<IAttackReplaceWithCantrip>())
        {
            return;
        }

        if (actionParams.actionDefinition.Id != Id.CastMain)
        {
            return;
        }

        if (actionParams.activeEffect is not RulesetEffectSpell spellEffect ||
            spellEffect.spellDefinition.spellLevel > 0)
        {
            return;
        }

        character.UsedMainCantrip = true;
        character.BurnOneMainAttack();
    }
}
