using System.Linq;
using System.Runtime.CompilerServices;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Interfaces;
using static ActionDefinitions;

namespace SolastaUnfinishedBusiness.Validators;

internal delegate bool IsPowerUseValidHandler(RulesetCharacter character, FeatureDefinitionPower power);

internal sealed class ValidatorsValidatePowerUse : IValidatePowerUse
{
    public static readonly IValidatePowerUse NotInCombat = new ValidatorsValidatePowerUse(_ => Gui.Battle == null);

    public static readonly IValidatePowerUse InCombat = new ValidatorsValidatePowerUse(_ => Gui.Battle != null);

    public static readonly IValidatePowerUse HasTacticalMovesAvailable = new ValidatorsValidatePowerUse(character =>
    {
        var glc = GameLocationCharacter.GetFromActor(character);

        return Gui.Battle == null || glc is { RemainingTacticalMoves: > 0 };
    });

    public static readonly IValidatePowerUse HasBonusAttackAvailable = new ValidatorsValidatePowerUse(
        rulesetCharacter =>
        {
            if (Gui.Battle == null)
            {
                return true;
            }

            var character = GameLocationCharacter.GetFromActor(rulesetCharacter);

            if (character == null ||
                character.GetActionStatus(Id.AttackOff, ActionScope.Battle) != ActionStatus.Available)
            {
                return false;
            }

            var maxAttacksNumber = rulesetCharacter.AttackModes
                .FirstOrDefault(attackMode => attackMode.ActionType == ActionType.Bonus)?.AttacksNumber ?? 0;

            return maxAttacksNumber - character.UsedBonusAttacks > 0;
        });

    public static readonly IValidatePowerUse HasMainAttackAvailable = new ValidatorsValidatePowerUse(
        rulesetCharacter =>
        {
            if (Gui.Battle == null)
            {
                return true;
            }

            var character = GameLocationCharacter.GetFromActor(rulesetCharacter);

            if (character == null ||
                character.GetActionStatus(Id.AttackMain, ActionScope.Battle) != ActionStatus.Available)
            {
                return false;
            }

            var maxAttacksNumber = rulesetCharacter.AttackModes
                .FirstOrDefault(attackMode => attackMode.ActionType == ActionType.Main)?.AttacksNumber ?? 0;

            return maxAttacksNumber - character.UsedMainAttacks > 0;
        });

    private readonly IsPowerUseValidHandler[] _validators;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ValidatorsValidatePowerUse(params IsCharacterValidHandler[] validators)
    {
        _validators = validators.Select(v => new IsPowerUseValidHandler((character, _) => v(character))).ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ValidatorsValidatePowerUse(params IsPowerUseValidHandler[] validators)
    {
        _validators = validators;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower power)
    {
        return _validators.All(v => v(character, power));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IValidatePowerUse UsedLessTimesThan(int limit)
    {
        return new ValidatorsValidatePowerUse((character, power) =>
        {
            var user = GameLocationCharacter.GetFromActor(character);

            if (user == null)
            {
                return false;
            }

            user.UsedSpecialFeatures.TryGetValue(power.Name, out var uses);
            return uses < limit;
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IValidatePowerUse HasNoneOfConditions(params string[] types)
    {
        return new ValidatorsValidatePowerUse(ValidatorsCharacter.HasNoneOfConditions(types));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsPowerNotValid(RulesetCharacter character, RulesetUsablePower power)
    {
        return !character.CanUsePower(power.PowerDefinition, false);
    }
}
