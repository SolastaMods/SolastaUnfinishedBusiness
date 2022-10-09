using SolastaUnfinishedBusiness.Api.Extensions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal static class CustomSituationalContext
{
    internal static bool IsContextValid(RulesetImplementationDefinitions.SituationalContextParams contextParams,
        bool def)
    {
        var context = contextParams.situationalContext;

        return (ExtraSituationalContext)context switch
        {
            ExtraSituationalContext.MainWeaponIsMelee =>
                ValidatorsCharacter.MainHandIsMeleeWeapon(contextParams.source),

            ExtraSituationalContext.WearingNoArmorOrLightArmorWithoutShield =>
                ValidatorsCharacter.NoArmor(contextParams.source)
                || (ValidatorsCharacter.LightArmor(contextParams.source)
                    && ValidatorsCharacter.NoShield(contextParams.source)),

            ExtraSituationalContext.MainWeaponIsFinesseOrRange =>
                ValidatorsCharacter.MainHandIsFinesseWeapon(contextParams.source)
                || ValidatorsCharacter.HasTwoHandedRangeWeapon(contextParams.source),

            ExtraSituationalContext.MainWeaponIsVersatileWithoutShield =>
                ValidatorsCharacter.MainHandIsVersatileWeapon(contextParams.source)
                && ValidatorsCharacter.NoShield(contextParams.source),

            _ => def
        };
    }
}
