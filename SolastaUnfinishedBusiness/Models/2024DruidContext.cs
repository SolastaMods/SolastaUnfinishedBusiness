using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Feats;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Subclasses;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionProficiencys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSubclassChoices;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

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
            Sprites.GetSprite("PowerGainWildShape", Resources.PowerGainWildShape, 128, 64))
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
        .AddCustomSubFeatures(new CustomBehaviorPowerDruidWildResurgenceShape())
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerDruidWildResurgenceSlot = FeatureDefinitionPowerBuilder
        .Create("PowerDruidWildResurgenceSlot")
        .SetGuiPresentation(Category.Feature,
            Sprites.GetSprite("PowerGainSlot", Resources.PowerGainSlot, 128, 64))
        .SetUsesFixed(ActivationTime.NoCost, RechargeRate.TurnStart)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .Build())
        .AddCustomSubFeatures(new CustomBehaviorSpendWildShape(1))
        .AddToDB();

    private static readonly FeatureDefinitionFeatureSet FeatureSetDruidWildResurgence =
        FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetDruidWildResurgence")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(PowerDruidWildResurgenceShape, PowerDruidWildResurgenceSlot)
            .AddToDB();

    private static readonly FeatureDefinition FeatureDruidEvergreenWildShape = FeatureDefinitionBuilder
        .Create("FeatureDruidEvergreenWildShape")
        .SetGuiPresentation(Category.Feature)
        .AddCustomSubFeatures(new InitiativeEndListenerDruidEvergreenWildShape())
        .AddToDB();

    private static readonly FeatureDefinitionFeatureSet FeatureSetDruidArchDruid =
        FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetDruidArchDruid")
            .SetGuiPresentation(Category.Feature)
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
            .Create("MagicAffinityAdditionalSpellSlot8")
            .SetGuiPresentationNoContent(true)
            .SetAdditionalSlots(new AdditionalSlotsDuplet { slotLevel = 8, slotsNumber = 1 })
            .AddToDB();

        //
        // Gain Slots
        //

        var powers = new List<FeatureDefinitionPower>();
        var powerPool = FeatureDefinitionPowerBuilder
            .Create("PowerDruidNatureMagician")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerDruidNatureMagician", Resources.PowerDruidNatureMagician, 128, 64))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.UntilLongRest)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .Build())
            .AddToDB();

        for (var i = 8; i >= 2; i -= 2)
        {
            var uses = i / 2;
            var powerGainSlot = FeatureDefinitionPowerSharedPoolBuilder
                .Create($"PowerNatureMagicianGainSlot{i}")
                .SetGuiPresentation(
                    Gui.Format("Feature/&PowerNatureMagicianGainSlotTitle", uses.ToString(), i.ToString()),
                    Gui.Format("Feature/&PowerNatureMagicianGainSlotDescription", uses.ToString(), i.ToString()))
                .SetSharedPool(ActivationTime.NoCost, powerPool)
                .SetShowCasting(false)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetDurationData(DurationType.UntilLongRest)
                        .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                        .SetEffectForms(
                            EffectFormBuilder
                                .Create()
                                .SetConditionForm(
                                    ConditionDefinitionBuilder
                                        .Create($"ConditionNatureMagicianGain{i}Slot")
                                        .SetGuiPresentationNoContent(true)
                                        .SetSilent(Silent.WhenAddedOrRemoved)
                                        .SetFeatures(
                                            GetDefinition<FeatureDefinitionMagicAffinity>(
                                                $"MagicAffinityAdditionalSpellSlot{i}"))
                                        .AddToDB(), ConditionForm.ConditionOperation.Add)
                                .Build())
                        .Build())
                .AddCustomSubFeatures(
                    ModifyPowerVisibility.Hidden,
                    new CustomBehaviorSpendWildShape(i))
                .AddToDB();

            powers.Add(powerGainSlot);
        }

        PowerBundle.RegisterPowerBundle(powerPool, false, powers);
        FeatureSetDruidArchDruid.FeatureSet.SetRange(powerPool, FeatureDruidEvergreenWildShape);
        FeatureSetDruidArchDruid.FeatureSet.AddRange(powers);
    }

    private static void LoadDruidElementalFury()
    {
        var featurePotentSpellcasting = FeatureDefinitionBuilder
            .Create("FeatureDruidElementalFuryPotentSpellcasting")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featurePotentSpellcasting.AddCustomSubFeatures(
            new ClassFeats.CustomBehaviorFeatPotentSpellcaster(featurePotentSpellcasting, Druid));

        var damageTypes = new (string, IMagicEffect)[]
        {
            (DamageTypeCold, ConeOfCold), (DamageTypeFire, FireBolt), (DamageTypeLightning, LightningBolt),
            (DamageTypeThunder, Shatter)
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

        var actionAffinityToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityElementalFuryToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((Id)ExtraActionId.ElementalFuryToggle)
            .AddToDB();

        var featureSetPrimalStrike = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetDruidElementalFuryPrimalStrike")
            .SetGuiPresentation(powerPrimalStrike.GuiPresentation)
            .SetFeatureSet(powerPrimalStrike, actionAffinityToggle)
            .AddFeatureSet([.. powers])
            .AddToDB();

        PowerBundle.RegisterPowerBundle(powerPrimalStrike, false, powers);
        FeatureSetDruidElementalFury.FeatureSet.SetRange(featurePotentSpellcasting, featureSetPrimalStrike);
        FeatureSetDruidElementalFury.FeatureSet.AddRange(powers);
    }

    private static void LoadDruidWildshape()
    {
        CircleOfTheNight.PowerCircleOfTheNightWildShapeCombat.AddCustomSubFeatures(
            PowerOrSpellFinishedByMeDruidWildShape.Marker);
        PowerDruidWildShape.AddCustomSubFeatures(
            HasModifiedUses.Marker,
            PowerOrSpellFinishedByMeDruidWildShape.Marker,
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
        if (Main.Settings.EnableDruidWildshape2024)
        {
            PowerDruidWildShape.activationTime = ActivationTime.BonusAction;
            PowerDruidWildShape.rechargeRate = RechargeRate.LongRest;
            PowerDruidWildShape.GuiPresentation.description = "Feature/&PowerDruidWildShapeAlternateDescription";
        }
        else
        {
            PowerDruidWildShape.activationTime = ActivationTime.Action;
            PowerDruidWildShape.rechargeRate = RechargeRate.ShortRest;
            PowerDruidWildShape.GuiPresentation.description = "Feature/&PowerDruidWildShapeDescription";
        }
    }

    internal static void SwitchDruidCircleLearningLevel()
    {
        var circles = DatabaseRepository.GetDatabase<CharacterSubclassDefinition>()
            .Where(x => x.Name.StartsWith("Circle"))
            .ToList();

        var fromLevel = 3;
        var toLevel = 2;

        if (Main.Settings.EnableDruidToLearnCircleAtLevel3)
        {
            fromLevel = 2;
            toLevel = 3;
        }

        foreach (var featureUnlock in circles
                     .SelectMany(school => school.FeatureUnlocks
                         .Where(featureUnlock => featureUnlock.level == fromLevel)))
        {
            featureUnlock.level = toLevel;
        }

        // change spell casting level on Druid itself
        Druid.FeatureUnlocks
                .FirstOrDefault(x =>
                    x.FeatureDefinition == SubclassChoiceDruidCircle)!
                .level =
            toLevel;

        foreach (var circle in circles)
        {
            circle.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        }

        Druid.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    private sealed class PowerOrSpellFinishedByMeDruidWildShape : IPowerOrSpellFinishedByMe
    {
        internal static readonly PowerOrSpellFinishedByMeDruidWildShape Marker = new();

        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (!Main.Settings.EnableDruidWildshape2024)
            {
                yield break;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var classLevel = rulesetCharacter.GetClassLevel(Druid);
            var rulesetMonster = ServiceRepository.GetService<IGameLocationCharacterService>().PartyCharacters
                .FirstOrDefault(x => x.RulesetCharacter.OriginalFormCharacter == rulesetCharacter)?.RulesetCharacter;

            rulesetMonster?.ReceiveTemporaryHitPoints(
                classLevel, DurationType.UntilAnyRest, 1, TurnOccurenceType.EndOfTurn, rulesetCharacter.Guid);
        }
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
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!rulesetAttacker.IsToggleEnabled((Id)ExtraActionId.ElementalFuryToggle) ||
                !attacker.OnceInMyTurnIsValid(ElementalFury))
            {
                yield break;
            }

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

            if (!rulesetAttacker.IsToggleEnabled((Id)ExtraActionId.ElementalFuryToggle) ||
                attacker.GetSpecialFeatureUses(ElementalFury) != 1)
            {
                yield break;
            }

            rulesetAttacker.RemoveAllConditionsOfType(
                "ConditionDruidElementalFuryDamageCold",
                "ConditionDruidElementalFuryDamageLightning",
                "ConditionDruidElementalFuryDamageThunder");
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
            if (effectDescription.RangeType is RangeType.Distance or RangeType.RangeHit)
            {
                effectDescription.rangeParameter *= 2;
            }

            return effectDescription;
        }
    }

    private sealed class InitiativeEndListenerDruidEvergreenWildShape : IInitiativeEndListener
    {
        public IEnumerator OnInitiativeEnded(GameLocationCharacter character)
        {
            var rulesetCharacter = character.RulesetCharacter;
            var power = character.RulesetCharacter.GetSubclassLevel(Druid, CircleOfTheNight.Name) == 0
                ? PowerDruidWildShape
                : CircleOfTheNight.PowerCircleOfTheNightWildShapeCombat;

            if (!power)
            {
                yield break;
            }

            var usablePower = PowerProvider.Get(power, rulesetCharacter);

            usablePower.remainingUses++;
            rulesetCharacter.LogCharacterUsedFeature(FeatureDruidEvergreenWildShape);
        }
    }

    internal sealed class CustomBehaviorPowerDruidWildResurgenceShape : IPowerOrSpellFinishedByMe, IValidatePowerUse
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition power)
        {
            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var repertoire = rulesetCharacter.GetClassSpellRepertoire(Druid);
            var usablePower = PowerProvider.Get(PowerDruidWildShape, rulesetCharacter);

            if (repertoire == null)
            {
                yield break;
            }

            repertoire.SpendSpellSlot(repertoire.GetLowestAvailableSlotLevel());
            rulesetCharacter.UpdateUsageForPowerPool(-1, usablePower);
        }

        public bool CanUsePower(RulesetCharacter rulesetCharacter, FeatureDefinitionPower power)
        {
            var slotLevel = rulesetCharacter.GetClassSpellRepertoire(Druid)?.GetLowestAvailableSlotLevel();

            return slotLevel > 0 && rulesetCharacter.GetRemainingPowerUses(PowerDruidWildShape) == 0;
        }
    }

    internal sealed class CustomBehaviorSpendWildShape(int usage) : IPowerOrSpellFinishedByMe, IValidatePowerUse
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition power)
        {
            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var usablePower = PowerProvider.Get(PowerDruidWildShape, rulesetCharacter);

            rulesetCharacter.UpdateUsageForPowerPool(usage, usablePower);

            yield break;
        }

        public bool CanUsePower(RulesetCharacter rulesetCharacter, FeatureDefinitionPower power)
        {
            return rulesetCharacter.GetRemainingPowerUses(PowerDruidWildShape) >= usage;
        }
    }
}
