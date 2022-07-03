using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaCommunityExpansion.Api.Extensions;

public static class RulesetCharacterHeroExtensions
{
    public static RulesetAttackMode RefreshAttackModePublic(
        [NotNull] this RulesetCharacterHero instance,
        ActionDefinitions.ActionType actionType,
        ItemDefinition itemDefinition,
        WeaponDescription weaponDescription,
        bool freeOffHand,
        bool canAddAbilityDamageBonus,
        string slotName,
        List<IAttackModificationProvider> attackModifiers,
        Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin> featuresOrigin,
        [CanBeNull] RulesetItem weapon = null)
    {
        var attackMode = instance.RefreshAttackMode(actionType, itemDefinition, weaponDescription,
            freeOffHand, canAddAbilityDamageBonus, slotName, attackModifiers, featuresOrigin, weapon);

        return attackMode;
    }

    [NotNull]
    public static List<(string, T)> GetTaggedFeaturesByType<T>([NotNull] this RulesetCharacterHero hero) where T : class
    {
        var list = new List<(string, T)>();

        foreach (var pair in hero.ActiveFeatures)
        {
            list.AddRange(GetTaggedFeatures<T>(pair.Key, pair.Value));
        }

        return list;
    }

    [NotNull]
    private static IEnumerable<(string, T)> GetTaggedFeatures<T>(string tag,
        [NotNull] IEnumerable<FeatureDefinition> features)
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
