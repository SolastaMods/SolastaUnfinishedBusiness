using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Utils;
using UnityEngine.AddressableAssets;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Classes.Inventor;

internal static class Infusions
{
    private const string ReplicaItemTitleFormat = "Item/&ReplicaItemFormatTitle";
    private const string ReplicaItemTitleDescription = "Item/&ReplicaItemFormatDescription";

    public static readonly FeatureDefinitionFeatureSet ImprovedInfusions = FeatureDefinitionFeatureSetBuilder
        .Create("FeatureSetInfusionUpgrade")
        .SetGuiPresentationNoContent(true)
        .AddToDB();

    public static void Build()
    {
        #region 02 Enhance Focus

        var name = "InfusionEnhanceArcaneFocus";
        var sprite = CustomIcons.GetSprite("EnhanceFocus", Resources.EnhanceFocus, 128);
        var power = BuildInfuseItemPowerInvocation(2, name, sprite, IsFocusOrStaff,
            FeatureDefinitionMagicAffinityBuilder
                //TODO: RAW needs to require attunement
                .Create($"MagicAffinity{name}")
                .SetGuiPresentation(name, Category.Feature, FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon3)
                .SetCastingModifiers(1, dcModifier: 1)
                .AddToDB());

        BuildUpgradedInfuseItemPower(name, power, sprite, IsFocusOrStaff, FeatureDefinitionMagicAffinityBuilder
            //TODO: RAW needs to require attunement
            .Create($"MagicAffinity{name}Upgraded")
            .SetGuiPresentation(name, Category.Feature, FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon3)
            .SetCastingModifiers(2, dcModifier: 2)
            .AddToDB());

        #endregion

        #region 02 Enhance Armor

        name = "InfusionEnhanceDefense";
        sprite = CustomIcons.GetSprite("EnhanceArmor", Resources.EnhanceArmor, 128);
        power = BuildInfuseItemPowerInvocation(2, name, sprite, IsArmor, FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{name}")
            .SetGuiPresentation(name, Category.Feature, ConditionDefinitions.ConditionShielded)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 1)
            .AddToDB());

