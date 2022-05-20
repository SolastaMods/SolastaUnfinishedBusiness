using System.Collections.Generic;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Api.AdditionalExtensions;

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
        var attackMode = instance.InvokeMethod("RefreshAttackMode", actionType, itemDefinition, weaponDescription,
            freeOffHand, canAddAbilityDamageBonus, slotName, attackModifiers, featuresOrigin, weapon);

        return (RulesetAttackMode)attackMode;
    }
}