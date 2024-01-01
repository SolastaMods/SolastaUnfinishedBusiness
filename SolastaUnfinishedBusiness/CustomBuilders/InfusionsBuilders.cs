using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Classes;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Subclasses;
using UnityEngine.AddressableAssets;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.CustomBuilders;

internal static class InventorInfusions
{
    private const string ReplicaItemTitleFormat = "Item/&ReplicaItemFormatTitle";
    private const string ReplicaItemTitleDescription = "Item/&ReplicaItemFormatDescription";
    private const int UpgradeLevel = 10;
    private static readonly ICustomUnicityTag MagicalDamage = new CustomUnicityTag("MagicalDamage");

    public static void Build()
    {
        #region 02 Enhance Focus

        var name = "InfusionEnhanceArcaneFocus";
        var sprite = Sprites.GetSprite("EnhanceFocus", Resources.EnhanceFocus, 128);
        var power = BuildInfuseItemPowerInvocation(1, name, sprite, IsFocusOrStaff,
            FeatureDefinitionMagicAffinityBuilder
                //TODO: RAW needs to require attunement
                .Create($"MagicAffinity{name}")
                .SetGuiPresentation(name, Category.Feature, FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon3)
                .SetCastingModifiers(1, dcModifier: 1)
                .AddToDB());

        UpgradeInfusionPower(
            power, UpgradeLevel, FeatureDefinitionMagicAffinityBuilder
                //TODO: RAW needs to require attunement
                .Create($"MagicAffinity{name}Upgraded")
                .SetGuiPresentation(name, Category.Feature, FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon3)
                .SetCastingModifiers(2, dcModifier: 2)
                .AddToDB());

        #endregion

        #region 02 Enhance Armor

        name = "InfusionEnhanceDefense";
        sprite = Sprites.GetSprite("EnhanceArmor", Resources.EnhanceArmor, 128);
        power = BuildInfuseItemPowerInvocation(1, name, sprite, IsArmor,
            FeatureDefinitionAttributeModifierBuilder.Create($"AttributeModifier{name}")
                .SetGuiPresentation(name, Category.Feature, ConditionDefinitions.ConditionShielded)
                .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 1)
                .AddToDB());

