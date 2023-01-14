using System.Linq;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal delegate bool IsPowerUseValidHandler(RulesetCharacter character, FeatureDefinitionPower power);

internal sealed class ValidatorsPowerUse : IPowerUseValidity
{
    public static readonly IPowerUseValidity NotInCombat = new ValidatorsPowerUse(_ =>
        !ServiceRepository.GetService<IGameLocationBattleService>().IsBattleInProgress);

    // public static readonly IPowerUseValidity InCombat = new ValidatorsPowerUse(_ =>
    //     ServiceRepository.GetService<IGameLocationBattleService>().IsBattleInProgress);

    private readonly IsPowerUseValidHandler[] validators;

    internal ValidatorsPowerUse(params IsCharacterValidHandler[] validators)
    {
        this.validators = validators.Select(v => new IsPowerUseValidHandler((character, _) => v(character))).ToArray();
    }

    private ValidatorsPowerUse(params IsPowerUseValidHandler[] validators)
    {
        this.validators = validators;
    }

    public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower power)
    {
        return validators.All(v => v(character, power));
    }

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

    internal static IPowerUseValidity HasNoCondition(params string[] types)
    {
        return new ValidatorsPowerUse(ValidatorsCharacter.HasNoCondition(types));
    }

    internal static bool IsPowerNotValid(RulesetCharacter character, RulesetUsablePower power)
    {
        return !character.CanUsePower(power.PowerDefinition, false);
    }
}
