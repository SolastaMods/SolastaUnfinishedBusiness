using System.Linq;
using System.Runtime.CompilerServices;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomValidators;

internal delegate bool IsPowerUseValidHandler(RulesetCharacter character, FeatureDefinitionPower power);

internal sealed class ValidatorsPowerUse : IPowerUseValidity
{
    public static readonly IPowerUseValidity NotInCombat = new ValidatorsPowerUse(_ =>
        !ServiceRepository.GetService<IGameLocationBattleService>().IsBattleInProgress);

    public static readonly IPowerUseValidity InCombat = new ValidatorsPowerUse(_ =>
        ServiceRepository.GetService<IGameLocationBattleService>().IsBattleInProgress);

    private readonly IsPowerUseValidHandler[] validators;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ValidatorsPowerUse(params IsCharacterValidHandler[] validators)
    {
        this.validators = validators.Select(v => new IsPowerUseValidHandler((character, _) => v(character))).ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ValidatorsPowerUse(params IsPowerUseValidHandler[] validators)
    {
        this.validators = validators;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower power)
    {
        return validators.All(v => v(character, power));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IPowerUseValidity UsedLessTimesThan(int limit)
    {
        return new ValidatorsPowerUse((character, power) =>
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
    internal static IPowerUseValidity HasNoneOfConditions(params string[] types)
    {
        return new ValidatorsPowerUse(ValidatorsCharacter.HasNoneOfConditions(types));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsPowerNotValid(RulesetCharacter character, RulesetUsablePower power)
    {
        return !character.CanUsePower(power.PowerDefinition, false);
    }
}
