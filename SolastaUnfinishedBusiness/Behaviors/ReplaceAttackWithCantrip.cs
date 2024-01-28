using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Interfaces;
using static ActionDefinitions;

namespace SolastaUnfinishedBusiness.Behaviors;

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

        if (character.usedMainAttacks == 0)
        {
            return;
        }

        if (character.usedMainCantrip)
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

        if (actionParams.RulesetEffect is not RulesetEffectSpell spellEffect ||
            spellEffect.spellDefinition.spellLevel > 0)
        {
            return;
        }

        const ActionType ACTION_TYPE = ActionType.Main;
        var rank = --character.currentActionRankByType[ACTION_TYPE];

        //If character can't attack on this action - do not refund it
        if (character.GetActionStatus(Id.AttackMain, ActionScope.Battle) != ActionStatus.Available)
        {
            character.currentActionRankByType[ACTION_TYPE]++;
            return;
        }

        var maxAllowedAttacks = character.actionPerformancesByType[ACTION_TYPE][rank].MaxAttacksNumber;
        var maxAttacksNumber = rulesetCharacter.AttackModes
            .Where(attackMode => attackMode.ActionType == ActionType.Main)
            .Max(attackMode => attackMode.AttacksNumber);

        if (maxAllowedAttacks < 0 || maxAllowedAttacks >= maxAttacksNumber)
        {
            maxAllowedAttacks = maxAttacksNumber;
        }

        character.UsedMainAttacks++;
        rulesetCharacter.ExecutedAttacks++;
        rulesetCharacter.RefreshAttackModes();

        if (character.UsedMainAttacks < maxAllowedAttacks)
        {
            return;
        }

        character.currentActionRankByType[ACTION_TYPE]++;
        character.UsedMainAttacks = 0;
    }
}
