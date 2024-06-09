using System.Linq;
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

        // very similar to BurnOneMainAttack but differences are to handle Action Surge and other scenarios
        var rulesetCharacter = character.RulesetCharacter;

        character.HandleMonkMartialArts();
        character.HasAttackedSinceLastTurn = true;
        character.UsedMainAttacks++;
        rulesetCharacter.ExecutedAttacks++;
        rulesetCharacter.RefreshAttackModes();

        var maxAttacks = rulesetCharacter.AttackModes
            .FirstOrDefault(attackMode => attackMode.ActionType == ActionType.Main)?.AttacksNumber ?? 0;

        if (character.UsedMainAttacks < maxAttacks)
        {
            character.currentActionRankByType[ActionType.Main]--;
        }
        else
        {
            character.UsedMainAttacks = 0;
        }
    }
}
