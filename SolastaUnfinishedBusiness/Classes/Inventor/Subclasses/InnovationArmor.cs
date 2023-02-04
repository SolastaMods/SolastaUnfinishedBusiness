using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Classes.Inventor.Subclasses;

public static class InnovationArmor
{
    private const string GuardianMarkerName = "ConditionInnovationArmorGuardianMode";
    private const string InfiltratorMarkerName = "ConditionInnovationArmorInfiltratorMode";

    public static CharacterSubclassDefinition Build()
    {
        return CharacterSubclassDefinitionBuilder
            .Create("InnovationArmor")
            .SetGuiPresentation(Category.Subclass, FightingStyleDefinitions.Defense)
            .AddFeaturesAtLevel(3, BuildArmoredUp(), BuildAutoPreparedSpells(), BuildArmorModes())
            .AddFeaturesAtLevel(5, BuildExtraAttack())
            .AddFeaturesAtLevel(9, BuildArmorModification())
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
            //TODO: find (or make) replacement for Cloud Kill - supposed to be Wall of Force
            .AddPreparedSpellGroup(17, SpellsContext.FarStep, CloudKill)
            .AddToDB();
    }

    private static FeatureDefinition BuildArmorModes()
    {
        var pool = FeatureDefinitionPowerBuilder
            .Create("PowerInnovationArmorModeSelectorPool")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetCustomSubFeatures(new CanUseAttributeForWeapon(AttributeDefinitions.Intelligence, IsBuiltInWeapon))
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
                    .SetGuiPresentationNoContent()
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
                new AddGauntletAttack()
            )
            .SetSharedPool(ActivationTime.BonusAction, pool)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(
                    EffectFormBuilder.Create()
                        .SetConditionForm(infiltratorMarker, ConditionForm.ConditionOperation.Remove, true, false)
                        .Build(),
                    EffectFormBuilder.Create()
                        .SetConditionForm(guardianMarker, ConditionForm.ConditionOperation.Add, true, false)
                        .Build()
                )
                .Build())
            .AddToDB();

        var infiltratorMode = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerInnovationArmorSwitchModeInfiltrator")
            .SetGuiPresentation(Category.Feature, Sprites.PowerInfiltratorMode)
            .SetCustomSubFeatures(
                new ValidatorsPowerUse(NotInfiltratorMode),
                ValidatorsPowerUse.NotInCombat,
                new AddLauncherAttack()
            )
            .SetSharedPool(ActivationTime.BonusAction, pool)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(
                    EffectFormBuilder.Create()
                        .SetConditionForm(guardianMarker, ConditionForm.ConditionOperation.Remove, true, false)
                        .Build(),
                    EffectFormBuilder.Create()
                        .SetConditionForm(infiltratorMarker, ConditionForm.ConditionOperation.Add, true, false)
                        .Build())
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

    private static FeatureDefinition BuildExtraAttack()
    {
        return FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierInnovationArmorExtraAttack")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.ForceIfBetter, AttributeDefinitions.AttacksNumber, 2)
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

    private static bool InGuardianMode(RulesetCharacter character)
    {
        return character.HasConditionOfType(GuardianMarkerName);
    }

    private static bool NotGuardianMode(RulesetCharacter character)
    {
        return !character.HasConditionOfType(GuardianMarkerName);
    }

    private static bool InInfiltratorMode(RulesetCharacter character)
    {
        return character.HasConditionOfType(InfiltratorMarkerName);
    }

    private static bool NotInfiltratorMode(RulesetCharacter character)
    {
        return !character.HasConditionOfType(InfiltratorMarkerName);
    }

    private static bool IsBuiltInWeapon(RulesetAttackMode mode, RulesetItem weapon, RulesetCharacter character)
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
        public AddGauntletAttack() : base(ActionDefinitions.ActionType.Main, false, InGuardianMode,
            ValidatorsCharacter.HasFreeHand)
        {
        }

        protected override List<RulesetAttackMode> GetAttackModes(RulesetCharacterHero hero)
        {
            var strikeDefinition = CustomWeaponsContext.ThunderGauntlet;

            var attackModifiers = hero.attackModifiers;

            var attackMode = hero.RefreshAttackMode(
                ActionType,
                strikeDefinition,
                strikeDefinition.WeaponDescription,
                ValidatorsCharacter.IsFreeOffhandForUnarmedTa(hero),
                true,
                EquipmentDefinitions.SlotTypeMainHand,
                attackModifiers,
                hero.FeaturesOrigin
            );

            AddArmorBonusesToBuiltinAttack(hero, attackMode);

            var modes = new List<RulesetAttackMode> { attackMode };

            if (!hero.HasEmptyMainHand())
            {
                return modes;
            }

            var reaction = RulesetAttackMode.AttackModesPool.Get();

            reaction.Copy(attackMode);
            reaction.ActionType = ActionDefinitions.ActionType.Reaction;
            modes.Add(reaction);

            return modes;
        }

        protected override AttackModeOrder GetOrder(RulesetCharacterHero hero)
        {
            return hero.HasEmptyMainHand() ? AttackModeOrder.Start : base.GetOrder(hero);
        }
    }


    private class AddLauncherAttack : AddExtraAttackBase
    {
        public AddLauncherAttack() : base(ActionDefinitions.ActionType.Main, false, InInfiltratorMode)
        {
        }

        protected override List<RulesetAttackMode> GetAttackModes(RulesetCharacterHero hero)
        {
            var strikeDefinition = CustomWeaponsContext.LightningLauncher;
            var attackModifiers = hero.attackModifiers;
            var attackMode = hero.RefreshAttackMode(
                ActionType,
                strikeDefinition,
                strikeDefinition.WeaponDescription,
                ValidatorsCharacter.IsFreeOffhandForUnarmedTa(hero),
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
