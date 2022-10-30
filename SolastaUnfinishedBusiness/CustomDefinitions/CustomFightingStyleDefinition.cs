using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

internal sealed class CustomFightingStyleDefinition : FightingStyleDefinition, ICustomFightingStyle
{
    public bool IsActive(RulesetCharacterHero character)
    {
        return true;
    }
}
