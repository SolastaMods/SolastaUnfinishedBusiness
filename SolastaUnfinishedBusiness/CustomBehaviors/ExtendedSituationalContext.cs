namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal enum ExtendedSituationalContext
{
    MainWeaponIsMelee = 1000,
    WearingNoArmorOrLightArmorWithoutShield = 1001
}

internal static class CustomSituationalContext
{
    internal static bool IsContextValid(RulesetImplementationDefinitions.SituationalContextParams contextParams,
        bool def)
    {
        var context = contextParams.situationalContext;

        return (ExtendedSituationalContext)context switch
        {
            ExtendedSituationalContext.MainWeaponIsMelee =>
                ValidatorsCharacter.MainHandIsMeleeWeapon(contextParams.source),

            ExtendedSituationalContext.WearingNoArmorOrLightArmorWithoutShield =>
                ValidatorsCharacter.NoArmor(contextParams.source)
                || (ValidatorsCharacter.LightArmor(contextParams.source)
                    && ValidatorsCharacter.NoShield(contextParams.source)),

            _ => def
        };
    }
}
