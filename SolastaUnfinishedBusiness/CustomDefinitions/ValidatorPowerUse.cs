using System.Linq;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

internal delegate bool IsPowerUseValidHandler(RulesetCharacter character, FeatureDefinitionPower power);

internal sealed class ValidatorPowerUse : IPowerUseValidity
{
    public static readonly IPowerUseValidity NotInCombat = new ValidatorPowerUse(_ =>
        !ServiceRepository.GetService<IGameLocationBattleService>().IsBattleInProgress);

    public static readonly IPowerUseValidity InCombat = new ValidatorPowerUse(_ =>
        ServiceRepository.GetService<IGameLocationBattleService>().IsBattleInProgress);

    private readonly IsPowerUseValidHandler[] validators;

    internal ValidatorPowerUse(params IsCharacterValidHandler[] validators)
    {
        this.validators = validators.Select(v => new IsPowerUseValidHandler((character, _) => v(character))).ToArray();
    }

    internal ValidatorPowerUse(params IsPowerUseValidHandler[] validators)
    {
        this.validators = validators;
    }

    public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower power)
    {
        return validators.All(v => v(character, power));
    }

    internal static IPowerUseValidity UsedLessTimesThan(int limit)
    {
        return new ValidatorPowerUse((character, power) =>
        {
            var user = GameLocationCharacter.GetFromActor(character);
            if (user == null) { return false; }

            user.UsedSpecialFeatures.TryGetValue(power.Name, out var uses);
            return uses < limit;
        });
    }

    internal static bool IsPowerNotValid(RulesetCharacter character, RulesetUsablePower power)
    {
        return !character.CanUsePower(power.PowerDefinition, false);
    }
}
