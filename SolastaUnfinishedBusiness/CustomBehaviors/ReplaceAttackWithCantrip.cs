using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal static class ReplaceAttackWithCantrip
{
    internal static void AllowCastDuringMainAttack(
        GameLocationCharacter character,
        ActionDefinitions.Id actionId,
        ActionDefinitions.ActionScope scope,
        ref ActionDefinitions.ActionStatus result)
    {
        if (scope != ActionDefinitions.ActionScope.Battle)
        {
            return;
        }

        if (actionId != ActionDefinitions.Id.CastMain)
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

        if (result == ActionDefinitions.ActionStatus.NoLongerAvailable)
        {
            result = ActionDefinitions.ActionStatus.Available;
        }
    }

    internal static void AllowAttacksAfterCantrip(GameLocationCharacter character, CharacterActionParams actionParams,
        ActionDefinitions.ActionScope scope)
    {
        if (scope != ActionDefinitions.ActionScope.Battle)
        {
            return;
        }

        var rulesetCharacter = character.RulesetCharacter;

        if (!rulesetCharacter.HasSubFeatureOfType<IAttackReplaceWithCantrip>())
        {
            return;
        }

        if (actionParams.actionDefinition.Id != ActionDefinitions.Id.CastMain)
        {
            return;
        }

        if (actionParams.RulesetEffect is not RulesetEffectSpell spellEffect ||
            spellEffect.spellDefinition.spellLevel > 0)
        {
            return;
        }

        const ActionDefinitions.ActionType ACTION_TYPE = ActionDefinitions.ActionType.Main;
        var rank = --character.currentActionRankByType[ACTION_TYPE];

        var maxAllowedAttacks = character.actionPerformancesByType[ACTION_TYPE][rank].MaxAttacksNumber;
        var maxAttacksNumber = rulesetCharacter.AttackModes
            .Where(attackMode => attackMode.ActionType == ActionDefinitions.ActionType.Main)
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
