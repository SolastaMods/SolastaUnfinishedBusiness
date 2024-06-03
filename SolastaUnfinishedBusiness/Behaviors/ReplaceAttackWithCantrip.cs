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
        if (scope != ActionScope.Battle ||
            actionId != Id.CastMain ||
            character.UsedMainCantrip ||
            character.UsedMainAttacks == 0 ||
            result != ActionStatus.NoLongerAvailable ||
            !character.RulesetCharacter.HasSubFeatureOfType<IAttackReplaceWithCantrip>())
        {
            return;
        }

        result = ActionStatus.Available;
    }

    internal static void AllowAttacksAfterCantrip(
        GameLocationCharacter character,
        CharacterActionParams actionParams,
        ActionScope scope)
    {
        if (scope != ActionScope.Battle ||
            actionParams.actionDefinition.Id != Id.CastMain ||
            actionParams.activeEffect is not RulesetEffectSpell spellEffect ||
            spellEffect.spellDefinition.spellLevel > 0 ||
            !character.RulesetCharacter.HasSubFeatureOfType<IAttackReplaceWithCantrip>())
        {
            return;
        }

        character.UsedMainCantrip = true;
        character.BurnOneMainAttack();
    }
}
