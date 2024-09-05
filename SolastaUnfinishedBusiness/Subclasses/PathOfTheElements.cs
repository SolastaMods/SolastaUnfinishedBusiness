using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class PathOfTheElements : AbstractSubclass
{
    internal const string Name = "PathOfTheElements";
    private const string ElementalBlessing = "ElementalBlessing";
    private const string ElementalBurst = "ElementalBurst";
    private const string ElementalConduit = "ElementalConduit";

    internal static readonly FeatureDefinitionFeatureSet FeatureSetElementalFury =
        FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}ElementalFury")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
            .SetAncestryType(ExtraAncestryType.PathOfTheElements)
            .AddToDB();

    public PathOfTheElements()
    {
        #region LEVEL 03

        // Elemental Fury

        var powerStorm = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Storm")
            .SetGuiPresentation($"Ancestry{Name}Wildfire", Category.Feature,
                Gui.Format($"Feature/&Ancestry{Name}AllDescription", Gui.Localize("Rules/&DamageLightningTitle")),
                hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeLightning, 1, DieType.D6))
                    .SetImpactEffectParameters(SpellDefinitions.LightningBolt)
                    .Build())
            .AddToDB();

        powerStorm.AddCustomSubFeatures(new ModifyEffectDescriptionElementalFuryDamage(powerStorm));

        var ancestryStorm = FeatureDefinitionAncestryBuilder
            .Create($"Ancestry{Name}Storm")
            .SetGuiPresentation(Category.Feature,
                Gui.Format($"Feature/&Ancestry{Name}AllDescription", Gui.Localize("Rules/&DamageLightningTitle")))
            .SetAncestry(ExtraAncestryType.PathOfTheElements)
            .SetDamageType(DamageTypeLightning)
            .AddToDB();

        ancestryStorm.AddCustomSubFeatures(new CharacterTurnEndedElementalFury(powerStorm));

        var powerBlizzard = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Blizzard")
            .SetGuiPresentation($"Ancestry{Name}Wildfire", Category.Feature,
                Gui.Format($"Feature/&Ancestry{Name}AllDescription", Gui.Localize("Rules/&DamageColdTitle")),
                hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeCold, 1, DieType.D6))
                    .SetImpactEffectParameters(SpellDefinitions.RayOfFrost)
                    .Build())
            .AddToDB();

        powerBlizzard.AddCustomSubFeatures(new ModifyEffectDescriptionElementalFuryDamage(powerBlizzard));

        var ancestryBlizzard = FeatureDefinitionAncestryBuilder
            .Create($"Ancestry{Name}Blizzard")
            .SetGuiPresentation(Category.Feature,
                Gui.Format($"Feature/&Ancestry{Name}AllDescription", Gui.Localize("Rules/&DamageColdTitle")))
            .SetAncestry(ExtraAncestryType.PathOfTheElements)
            .SetDamageType(DamageTypeCold)
            .AddToDB();

        ancestryBlizzard.AddCustomSubFeatures(new CharacterTurnEndedElementalFury(powerBlizzard));

        var powerWildfire = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Wildfire")
            .SetGuiPresentation($"Ancestry{Name}Wildfire", Category.Feature,
                Gui.Format($"Feature/&Ancestry{Name}AllDescription", Gui.Localize("Rules/&DamageFireTitle")),
                hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeFire, 1, DieType.D6))
                    .SetImpactEffectParameters(SpellDefinitions.FireBolt)
                    .Build())
            .AddToDB();

        powerWildfire.AddCustomSubFeatures(new ModifyEffectDescriptionElementalFuryDamage(powerWildfire));

        var ancestryWildfire = FeatureDefinitionAncestryBuilder
            .Create($"Ancestry{Name}Wildfire")
            .SetGuiPresentation(Category.Feature,
                Gui.Format($"Feature/&Ancestry{Name}AllDescription", Gui.Localize("Rules/&DamageFireTitle")))
            .SetAncestry(ExtraAncestryType.PathOfTheElements)
            .SetDamageType(DamageTypeFire)
            .AddToDB();

        ancestryWildfire.AddCustomSubFeatures(new CharacterTurnEndedElementalFury(powerWildfire));

        // keep sorted
        FeatureSetElementalFury.FeatureSet.Add(ancestryBlizzard);
        FeatureSetElementalFury.FeatureSet.Add(ancestryStorm);
        FeatureSetElementalFury.FeatureSet.Add(ancestryWildfire);

        #endregion

        #region LEVEL 06

        // Storm

        var conditionElementalBlessingStorm = ConditionDefinitionBuilder
            .Create($"Condition{Name}{ElementalBlessing}Storm")
            .SetGuiPresentation($"Condition{Name}{ElementalBlessing}", Category.Condition,
                ConditionDefinitions.ConditionBlessed)
            .SetPossessive()
            .SetSpecialInterruptions(ExtraConditionInterruption.SourceRageStop)
            .SetFeatures(FeatureDefinitionDamageAffinitys.DamageAffinityLightningResistance)
            .AddToDB();

        var powerElementalBlessingStorm = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{ElementalBlessing}Storm")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 2)
                    .SetDurationData(DurationType.Permanent)
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionElementalBlessingStorm))
                    .Build())
            .AddToDB();

        // Blizzard

        var conditionElementalBlessingBlizzard = ConditionDefinitionBuilder
            .Create($"Condition{Name}{ElementalBlessing}Blizzard")
            .SetGuiPresentation($"Condition{Name}{ElementalBlessing}", Category.Condition,
                ConditionDefinitions.ConditionBlessed)
            .SetPossessive()
            .SetSpecialInterruptions(ExtraConditionInterruption.SourceRageStop)
            .SetFeatures(FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance)
            .AddToDB();

        var powerElementalBlessingBlizzard = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{ElementalBlessing}Blizzard")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 2)
                    .SetDurationData(DurationType.Permanent)
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionElementalBlessingBlizzard))
                    .Build())
            .AddToDB();

        // Wildfire

        var conditionElementalBlessingWildfire = ConditionDefinitionBuilder
            .Create($"Condition{Name}{ElementalBlessing}Wildfire")
            .SetGuiPresentation($"Condition{Name}{ElementalBlessing}", Category.Condition,
                ConditionDefinitions.ConditionBlessed)
            .SetPossessive()
            .SetSpecialInterruptions(ExtraConditionInterruption.SourceRageStop)
            .SetFeatures(FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance)
            .AddToDB();

        var powerElementalBlessingWildfire = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{ElementalBlessing}Wildfire")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 2)
                    .SetDurationData(DurationType.Permanent)
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionElementalBlessingWildfire))
                    .Build())
            .AddToDB();

        // Elemental Blessing

        var featureSetElementalBlessing = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}{ElementalBlessing}")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.DeterminedByAncestry)
            .SetAncestryType(ExtraAncestryType.PathOfTheElements, DamageTypeCold, DamageTypeLightning, DamageTypeFire)
            .AddToDB();

        // keep sorted
        featureSetElementalBlessing.FeatureSet.Add(powerElementalBlessingBlizzard);
        featureSetElementalBlessing.FeatureSet.Add(powerElementalBlessingStorm);
        featureSetElementalBlessing.FeatureSet.Add(powerElementalBlessingWildfire);

        #endregion

        #region LEVEL 10

        // Storm

        var powerElementalBurstStorm = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{ElementalBurst}Storm")
            .SetGuiPresentation(Category.Feature, PowerDomainElementalLightningBlade)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.Cube, 3)
                    //.SetParticleEffectParameters(PowerDomainElementalLightningBlade)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Dexterity,
                        true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                        AttributeDefinitions.Constitution)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeLightning, 3, DieType.D10)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 4, 13)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(ConditionDefinitions.ConditionShocked,
                                ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(PowerDomainElementalLightningBlade)
                    .Build())
            .AddCustomSubFeatures(
                new ValidatorsValidatePowerUse(ValidatorsCharacter.HasAnyOfConditions(ConditionRaging)))
            .AddToDB();

        // Blizzard

        var powerElementalBurstBlizzard = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{ElementalBurst}Blizzard")
            .SetGuiPresentation(Category.Feature, PowerDomainElementalIceLance)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.Cube, 3)
                    //.SetParticleEffectParameters(PowerDomainElementalIceLance)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Strength,
                        true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                        AttributeDefinitions.Constitution)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeCold, 3, DieType.D8)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 4, 13)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.FallProne)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetCasterEffectParameters(PowerDomainElementalIceLance)
                    .SetEffectEffectParameters(PowerDomainElementalIceLance)
                    .Build())
            .AddCustomSubFeatures(
                new ValidatorsValidatePowerUse(ValidatorsCharacter.HasAnyOfConditions(ConditionRaging)))
            .AddToDB();

        // Wildfire

        var powerElementalBurstWildfire = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{ElementalBurst}Wildfire")
            .SetGuiPresentation(Category.Feature, PowerDomainElementalFireBurst)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.Cube, 3)
                    .SetDurationData(DurationType.Round, 1)
                    //.SetParticleEffectParameters(PowerDomainElementalFireBurst)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Dexterity,
                        true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                        AttributeDefinitions.Constitution)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeFire, 4, DieType.D6)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 1, 4, 13)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(ConditionDefinitions.ConditionOnFire,
                                ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetCasterEffectParameters(PowerDomainElementalFireBurst)
                    .SetEffectEffectParameters(PowerDomainElementalFireBurst)
                    .Build())
            .AddCustomSubFeatures(
                new ValidatorsValidatePowerUse(ValidatorsCharacter.HasAnyOfConditions(ConditionRaging)))
            .AddToDB();

        // Elemental Burst

        var featureSetElementalBurst = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}{ElementalBurst}")
            .SetGuiPresentation(Category.Feature,
                Gui.Format($"Feature/&FeatureSet{Name}{ElementalBurst}Description",
                    Gui.Localize($"Feature/&Power{Name}{ElementalBurst}StormDescription"),
                    Gui.Localize($"Feature/&Power{Name}{ElementalBurst}BlizzardDescription"),
                    Gui.Localize($"Feature/&Power{Name}{ElementalBurst}WildfireDescription")))
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.DeterminedByAncestry)
            .SetAncestryType(ExtraAncestryType.PathOfTheElements, DamageTypeCold, DamageTypeLightning, DamageTypeFire)
            .AddToDB();

        // keep sorted
        featureSetElementalBurst.FeatureSet.Add(powerElementalBurstBlizzard);
        featureSetElementalBurst.FeatureSet.Add(powerElementalBurstStorm);
        featureSetElementalBurst.FeatureSet.Add(powerElementalBurstWildfire);

        #endregion

        #region LEVEL 14

        // Storm

        var conditionElementalConduitStorm = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionFlyingAdaptive, $"Condition{Name}{ElementalConduit}Storm")
            // don't use vanilla RageStop with permanent conditions
            .SetSpecialInterruptions(ExtraConditionInterruption.SourceRageStop)
            .AddToDB();

        var powerElementalConduitStorm = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{ElementalConduit}Storm")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Permanent)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionElementalConduitStorm, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        // Blizzard

        var featureElementalConduitBlizzard = FeatureDefinitionBuilder
            .Create($"Feature{Name}{ElementalConduit}Blizzard")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureElementalConduitBlizzard.AddCustomSubFeatures(
            new CharacterBeforeTurnEndedElementalConduitBlizzard(featureElementalConduitBlizzard));

        // Wildfire

        // kept name for backward compatibility
        var powerElementalConduitWildfire = FeatureDefinitionPowerBuilder
            .Create($"Feature{Name}{ElementalConduit}Wildfire")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Constitution)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetDamageForm(DamageTypeFire)
                            .Build())
                    .SetImpactEffectParameters(SpellDefinitions.HellishRebuke)
                    .Build())
            .AddToDB();

        powerElementalConduitWildfire.AddCustomSubFeatures(
            new PhysicalAttackFinishedOnMeElementalConduitWildfire(powerElementalConduitWildfire),
            new UpgradeEffectDamageBonusBasedOnClassLevel(
                powerElementalConduitWildfire, CharacterClassDefinitions.Barbarian));

        // Elemental Conduit

        var featureSetElementalConduit = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}{ElementalConduit}")
            .SetGuiPresentation(Category.Feature,
                Gui.Format($"Feature/&FeatureSet{Name}{ElementalBurst}Description",
                    Gui.Localize($"Feature/&Power{Name}{ElementalConduit}StormDescription"),
                    Gui.Localize($"Feature/&Feature{Name}{ElementalConduit}BlizzardDescription"),
                    Gui.Localize($"Feature/&Feature{Name}{ElementalConduit}WildfireDescription")))
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.DeterminedByAncestry)
            .SetAncestryType(ExtraAncestryType.PathOfTheElements, DamageTypeCold, DamageTypeLightning, DamageTypeFire)
            .AddToDB();

        // keep sorted
        featureSetElementalConduit.FeatureSet.Add(featureElementalConduitBlizzard);
        featureSetElementalConduit.FeatureSet.Add(powerElementalConduitStorm);
        featureSetElementalConduit.FeatureSet.Add(powerElementalConduitWildfire);

        #endregion

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PathOfTheElements, 256))
            .AddFeaturesAtLevel(3, FeatureSetElementalFury)
            .AddFeaturesAtLevel(6, featureSetElementalBlessing)
            .AddFeaturesAtLevel(10, featureSetElementalBurst)
            .AddFeaturesAtLevel(14, featureSetElementalConduit)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Barbarian;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Elemental Fury
    //

    private sealed class ModifyEffectDescriptionElementalFuryDamage(FeatureDefinitionPower powerDamage)
        : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerDamage;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var classLevel = character.GetClassLevel(CharacterClassDefinitions.Barbarian);
            int diceNumber;
            DieType dieType;

            switch (classLevel)
            {
                case < 6:
                    diceNumber = 1;
                    dieType = DieType.D6;
                    break;
                case < 10:
                    diceNumber = 1;
                    dieType = DieType.D10;
                    break;
                case < 14:
                    diceNumber = 2;
                    dieType = DieType.D6;
                    break;
                default:
                    diceNumber = 2;
                    dieType = DieType.D10;
                    break;
            }

            var damageForm = effectDescription.FindFirstDamageForm();

            damageForm.DiceNumber = diceNumber;
            damageForm.DieType = dieType;

            return effectDescription;
        }
    }

    private sealed class CharacterTurnEndedElementalFury(FeatureDefinitionPower powerDamage)
        : ICharacterBeforeTurnEndListener
    {
        public void OnCharacterBeforeTurnEnded(GameLocationCharacter locationCharacter)
        {
            if (Gui.Battle == null)
            {
                return;
            }

            var rulesetAttacker = locationCharacter.RulesetCharacter;

            if (!rulesetAttacker.HasConditionOfTypeOrSubType(ConditionRaging))
            {
                return;
            }

            var usablePower = PowerProvider.Get(powerDamage, rulesetAttacker);
            var targets = Gui.Battle.GetContenders(locationCharacter, withinRange: 1).ToArray();

            locationCharacter.MyExecuteActionSpendPower(usablePower, false, targets);
        }
    }

    //
    // Elemental Conduit - Blizzard
    //

    private sealed class CharacterBeforeTurnEndedElementalConduitBlizzard(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureDefinition) : ICharacterBeforeTurnEndListener
    {
        public void OnCharacterBeforeTurnEnded(GameLocationCharacter locationCharacter)
        {
            var battle = Gui.Battle;

            if (battle == null)
            {
                return;
            }

            var rulesetAttacker = locationCharacter.RulesetCharacter;

            if (!rulesetAttacker.HasConditionOfTypeOrSubType(ConditionRaging))
            {
                return;
            }

            foreach (var targetLocationCharacter in battle.GetContenders(locationCharacter, withinRange: 1))
            {
                var rulesetDefender = targetLocationCharacter.RulesetCharacter;
                var modifierTrend = rulesetDefender.actionModifier.savingThrowModifierTrends;
                var advantageTrends = rulesetDefender.actionModifier.savingThrowAdvantageTrends;
                var attackerConModifier =
                    AttributeDefinitions.ComputeAbilityScoreModifier(
                        rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.Constitution));
                var profBonus = rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
                var defenderStrModifier =
                    AttributeDefinitions.ComputeAbilityScoreModifier(
                        rulesetDefender.TryGetAttributeValue(AttributeDefinitions.Strength));

                rulesetDefender.RollSavingThrow(0, AttributeDefinitions.Strength, featureDefinition, modifierTrend,
                    advantageTrends, defenderStrModifier, 8 + profBonus + attackerConModifier, false,
                    out var savingOutcome,
                    out _);

                if (savingOutcome == RollOutcome.Success)
                {
                    continue;
                }

                var actionService = ServiceRepository.GetService<IGameLocationActionService>();

                actionService.StopCharacterActions(targetLocationCharacter, CharacterAction.InterruptionType.Abort);
                rulesetDefender.InflictCondition(
                    CustomConditionsContext.StopMovement.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetAttacker.guid,
                    rulesetAttacker.CurrentFaction.Name,
                    1,
                    CustomConditionsContext.StopMovement.Name,
                    0,
                    0,
                    0);
            }
        }
    }

    //
    // Elemental Conduit - Wildfire
    //

    private sealed class PhysicalAttackFinishedOnMeElementalConduitWildfire(
        FeatureDefinitionPower powerElementalConduitWildfire) : IPhysicalAttackFinishedOnMe
    {
        public IEnumerator OnPhysicalAttackFinishedOnMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (rollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure ||
                defender.IsMyTurn() ||
                !defender.CanReact() ||
                rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false } ||
                !rulesetDefender.HasConditionOfTypeOrSubType(ConditionRaging))
            {
                yield break;
            }

            var usablePower = PowerProvider.Get(powerElementalConduitWildfire, rulesetDefender);

            yield return defender.MyReactToUsePower(
                Id.PowerReaction,
                usablePower,
                [attacker],
                attacker,
                "ElementalConduitWildfire",
                battleManager: battleManager);
        }
    }
}
