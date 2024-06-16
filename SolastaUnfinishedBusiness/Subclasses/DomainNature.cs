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
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class DomainNature : AbstractSubclass
{
    private const string Name = "DomainNature";

    private static readonly string[] DampenElementsDamageTypes =
    [
        DamageTypeAcid,
        DamageTypeCold,
        DamageTypeFire,
        DamageTypeLightning,
        DamageTypeThunder
    ];

    private static FeatureDefinitionCastSpell _castSpellDomainNature;
    private static CharacterSubclassDefinition _domainNature;

    public DomainNature()
    {
        var divinePowerPrefix = Gui.Localize("Feature/&ClericChannelDivinityTitle") + ": ";

        var autoPreparedSpellsDomainNature = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Domain")
            .SetPreparedSpellGroups(
                BuildSpellGroup(1, AnimalFriendship, Entangle),
                BuildSpellGroup(3, Barkskin, SpikeGrowth),
                BuildSpellGroup(5, ConjureAnimals, WindWall),
                BuildSpellGroup(7, DominateBeast, FreedomOfMovement),
                BuildSpellGroup(9, InsectPlague, CloudKill))
            .SetSpellcastingClass(CharacterClassDefinitions.Cleric)
            .AddToDB();

        // LEVEL 01 - Acolyte of Nature

        var spellListDomainNature = SpellListDefinitionBuilder
            .Create($"SpellList{Name}")
            .SetGuiPresentationNoContent(true)
            .FinalizeSpells()
            .AddToDB();

        //explicitly re-use druid spell list, so custom cantrips selected for druid will show here 
        spellListDomainNature.SpellsByLevel[0].Spells = SpellListDefinitions.SpellListDruid.SpellsByLevel[0].Spells;

        _castSpellDomainNature = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellElfHigh, $"CastSpell{Name}")
            .SetGuiPresentationNoContent(true)
            .SetSpellCastingAbility(AttributeDefinitions.Wisdom)
            .SetSpellList(spellListDomainNature)
            .AddToDB();

        var pointPoolCantrip = FeatureDefinitionPointPoolBuilder
            .Create($"PointPool{Name}Cantrip")
            .SetGuiPresentationNoContent(true)
            .SetSpellOrCantripPool(HeroDefinitions.PointsPoolType.Cantrip, 1, SpellListDefinitions.SpellListDruid,
                "DomainNature")
            .AddToDB();

        var pointPoolSkills = FeatureDefinitionPointPoolBuilder
            .Create($"PointPool{Name}Skills")
            .SetGuiPresentationNoContent(true)
            .SetPool(HeroDefinitions.PointsPoolType.Skill, 1)
            .OnlyUniqueChoices()
            .RestrictChoices(
                SkillDefinitions.AnimalHandling,
                SkillDefinitions.Nature,
                SkillDefinitions.Survival)
            .AddToDB();

        var proficiencyHeavyArmor = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}HeavyArmor")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Armor, EquipmentDefinitions.HeavyArmorCategory)
            .AddToDB();

        var featureSetBonusProficiency = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}BonusProficiency")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(proficiencyHeavyArmor)
            .AddToDB();

        // cannot add cast spell here as for whatever reason game tries to offer cleric cantrips
        // custom added later on GrantCantrip
        var featureSetAcolyteOfNature = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}AcolyteOfNature")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(pointPoolCantrip, pointPoolSkills)
            .AddToDB();

        //
        // Level 2 - Charm Animals and Plants
        //

        var powerCharmAnimalsAndPlants = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}CharmAnimalsAndPlants")
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
                    .SetCasterEffectParameters(PowerDruidCircleBalanceBalanceOfPower)
                    .Build())
            .AddToDB();

        var featureSetCharmAnimalsAndPlants = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}CharmAnimalsAndPlants")
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
            _ = ConditionDefinitionBuilder
                .Create($"Condition{Name}{damageType}")
                .SetGuiPresentationNoContent(true)
                .SetSilent(Silent.WhenAddedOrRemoved)
                .SetFeatures(db.GetElement($"DamageAffinity{damageType.Replace("Damage", string.Empty)}Resistance"))
                .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
                .AddToDB();
        }

        var conditionDampenElements = ConditionDefinitionBuilder
            .Create($"Condition{Name}DampenElements")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        conditionDampenElements.AddCustomSubFeatures(new CustomBehaviorDampenElements(conditionDampenElements));

        var powerDampenElements = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DampenElements")
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

        var additionalDamageDivineStrike = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}DivineStrike")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag("DivineStrike")
            .SetDamageDice(DieType.D8, 1)
            .SetSpecificDamageType(DamageTypeCold)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 8, 6)
            .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
            .SetAttackModeOnly()
            .SetImpactParticleReference(ConeOfCold)
            .AddToDB();

        // LEVEL 17 - Master of Nature

        var powerMasterOfNature = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}MasterOfNature")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("MasterOfNature", Resources.PowerCharmAnimalsAndPlants, 256, 128))
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
                        EffectFormBuilder.ConditionForm(
                            ConditionDefinitions.ConditionCharmed, ConditionForm.ConditionOperation.Remove),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .SetConditionForm(ConditionDefinitions.ConditionMindControlledByCaster,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetCasterEffectParameters(PowerDruidCircleBalanceBalanceOfPower)
                    .Build())
            .AddToDB();

        var featureSetMasterOfNature = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}MasterOfNature")
            .SetGuiPresentation(
                divinePowerPrefix + powerMasterOfNature.FormatTitle(),
                powerMasterOfNature.FormatDescription())
            .AddFeatureSet(powerMasterOfNature)
            .AddToDB();

        // MAIN

        _domainNature = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.TraditionGreenmage)
            .AddFeaturesAtLevel(1,
                autoPreparedSpellsDomainNature, featureSetAcolyteOfNature, featureSetBonusProficiency)
            .AddFeaturesAtLevel(2, featureSetCharmAnimalsAndPlants)
            .AddFeaturesAtLevel(6, powerDampenElements)
            .AddFeaturesAtLevel(8, additionalDamageDivineStrike)
            .AddFeaturesAtLevel(10, PowerClericDivineInterventionWizard)
            .AddFeaturesAtLevel(17, featureSetMasterOfNature)
            .AddToDB();

        Subclass = _domainNature;
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Cleric;

    internal override CharacterSubclassDefinition Subclass { get; }

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override FeatureDefinitionSubclassChoice SubclassChoice { get; }

    internal override DeityDefinition DeityDefinition => DeityDefinitions.Maraike;

    internal static void GrantCantrip(RulesetCharacterHero hero)
    {
        var heroBuildingData = hero.GetHeroBuildingData();

        if (!heroBuildingData.AcquiredCantrips.TryGetValue(
                $"06SubclassCleric1{Name}{Name}", out var cantrips))
        {
            return;
        }

        var selectedClass = LevelUpContext.GetSelectedClass(hero);
        var selectedSubclass = LevelUpContext.GetSelectedSubclass(hero);
        var subclassTag = AttributeDefinitions.GetSubclassTag(selectedClass, 1, selectedSubclass);

        hero.ActiveFeatures[subclassTag].Add(_castSpellDomainNature);
        hero.GrantSpellRepertoire(_castSpellDomainNature, null, _domainNature, null);

        foreach (var cantrip in cantrips)
        {
            hero.GrantCantrip(cantrip, _castSpellDomainNature);
        }
    }

    private sealed class CustomBehaviorDampenElements(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionDampenElements)
        : IMagicEffectBeforeHitConfirmedOnMe, ITryAlterOutcomeAttack
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
            if (rulesetEffect.EffectDescription.RangeType is not (RangeType.MeleeHit or RangeType.RangeHit))
            {
                yield return Handler(battleManager, attacker, defender, actualEffectForms);
            }
        }

        public int HandlerPriority => 10;

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            var battleManager =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (!battleManager)
            {
                yield break;
            }

            var actualEffectForms =
                attackMode?.EffectDescription.EffectForms ?? rulesetEffect?.EffectDescription.EffectForms ?? [];

            yield return Handler(battleManager, attacker, defender, actualEffectForms);
        }

        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        private IEnumerator Handler(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            // ReSharper disable once SuggestBaseTypeForParameter
            GameLocationCharacter defender,
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

            var rulesetDefender = defender.RulesetCharacter;

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

            EffectHelpers.StartVisualEffect(glc, defender, PowerPaladinNeutralizePoison,
                EffectHelpers.EffectType.Effect);
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
