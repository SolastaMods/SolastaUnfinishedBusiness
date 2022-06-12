using System.Collections.Generic;

namespace SolastaCommunityExpansion.Api.Extensions;

public static class RulesetCharacterHeroExtensions
{
    public static RulesetAttackMode RefreshAttackModePublic(
        this RulesetCharacterHero instance,
        ActionDefinitions.ActionType actionType,
        ItemDefinition itemDefinition,
        WeaponDescription weaponDescription,
        bool freeOffHand,
        bool canAddAbilityDamageBonus,
        string slotName,
        List<IAttackModificationProvider> attackModifiers,
        Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin> featuresOrigin,
        RulesetItem weapon = null)
    {
        var attackMode = instance.RefreshAttackMode(actionType, itemDefinition, weaponDescription,
            freeOffHand, canAddAbilityDamageBonus, slotName, attackModifiers, featuresOrigin, weapon);

        return attackMode;
    }

    public static List<(string, T)> GetTaggedFeaturesByType<T>(this RulesetCharacterHero hero) where T : class
    {
        var list = new List<(string, T)>();

        foreach (var pair in hero.ActiveFeatures)
        {
            list.AddRange(GetTaggedFeatures<T>(pair.Key, pair.Value));
        }

        return list;
    }

    private static IEnumerable<(string, T)> GetTaggedFeatures<T>(string tag, IEnumerable<FeatureDefinition> features)
        where T : class
    {
        var list = new List<(string, T)>();
        foreach (var feature in features)
        {
            if (feature is FeatureDefinitionFeatureSet {Mode: FeatureDefinitionFeatureSet.FeatureSetMode.Union} set)
            {
                list.AddRange(GetTaggedFeatures<T>(tag, set.FeatureSet));
            }
            else if (feature is T typedFeature)
            {
                list.Add((tag, typedFeature));
            }
        }

        return list;
    }
}