        BuildUpgradedInfuseItemPower(name, power, sprite, IsArmor, FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{name}Upgraded")
            .SetGuiPresentation(name, Category.Feature, ConditionDefinitions.ConditionShielded)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 2)
            .AddToDB());

        #endregion

        #region 02 Enhance Weapon

        name = "InfusionEnhanceWeapon";
        sprite = CustomIcons.GetSprite("EnhanceWeapon", Resources.EnhanceWeapon, 128);
        power = BuildInfuseItemPowerInvocation(2, name, sprite, IsWeapon, FeatureDefinitionAttackModifierBuilder
            .Create($"AttackModifier{name}")
            .SetGuiPresentation(name, Category.Feature, FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon3)
            .SetAttackRollModifier(1)
            .SetDamageRollModifier(1)
            .SetMagicalWeapon()
            .AddToDB());

        BuildUpgradedInfuseItemPower(name, power, sprite, IsWeapon,
            FeatureDefinitionAttackModifierBuilder
                .Create($"AttackModifier{name}Upgraded")
                .SetGuiPresentation(name, Category.Feature, FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon3)
                .SetAttackRollModifier(2)
                .SetDamageRollModifier(2)
                .SetMagicalWeapon()
                .AddToDB());

        #endregion

        #region 02 Mind Sharpener

        name = "InfusionMindSharpener";
        sprite = CustomIcons.GetSprite("MindSharpener", Resources.MindSharpener, 128);
        BuildInfuseItemPowerInvocation(2, name, sprite, IsBodyArmor, FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{name}")
            .SetGuiPresentation(name, Category.Feature, ConditionDefinitions.ConditionCalmedByCalmEmotionsAlly)
            //RAW it adds reaction to not break concentration
            .SetConcentrationModifiers(ConcentrationAffinity.Advantage, 10)
            .AddToDB());

        #endregion

        #region 02 Returning Weapon

        sprite = CustomIcons.GetSprite("ReturningWeapon", Resources.ReturningWeapon, 128);
        name = "InfusionReturningWeaponWithBonus";
        var infuseWithBonus = BuildInfuseItemPower(name, name, sprite, IsThrownWeapon,
            FeatureDefinitionAttackModifierBuilder
                .Create($"AttackModifier{name}")
                .SetGuiPresentation(name, Category.Feature, ConditionDefinitions.ConditionRevealedByDetectGoodOrEvil)
                .SetCustomSubFeatures(ReturningWeapon.Instance)
                .SetAttackRollModifier(1)
                .SetDamageRollModifier(1)
                .SetMagicalWeapon()
                .AddToDB());

        name = "InfusionReturningWeaponNoBonus";
        var noBonusModifier = FeatureDefinitionAttackModifierBuilder
            .Create($"AttackModifier{name}")
            .SetGuiPresentation(name, Category.Feature, ConditionDefinitions.ConditionRevealedByDetectGoodOrEvil)
            .SetCustomSubFeatures(ReturningWeapon.Instance)
            .SetAttackRollModifier(0)
            .SetDamageRollModifier(0)
            .AddToDB();

        var infuseNoBonus = BuildInfuseItemPower(name, name, sprite, new CustomItemFilter(IsThrownWeapon),
            noBonusModifier);

        //remove Infused marker by setting Returning marker
        noBonusModifier.SetCustomSubFeatures(ReturningWeapon.Instance);

        name = "InfusionReturningWeapon";
        var masterPower = BuildInfuseItemPowerInvocation(2, name, sprite, FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{name}")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetUsesFixed(ActivationTime.Action)
            .SetCustomSubFeatures(ValidatorsPowerUse.NotInCombat)
            .SetUniqueInstance()
            .SetSharedPool(InventorClass.InfusionPool)
            .AddToDB());

        PowersBundleContext.RegisterPowerBundle(masterPower, true, infuseWithBonus, infuseNoBonus);

        #endregion

        #region 06 Resistant Armor

        sprite = CustomIcons.GetSprite("ResistantArmor", Resources.ResistantArmor, 128);
        name = "InfusionResistantArmor";
        //TODO: RAW needs to require attunement

        var elements = new[]
        {
            DamageTypeAcid, DamageTypeCold, DamageTypeFire, DamageTypeForce, DamageTypeLightning,
            DamageTypeNecrotic, DamageTypePoison, DamageTypePsychic, DamageTypeRadiant, DamageTypeThunder
        };
        var powers = new List<FeatureDefinitionPower>();

        foreach (var element in elements)
        {
            power = BuildInfuseItemPower(name + element, element, sprite, IsBodyArmor,
                FeatureDefinitionDamageAffinityBuilder
                    .Create($"DamageAffinity{name}{element}")
                    .SetGuiPresentation($"Feature/&{name}Title",
                        Gui.Format("Feature/&DamageResistanceFormat", Gui.Localize($"Rules/&{element}Title")),
                        ConditionDefinitions.ConditionProtectedFromEnergyLightning)
                    .SetCustomSubFeatures(ReturningWeapon.Instance)
                    .SetDamageAffinityType(DamageAffinityType.Resistance)
                    .SetDamageType(element)
                    .AddToDB());

            power.GuiPresentation.Title = $"Rules/&{element}Title";
            power.GuiPresentation.Description = $"Rules/&{element}Description";

            powers.Add(power);
        }

        masterPower = BuildInfuseItemPowerInvocation(6, name, sprite, FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{name}")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetCustomSubFeatures(ValidatorsPowerUse.NotInCombat)
            .SetUsesFixed(ActivationTime.Action)
            .SetUniqueInstance()
            .SetSharedPool(InventorClass.InfusionPool)
            .AddToDB());

        PowersBundleContext.RegisterPowerBundle(masterPower, true, powers);

        #endregion

        #region Replicate Magic Item

        //Level 02
        BuildCreateItemPowerInvocation(ItemDefinitions.Backpack_Bag_Of_Holding);
        BuildCreateItemPowerInvocation(ItemDefinitions.WandOfMagicDetection);
        BuildCreateItemPowerInvocation(ItemDefinitions.WandOfIdentify);
        //RAW they are level 6, but at that level you get Cloak Of Elvenkind which is better
        BuildCreateItemPowerInvocation(ItemDefinitions.BootsOfElvenKind);

        //Level 06
        BuildCreateItemPowerInvocation(ItemDefinitions.CloakOfElvenkind, 6);
        BuildCreateItemPowerInvocation(ItemDefinitions.PipesOfHaunting, 6);
        //RAW they are level 14, but at 10 you get much better Winged Boots
        BuildCreateItemPowerInvocation(ItemDefinitions.BootsLevitation, 6);


        //Level 10
        BuildCreateItemPowerInvocation(ItemDefinitions.BootsOfStridingAndSpringing, 10);
        BuildCreateItemPowerInvocation(ItemDefinitions.Bracers_Of_Archery, 10);
        BuildCreateItemPowerInvocation(ItemDefinitions.BroochOfShielding, 10);
        BuildCreateItemPowerInvocation(ItemDefinitions.CloakOfProtection, 10);
        BuildCreateItemPowerInvocation(ItemDefinitions.GauntletsOfOgrePower, 10);
        BuildCreateItemPowerInvocation(ItemDefinitions.GlovesOfMissileSnaring, 10);
        BuildCreateItemPowerInvocation(ItemDefinitions.HeadbandOfIntellect, 10);
        BuildCreateItemPowerInvocation(ItemDefinitions.SlippersOfSpiderClimbing, 10);
        BuildCreateItemPowerInvocation(ItemDefinitions.BootsWinged, 10);

        //Level 14
        BuildCreateItemPowerInvocation(ItemDefinitions.AmuletOfHealth, 14);
        BuildCreateItemPowerInvocation(ItemDefinitions.BeltOfGiantHillStrength, 14);
        BuildCreateItemPowerInvocation(ItemDefinitions.Bracers_Of_Defense, 14);
        BuildCreateItemPowerInvocation(ItemDefinitions.RingProtectionPlus1, 14);
        BuildCreateItemPowerInvocation(ItemDefinitions.GemOfSeeing, 14);
        BuildCreateItemPowerInvocation(ItemDefinitions.HornOfBlasting, 14);

        //Sadly this one is just a copy of Cloak of Protection as of v1.4.13
        // BuildCreateItemPowerInvocation(ItemDefinitions.CloakOfBat, 14);

        #endregion
    }

    private static FeatureDefinitionPower BuildInfuseItemPowerInvocation(int level,
        string name, AssetReferenceSprite icon,
        IsValidItemHandler filter, params FeatureDefinition[] features)
    {
        var power = BuildInfuseItemPower(name, name, icon, filter, features);
        BuildInfuseItemPowerInvocation(level, name, icon, power);
        return power;
    }

    private static FeatureDefinitionPower BuildInfuseItemPowerInvocation(int level, string name,
        AssetReferenceSprite icon,
        FeatureDefinitionPower power)
    {
        CustomInvocationDefinitionBuilder
            .Create($"Invocation{name}")
            .SetGuiPresentation(name, Category.Feature, icon)
            .SetCustomSubFeatures(Hidden.Marker)
            .SetPoolType(CustomInvocationPoolType.Pools.Infusion)
            .SetRequiredLevel(level)
            .SetGrantedFeature(power)
            .AddToDB();
        return power;
    }

    private static void BuildCreateItemPowerInvocation(ItemDefinition item, int level = 2)
    {
        var replica = BuildItemReplica(item);
        var description = BuildReplicaDescription(item);
        var invocation = CustomInvocationDefinitionBuilder
            .Create($"InvocationCreate{replica.name}")
            .SetGuiPresentation(Category.Feature, replica)
            .SetCustomSubFeatures(Hidden.Marker)
            .SetPoolType(CustomInvocationPoolType.Pools.Infusion)
            .SetRequiredLevel(level)
            .SetGrantedFeature(BuildCreateItemPower(replica, description))
            .AddToDB();

        invocation.Item = replica;
        invocation.GuiPresentation.title = replica.FormatTitle();
        invocation.GuiPresentation.description = description;
    }

    private static FeatureDefinitionPowerSharedPool BuildInfuseItemPower(string name, string guiName,
        AssetReferenceSprite icon, IsValidItemHandler itemFilter, params FeatureDefinition[] features)
    {
        return BuildInfuseItemPower(name, guiName, icon, new InfusionItemFilter(itemFilter), features);
    }

    private static FeatureDefinitionPowerSharedPool BuildInfuseItemPower(string name, string guiName,
        AssetReferenceSprite icon, ICustomItemFilter itemFilter, params FeatureDefinition[] features)
    {
        var properties = features.Select(f =>
        {
            f.AddCustomSubFeatures(Infused.Marker);
            return new FeatureUnlockByLevel(f, 0);
        });

        return FeatureDefinitionPowerSharedPoolBuilder.Create($"Power{name}")
            .SetGuiPresentation(guiName, Category.Feature, icon)
            .SetUsesFixed(ActivationTime.Action)
            .SetUniqueInstance()
            .SetSharedPool(InventorClass.InfusionPool)
            .SetCustomSubFeatures(ExtraCarefulTrackedItem.Marker, InventorClass.InfusionLimiter,
                SkipEffectRemovalOnLocationChange.Always, ValidatorsPowerUse.NotInCombat, itemFilter)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetAnimationMagicEffect(AnimationDefinitions.AnimationMagicEffect.Animation1)
                .SetTargetingData(Side.Ally, RangeType.Distance, 3, TargetType.Item,
                    itemSelectionType: ActionDefinitions.ItemSelectionType.Carried)
                .SetParticleEffectParameters(FeatureDefinitionPowers.PowerOathOfJugementWeightOfJustice)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .HasSavingThrow(EffectSavingThrowType.None)
                    .SetItemPropertyForm(ItemPropertyUsage.Unlimited, 1, properties.ToArray())
                    .Build())
                .Build())
            .AddToDB();
    }

    private static void BuildUpgradedInfuseItemPower(string name, FeatureDefinitionPower power,
        AssetReferenceSprite sprite, IsValidItemHandler itemFilter, params FeatureDefinition[] features)
    {
        var upgrade = BuildInfuseItemPower($"{name}Upgraded", name,
            sprite, itemFilter, features);
        upgrade.overriddenPower = power;
        upgrade.AddCustomSubFeatures(new ValidatorsPowerUse(ValidatorsCharacter.HasAnyFeature(power)));
        ImprovedInfusions.FeatureSet.Add(upgrade);
    }

    private static FeatureDefinitionPowerSharedPool BuildCreateItemPower(ItemDefinition item, string description)
    {
        var power = FeatureDefinitionPowerSharedPoolBuilder.Create($"PowerCreate{item.name}")
            .SetGuiPresentation(Category.Feature, item)
            .SetUsesFixed(ActivationTime.Action)
            .SetSharedPool(InventorClass.InfusionPool)
            .SetCustomSubFeatures(
                ExtraCarefulTrackedItem.Marker,
                SkipEffectRemovalOnLocationChange.Always,
                InventorClass.InfusionLimiter,
                ValidatorsPowerUse.NotInCombat)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetAnimationMagicEffect(AnimationDefinitions.AnimationMagicEffect.Animation1)
                .SetTargetingData(Side.All, RangeType.Self, 1, TargetType.Self)
                .SetParticleEffectParameters(SpellDefinitions.Bless)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .HasSavingThrow(EffectSavingThrowType.None)
                    .SetSummonItemForm(item, 1, true)
                    .Build())
                .Build())
            .AddToDB();

        power.GuiPresentation.title = item.FormatTitle();
        power.GuiPresentation.description = description;

        return power;
    }

    private static ItemDefinition BuildItemReplica(ItemDefinition baseItem)
    {
        var replica = ItemDefinitionBuilder
            .Create(baseItem, $"InfusedReplica{baseItem.name}")
            .AddItemTags(TagsDefinitions.ItemTagQuest) //TODO: implement custom tag, instead of quest
            .SetGold(0)
            .HideFromDungeonEditor()
            .SetRequiresIdentification(false)
            .AddToDB();

        replica.GuiPresentation.title = GuiReplicaTitle(baseItem);

        return replica;
    }

    private static string GuiReplicaTitle(ItemDefinition item)
    {
        return Gui.Format(ReplicaItemTitleFormat, item.FormatTitle());
    }

    private static string BuildReplicaDescription(ItemDefinition item)
    {
        return Gui.Format(ReplicaItemTitleDescription, item.FormatTitle(), item.FormatDescription());
    }

    #region Item Filters

    private class Infused
    {
        private Infused() { }
        public static Infused Marker { get; } = new();
    }

    private class InfusionItemFilter : CustomItemFilter
    {
        internal InfusionItemFilter(IsValidItemHandler handler) : base(handler)
        {
        }

        public override bool IsValid(RulesetCharacter character, RulesetItem rulesetItem)
        {
            if (rulesetItem.ItemDefinition.magical)
            {
                return false;
            }

            if (rulesetItem.HasSubFeatureOfType<Infused>())
            {
                return false;
            }

            return base.IsValid(character, rulesetItem);
        }
    }

    private static bool IsFocusOrStaff(RulesetCharacter _, RulesetItem item)
    {
        var definition = item.ItemDefinition;
        var staffType = WeaponTypeDefinitions.QuarterstaffType.Name;
        return definition.IsFocusItem
               || (definition.IsWeapon && definition.WeaponDescription.WeaponType == staffType);
    }

    private static bool IsWeapon(RulesetCharacter _, RulesetItem item)
    {
        return item.ItemDefinition.IsWeapon;
    }

    private static bool IsThrownWeapon(RulesetCharacter _, RulesetItem item)
    {
        var definition = item.ItemDefinition;
        return definition.IsWeapon
               && definition.WeaponDescription.WeaponTags.Contains(TagsDefinitions.WeaponTagThrown);
    }

    private static bool IsArmor(RulesetCharacter _, RulesetItem item)
    {
        return item.ItemDefinition.IsArmor;
    }

    private static bool IsBodyArmor(RulesetCharacter _, RulesetItem item)
    {
        var definition = item.ItemDefinition;
        return definition.IsArmor
               && definition.SlotsWhereActive.Contains(SlotTypeDefinitions.TorsoSlot.Name);
    }

    #endregion
}
