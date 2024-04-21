using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using Category = SolastaUnfinishedBusiness.Builders.Category;
using CharacterSubclassDefinitionBuilder = SolastaUnfinishedBusiness.Builders.CharacterSubclassDefinitionBuilder;
using EffectDescriptionBuilder = SolastaUnfinishedBusiness.Builders.EffectDescriptionBuilder;
using EffectFormBuilder = SolastaUnfinishedBusiness.Builders.EffectFormBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class DomainNature : AbstractSubclass
{
    private static readonly string[] DampenElementsDamageTypes =
    [
        DamageTypeAcid,
        DamageTypeCold,
        DamageTypeFire,
        DamageTypeLightning,
        DamageTypeThunder
    ];

    public DomainNature()
    {
        const string NAME = "DomainNature";

        var autoPreparedSpellsDomainNature = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{NAME}")
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Domain")
            .SetPreparedSpellGroups(
                BuildSpellGroup(1, AnimalFriendship, Entangle),
                BuildSpellGroup(3, Barkskin, SpikeGrowth),
                BuildSpellGroup(5, ConjureAnimals, WindWall),
                BuildSpellGroup(7, DominateBeast, FreedomOfMovement),
                BuildSpellGroup(9, InsectPlague, Contagion))
            .SetSpellcastingClass(CharacterClassDefinitions.Cleric)
            .AddToDB();

        // LEVEL 01 - Acolyte of Nature

        var pointPoolCantrip = FeatureDefinitionPointPoolBuilder
            .Create($"PointPool{NAME}Cantrip")
            .SetGuiPresentationNoContent(true)
            .SetSpellOrCantripPool(HeroDefinitions.PointsPoolType.Cantrip, 1, SpellListDefinitions.SpellListDruid,
                "DomainNature")
            .AddToDB();

        var pointPoolSkills = FeatureDefinitionPointPoolBuilder
            .Create($"PointPool{NAME}Skills")
            .SetGuiPresentationNoContent(true)
            .SetPool(HeroDefinitions.PointsPoolType.Skill, 1)
            .OnlyUniqueChoices()
            .RestrictChoices(
                SkillDefinitions.AnimalHandling,
                SkillDefinitions.Nature,
                SkillDefinitions.Survival)
            .AddToDB();

        var proficiencyHeavyArmor = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{NAME}HeavyArmor")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Armor, EquipmentDefinitions.HeavyArmorCategory)
            .AddToDB();

        var featureSetAcolyteOfNature = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}AcolyteOfNature")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(pointPoolCantrip, pointPoolSkills, proficiencyHeavyArmor)
            .AddToDB();

        //
        // Level 2 - Charm Animals and Plants
        //

        var divinePowerPrefix = Gui.Localize("Feature/&ClericChannelDivinityTitle") + ": ";

        var powerCharmAnimalsAndPlants = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}CharmAnimalsAndPlants")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("CharmAnimalsAndPlants", Resources.PowerCharmAnimalsAndPlants, 256, 128))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetRestrictedCreatureFamilies("Beast", "Plant")
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Wisdom,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(ConditionDefinitions.ConditionCharmed,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        var featureSetCharmAnimalsAndPlants = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}CharmAnimalsAndPlants")
            .SetGuiPresentation(
                divinePowerPrefix + powerCharmAnimalsAndPlants.FormatTitle(),
                powerCharmAnimalsAndPlants.FormatDescription())
            .AddFeatureSet(powerCharmAnimalsAndPlants)
            .AddToDB();

        //
        // LEVEL 6 - Dampen Elements
        //

        var db = DatabaseRepository.GetDatabase<FeatureDefinitionDamageAffinity>();

        foreach (var damageType in DampenElementsDamageTypes)
        {
            var shortDamageType = damageType.Replace("Damage", string.Empty);

            var conditionResistance = ConditionDefinitionBuilder
                .Create($"Condition{NAME}{damageType}")
                .SetGuiPresentationNoContent(true)
                .SetSilent(Silent.WhenAddedOrRemoved)
                .SetFeatures(db.GetElement($"DamageAffinity{shortDamageType}Resistance"))
                .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
                .AddToDB();

            conditionResistance.GuiPresentation.description = Gui.NoLocalization;
        }

        var conditionDampenElements = ConditionDefinitionBuilder
            .Create($"Condition{NAME}DampenElements")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        conditionDampenElements.AddCustomSubFeatures(new CustomBehaviorDampenElements(conditionDampenElements));

        var powerDampenElements = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}DampenElements")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Permanent)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Permanent)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionDampenElements))
                    .SetCasterEffectParameters(PowerDruidCircleBalanceBalanceOfPower)
                    .SetEffectEffectParameters(PowerPaladinNeutralizePoison)
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        //
        // LEVEL 08 - Divine Strike
        //

        var additionalDamageDivineStrikeCold = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}DivineStrikeCold")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("DivineStrike")
            .SetDamageDice(DieType.D8, 1)
            .SetSpecificDamageType(DamageTypeCold)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 8, 6)
            .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
            .SetAttackModeOnly()
            .AddToDB();

        var conditionDivineStrikeCold = ConditionDefinitionBuilder
            .Create($"Condition{NAME}DivineStrikeCold")
            .SetGuiPresentationNoContent(true)
            .SetFeatures(additionalDamageDivineStrikeCold)
            .SetSpecialInterruptions(ConditionInterruption.AttacksAndDamages)
            .AddToDB();

        var additionalDamageDivineStrikeFire = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}DivineStrikeFire")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("DivineStrike")
            .SetDamageDice(DieType.D8, 1)
            .SetSpecificDamageType(DamageTypeFire)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 8, 6)
            .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
            .SetAttackModeOnly()
            .AddToDB();

        var conditionDivineStrikeFire = ConditionDefinitionBuilder
            .Create($"Condition{NAME}DivineStrikeFire")
            .SetGuiPresentationNoContent(true)
            .SetFeatures(additionalDamageDivineStrikeFire)
            .SetSpecialInterruptions(ConditionInterruption.AttacksAndDamages)
            .AddToDB();

        var additionalDamageDivineStrikeLightning = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}DivineStrikeLightning")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("DivineStrike")
            .SetDamageDice(DieType.D8, 1)
            .SetSpecificDamageType(DamageTypeLightning)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 8, 6)
            .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
            .SetAttackModeOnly()
            .AddToDB();

        var conditionDivineStrikeLightning = ConditionDefinitionBuilder
            .Create($"Condition{NAME}DivineStrikeLightning")
            .SetGuiPresentationNoContent(true)
            .SetFeatures(additionalDamageDivineStrikeLightning)
            .SetSpecialInterruptions(ConditionInterruption.AttacksAndDamages)
            .AddToDB();

        var featureDivineStrike = FeatureDefinitionBuilder
            .Create($"Feature{NAME}DivineStrike")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureDivineStrike.AddCustomSubFeatures(
            new PhysicalAttackBeforeHitConfirmedOnEnemyDivineStrike(
                featureDivineStrike,
                conditionDivineStrikeCold, conditionDivineStrikeFire, conditionDivineStrikeLightning));

        // LEVEL 17 - Master of Nature

        var powerMasterOfNature = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}MasterOfNature")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("MasterOfNature", Resources.InventorQuickWit, 256, 128))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetRestrictedCreatureFamilies("Beast", "Plants")
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Wisdom,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(ConditionDefinitions.ConditionCharmed,
                                ConditionForm.ConditionOperation.Add)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(ConditionDefinitions.ConditionMindDominatedByCaster,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.TraditionGreenmage)
            .AddFeaturesAtLevel(1, autoPreparedSpellsDomainNature, featureSetAcolyteOfNature)
            .AddFeaturesAtLevel(2, featureSetCharmAnimalsAndPlants)
            .AddFeaturesAtLevel(6, powerDampenElements)
            .AddFeaturesAtLevel(8, featureDivineStrike)
            .AddFeaturesAtLevel(10, PowerClericDivineInterventionWizard)
            .AddFeaturesAtLevel(17, powerMasterOfNature)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Cleric;

    internal override CharacterSubclassDefinition Subclass { get; }

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override FeatureDefinitionSubclassChoice SubclassChoice { get; }

    internal override DeityDefinition DeityDefinition => DeityDefinitions.Maraike;

    private sealed class PhysicalAttackBeforeHitConfirmedOnEnemyDivineStrike(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureDivineStrike,
        params ConditionDefinition[] conditions) : IPhysicalAttackBeforeHitConfirmedOnEnemy
    {
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
            if (!attacker.OncePerTurnIsValid(featureDivineStrike.Name))
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var roll = RollDie(DieType.D3, AdvantageType.None, out _, out _);

            rulesetAttacker.ShowDieRoll(DieType.D3, roll, title: featureDivineStrike.GuiPresentation.Title);

            var condition = conditions[roll - 1];

            attacker.UsedSpecialFeatures.TryAdd(featureDivineStrike.Name, 0);
            rulesetAttacker.InflictCondition(
                condition.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                condition.Name,
                0,
                0,
                0);
        }
    }

    private sealed class CustomBehaviorDampenElements(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionDampenElements)
        : IMagicEffectBeforeHitConfirmedOnMe, IPhysicalAttackBeforeHitConfirmedOnMe
    {
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnMe(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            yield return Handler(battleManager, attacker, defender.RulesetCharacter, actualEffectForms);
        }

        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnMe(
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
            yield return Handler(battleManager, attacker, defender.RulesetCharacter, actualEffectForms);
        }

        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        private IEnumerator Handler(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            // ReSharper disable once SuggestBaseTypeForParameter
            RulesetCharacter rulesetDefender,
            List<EffectForm> actualEffectForms)
        {
            var actionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (!actionManager)
            {
                yield break;
            }

            var damageTypes = DampenElementsDamageTypes.Intersect(
                    actualEffectForms
                        .Where(x => x.FormType == EffectForm.EffectFormType.Damage)
                        .Select(x => x.DamageForm.DamageType))
                .ToList();

            if (damageTypes.Count == 0)
            {
                yield break;
            }

            if (!rulesetDefender.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionDampenElements.Name, out var activeCondition))
            {
                yield break;
            }

            var rulesetCaster = EffectHelpers.GetCharacterByGuid(activeCondition.SourceGuid);

            if (rulesetCaster == null)
            {
                yield break;
            }

            var glc = GameLocationCharacter.GetFromActor(rulesetCaster);

            if (!glc.CanReact())
            {
                yield break;
            }

            var actionParams = new CharacterActionParams(glc, (ActionDefinitions.Id)ExtraActionId.DoNothingReaction)
            {
                StringParameter = "CustomReactionDampenElementsDescription"
                    .Formatted(Category.Reaction, rulesetDefender.Name)
            };
            var reactionRequest = new ReactionRequestCustom("DampenElements", actionParams);
            var count = actionManager.PendingReactionRequestGroups.Count;

            actionManager.AddInterruptRequest(reactionRequest);

            yield return battleManager.WaitForReactions(attacker, actionManager, count);

            if (!actionParams.ReactionValidated)
            {
                yield break;
            }

            foreach (var conditionName in damageTypes.Select(damageType => $"ConditionDomainNature{damageType}"))
            {
                rulesetDefender.InflictCondition(
                    conditionName,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetDefender.guid,
                    rulesetDefender.CurrentFaction.Name,
                    1,
                    conditionName,
                    0,
                    0,
                    0);
            }
        }
    }
}
