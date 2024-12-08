using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Classes;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Validators;
using UnityEngine;
using static RuleDefinitions;
using static ConditionForm;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;
using Resources = SolastaUnfinishedBusiness.Properties.Resources;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class InnovationArmor : AbstractSubclass
{
    private const string GuardianMarkerName = "ConditionInnovationArmorGuardianMode";
    private const string InfiltratorMarkerName = "ConditionInnovationArmorInfiltratorMode";

    // ReSharper disable once ConvertConstructorToMemberInitializers
    public InnovationArmor()
    {
        Subclass = CharacterSubclassDefinitionBuilder
            .Create("InnovationArmor")
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite("InventorArmor", Resources.InventorArmor, 256))
            .AddFeaturesAtLevel(3, BuildArmoredUp(), BuildAutoPreparedSpells(), BuildArmorModes())
            .AddFeaturesAtLevel(5, AttributeModifierCasterFightingExtraAttack)
            .AddFeaturesAtLevel(9, BuildArmorModification())
            .AddFeaturesAtLevel(15, BuildPerfectedArmor())
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }
    internal override CharacterClassDefinition Klass => InventorClass.Class;
    internal override FeatureDefinitionSubclassChoice SubclassChoice => InventorClass.SubclassChoice;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static FeatureDefinitionFeatureSet BuildArmoredUp()
    {
        var proficiency = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyInnovationArmorArmoredUp")
            .SetGuiPresentationNoContent()
            .SetProficiencies(ProficiencyType.Armor, EquipmentDefinitions.HeavyArmorCategory)
            .AddToDB();

        var heavyImmunity = FeatureDefinitionMovementAffinityBuilder
            .Create("MovementAffinityInnovationArmorArmoredUp")
            .SetGuiPresentationNoContent()
            .SetImmunities(heavyArmorImmunity: true)
            .AddToDB();

        return FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetInnovationArmorArmoredUp")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(proficiency, heavyImmunity)
            .AddToDB();
    }

    private static FeatureDefinitionAutoPreparedSpells BuildAutoPreparedSpells()
    {
        return FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("AutoPreparedSpellsInnovationArmor")
            .SetGuiPresentation(Category.Feature)
            .SetSpellcastingClass(InventorClass.Class)
            .SetAutoTag("InventorArmorer")
            .AddPreparedSpellGroup(3, MagicMissile, Shield)
            .AddPreparedSpellGroup(5, MirrorImage, Shatter)
            .AddPreparedSpellGroup(9, HypnoticPattern, LightningBolt)
            .AddPreparedSpellGroup(13, FireShield, GreaterInvisibility)
            .AddPreparedSpellGroup(17, SpellsContext.FarStep, HoldMonster)
            .AddToDB();
    }

    private static FeatureDefinitionFeatureSet BuildArmorModes()
    {
        var pool = FeatureDefinitionPowerBuilder
            .Create("PowerInnovationArmorModeSelectorPool")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .AddCustomSubFeatures(new CanUseAttribute(AttributeDefinitions.Intelligence, IsBuiltInWeapon))
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest)
            .AddToDB();

        var guardianMarker = ConditionDefinitionBuilder
            .Create(GuardianMarkerName)
            .SetGuiPresentation(Category.Condition,
                Sprites.GetSprite("ConditionGuardian", Resources.ConditionGuardian, 32))
            .SetSilent(Silent.WhenRemoved)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create("FeatureInnovationArmorGuardianMode")
                    .SetGuiPresentation(GuardianMarkerName, Category.Condition)
                    .AddToDB())
            .AddToDB();

        var infiltratorMarker = ConditionDefinitionBuilder
            .Create(InfiltratorMarkerName)
            .SetGuiPresentation(Category.Condition,
                Sprites.GetSprite("ConditionInfiltrate", Resources.ConditionInfiltrate, 32))
            .SetSilent(Silent.WhenRemoved)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create("FeatureInnovationArmorInfiltratorMode")
                    .SetGuiPresentation(InfiltratorMarkerName, Category.Condition)
                    .AddToDB(),
                FeatureDefinitionMovementAffinityBuilder
                    .Create("MovementAffinityInnovationArmorInfiltratorMode")
                    .SetGuiPresentation(InfiltratorMarkerName, Category.Condition, Gui.NoLocalization)
                    .SetBaseSpeedAdditiveModifier(1)
                    .AddToDB(),
                FeatureDefinitionAbilityCheckAffinityBuilder
                    .Create("AbilityCheckAffinityInnovationArmorInfiltratorMode")
                    .SetGuiPresentation(InfiltratorMarkerName, Category.Condition, Gui.NoLocalization)
                    .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.Advantage,
                        abilityProficiencyPairs: (AttributeDefinitions.Dexterity, SkillDefinitions.Stealth))
                    .AddToDB())
            .AddToDB();

        var guardianMode = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerInnovationArmorSwitchModeGuardian")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerGuardianMode", Resources.PowerGuardianMode, 256, 128))
            .AddCustomSubFeatures(
                new ValidatorsValidatePowerUse(NotGuardianMode),
                ValidatorsValidatePowerUse.NotInCombat,
                new AddGauntletAttack(),
                RestrictEffectToNotTerminateWhileUnconscious.Marker,
                SkipEffectRemovalOnLocationChange.Always)
            .SetSharedPool(ActivationTime.BonusAction, pool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Permanent)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(infiltratorMarker, ConditionOperation.Remove),
                        EffectFormBuilder.ConditionForm(guardianMarker))
                    .Build())
            .AddToDB();

        var infiltratorMode = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerInnovationArmorSwitchModeInfiltrator")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerInfiltratorMode", Resources.PowerInfiltratorMode, 256, 128))
            .AddCustomSubFeatures(
                new ValidatorsValidatePowerUse(NotInfiltratorMode),
                ValidatorsValidatePowerUse.NotInCombat,
                new AddLauncherAttack(ActionDefinitions.ActionType.Main, InInfiltratorMode),
                RestrictEffectToNotTerminateWhileUnconscious.Marker,
                SkipEffectRemovalOnLocationChange.Always)
            .SetSharedPool(ActivationTime.BonusAction, pool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Permanent)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(guardianMarker, ConditionOperation.Remove),
                        EffectFormBuilder.ConditionForm(infiltratorMarker))
                    .Build())
            .AddToDB();

        var defensiveField = FeatureDefinitionPowerBuilder
            .Create("PowerInnovationArmorDefensiveField")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerDefensiveField", Resources.PowerDefensiveField, 256, 128))
            .AddCustomSubFeatures(
                new ValidatorsValidatePowerUse(InGuardianMode),
                // required as in a feature set
                ClassHolder.Inventor,
                RestrictRecurrentEffectsOnSelfTurnOnly.Mark)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.UntilAnyRest)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetTempHpForm(1)
                            .SetLevelAdvancement(
                                EffectForm.LevelApplianceType.MultiplyBonus, LevelSourceType.ClassLevel)
                            .Build())
                    .Build())
            .AddToDB();

        return FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetInnovationArmorModes")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(pool, guardianMode, infiltratorMode, defensiveField)
            .AddToDB();
    }

    private static FeatureDefinitionPowerUseModifier BuildArmorModification()
    {
        return FeatureDefinitionPowerUseModifierBuilder
            .Create("PowerUseModifierInventorInfusionPoolArmorModification")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(ArmorerInfusions.Marker)
            .SetFixedValue(InventorClass.InfusionPool, 2)
            .AddToDB();
    }

    private static FeatureDefinitionFeatureSet BuildPerfectedArmor()
    {
        var guardian = FeatureDefinitionPowerBuilder
            .Create("PowerInventorArmorerPerfectedArmorGuardian")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnAttackHitMeleeAuto)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.Individuals)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(ConditionDefinitions.ConditionSlowed, ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(new RestrictReactionAttackMode(
                (_, _, _, attackMode, _) =>
                    attackMode?.sourceDefinition is ItemDefinition weapon &&
                    weapon.weaponDefinition?.WeaponType == CustomWeaponsContext.ThunderGauntletType.Name))
            .AddToDB();

        var infiltrator = FeatureDefinitionPowerBuilder
            .Create("PowerInventorArmorerPerfectedArmorInfiltrator")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.OnAttackHitAuto)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.Individuals)
                    .SetEffectForms(EffectFormBuilder.LightSourceForm(LightSourceType.Basic, 0, 1,
                            new Color(0.9f, 0.78f, 0.62f),
                            AdditionalDamageBrandingSmite.LightSourceForm.graphicsPrefabReference),
                        EffectFormBuilder.ConditionForm(
                            ConditionDefinitionBuilder
                                .Create("ConditionInventorArmorerInfiltratorGlimmer")
                                .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDazzled)
                                .Detrimental()
                                .SetPossessive()
                                //.AllowMultipleInstances() //TODO: add a way to make only last condition from same source active on same target
                                .SetFeatures(
                                    FeatureDefinitionCombatAffinityBuilder
                                        .Create("CombatAffinityInventorArmorerInfiltratorGlimmer")
                                        .SetGuiPresentation("ConditionInventorArmorerInfiltratorGlimmer",
                                            Category.Condition, Gui.NoLocalization)
                                        .SetMyAttackAdvantage(AdvantageType.Disadvantage)
                                        .SetSituationalContext(SituationalContext.TargetIsEffectSource)
                                        .AddToDB())
                                .AddToDB()),
                        EffectFormBuilder.ConditionForm(
                            ConditionDefinitionBuilder
                                .Create("ConditionInventorArmorerInfiltratorDamage")
                                .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBranded)
                                .Detrimental()
                                .SetPossessive()
                                .SetSpecialInterruptions(ExtraConditionInterruption.AfterWasAttacked)
                                .AdditionalDiceDamageWhenHit(1, DieType.D6, AdditionalDamageType.Specific,
                                    DamageTypeLightning)
                                .SetFeatures(
                                    FeatureDefinitionCombatAffinityBuilder
                                        .Create("CombatAffinityInventorArmorerInfiltratorDamage")
                                        .SetGuiPresentation("ConditionInventorArmorerInfiltratorDamage",
                                            Category.Condition, Gui.NoLocalization)
                                        .SetAttackOnMeAdvantage(AdvantageType.Advantage)
                                        .AddToDB())
                                .AddToDB()))
                    .Build())
            .AddCustomSubFeatures(new RestrictReactionAttackMode(
                (_, _, _, attackMode, _) =>
                    attackMode?.sourceDefinition is ItemDefinition weapon &&
                    weapon.weaponDefinition?.WeaponType == CustomWeaponsContext.LightningLauncherType.Name))
            .AddToDB();

        return FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetInventorArmorerPerfectedArmor")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                guardian,
                infiltrator)
            .AddToDB();
    }

    internal static bool InGuardianMode(RulesetCharacter character)
    {
        return character.HasConditionOfType(GuardianMarkerName);
    }

    private static bool NotGuardianMode(RulesetCharacter character)
    {
        return !character.HasConditionOfType(GuardianMarkerName);
    }

    internal static bool InInfiltratorMode(RulesetCharacter character)
    {
        return character.HasConditionOfType(InfiltratorMarkerName);
    }

    private static bool NotInfiltratorMode(RulesetCharacter character)
    {
        return !character.HasConditionOfType(InfiltratorMarkerName);
    }

    internal static bool IsBuiltInWeapon(RulesetAttackMode mode, RulesetItem weapon, RulesetCharacter character)
    {
        var item = mode?.sourceDefinition as ItemDefinition;

        return item == CustomWeaponsContext.ThunderGauntlet || item == CustomWeaponsContext.LightningLauncher;
    }

    private static void AddArmorBonusesToBuiltinAttack(RulesetCharacter hero, RulesetAttackMode attackMode)
    {
        var features = new List<FeatureDefinition>();
        var inventorySlotsByName = hero.CharacterInventory.InventorySlotsByName;
        var armor = inventorySlotsByName[EquipmentDefinitions.SlotTypeTorso].EquipedItem;

        if (armor == null)
        {
            return;
        }

        armor.EnumerateFeaturesToBrowse<FeatureDefinitionAttackModifier>(features);

        var hitBonus = 0;
        var damageBonus = 0;
        var magical = armor.ItemDefinition.Magical;

        foreach (var modifier in features.OfType<FeatureDefinitionAttackModifier>())
        {
            if (modifier.magicalWeapon)
            {
                magical = true;
            }

            hitBonus += modifier.AttackRollModifier;
            damageBonus += modifier.DamageRollModifier;
        }

        if (magical)
        {
            attackMode.AddAttackTagAsNeeded(TagsDefinitions.MagicalWeapon);
        }

        TrendInfo trendInfo;
        if (hitBonus != 0)
        {
            trendInfo = new TrendInfo(hitBonus, FeatureSourceType.Equipment, armor.ItemDefinition.GuiPresentation.Title,
                null);

            attackMode.ToHitBonus += hitBonus;
            attackMode.ToHitBonusTrends.Add(trendInfo);
        }

        var damage = attackMode.EffectDescription?.FindFirstDamageForm();


        if (damageBonus == 0 || damage == null)
        {
            return;
        }

        trendInfo = new TrendInfo(damageBonus, FeatureSourceType.Equipment, armor.ItemDefinition.GuiPresentation.Title,
            null);
        damage.BonusDamage += damageBonus;
        damage.DamageBonusTrends.Add(trendInfo);
    }

    private class AddGauntletAttack : AddExtraAttackBase
    {
        public AddGauntletAttack() : base(ActionDefinitions.ActionType.Main, InGuardianMode,
            ValidatorsCharacter.HasFreeHandWithoutTwoHandedInMain)
        {
        }

        protected override List<RulesetAttackMode> GetAttackModes(RulesetCharacter character)
        {
            if (character is not RulesetCharacterHero hero)
            {
                return null;
            }

            var strikeDefinition = CustomWeaponsContext.ThunderGauntlet;

            var attackModifiers = hero.attackModifiers;

            var attackMode = hero.RefreshAttackMode(
                ActionType,
                strikeDefinition,
                strikeDefinition.WeaponDescription,
                ValidatorsCharacter.IsFreeOffhandVanilla(hero),
                true,
                EquipmentDefinitions.SlotTypeMainHand,
                attackModifiers,
                hero.FeaturesOrigin
            );

            AddArmorBonusesToBuiltinAttack(hero, attackMode);

            var modes = new List<RulesetAttackMode> { attackMode };

            var main = hero.GetMainWeapon();
            var off = hero.GetOffhandWeapon();

            WeaponDescription weapon = null;
            if (main != null && main.itemDefinition.isWeapon)
            {
                weapon = main.itemDefinition.WeaponDescription;
            }


            if (off == null //empty off-hand
                && (main == null //empty main hand
                    || (weapon != null //or main hand is 1-handed melee weapon
                        && !weapon.WeaponTags.Contains(TagsDefinitions.WeaponTagTwoHanded)
                        && weapon.WeaponTypeDefinition.weaponProximity == AttackProximity.Melee))
                && hero.CanDualWieldNonLight)
            {
                var offhand = RulesetAttackMode.AttackModesPool.Get();

                offhand.Copy(attackMode);
                offhand.attacksNumber = 1;
                offhand.ActionType = ActionDefinitions.ActionType.Bonus;
                modes.Add(offhand);
            }

            if (main != null)
            {
                return modes;
            }

            var reaction = RulesetAttackMode.AttackModesPool.Get();

            reaction.Copy(attackMode);
            reaction.attacksNumber = 1;
            reaction.ActionType = ActionDefinitions.ActionType.Reaction;
            modes.Add(reaction);

            return modes;
        }

        protected override AttackModeOrder GetOrder(RulesetCharacter character)
        {
            return character is RulesetCharacterHero hero && hero.HasEmptyMainHand()
                ? AttackModeOrder.Start
                : base.GetOrder(character);
        }
    }

    internal class AddLauncherAttack(
        ActionDefinitions.ActionType actionType,
        params IsCharacterValidHandler[] validators)
        : AddExtraAttackBase(actionType, validators)
    {
        protected override List<RulesetAttackMode> GetAttackModes(RulesetCharacter character)
        {
            if (character is not RulesetCharacterHero hero)
            {
                return null;
            }

            var strikeDefinition = CustomWeaponsContext.LightningLauncher;
            var attackModifiers = hero.attackModifiers;
            var attackMode = hero.RefreshAttackMode(
                ActionType,
                strikeDefinition,
                strikeDefinition.WeaponDescription,
                ValidatorsCharacter.IsFreeOffhandVanilla(hero),
                true,
                EquipmentDefinitions.SlotTypeMainHand,
                attackModifiers,
                hero.FeaturesOrigin
            );

            var attacked = hero.HasConditionOfType(CustomWeaponsContext.AttackedWithLauncherConditionName);

            if (!attacked)
            {
                var damage = attackMode.EffectDescription.FindFirstDamageForm();

                if (damage != null)
                {
                    damage.diceNumber = 2;
                }
            }

            AddArmorBonusesToBuiltinAttack(hero, attackMode);

            return [attackMode];
        }
    }

    internal class ArmorerInfusions
    {
        private ArmorerInfusions()
        {
        }

        public static ArmorerInfusions Marker { get; } = new();
    }
}
