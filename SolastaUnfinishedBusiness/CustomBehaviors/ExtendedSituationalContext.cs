using SolastaUnfinishedBusiness.Api.Extensions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal static class CustomSituationalContext
{
    internal static bool IsContextValid(
        RulesetImplementationDefinitions.SituationalContextParams contextParams,
        bool def)
    {
        var context = contextParams.situationalContext;
        RulesetEntity effectSource = null;
        
        if (contextParams.sourceEffectId != 0)
        {
            RulesetEntity.TryGetEntity(contextParams.sourceEffectId, out effectSource);
        }

        return (ExtraSituationalContext)context switch
        {
            ExtraSituationalContext.MainWeaponIsMelee =>
                ValidatorsCharacter.MainHandIsMeleeWeapon(contextParams.source),

            ExtraSituationalContext.WearingNoArmorOrLightArmorWithoutShield =>
                ValidatorsCharacter.NoArmor(contextParams.source)
                || (ValidatorsCharacter.LightArmor(contextParams.source)
                    && ValidatorsCharacter.NoShield(contextParams.source)),

            ExtraSituationalContext.MainWeaponIsFinesseOrLightRange =>
                ValidatorsCharacter.MainHandIsFinesseWeapon(contextParams.source)
                || ValidatorsCharacter.HasLightRangeWeapon(contextParams.source),

            ExtraSituationalContext.MainWeaponIsVersatileWithoutShield =>
                ValidatorsCharacter.MainHandIsVersatileWeapon(contextParams.source)
                && ValidatorsCharacter.NoShield(contextParams.source),

            ExtraSituationalContext.TargetIsNotEffectSource =>
                contextParams.target != effectSource,

            _ => def
        };
    }
}
