using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Classes;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;
using static ConditionForm;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;
using Resources = SolastaUnfinishedBusiness.Properties.Resources;

namespace SolastaUnfinishedBusiness.Subclasses;

public static class InnovationArmor
{
    private const string GuardianMarkerName = "ConditionInnovationArmorGuardianMode";
    private const string InfiltratorMarkerName = "ConditionInnovationArmorInfiltratorMode";

    public static CharacterSubclassDefinition Build()
    {
        return CharacterSubclassDefinitionBuilder
            .Create("InnovationArmor")
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite("InventorArmor", Resources.InventorArmor, 256))
            .AddFeaturesAtLevel(3, BuildArmoredUp(), BuildAutoPreparedSpells(), BuildArmorModes())
            .AddFeaturesAtLevel(5, AttributeModifierCasterFightingExtraAttack)
            .AddFeaturesAtLevel(9, BuildArmorModification())
            .AddFeaturesAtLevel(15, BuildPerfectedArmor())
            .AddToDB();
    }

    private static FeatureDefinition BuildArmoredUp()
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

    private static FeatureDefinition BuildAutoPreparedSpells()
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
            .AddPreparedSpellGroup(17, SpellsContext.FarStep, WallOfForce)
            .AddToDB();
    }

    private static FeatureDefinition BuildArmorModes()
    {
        var pool = FeatureDefinitionPowerBuilder
            .Create("PowerInnovationArmorModeSelectorPool")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetCustomSubFeatures(new CanUseAttribute(AttributeDefinitions.Intelligence, IsBuiltInWeapon))
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest)
            .AddToDB();

        var guardianMarker = ConditionDefinitionBuilder
            .Create(GuardianMarkerName)
            .SetGuiPresentation(Category.Condition, Sprites.ConditionGuardian)
            .SetSilent(Silent.WhenRemoved)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create("FeatureInnovationArmorGuardianMode")
                    .SetGuiPresentation(GuardianMarkerName, Category.Condition)
                    .AddToDB())
            .AddToDB();

        var infiltratorMarker = ConditionDefinitionBuilder
            .Create(InfiltratorMarkerName)
            .SetGuiPresentation(Category.Condition, Sprites.ConditionInfiltrate)
            .SetSilent(Silent.WhenRemoved)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create("FeatureInnovationArmorInfiltratorMode")
                    .SetGuiPresentation(InfiltratorMarkerName, Category.Condition)
                    .AddToDB(),
                FeatureDefinitionMovementAffinityBuilder
                    .Create("MovementAffinityInnovationArmorInfiltratorMode")
                    .SetGuiPresentationNoContent()
                    .SetBaseSpeedAdditiveModifier(1)
                    .AddToDB(),
                FeatureDefinitionAbilityCheckAffinityBuilder
                    .Create("AbilityCheckAffinityInnovationArmorInfiltratorMode")
                    .SetGuiPresentation(InfiltratorMarkerName, Category.Condition)
                    .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.Advantage,
                        abilityProficiencyPairs: (AttributeDefinitions.Dexterity, SkillDefinitions.Stealth))
                    .AddToDB()
            )
            .AddToDB();

        var guardianMode = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerInnovationArmorSwitchModeGuardian")
            .SetGuiPresentation(Category.Feature, Sprites.PowerGuardianMode)
            .SetCustomSubFeatures(
                new ValidatorsPowerUse(NotGuardianMode),
                ValidatorsPowerUse.NotInCombat,
                new AddGauntletAttack(),
                DoNotTerminateWhileUnconscious.Marker,
                SkipEffectRemovalOnLocationChange.Always
            )
            .SetSharedPool(ActivationTime.BonusAction, pool)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(
                    EffectFormBuilder.ConditionForm(infiltratorMarker, ConditionOperation.Remove, true),
                    EffectFormBuilder.ConditionForm(guardianMarker, ConditionOperation.Add, true)
                )
                .Build())
            .AddToDB();

        var infiltratorMode = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerInnovationArmorSwitchModeInfiltrator")
            .SetGuiPresentation(Category.Feature, Sprites.PowerInfiltratorMode)
            .SetCustomSubFeatures(
                new ValidatorsPowerUse(NotInfiltratorMode),
                ValidatorsPowerUse.NotInCombat,
                new AddLauncherAttack(ActionDefinitions.ActionType.Main, InInfiltratorMode),
                DoNotTerminateWhileUnconscious.Marker,
                SkipEffectRemovalOnLocationChange.Always
            )
            .SetSharedPool(ActivationTime.BonusAction, pool)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(
                    EffectFormBuilder.ConditionForm(guardianMarker, ConditionOperation.Remove, true),
                    EffectFormBuilder.ConditionForm(infiltratorMarker, ConditionOperation.Add, true))
                .Build())
            .AddToDB();

        var defensiveField = FeatureDefinitionPowerBuilder
            .Create("PowerInnovationArmorDefensiveField")
            .SetGuiPresentation(Category.Feature, Sprites.PowerDefensiveField)
            .SetCustomSubFeatures(new ValidatorsPowerUse(InGuardianMode), InventorClassHolder.Marker,
                RecurrenceOnlyOnSelfTurn.Mark)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Minute, 1)
                .SetRecurrentEffect(RecurrentEffect.OnTurnStart | RecurrentEffect.OnActivation)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetTempHpForm(1)
                    .SetLevelAdvancement(EffectForm.LevelApplianceType.MultiplyBonus, LevelSourceType.ClassLevel)
                    .Build())
                .Build())
            .AddToDB();

        return FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetInnovationArmorModes")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(pool, guardianMode, infiltratorMode, defensiveField)
            .AddToDB();
    }

    private static FeatureDefinition BuildArmorModification()
    {
        return FeatureDefinitionPowerUseModifierBuilder
            .Create("PowerUseModifierInventorInfusionPoolArmorModification")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(ArmorerInfusions.Marker)
            .SetFixedValue(InventorClass.InfusionPool, 2)
            .AddToDB();
    }

    private static FeatureDefinition BuildPerfectedArmor()
    {
        var guardian = FeatureDefinitionPowerBuilder
            .Create("PowerInventorArmorerPerfectedArmorGuardian")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new RestrictReactionAttackMode(
                (_, _, _, attackMode, _) =>
                {
                    if (attackMode.sourceDefinition is not ItemDefinition weapon)
                    {
                        return false;
                    }

                    return weapon.weaponDefinition?.WeaponType == CustomWeaponsContext.ThunderGauntletType.Name;
                }))
            .SetUsesFixed(ActivationTime.OnAttackHitMeleeAuto)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.Individuals)
                .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                    EffectDifficultyClassComputation.SpellCastingFeature)
                .SetEffectForms(
                    EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionSlowed))
                .Build())
            .AddToDB();

        var infiltrator = FeatureDefinitionPowerBuilder
            .Create("PowerInventorArmorerPerfectedArmorInfiltrator")
            .SetGuiPresentationNoContent() //since this power has no saving throw payer won't see it anywhere
            .SetCustomSubFeatures(new RestrictReactionAttackMode(
                (_, _, _, attackMode, _) =>
                {
                    if (attackMode.sourceDefinition is not ItemDefinition weapon)
                    {
                        return false;
                    }

                    return weapon.weaponDefinition?.WeaponType == CustomWeaponsContext.LightningLauncherType.Name;
                }))
            .SetUsesFixed(ActivationTime.OnAttackHitAuto)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.Individuals)
                .SetNoSavingThrow()
                .AddEffectForms(EffectFormBuilder.LightSourceForm(LightSourceType.Basic, 0, 1,
                    new Color(0.9f, 0.78f, 0.62f),
                    FeatureDefinitionAdditionalDamages.AdditionalDamageBrandingSmite.LightSourceForm
                        .graphicsPrefabReference))
                .AddEffectForms(EffectFormBuilder.ConditionForm(ConditionDefinitionBuilder
                    .Create("ConditionInventorArmorerInfiltratorGlimmer")
                    .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDazzled)
                    .Detrimental()
                    .SetPossessive()
                    //.AllowMultipleInstances() //TODO: add a way to make only last condition from same source active on same target
                    .SetFeatures(FeatureDefinitionCombatAffinityBuilder
                        .Create("CombatAffinityInventorArmorerInfiltratorGlimmer")
                        .SetGuiPresentation("ConditionInventorArmorerInfiltratorGlimmer", Category.Condition)
                        .SetMyAttackAdvantage(AdvantageType.Disadvantage)
                        .SetSituationalContext(SituationalContext.TargetIsEffectSource)
                        .AddToDB())
                    .AddToDB()))
                .AddEffectForms(EffectFormBuilder.ConditionForm(ConditionDefinitionBuilder
                    .Create("ConditionInventorArmorerInfiltratorDamage")
                    .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBranded)
                    .Detrimental()
                    .SetPossessive()
                    .SetSpecialInterruptions(ExtraConditionInterruption.AfterWasAttacked)
                    .AdditionalDiceDamageWhenHit(1, DieType.D6, AdditionalDamageType.Specific, DamageTypeLightning)
                    .SetFeatures(FeatureDefinitionCombatAffinityBuilder
                        .Create("CombatAffinityInventorArmorerInfiltratorDamage")
                        .SetGuiPresentation("ConditionInventorArmorerInfiltratorDamage", Category.Condition)
                        .SetAttackOnMeAdvantage(AdvantageType.Advantage)
                        .AddToDB())
                    .AddToDB()))
                .Build())
            .AddToDB();

        return FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetInventorArmorerPerfectedArmor")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                guardian,
                infiltrator
            )
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

            hitBonus += modifier.attackRollModifier;
            damageBonus += modifier.damageRollModifier;
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

    internal class AddLauncherAttack : AddExtraAttackBase
    {
        public AddLauncherAttack(
            ActionDefinitions.ActionType actionType,
            params IsCharacterValidHandler[] validators) : base(actionType, validators)
        {
        }

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

            return new List<RulesetAttackMode> { attackMode };
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
