using System.Collections.Generic;
using System.Linq;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.CustomDefinitions;

public interface ICanIgnoreAoOImmunity
{
    bool CanIgnoreAoOImmunity(RulesetCharacter character, RulesetCharacter attacker);
}

public static class AttacksOfOpportunity
{
    public static readonly ICanIgnoreAoOImmunity CanIgnoreDisengage = new CanIgnoreDisengage();

    public static bool IsSubjectToAttackOfOpportunity(RulesetCharacter character, RulesetCharacter attacker,
        bool def)
    {
        return def || attacker.GetSubFeaturesByType<ICanIgnoreAoOImmunity>()
            .Any(f => f.CanIgnoreAoOImmunity(character, attacker));
    }
}

internal class CanIgnoreDisengage : ICanIgnoreAoOImmunity
{
    public bool CanIgnoreAoOImmunity(RulesetCharacter character, RulesetCharacter attacker)
    {
        var FeaturesToBrowse = new List<FeatureDefinition>();
        character.EnumerateFeaturesToBrowse<ICombatAffinityProvider>(FeaturesToBrowse);
        var service = ServiceRepository.GetService<IRulesetImplementationService>();
        var disengaging = DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityDisengaging;
        foreach (var feature in FeaturesToBrowse)
        {
            if (feature != disengaging
                && feature is ICombatAffinityProvider affinityProvider
                && service.IsSituationalContextValid(
                    new RulesetImplementationDefinitions.SituationalContextParams(
                        affinityProvider.SituationalContext, attacker,
                        character, service.FindSourceIdOfFeature(character, feature),
                        affinityProvider.RequiredCondition, null))
                && affinityProvider.IsImmuneToOpportunityAttack(character, attacker))
            {
                return false;
            }
        }

        return true;
    }
}