        UpgradeInfusionPower(
            power, UpgradeLevel, FeatureDefinitionAttributeModifierBuilder
                .Create($"AttributeModifier{name}Upgraded")
                .SetGuiPresentation(name, Category.Feature, ConditionDefinitions.ConditionShielded)
                .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 2)
                .AddToDB());

        #endregion

        #region 02 Enhance Weapon

        name = "InfusionEnhanceWeapon";
        sprite = Sprites.GetSprite("EnhanceWeapon", Resources.EnhanceWeapon, 128);
        power = BuildInfuseItemPowerInvocation(
            1, name, sprite, IsWeapon, FeatureDefinitionAttackModifierBuilder
                .Create($"AttackModifier{name}")
                .SetGuiPresentation(name, Category.Feature, FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon3)
                .SetAttackRollModifier(1)
                .SetDamageRollModifier(1)
                .SetMagicalWeapon()
                .AddToDB());

        UpgradeInfusionPower(
            power, UpgradeLevel, FeatureDefinitionAttackModifierBuilder
                .Create($"AttackModifier{name}Upgraded")
                .SetGuiPresentation(name, Category.Feature, FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon3)
                .SetAttackRollModifier(2)
                .SetDamageRollModifier(2)
                .SetMagicalWeapon()
                .AddToDB());

        #endregion

        #region 02 Mind Sharpener

        name = "InfusionMindSharpener";
        sprite = Sprites.GetSprite("MindSharpener", Resources.MindSharpener, 128);
        BuildInfuseItemPowerInvocation(
            1, name, sprite, IsBodyArmor, FeatureDefinitionMagicAffinityBuilder
                .Create($"MagicAffinity{name}")
                .SetGuiPresentation(name, Category.Feature, ConditionDefinitions.ConditionCalmedByCalmEmotionsAlly)
                //RAW it adds reaction to not break concentration
                .SetConcentrationModifiers(ConcentrationAffinity.Advantage, 10)
                .AddToDB());

        #endregion

        #region 02 Returning Weapon

        sprite = Sprites.GetSprite("ReturningWeapon", Resources.ReturningWeapon, 128);
        name = "InfusionReturningWeapon";
        BuildInfuseItemPowerInvocation(
            1, name, sprite, IsThrownWeapon, FeatureDefinitionAttackModifierBuilder
                .Create($"AttackModifier{name}")
                .SetGuiPresentation(name, Category.Feature, ConditionDefinitions.ConditionRevealedByDetectGoodOrEvil)
                .AddCustomSubFeatures(ReturningWeapon.AlwaysValid)
                .SetAttackRollModifier(1)
                .SetDamageRollModifier(1)
                .SetMagicalWeapon()
                .AddToDB());

        #endregion

        #region 02 Repeating Shot

        sprite = Sprites.GetSprite("RepeatingShot", Resources.RepeatingShot, 128);
        name = "InfusionRepeatingShot";
        BuildInfuseItemPowerInvocation(
            1, name, sprite, IsLoading, FeatureDefinitionAttackModifierBuilder
                .Create($"AttackModifier{name}")
                .SetGuiPresentation(name, Category.Feature, ConditionDefinitions.ConditionJump)
                .AddCustomSubFeatures(RepeatingShot.Instance)
                .SetAttackRollModifier(1)
                .SetDamageRollModifier(1)
                .SetMagicalWeapon()
                .AddToDB());

        #endregion

        #region 02 Minor Elemental

        sprite = SpellDefinitions.Counterspell.GuiPresentation.SpriteReference;
        name = "InfusionMinorElemental";
        //TODO: RAW needs to require attunement

        var elements = new[] { DamageTypeAcid, DamageTypeCold, DamageTypeFire, DamageTypeLightning, DamageTypeThunder };
        var powers = new List<FeatureDefinitionPower>();

        foreach (var element in elements)
        {
            var term = Gui.Localize($"Rules/&{element}Title");
            var title = Gui.Format($"Feature/&AdditionalDamage{name}Title", term);
            var description = Gui.Format($"Feature/&AdditionalDamage{name}Description", term);

            power = BuildInfuseItemPower(name + element, element, sprite, IsWeapon,
                FeatureDefinitionAdditionalDamageBuilder
                    .Create($"AdditionalDamage{name}{element}")
                    .SetGuiPresentation(title, description, ConditionDefinitions.ConditionProtectedFromEnergyLightning)
                    .AddCustomSubFeatures(MagicalDamage)
                    .SetNotificationTag(name)
                    .SetDamageDice(DieType.D6, 1)
                    .SetSpecificDamageType(element)
                    .SetAdvancement(AdditionalDamageAdvancement.None)
                    .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
                    .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
                    .AddToDB());

            power.GuiPresentation.Title = $"Rules/&{element}Title";
            power.GuiPresentation.Description = $"Rules/&{element}Description";

            powers.Add(power);
        }

        var masterPower = BuildInfuseItemPowerInvocation(
            1, name, sprite, FeatureDefinitionPowerSharedPoolBuilder
                .Create($"Power{name}")
                .SetGuiPresentation(name, Category.Feature, sprite)
                .SetSharedPool(ActivationTime.Action, InventorClass.InfusionPool)
                .AddCustomSubFeatures(PowerFromInvocation.Marker)
                .AddToDB());

        PowerBundle.RegisterPowerBundle(masterPower, true, powers);

        #endregion

        #region 06 Bloody

        sprite = SpellDefinitions.CircleOfDeath.GuiPresentation.SpriteReference;
        name = "InfusionBloody";
        BuildInfuseItemPowerInvocation(6, name, sprite, IsWeapon,
            FeatureDefinitionAdditionalDamageBuilder
                .Create($"AdditionalDamage{name}")
                .SetGuiPresentation(name, Category.Feature)
                .AddCustomSubFeatures(MagicalDamage)
                .SetNotificationTag(name)
                .SetDamageDice(DieType.D6, 2)
                .SetRequiredProperty(RestrictedContextRequiredProperty.FinesseOrRangeWeapon)
                .SetTriggerCondition(AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly)
                .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
                .AddToDB());

        #endregion

        #region 06 Resistant Armor

        sprite = Sprites.GetSprite("ResistantArmor", Resources.ResistantArmor, 128);
        name = "InfusionResistantArmor";
        //TODO: RAW needs to require attunement

        elements =
        [
            DamageTypeAcid, DamageTypeCold, DamageTypeFire, DamageTypeForce, DamageTypeLightning,
            DamageTypeNecrotic, DamageTypePoison, DamageTypePsychic, DamageTypeRadiant, DamageTypeThunder
        ];
        powers = [];

        foreach (var element in elements)
        {
            power = BuildInfuseItemPower(name + element, element, sprite, IsBodyArmor,
                FeatureDefinitionDamageAffinityBuilder
                    .Create($"DamageAffinity{name}{element}")
                    .SetGuiPresentation($"Feature/&{name}Title",
                        Gui.Format("Feature/&DamageResistanceFormat", Gui.Localize($"Rules/&{element}Title")),
                        ConditionDefinitions.ConditionProtectedFromEnergyLightning)
                    .AddCustomSubFeatures(ReturningWeapon.AlwaysValid)
                    .SetDamageAffinityType(DamageAffinityType.Resistance)
                    .SetDamageType(element)
                    .AddToDB());

            power.GuiPresentation.Title = $"Rules/&{element}Title";
            power.GuiPresentation.Description = $"Rules/&{element}Description";

            powers.Add(power);
        }

        masterPower = BuildInfuseItemPowerInvocation(
            6, name, sprite, FeatureDefinitionPowerSharedPoolBuilder
                .Create($"Power{name}")
                .SetGuiPresentation(name, Category.Feature, sprite)
                .SetSharedPool(ActivationTime.Action, InventorClass.InfusionPool)
                .AddCustomSubFeatures(PowerFromInvocation.Marker)
                .AddToDB());

        PowerBundle.RegisterPowerBundle(masterPower, true, powers);

        #endregion

        #region 10 Major Elemental

        sprite = SpellDefinitions.Counterspell.GuiPresentation.SpriteReference;
        name = "InfusionMajorElemental";
        //TODO: RAW needs to require attunement

        elements = [DamageTypeAcid, DamageTypeCold, DamageTypeFire, DamageTypeLightning, DamageTypeThunder];
        powers = [];

        foreach (var element in elements)
        {
            var term = Gui.Localize($"Rules/&{element}Title");
            var title = Gui.Format($"Feature/&AdditionalDamage{name}Title", term);
            var description = Gui.Format($"Feature/&AdditionalDamage{name}Description", term);

            power = BuildInfuseItemPower(name + element, element, sprite, IsWeapon,
                FeatureDefinitionAdditionalDamageBuilder
                    .Create($"AdditionalDamage{name}{element}")
                    .SetGuiPresentation(title, description, ConditionDefinitions.ConditionProtectedFromEnergyLightning)
                    .AddCustomSubFeatures(MagicalDamage)
                    .SetNotificationTag(name)
                    .SetDamageDice(DieType.D4, 1)
                    .SetSpecificDamageType(element)
                    .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
                    .AddToDB());

            power.GuiPresentation.Title = $"Rules/&{element}Title";
            power.GuiPresentation.Description = $"Rules/&{element}Description";

            powers.Add(power);
        }

        masterPower = BuildInfuseItemPowerInvocation(
            10, name, sprite, FeatureDefinitionPowerSharedPoolBuilder
                .Create($"Power{name}")
                .SetGuiPresentation(name, Category.Feature, sprite)
                .SetSharedPool(ActivationTime.Action, InventorClass.InfusionPool)
                .AddCustomSubFeatures(PowerFromInvocation.Marker)
                .AddToDB());

        PowerBundle.RegisterPowerBundle(masterPower, true, powers);

        #endregion

        #region Replicate Magic Item

        #region Level 02

        var level = 1;
        BuildCreateItemPowerInvocation(ItemDefinitions.Backpack_Bag_Of_Holding, level);
        //RAW this should be spectacles that don't require attunement
        BuildCreateItemPowerInvocation(ItemDefinitions.RingDarkvision, level);
        BuildCreateItemPowerInvocation(ItemDefinitions.WandOfMagicDetection, level);
        BuildCreateItemPowerInvocation(ItemDefinitions.WandOfIdentify, level);
        //RAW they are level 6, but at that level you get Cloak Of Elvenkind which is better
        BuildCreateItemPowerInvocation(ItemDefinitions.BootsOfElvenKind, level);

        #endregion

        #region Level 06

        level = 6;
        BuildCreateItemPowerInvocation(ItemDefinitions.CloakOfElvenkind, level);
        BuildCreateItemPowerInvocation(CustomItemsContext.GlovesOfThievery, level);
        BuildCreateItemPowerInvocation(ItemDefinitions.PipesOfHaunting, level);
        //RAW they are level 14, but at 10 you get much better Winged Boots
        BuildCreateItemPowerInvocation(ItemDefinitions.BootsLevitation, level);
        //RAW they are level 10, but at 10 you get Winged Boots and Slippers Of Spider Climbing
        BuildCreateItemPowerInvocation(ItemDefinitions.BootsOfStridingAndSpringing, level);

        #endregion

        #region Level 10

        level = 10;
        BuildCreateItemPowerInvocation(ItemDefinitions.Bracers_Of_Archery, level);
        BuildCreateItemPowerInvocation(ItemDefinitions.BroochOfShielding, level);
        BuildCreateItemPowerInvocation(ItemDefinitions.CloakOfProtection, level);
        BuildCreateItemPowerInvocation(ItemDefinitions.GauntletsOfOgrePower, level);
        BuildCreateItemPowerInvocation(ItemDefinitions.GlovesOfMissileSnaring, level);
        BuildCreateItemPowerInvocation(ItemDefinitions.HeadbandOfIntellect, level);
        BuildCreateItemPowerInvocation(CustomItemsContext.HelmOfAwareness, level);
        BuildCreateItemPowerInvocation(ItemDefinitions.SlippersOfSpiderClimbing, level);
        BuildCreateItemPowerInvocation(ItemDefinitions.BootsWinged, level);

        #endregion

        #region Level 14

        level = 14;
        BuildCreateItemPowerInvocation(ItemDefinitions.AmuletOfHealth, level);
        BuildCreateItemPowerInvocation(ItemDefinitions.BeltOfGiantHillStrength, level);
        BuildCreateItemPowerInvocation(ItemDefinitions.Bracers_Of_Defense, level);
        BuildCreateItemPowerInvocation(ItemDefinitions.RingProtectionPlus1, level);
        BuildCreateItemPowerInvocation(ItemDefinitions.GemOfSeeing, level);
        BuildCreateItemPowerInvocation(ItemDefinitions.HornOfBlasting, level);

        //Sadly this one is just a copy of Cloak of Protection as of v1.4.13
        // BuildCreateItemPowerInvocation(ItemDefinitions.CloakOfBat, 14);

        #endregion

        #endregion
    }

    private static FeatureDefinitionPowerSharedPool BuildInfuseItemPowerInvocation(
        int level,
        string name,
        AssetReferenceSprite icon,
        IsValidItemHandler filter,
        params FeatureDefinition[] features)
    {
        var power = BuildInfuseItemPower(name, name, icon, filter, features);

        BuildInfuseItemPowerInvocation(level, name, icon, power);

        return power;
    }

    private static FeatureDefinitionPower BuildInfuseItemPowerInvocation(
        int level,
        string name,
        AssetReferenceSprite icon,
        FeatureDefinitionPower power)
    {
        CustomInvocationDefinitionBuilder
            .Create($"CustomInvocation{name}")
            .SetGuiPresentation(name, Category.Feature, icon)
            .SetPoolType(InvocationPoolTypeCustom.Pools.Infusion)
            .SetRequirements(level)
            .SetGrantedFeature(power)
            .AddToDB();

        return power;
    }

    private static void BuildCreateItemPowerInvocation(ItemDefinition item, int level = 2)
    {
        var replica = BuildItemReplica(item);
        var description = BuildReplicaDescription(item);
        var invocation = CustomInvocationDefinitionBuilder
            .Create($"CustomInvocationCreate{replica.name}")
            .SetGuiPresentation(Category.Feature, replica)
            .SetPoolType(InvocationPoolTypeCustom.Pools.Infusion)
            .SetRequirements(level)
            .SetGrantedFeature(BuildCreateItemPower(replica, description))
            .AddToDB();

        invocation.Item = replica;
        invocation.GuiPresentation.title = replica.FormatTitle();
        invocation.GuiPresentation.description = description;
    }

    private static FeatureDefinitionPowerSharedPool BuildInfuseItemPower(
        string name,
        string guiName,
        AssetReferenceSprite icon,
        IsValidItemHandler itemFilter,
        params FeatureDefinition[] features)
    {
        return BuildInfuseItemPower(name, guiName, icon, new InfusionItemFilter(itemFilter), features);
    }

    private static FeatureDefinitionPowerSharedPool BuildInfuseItemPower(
        string name,
        string guiName,
        AssetReferenceSprite icon,
        ICustomItemFilter itemFilter,
        params FeatureDefinition[] features)
    {
        var powerName = $"Power{name}";

        return FeatureDefinitionPowerSharedPoolBuilder.Create(powerName)
            .SetGuiPresentation(guiName, Category.Feature, icon)
            .SetSharedPool(ActivationTime.Action, InventorClass.InfusionPool)
            .AddCustomSubFeatures(
                DoNotTerminateWhileUnconscious.Marker,
                ExtraCarefulTrackedItem.Marker,
                SkipEffectRemovalOnLocationChange.Always,
                InventorClass.InfusionLimiter,
                PowerFromInvocation.Marker,
                itemFilter)
            .SetEffectDescription(BuildInfuseItemWithFeaturesEffect(features))
            .AddToDB();
    }

    private static void UpgradeInfusionPower(
        FeatureDefinitionPower power,
        int level,
        params FeatureDefinition[] features)
    {
        power.AddCustomSubFeatures(
            new ModifyEffectDescriptionOnLevels(
                InventorClass.Class,
                power,
                (level, BuildInfuseItemWithFeaturesEffect(features))));
    }

    private static EffectDescription BuildInfuseItemWithFeaturesEffect(params FeatureDefinition[] features)
    {
        var properties = features.Select(f =>
        {
            f.AddCustomSubFeatures(ExtraCarefulTrackedItem.Marker);
            return new FeatureUnlockByLevel(f, 0);
        });

        return EffectDescriptionBuilder
            .Create()
            .SetAnimationMagicEffect(AnimationDefinitions.AnimationMagicEffect.Animation1)
            .SetTargetingData(Side.Ally, RangeType.Distance, 3, TargetType.Item,
                itemSelectionType: ActionDefinitions.ItemSelectionType.Carried)
            .SetParticleEffectParameters(FeatureDefinitionPowers.PowerOathOfJugementWeightOfJustice)
            .SetDurationData(DurationType.Permanent)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .HasSavingThrow(EffectSavingThrowType.None)
                    .SetItemPropertyForm(ItemPropertyUsage.Unlimited, 1, properties.ToArray())
                    .Build())
            .Build();
    }

    private static FeatureDefinitionPowerSharedPool BuildCreateItemPower(ItemDefinition item, string description)
    {
        var powerName = $"PowerCreate{item.name}";
        var power = FeatureDefinitionPowerSharedPoolBuilder.Create(powerName)
            .SetGuiPresentation(Category.Feature, item)
            .SetSharedPool(ActivationTime.Action, InventorClass.InfusionPool)
            .AddCustomSubFeatures(
                DoNotTerminateWhileUnconscious.Marker,
                ExtraCarefulTrackedItem.Marker,
                SkipEffectRemovalOnLocationChange.Always,
                InventorClass.InfusionLimiter,
                PowerFromInvocation.Marker)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetAnimationMagicEffect(AnimationDefinitions.AnimationMagicEffect.Animation1)
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Self)
                    .SetParticleEffectParameters(SpellDefinitions.Bless)
                    .SetDurationData(DurationType.Permanent)
                    .SetEffectForms(
                        EffectFormBuilder
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

    // ReSharper disable once SuggestBaseTypeForParameter
    private static string GuiReplicaTitle(ItemDefinition item)
    {
        return Gui.Format(ReplicaItemTitleFormat, item.FormatTitle());
    }

    // ReSharper disable once SuggestBaseTypeForParameter
    private static string BuildReplicaDescription(ItemDefinition item)
    {
        return Gui.Format(ReplicaItemTitleDescription, item.FormatTitle(), item.FormatDescription());
    }

    #region Item Filters

    private class InfusionItemFilter : CustomItemFilter
    {
        internal InfusionItemFilter(IsValidItemHandler handler) : base(handler)
        {
        }

        public override bool IsValid(RulesetCharacter character, RulesetItem rulesetItem, RulesetEffect rulesetEffect)
        {
            return !rulesetItem.dynamicItemProperties
                       .Select(property => EffectHelpers.GetEffectByGuid(property.sourceEffectGuid))
                       .Any(effect => effect != null && effect.SourceDefinition == rulesetEffect.SourceDefinition) &&
                   base.IsValid(character, rulesetItem, rulesetEffect);
        }
    }

    private static bool IsArmorSmithItem(RulesetCharacter character, RulesetItem item)
    {
        //Weapon, or armor if character is level 9 armor smith
        return character.HasSubFeatureOfType<InnovationArmor.ArmorerInfusions>()
               && IsBodyArmor(character, item);
    }

    private static bool IsFocusOrStaff(RulesetCharacter _, RulesetItem item)
    {
        var definition = item.ItemDefinition;
        var staffType = WeaponTypeDefinitions.QuarterstaffType.Name;
        return definition.IsFocusItem
               || (definition.IsWeapon && definition.WeaponDescription.WeaponType == staffType);
    }

    private static bool IsWeapon(RulesetCharacter character, RulesetItem item)
    {
        //Weapon, or armor if character is level 9 armor smith
        return item.ItemDefinition.IsWeapon || IsArmorSmithItem(character, item);
    }

    private static bool IsThrownWeapon(RulesetCharacter _, RulesetItem item)
    {
        var definition = item.ItemDefinition;
        return definition.IsWeapon
               && definition.WeaponDescription.WeaponTags.Contains(TagsDefinitions.WeaponTagThrown);
    }

    private static bool IsLoading(RulesetCharacter _, RulesetItem item)
    {
        var definition = item.ItemDefinition;
        return definition.IsWeapon
               && definition.WeaponDescription.WeaponTags.Contains(TagsDefinitions.WeaponTagLoading);
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
