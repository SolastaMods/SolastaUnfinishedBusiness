using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

/// <summary>
///     Implement on a FeatureDefinition to be able to change the min roll value on ability checks
/// </summary>
internal interface IChangeAbilityCheck
{
    public int MinRoll(
        RulesetCharacter character,
        int baseBonus,
        int rollModifier,
        string abilityScoreName,
        string proficiencyName,
        List<RuleDefinitions.TrendInfo> advantageTrends,
        List<RuleDefinitions.TrendInfo> modifierTrends);
}
