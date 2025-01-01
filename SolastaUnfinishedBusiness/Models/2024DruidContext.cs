using System;
using System.Collections;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Feats;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionProficiencys;
using static MetricsDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static partial class Tabletop2024Context
{
    private const string ElementalFury = "ElementaFury";

    private static readonly FeatureDefinitionFeatureSet FeatureSetDruidPrimalOrder = FeatureDefinitionFeatureSetBuilder
        .Create("FeatureSetDruidPrimalOrder")
        .SetGuiPresentation(Category.Feature)
        .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
        .SetFeatureSet(
            FeatureDefinitionFeatureSetBuilder
                .Create("FeatureSetDruidPrimalOrderMagician")
                .SetGuiPresentation(Category.Feature)
                .SetFeatureSet(
                    FeatureDefinitionPointPoolBuilder
                        .Create("PointPoolDruidPrimalOrderMagician")
                        .SetGuiPresentationNoContent(true)
                        .SetSpellOrCantripPool(HeroDefinitions.PointsPoolType.Cantrip, 1, extraSpellsTag: "PrimalOrder")
                        .AddCustomSubFeatures(new ModifyAbilityCheckDruidPrimalOrder())
                        .AddToDB())
                .AddToDB(),
            FeatureDefinitionFeatureSetBuilder
                .Create("FeatureSetDruidPrimalOrderWarden")
                .SetGuiPresentation(Category.Feature)
                .SetFeatureSet(
                    FeatureDefinitionProficiencyBuilder
                        .Create("ProficiencyDruidPrimalOrderWardenArmor")
                        .SetGuiPresentationNoContent(true)
                        .SetProficiencies(ProficiencyType.Armor, EquipmentDefinitions.MediumArmorCategory)
                        .AddToDB(),
                    FeatureDefinitionProficiencyBuilder
                        .Create("ProficiencyDruidPrimalOrderWardenWeapon")
                        .SetGuiPresentationNoContent(true)
                        .SetProficiencies(ProficiencyType.Weapon, EquipmentDefinitions.MartialWeaponCategory)
                        .AddToDB())
                .AddToDB())
        .AddToDB();

    private static readonly List<string> DruidWeaponsCategories =
        [.. ProficiencyDruidWeapon.Proficiencies, "ConjuredWeaponType"];

    private static readonly FeatureDefinitionPower FeatureDefinitionPowerNatureShroud = FeatureDefinitionPowerBuilder
        .Create("PowerRangerNatureShroud")
        .SetGuiPresentation(Category.Feature, Invisibility)
        .SetUsesAbilityBonus(ActivationTime.BonusAction, RechargeRate.LongRest, AttributeDefinitions.Wisdom)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                .SetEffectForms(EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionInvisible))
                .SetParticleEffectParameters(PowerDruidCircleBalanceBalanceOfPower)
                .Build())
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerDruidWildResurgenceShape = FeatureDefinitionPowerBuilder
        .Create("PowerDruidWildResurgenceShape")
        .SetGuiPresentation(Category.Feature,
            Sprites.GetSprite("PowerGainSlot", Resources.PowerGainSlot, 128, 64))
        .SetUsesFixed(ActivationTime.NoCost, RechargeRate.LongRest)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.UntilLongRest)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetEffectForms(EffectFormBuilder.ConditionForm(
                    ConditionDefinitionBuilder
                        .Create("ConditionDruidWildResurgenceShape")
                        .SetGuiPresentationNoContent(true)
                        .SetSilent(Silent.WhenAddedOrRemoved)
                        .SetFeatures(FeatureDefinitionMagicAffinitys.MagicAffinityAdditionalSpellSlot1)
                        .AddToDB()))
                .Build())
        .AddCustomSubFeatures(new ClassFeats.SpendWildShapeUse())
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerDruidWildResurgenceSlot = FeatureDefinitionPowerBuilder
        .Create("PowerDruidWildResurgenceSlot")
        .SetGuiPresentation(Category.Feature,
            Sprites.GetSprite("PowerGainWildShape", Resources.PowerGainWildShape, 128, 64))
        .SetUsesFixed(ActivationTime.NoCost, RechargeRate.TurnStart)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .Build())
        .AddCustomSubFeatures(new ClassFeats.GainWildShapeCharges(1, 1))
        .AddToDB();

    private static readonly FeatureDefinitionFeatureSet FeatureSetDruidWildResurgence =
        FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetDruidWildResurgence")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(PowerDruidWildResurgenceShape, PowerDruidWildResurgenceSlot)
            .AddToDB();

    private static readonly FeatureDefinitionPower PowerDruidNatureMagician = FeatureDefinitionPowerBuilder
        .Create("PowerDruidNatureMagician")
        .SetGuiPresentation(Category.Feature,
            Sprites.GetSprite("PowerGainSlot", Resources.PowerGainSlot, 128, 64))
        .SetUsesFixed(ActivationTime.NoCost, RechargeRate.LongRest)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.UntilLongRest)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .Build())
        .AddCustomSubFeatures(new ClassFeats.SpendWildShapeUse())
        .AddToDB();

    private static readonly FeatureDefinitionFeatureSet FeatureSetDruidArchDruid =
        FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetDruidArchDruid")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(PowerDruidNatureMagician)
            .AddToDB();

    private static readonly FeatureDefinitionFeatureSet FeatureSetDruidElementalFury =
        FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetDruidElementalFury")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
            .AddToDB();

    private static readonly FeatureDefinition FeatureDruidImprovedElementalFury =
        FeatureDefinitionBuilder
            .Create("FeatureDruidImprovedElementalFury")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new CustomBehaviorPotentSpellcasting())
            .AddToDB();

    private static void LoadDruidArchDruid()
    {
        _ = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityAdditionalSpellSlot6")
            .SetGuiPresentationNoContent(true)
            .SetAdditionalSlots(new AdditionalSlotsDuplet { slotLevel = 6, slotsNumber = 1 })
            .AddToDB();

        _ = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityAdditionalSpellSlot6")
            .SetGuiPresentationNoContent(true)
            .SetAdditionalSlots(new AdditionalSlotsDuplet { slotLevel = 8, slotsNumber = 1 })
            .AddToDB();
    }

    private static void LoadDruidElementalFury()
    {
        var featurePotentSpellcasting = FeatureDefinitionBuilder
            .Create("FeatureDruidElementalFuryPotentSpellcasting")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var damageTypes = new (string, IMagicEffect)[]
        {
            (DamageTypeCold, ConeOfCold), (DamageTypeLightning, LightningBolt), (DamageTypeThunder, Shatter)
        };

        var powers = new List<FeatureDefinitionPower>();
        var powerPrimalStrike = FeatureDefinitionPowerBuilder
            .Create("PowerDruidElementalFury")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .Build())
            .AddToDB();

        powerPrimalStrike.AddCustomSubFeatures(new CustomBehaviorElementalFury(powerPrimalStrike));

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var (damageType, effect) in damageTypes)
        {
            var additionalDamageElementalFury = FeatureDefinitionAdditionalDamageBuilder
                .Create($"AdditionalDamageDruidElementalFury{damageType}")
                .SetGuiPresentationNoContent(true)
                .SetNotificationTag(ElementalFury)
                .SetDamageDice(DieType.D8, 1)
                .SetSpecificDamageType(damageType)
                .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 8, 7)
                .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
                .SetAttackModeOnly()
                .AddCustomSubFeatures(ClassHolder.Druid)
                .SetImpactParticleReference(effect)
                .AddToDB();

            var conditionElementalFury = ConditionDefinitionBuilder
                .Create($"ConditionDruidElementalFury{damageType}")
                .SetGuiPresentationNoContent(true)
                .SetSilent(Silent.WhenAddedOrRemoved)
                .SetFeatures(additionalDamageElementalFury)
                .AddToDB();

            var damageTitle = Gui.Localize($"Tooltip/&Tag{damageType}Title");

            var powerElementalFury = FeatureDefinitionPowerSharedPoolBuilder
                .Create($"PowerDruidElementalFury{damageType}")
                .SetGuiPresentation(
                    Gui.Format("Feature/&PowerDruidElementalFurySubPowerTitle", damageTitle),
                    Gui.Format("Feature/&PowerDruidElementalFurySubPowerTitle", damageTitle))
                .SetShowCasting(false)
                .SetSharedPool(ActivationTime.NoCost, powerPrimalStrike)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetDurationData(DurationType.Round)
                        .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                        .SetEffectForms(EffectFormBuilder.ConditionForm(conditionElementalFury))
                        .Build())
                .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
                .AddToDB();

            powers.Add(powerElementalFury);
        }

        PowerBundle.RegisterPowerBundle(powerPrimalStrike, false, powers);
        FeatureSetDruidElementalFury.FeatureSet.SetRange(featurePotentSpellcasting, powerPrimalStrike);
        FeatureSetDruidElementalFury.FeatureSet.AddRange(powers);
    }

    private static void LoadDruidWildshape()
    {
        PowerDruidWildShape.AddCustomSubFeatures(
            HasModifiedUses.Marker,
            new ModifyPowerPoolAmount
            {
                PowerPool = PowerDruidWildShape,
                Type = PowerPoolBonusCalculationType.Wildshape2024,
                Attribute = DruidClass
            });
    }

    internal static void SwitchDruidArchDruid()
    {
        Druid.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == FeatureSetDruidArchDruid ||
            x.FeatureDefinition == Level20Context.MagicAffinityArchDruid);

        Druid.FeatureUnlocks.Add(Main.Settings.EnableDruidArchDruid2024
            ? new FeatureUnlockByLevel(FeatureSetDruidArchDruid, 20)
            : new FeatureUnlockByLevel(Level20Context.MagicAffinityArchDruid, 20));

        Druid.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchDruidElementalFury()
    {
        Druid.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == FeatureSetDruidElementalFury ||
            x.FeatureDefinition == FeatureDruidImprovedElementalFury);

        if (Main.Settings.EnableDruidElementalFury2024)
        {
            Druid.FeatureUnlocks.AddRange(
                new FeatureUnlockByLevel(FeatureSetDruidElementalFury, 7),
                new FeatureUnlockByLevel(FeatureDruidImprovedElementalFury, 15));
        }

        Druid.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchDruidMetalArmor()
    {
        var active = Main.Settings.EnableDruidMetalArmor2024;

        if (active)
        {
            ProficiencyDruidArmor.ForbiddenItemTags.Clear();
        }
        else
        {
            if (!ProficiencyDruidArmor.ForbiddenItemTags.Contains(
                    TagsDefinitions.ItemTagMetal))
            {
                ProficiencyDruidArmor.ForbiddenItemTags.Add(
                    TagsDefinitions.ItemTagMetal);
            }
        }
    }

    internal static void SwitchDruidPrimalOrder()
    {
        Druid.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == FeatureSetDruidPrimalOrder);
        ProficiencyDruidArmor.Proficiencies.Remove(EquipmentDefinitions.MediumArmorCategory);

        if (Main.Settings.EnableDruidPrimalOrder2024)
        {
            Druid.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetDruidPrimalOrder, 1));
        }
        else
        {
            ProficiencyDruidArmor.Proficiencies.Add(EquipmentDefinitions.MediumArmorCategory);
        }

        Druid.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchDruidWeaponProficiency()
    {
        ProficiencyDruidWeapon.proficiencies =
            Main.Settings.EnableDruidWeaponProficiency2024
                ? [WeaponCategoryDefinitions.SimpleWeaponCategory.Name]
                : DruidWeaponsCategories;
    }

    internal static void SwitchDruidWildResurgence()
    {
        Druid.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == FeatureSetDruidWildResurgence);

        if (Main.Settings.EnableDruidWildResurgence2024)
        {
            Druid.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetDruidWildResurgence, 5));
        }

        Druid.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchDruidWildshape()
    {
        PowerFighterSecondWind.rechargeRate = Main.Settings.EnableDruidWildshape2024
            ? RechargeRate.LongRest
            : RechargeRate.ShortRest;
    }

    private sealed class ModifyAbilityCheckDruidPrimalOrder : IModifyAbilityCheck
    {
        public void MinRoll(
            RulesetCharacter character,
            int baseBonus,
            string abilityScoreName,
            string proficiencyName,
            List<TrendInfo> advantageTrends,
            List<TrendInfo> modifierTrends,
            ref int rollModifier,
            ref int minRoll)
        {
            if (abilityScoreName is not AttributeDefinitions.Intelligence ||
                proficiencyName is not (SkillDefinitions.Arcana or SkillDefinitions.Nature))
            {
                return;
            }

            var wisdom = character.TryGetAttributeValue(AttributeDefinitions.Wisdom);
            var wisMod = AttributeDefinitions.ComputeAbilityScoreModifier(wisdom);
            var modifier = Math.Max(wisMod, 1);

            rollModifier += modifier;

            modifierTrends.Add(new TrendInfo(modifier, FeatureSourceType.CharacterFeature,
                "FeatureSetDruidPrimalOrderMagician", null));
        }
    }

    private sealed class CustomBehaviorElementalFury(FeatureDefinitionPower powerElementalFury)
        : IPhysicalAttackBeforeHitConfirmedOnEnemy, IPhysicalAttackFinishedByMe,
            IMagicEffectBeforeHitConfirmedOnEnemy, IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget, bool criticalHit)
        {
            yield return HandleReaction(attacker, battleManager);
        }

        public IEnumerator OnMagicEffectFinishedByMe(CharacterAction action, GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            yield return HandleOutcome(attacker);
        }

        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            yield return HandleReaction(attacker, battleManager);
        }

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            yield return HandleOutcome(attacker);
        }

        private IEnumerator HandleReaction(GameLocationCharacter attacker, GameLocationBattleManager battleManager)
        {
            if (!attacker.OnceInMyTurnIsValid(ElementalFury))
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerElementalFury, rulesetAttacker);

            yield return attacker.MyReactToSpendPowerBundle(
                usablePower,
                [attacker],
                attacker,
                powerElementalFury.Name,
                reactionValidated: ReactionValidated,
                battleManager: battleManager);

            yield break;

            void ReactionValidated(ReactionRequestSpendBundlePower reactionRequest)
            {
                attacker.SetSpecialFeatureUses(ElementalFury, 1);
            }
        }

        private static IEnumerator HandleOutcome(GameLocationCharacter attacker)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (attacker.GetSpecialFeatureUses(ElementalFury) == 1)
            {
                rulesetAttacker.RemoveAllConditionsOfType(
                    "ConditionDruidElementalFuryDamageCold",
                    "ConditionDruidElementalFuryDamageLightning",
                    "ConditionDruidElementalFuryDamageThunder");
            }

            yield break;
        }
    }

    private sealed class CustomBehaviorPotentSpellcasting : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition is SpellDefinition { SpellLevel: 0 };
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var shapeType = effectDescription.TargetType switch
            {
                TargetType.Line => GeometricShapeType.Line,
                TargetType.Cone => GeometricShapeType.Cone,
                TargetType.Cube
                    or TargetType.CubeWithOffset => GeometricShapeType.Cube,
                TargetType.Cylinder
                    or TargetType.CylinderWithDiameter => GeometricShapeType.Cylinder,
                TargetType.Sphere
                    or TargetType.PerceivingWithinDistance
                    or TargetType.InLineOfSightWithinDistance
                    or TargetType.ClosestWithinDistance => GeometricShapeType.Sphere,
                TargetType.WallLine => GeometricShapeType.WallLine,
                TargetType.WallRing => GeometricShapeType.WallRing,
                _ => GeometricShapeType.None
            };

            switch (shapeType)
            {
                case GeometricShapeType.None:
                    effectDescription.rangeParameter *= 2;
                    break;
                case GeometricShapeType.Cube:
                    effectDescription.targetParameter += 2;
                    break;
                case GeometricShapeType.Cone or GeometricShapeType.Cylinder or GeometricShapeType.Sphere:
                    effectDescription.targetParameter += 1;
                    break;
            }

            return effectDescription;
        }
    }
}
