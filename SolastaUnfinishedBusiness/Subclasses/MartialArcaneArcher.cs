using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSubclassChoices;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class MartialArcaneArcher : AbstractSubclass
{
    private const string Name = "MartialArcaneArcher";
    private const string ArcaneShotMarker = "ArcaneShot";
    private const ActionDefinitions.Id ArcaneArcherToggle = (ActionDefinitions.Id)ExtraActionId.ArcaneArcherToggle;

    private static readonly Dictionary<FeatureDefinitionPower, ArcaneArcherData> ArcaneShotPowers = new();
    private static FeatureDefinitionPowerSharedPool _powerBurstingArrow;

    internal static FeatureDefinitionPower PowerArcaneShot;
    internal static FeatureDefinitionPowerUseModifier ModifyPowerArcaneShotAdditionalUse1;
    internal static FeatureDefinitionActionAffinity ActionAffinityArcaneArcherToggle;
    internal static FeatureDefinitionCustomInvocationPool InvocationPoolArcaneShotChoice2;

    public MartialArcaneArcher()
    {
        // LEVEL 03

        // Arcane Lore

        var proficiencyArcana = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}Arcana")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Skill, SkillDefinitions.Arcana)
            .AddToDB();

        var proficiencyNature = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}Nature")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Skill, SkillDefinitions.Nature)
            .AddToDB();

        var featureSetArcaneLore = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}ArcaneLore")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(proficiencyArcana, proficiencyNature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
            .AddToDB();

        // Arcane Magic

        var spellListArcaneMagic = SpellListDefinitionBuilder
            .Create($"SpellList{Name}ArcaneMagic")
            .SetGuiPresentationNoContent(true)
            .FinalizeSpells()
            .AddToDB();

        //explicitly re-use wizard spell list, so custom cantrips selected for wizard will show here 
        spellListArcaneMagic.SpellsByLevel[0].Spells = SpellListDefinitions.SpellListWizard.SpellsByLevel[0].Spells;

        var castSpellArcaneMagic = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellElfHigh, $"CastSpell{Name}ArcaneMagic")
            .SetGuiPresentation(Category.Feature)
            .SetSpellCastingAbility(AttributeDefinitions.Intelligence)
            .SetSpellList(spellListArcaneMagic)
            .AddToDB();

        // Arcane Shot

        PowerArcaneShot = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ArcaneShot")
            .SetGuiPresentation($"FeatureSet{Name}ArcaneShot", Category.Feature)
            .SetUsesFixed(ActivationTime.OnAttackHitWithBow, RechargeRate.ShortRest, 1, 0)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.IndividualsUnique)
                    .Build())
            .AddCustomSubFeatures(
                IsModifyPowerPool.Marker,
                HasModifiedUses.Marker,
                ReactionResourceArcaneShot.Instance,
                new SpendPowerFinishedByMeArcaneShot(),
                new RestrictReactionAttackMode((_, attacker, _, _, _) =>
                    attacker.OnceInMyTurnIsValid(ArcaneShotMarker) &&
                    attacker.RulesetCharacter.IsToggleEnabled(ArcaneArcherToggle)))
            .AddToDB();

        ActionAffinityArcaneArcherToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityArcaneArcherToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions(ArcaneArcherToggle)
            .AddCustomSubFeatures(
                new ValidateDefinitionApplication(ValidatorsCharacter.HasAvailablePowerUsage(PowerArcaneShot)))
            .AddToDB();

        BuildArcaneShotPowers(PowerArcaneShot);
        CreateArcaneArcherChoices(ArcaneShotPowers.Keys);
        PowerBundle.RegisterPowerBundle(PowerArcaneShot, true, ArcaneShotPowers.Keys);

        ModifyPowerArcaneShotAdditionalUse1 = FeatureDefinitionPowerUseModifierBuilder
            .Create($"PowerUseModifier{Name}ArcaneShotUse1")
            .SetGuiPresentation(Category.Feature)
            .SetFixedValue(PowerArcaneShot, 1)
            .AddToDB();

        var powerArcaneShotAdditionalUse2 = FeatureDefinitionPowerUseModifierBuilder
            .Create($"PowerUseModifier{Name}ArcaneShotUse2")
            .SetGuiPresentationNoContent(true)
            .SetFixedValue(PowerArcaneShot, 2)
            .AddToDB();

        var invocationPoolArcaneShotChoice1 =
            CustomInvocationPoolDefinitionBuilder
                .Create("InvocationPoolArcaneShotChoice1")
                .SetGuiPresentation(Category.Feature)
                .Setup(InvocationPoolTypeCustom.Pools.ArcaneShotChoice)
                .AddToDB();

        InvocationPoolArcaneShotChoice2 =
            CustomInvocationPoolDefinitionBuilder
                .Create("InvocationPoolArcaneShotChoice2")
                .SetGuiPresentation(Category.Feature)
                .Setup(InvocationPoolTypeCustom.Pools.ArcaneShotChoice, 2)
                .AddToDB();

        var featureSetArcaneShot = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}ArcaneShot")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                ActionAffinityArcaneArcherToggle,
                InvocationPoolArcaneShotChoice2,
                powerArcaneShotAdditionalUse2,
                PowerArcaneShot)
            .AddToDB();

        // LEVEL 07

        // Magic Arrow

        var featureMagicArrow = FeatureDefinitionBuilder
            .Create($"Feature{Name}MagicArrow")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new AddTagToWeaponWeaponAttack(TagsDefinitions.MagicalWeapon, IsBow))
            .AddToDB();

        // Guided Shot

        var featureGuidedShot = FeatureDefinitionBuilder
            .Create($"Feature{Name}GuidedShot")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureGuidedShot.AddCustomSubFeatures(new TryAlterOutcomePhysicalAttackGuidedShot(featureGuidedShot));

        // LEVEL 10

        // Arcane Shot Additional Use

        var powerArcaneShotAdditionalUse1At10 = FeatureDefinitionPowerUseModifierBuilder
            .Create($"PowerUseModifier{Name}ArcaneShotUse1At10")
            .SetGuiPresentation($"PowerUseModifier{Name}ArcaneShotUse1", Category.Feature)
            .SetFixedValue(PowerArcaneShot, 1)
            .AddToDB();

        // Arcane Shot Choice

        // LEVEL 15

        // Ever-Ready Shot

        var featureEverReadyShot = FeatureDefinitionBuilder
            .Create($"Feature{Name}EverReadyShot")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureEverReadyShot.AddCustomSubFeatures(new BattleStartedListenerEverReadyShot(featureEverReadyShot));

        // LEVEL 18

        // Arcane Shot Additional Use

        var powerArcaneShotAdditionalUse1At18 = FeatureDefinitionPowerUseModifierBuilder
            .Create($"PowerUseModifier{Name}ArcaneShotUse1At18")
            .SetGuiPresentation($"PowerUseModifier{Name}ArcaneShotUse1", Category.Feature)
            .SetFixedValue(PowerArcaneShot, 1)
            .AddToDB();

        // Arcane Shot Choice

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.RangerSwiftBlade)
            .AddFeaturesAtLevel(3,
                featureSetArcaneLore,
                castSpellArcaneMagic,
                featureSetArcaneShot)
            .AddFeaturesAtLevel(7,
                featureMagicArrow,
                featureGuidedShot,
                invocationPoolArcaneShotChoice1)
            .AddFeaturesAtLevel(10,
                powerArcaneShotAdditionalUse1At10,
                invocationPoolArcaneShotChoice1)
            .AddFeaturesAtLevel(15,
                featureEverReadyShot,
                invocationPoolArcaneShotChoice1)
            .AddFeaturesAtLevel(18,
                powerArcaneShotAdditionalUse1At18,
                invocationPoolArcaneShotChoice1)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Fighter;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => SubclassChoiceFighterMartialArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static IsWeaponValidHandler IsBow =>
        ValidatorsWeapon.IsOfWeaponType(WeaponTypeDefinitions.LongbowType, WeaponTypeDefinitions.ShortbowType);

    private static void BuildArcaneShotPowers(FeatureDefinitionPower pool)
    {
        // Banishing Arrow

        var powerBanishingArrow = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}BanishingArrow")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Banishment)
            .SetSharedPool(ActivationTime.NoCost, pool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetParticleEffectParameters(SpellDefinitions.Banishment)
                    .SetSavingThrowData(
                        false, AttributeDefinitions.Charisma, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Intelligence,
                        8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, 2, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 6, 11)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        ArcaneShotPowers.Add(powerBanishingArrow,
            new ArcaneArcherData
            {
                DebuffCondition = ConditionDefinitions.ConditionBanished,
                EffectSpell = SpellDefinitions.Banishment,
                EffectType = EffectHelpers.EffectType.Effect
            });

        // Beguiling Arrow

        var powerBeguilingArrow = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}BeguilingArrow")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.CharmPerson)
            .SetSharedPool(ActivationTime.NoCost, pool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .SetDurationData(DurationType.Round, 1)
                    .SetParticleEffectParameters(SpellDefinitions.CharmPerson)
                    .SetSavingThrowData(
                        false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Intelligence,
                        8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypePsychic, 2, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 6, 11)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        ArcaneShotPowers.Add(powerBeguilingArrow,
            new ArcaneArcherData
            {
                DebuffCondition = ConditionDefinitions.ConditionCharmed,
                EffectSpell = SpellDefinitions.CharmPerson,
                EffectType = EffectHelpers.EffectType.Effect
            });

        // Bursting Arrow

        _powerBurstingArrow = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}BurstingArrow")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.EldritchBlast)
            .SetSharedPool(ActivationTime.NoCost, pool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, 2, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 6, 11)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        ArcaneShotPowers.Add(_powerBurstingArrow,
            new ArcaneArcherData
            {
                EffectSpell = FeatureDefinitionPowers.PowerSymbolOfSleep, EffectType = EffectHelpers.EffectType.Zone
            });

        // Enfeebling Arrow

        var powerEnfeeblingArrow = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}EnfeeblingArrow")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.RayOfEnfeeblement)
            .SetSharedPool(ActivationTime.NoCost, pool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetParticleEffectParameters(SpellDefinitions.RayOfEnfeeblement)
                    .SetSavingThrowData(
                        false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Intelligence,
                        8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeNecrotic, 2, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 6, 11)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        var abilityCheckAffinityEnfeeblingArrow = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create($"AbilityCheckAffinity{Name}EnfeeblingArrow")
            .SetGuiPresentation($"Condition{Name}EnfeeblingArrow", Category.Condition, Gui.NoLocalization)
            .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.Disadvantage,
                AttributeDefinitions.Strength,
                AttributeDefinitions.Dexterity,
                AttributeDefinitions.Constitution)
            .AddToDB();

        var savingThrowAffinityEnfeeblingArrow = FeatureDefinitionSavingThrowAffinityBuilder
            .Create($"SavingThrowAffinity{Name}EnfeeblingArrow")
            .SetGuiPresentation($"Condition{Name}EnfeeblingArrow", Category.Condition, Gui.NoLocalization)
            .SetAffinities(CharacterSavingThrowAffinity.Disadvantage, false,
                AttributeDefinitions.Strength,
                AttributeDefinitions.Dexterity,
                AttributeDefinitions.Constitution)
            .AddToDB();

        var conditionEnfeeblingArrow = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionEnfeebled, $"Condition{Name}EnfeeblingArrow")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetParentCondition(ConditionDefinitions.ConditionEnfeebled)
            .AddFeatures(
                abilityCheckAffinityEnfeeblingArrow,
                savingThrowAffinityEnfeeblingArrow)
            .AddToDB();

        ArcaneShotPowers.Add(powerEnfeeblingArrow,
            new ArcaneArcherData
            {
                DebuffCondition = conditionEnfeeblingArrow,
                EffectSpell = SpellDefinitions.RayOfEnfeeblement,
                EffectType = EffectHelpers.EffectType.Effect
            });

        // Grasping Arrow

        var powerGraspingArrow = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}GraspingArrow")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Entangle)
            .SetSharedPool(ActivationTime.NoCost, pool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetParticleEffectParameters(SpellDefinitions.Entangle)
                    .SetSavingThrowData(
                        false, AttributeDefinitions.Strength, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Intelligence,
                        8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeSlashing, 2, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 6, 11)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        var conditionGraspingArrow = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionRestrained, $"Condition{Name}GraspingArrow")
            .SetConditionParticleReference(
                ConditionDefinitions.ConditionRestrainedByMagicalArrow.conditionParticleReference)
            .AddToDB();

        ArcaneShotPowers.Add(powerGraspingArrow,
            new ArcaneArcherData
            {
                DebuffCondition = conditionGraspingArrow,
                EffectSpell = SpellDefinitions.Entangle,
                EffectType = EffectHelpers.EffectType.Effect
            });

        // Insight Arrow

        var lightSourceForm = SpellDefinitions.FaerieFire.EffectDescription
            .GetFirstFormOfType(EffectForm.EffectFormType.LightSource).LightSourceForm;

        var conditionInsightArrow = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionHighlighted, $"Condition{Name}InsightArrow")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetConditionParticleReference(ConditionDefinitions.ConditionShine.conditionParticleReference)
            .AddToDB();

        var powerInsightArrow = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}InsightArrow")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.TrueStrike)
            .SetSharedPool(ActivationTime.NoCost, pool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetParticleEffectParameters(SpellDefinitions.FaerieFire)
                    .SetSavingThrowData(
                        false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Intelligence,
                        8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeRadiant, 2, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 6, 11)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetLightSourceForm(
                                LightSourceType.Basic, 2, 2, lightSourceForm.Color,
                                lightSourceForm.graphicsPrefabReference)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        ArcaneShotPowers.Add(powerInsightArrow,
            new ArcaneArcherData
            {
                DebuffCondition = conditionInsightArrow,
                EffectSpell = SpellDefinitions.Shine,
                EffectType = EffectHelpers.EffectType.Effect
            });

        // Shadow Arrow

        var powerShadowArrow = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}ShadowArrow")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Blindness)
            .SetSharedPool(ActivationTime.NoCost, pool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetParticleEffectParameters(SpellDefinitions.Blindness)
                    .SetSavingThrowData(
                        false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Intelligence,
                        8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypePsychic, 2, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 6, 11)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        ArcaneShotPowers.Add(powerShadowArrow,
            new ArcaneArcherData
            {
                DebuffCondition = ConditionDefinitions.ConditionBlinded,
                EffectSpell = SpellDefinitions.Blindness,
                EffectType = EffectHelpers.EffectType.Effect
            });

        // Slowing Arrow

        var powerSlowingArrow = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}SlowingArrow")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Slow)
            .SetSharedPool(ActivationTime.NoCost, pool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.Individuals)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetParticleEffectParameters(SpellDefinitions.Slow)
                    .SetSavingThrowData(
                        false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Intelligence,
                        8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, 2, DieType.D6)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 6, 11)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        ArcaneShotPowers.Add(powerSlowingArrow,
            new ArcaneArcherData
            {
                DebuffCondition = ConditionDefinitions.ConditionSlowed,
                EffectSpell = SpellDefinitions.Slow,
                EffectType = EffectHelpers.EffectType.Effect
            });
    }

    private static void CreateArcaneArcherChoices(IEnumerable<FeatureDefinitionPower> powers)
    {
        foreach (var power in powers)
        {
            var name = power.Name.Replace("Power", string.Empty);
            var guiPresentation = power.guiPresentation;

            _ = CustomInvocationDefinitionBuilder
                .Create($"CustomInvocation{name}")
                .SetGuiPresentation(guiPresentation)
                .SetPoolType(InvocationPoolTypeCustom.Pools.ArcaneShotChoice)
                .SetGrantedFeature(power)
                .AddCustomSubFeatures(ModifyInvocationVisibility.Marker)
                .AddToDB();
        }
    }

    private static void TryInflictArcaneShotCondition(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        ArcaneArcherData arcaneArcherData)
    {
        var rulesetAttacker = attacker.RulesetCharacter;
        var rulesetDefender = defender.RulesetCharacter;

        if (rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false } ||
            rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
        {
            return;
        }

        EffectHelpers.StartVisualEffect(attacker, defender, arcaneArcherData.EffectSpell, arcaneArcherData.EffectType);
        rulesetDefender.InflictCondition(
            arcaneArcherData.DebuffCondition.Name,
            DurationType.Round,
            1,
            TurnOccurenceType.EndOfSourceTurn,
            AttributeDefinitions.TagEffect,
            rulesetAttacker.guid,
            rulesetAttacker.CurrentFaction.Name,
            1,
            arcaneArcherData.DebuffCondition.Name,
            0,
            0,
            0);
    }

    private static void InflictBurstingArrowAreaDamage(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RulesetAttackMode attackerAttackMode,
        ArcaneArcherData arcaneArcherData,
        CharacterAction action)
    {
        if (Gui.Battle == null)
        {
            return;
        }

        var criticalSuccess = action.AttackRollOutcome == RollOutcome.CriticalSuccess;
        var rulesetAttacker = attacker.RulesetCharacter;
        var classLevel = rulesetAttacker.GetClassLevel(CharacterClassDefinitions.Fighter);
        var diceNumber = classLevel switch
        {
            >= 17 => 4,
            >= 11 => 3,
            _ => 2
        };

        // apply damage to all targets
        foreach (var target in Gui.Battle.GetContenders(defender, false, isWithinXCells: 3))
        {
            var rulesetTarget = target.RulesetCharacter;
            var damageForm = new DamageForm
            {
                DamageType = DamageTypeForce, DieType = DieType.D6, DiceNumber = diceNumber, BonusDamage = 0
            };
            var rolls = new List<int>();
            var damageRoll = rulesetAttacker.RollDamage(
                damageForm, 0, criticalSuccess, 0, 0, 1, false, false, false, rolls);

            EffectHelpers.StartVisualEffect(
                attacker, defender, arcaneArcherData.EffectSpell, arcaneArcherData.EffectType);
            RulesetActor.InflictDamage(
                damageRoll,
                damageForm,
                damageForm.DamageType,
                new RulesetImplementationDefinitions.ApplyFormsParams { targetCharacter = rulesetTarget },
                rulesetTarget,
                false,
                attacker.Guid,
                false,
                attackerAttackMode.AttackTags,
                new RollInfo(damageForm.DieType, rolls, 0),
                false,
                out _);
        }
    }

    private struct ArcaneArcherData
    {
        public ConditionDefinition DebuffCondition;
        public IMagicEffect EffectSpell;
        public EffectHelpers.EffectType EffectType;
    }

    //
    // Arcane Shot
    //

    private sealed class SpendPowerFinishedByMeArcaneShot : IActionFinishedByMe, IPhysicalAttackFinishedByMe
    {
        private FeatureDefinitionPower PowerSpent { get; set; }
        private RollOutcome SaveOutcome { get; set; } = RollOutcome.Success;

        // collect the spent power and save outcome
        public IEnumerator OnActionFinishedByMe(CharacterAction characterAction)
        {
            if (characterAction is not CharacterActionSpendPower action)
            {
                PowerSpent = null;

                yield break;
            }

            PowerSpent = action.activePower.PowerDefinition;
            SaveOutcome = action.SaveOutcome;
        }

        // apply arrow behavior after attack finishes
        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackerAttackMode,
            RollOutcome attackRollOutcome,
            int damageAmount)
        {
            if (PowerSpent == null || !ArcaneShotPowers.TryGetValue(PowerSpent, out var arcaneArcherData))
            {
                yield break;
            }

            attacker.UsedSpecialFeatures.TryAdd(ArcaneShotMarker, 1);

            if (SaveOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                PowerSpent = null;

                yield break;
            }

            // apply arrow behaviors after attack is complete
            if (PowerSpent == _powerBurstingArrow)
            {
                InflictBurstingArrowAreaDamage(attacker, defender, attackerAttackMode, arcaneArcherData, action);
            }
            else
            {
                TryInflictArcaneShotCondition(attacker, defender, arcaneArcherData);
            }

            PowerSpent = null;
            SaveOutcome = RollOutcome.Success;
        }
    }

    //
    // Guided Shot
    //

    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    private class TryAlterOutcomePhysicalAttackGuidedShot(FeatureDefinition featureDefinition)
        : ITryAlterOutcomePhysicalAttack
    {
        public IEnumerator OnAttackTryAlterOutcome(
            GameLocationBattleManager battle,
            CharacterAction action,
            GameLocationCharacter me,
            GameLocationCharacter target,
            ActionModifier attackModifier)
        {
            if (action.AttackRollOutcome is not (RollOutcome.Failure or RollOutcome.CriticalFailure))
            {
                yield break;
            }

            var attackMode = action.actionParams.attackMode;
            var rulesetAttacker = me.RulesetCharacter;

            if (!IsBow(attackMode, null, null) || !me.CanPerceiveTarget(target))
            {
                yield break;
            }

            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (manager == null)
            {
                yield break;
            }

            var reactionParams = new CharacterActionParams(me, (ActionDefinitions.Id)ExtraActionId.DoNothingFree);
            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("MartialArcaneArcherGuidedShot", reactionParams);

            manager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(me, manager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var totalRoll = (action.AttackRoll + attackMode.ToHitBonus).ToString();
            var rollCaption = action.AttackRoll == 1
                ? "Feedback/&RollCheckCriticalFailureTitle"
                : "Feedback/&CriticalAttackFailureOutcome";

            rulesetAttacker.LogCharacterUsedFeature(featureDefinition,
                "Feedback/&TriggerRerollLine",
                false,
                (ConsoleStyleDuplet.ParameterType.Base, $"{action.AttackRoll}+{attackMode.ToHitBonus}"),
                (ConsoleStyleDuplet.ParameterType.FailedRoll, Gui.Format(rollCaption, totalRoll)));

            var roll = rulesetAttacker.RollAttack(
                attackMode.toHitBonus,
                target.RulesetCharacter,
                attackMode.sourceDefinition,
                attackModifier.attackToHitTrends,
                false,
                [new TrendInfo(1, FeatureSourceType.CharacterFeature, featureDefinition.Name, featureDefinition)],
                attackMode.ranged,
                false,
                attackModifier.attackRollModifier,
                out var outcome,
                out var successDelta,
                -1,
                // testMode true avoids the roll to display on combat log as the original one will get there with altered results
                true);

            attackModifier.ignoreAdvantage = false;
            attackModifier.attackAdvantageTrends =
                [new TrendInfo(1, FeatureSourceType.CharacterFeature, featureDefinition.Name, featureDefinition)];
            action.AttackRollOutcome = outcome;
            action.AttackSuccessDelta = successDelta;
            action.AttackRoll = roll;
        }
    }

    //
    // Ready Shot
    //

    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    private sealed class BattleStartedListenerEverReadyShot(FeatureDefinition featureDefinition)
        : IInitiativeEndListener
    {
        public IEnumerator OnInitiativeEnded(GameLocationCharacter locationCharacter)
        {
            var character = locationCharacter.RulesetCharacter;

            if (character is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var levels = character.GetClassLevel(CharacterClassDefinitions.Fighter);

            if (levels < 15)
            {
                yield break;
            }

            var usablePower = PowerProvider.Get(PowerArcaneShot, character);

            if (character.GetRemainingUsesOfPower(usablePower) > 0)
            {
                yield break;
            }

            character.RepayPowerUse(usablePower);
            character.LogCharacterUsedFeature(featureDefinition);
        }
    }
}
