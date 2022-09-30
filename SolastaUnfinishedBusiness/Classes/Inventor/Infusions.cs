using System.Linq;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using UnityEngine.AddressableAssets;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Classes.Inventor;

internal static class Infusions
{
    public static void Build()
    {
        var name = "InfusionEnhanceArcaneFocus";
        BuildInfuseItemPowerInvocation(name,
            FeatureDefinitionPowers.PowerDomainOblivionHeraldOfPain.GuiPresentation.SpriteReference, IsFocusOrStaff,
            FeatureDefinitionMagicAffinityBuilder
                //TODO: RAW needs to require attunement
                .Create($"MagicAffinity{name}")
                .SetGuiPresentation(name, Category.Feature, FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon3)
                .SetCastingModifiers(10, dcModifier: 10)
                .AddToDB());

        name = "InfusionEnhanceDefense";
        BuildInfuseItemPowerInvocation(name,
            FeatureDefinitionPowers.PowerPaladinAuraOfProtection.GuiPresentation.SpriteReference, IsArmor,
            FeatureDefinitionAttributeModifierBuilder
                .Create($"AttributeModifier{name}")
                .SetGuiPresentation(name, Category.Feature, FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon3)
                .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 1)
                .AddToDB());

        name = "InfusionEnhanceWeapon";
        BuildInfuseItemPowerInvocation(name,
            FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference, IsWeapon,
            FeatureDefinitionAttackModifierBuilder
                .Create($"AttackModifier{name}")
                .SetGuiPresentation(name, Category.Feature, FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon3)
                .Configure(attackRollModifier: 1, damageRollModifier: 1)
                .AddToDB());
    }

    private static void BuildInfuseItemPowerInvocation(string name, AssetReferenceSprite icon,
        IsValidItemHandler filter, params FeatureDefinition[] features)
    {
        CustomInvocationDefinitionBuilder
            .Create($"Invocation{name}")
            .SetGuiPresentation(name, Category.Feature, icon)
            .SetPoolType(CustomInvocationPoolType.Pools.Infusion)
            .SetGrantedFeature(BuildInfuseItemPower(name, icon, filter, features))
            .AddToDB();
    }

    private static FeatureDefinitionPowerSharedPool BuildInfuseItemPower(string name,
        AssetReferenceSprite icon, IsValidItemHandler itemFilter, params FeatureDefinition[] features)
    {
        return FeatureDefinitionPowerSharedPoolBuilder.Create($"Power{name}")
            .SetGuiPresentation(name, Category.Feature, icon)
            .SetActivationTime(ActivationTime.Action)
            .SetCostPerUse(1)
            .SetUniqueInstance()
            .SetSharedPool(InventorClass.InfusionPool)
            .SetCustomSubFeatures(ExtraCarefulTrackedItem.Marker,
                InventorClass.InfusionLimiter,
                new CustomItemFilter(itemFilter))
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetAnimation(AnimationDefinitions.AnimationMagicEffect.Animation1)
                .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Item,
                    itemSelectionType: ActionDefinitions.ItemSelectionType.Carried)
                .SetParticleEffectParameters(FeatureDefinitionPowers.PowerOathOfJugementWeightOfJustice)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(new EffectFormBuilder()
                    .HasSavingThrow(EffectSavingThrowType.None)
                    .SetItemPropertyForm(features.Select(f => new FeatureUnlockByLevel(f, 0)),
                        ItemPropertyUsage.Unlimited, 1)
                    .Build())
                .Build())
            .AddToDB();
    }

    #region Item Filters

    private static bool IsFocusOrStaff(RulesetCharacter _, RulesetItem item)
    {
        var definition = item.ItemDefinition;
        var staffType = WeaponTypeDefinitions.QuarterstaffType.Name;
        return !definition.Magical
               && (definition.IsFocusItem
                   || (definition.IsWeapon && definition.WeaponDescription.WeaponType == staffType));
    }

    private static bool IsWeapon(RulesetCharacter _, RulesetItem item)
    {
        var definition = item.ItemDefinition;
        return !definition.Magical && definition.IsWeapon;
    }

    private static bool IsArmor(RulesetCharacter _, RulesetItem item)
    {
        var definition = item.ItemDefinition;
        return !definition.Magical && definition.IsArmor;
    }

    #endregion
}
