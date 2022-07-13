using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.CustomDefinitions;

public enum ExtendedSituationalContext
{
    MainWeaponIsMelee = 1000
}

public static class CustomSituationalContext
{
    public static bool IsContextValid(RulesetImplementationDefinitions.SituationalContextParams contextParams, bool def)
    {
        var context = contextParams.situationalContext;
        
        return (ExtendedSituationalContext)context switch
        {
            ExtendedSituationalContext.MainWeaponIsMelee => CharacterValidators.MainHandIsMeleeWeapon(contextParams
                .source),
            _ => def
        };
    }
}
