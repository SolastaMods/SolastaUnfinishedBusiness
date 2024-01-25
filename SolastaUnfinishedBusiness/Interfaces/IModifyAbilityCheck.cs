using System.Collections.Generic;
using JetBrains.Annotations;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Interfaces;

/// <summary>
///     Implement on a FeatureDefinition to be able to change the min roll value on ability checks
/// </summary>
public interface IModifyAbilityCheck
{
    [UsedImplicitly]
    public int MinRoll(
        RulesetCharacter character,
        int baseBonus,
        int rollModifier,
        string abilityScoreName,
        string proficiencyName,
        List<TrendInfo> advantageTrends,
        List<TrendInfo> modifierTrends);
}
