using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

public enum ExtendedSituationalContext
{
    MainWeaponIsMelee = 1000,
    WearingNoArmorOrLightArmorWithoutShield = 1001
}

public static class CustomSituationalContext
{
    public static bool IsContextValid(RulesetImplementationDefinitions.SituationalContextParams contextParams, bool def)
    {
        var context = contextParams.situationalContext;

        return (ExtendedSituationalContext)context switch
        {
            ExtendedSituationalContext.MainWeaponIsMelee =>
                CharacterValidators.MainHandIsMeleeWeapon(contextParams.source),

            ExtendedSituationalContext.WearingNoArmorOrLightArmorWithoutShield =>
                CharacterValidators.NoArmor(contextParams.source)
                || (CharacterValidators.LightArmor(contextParams.source)
                    && CharacterValidators.NoShield(contextParams.source)),

            _ => def
        };
    }
}
