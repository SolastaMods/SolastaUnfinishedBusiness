using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
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

        var ancestryStorm = FeatureDefinitionAncestryBuilder
            .Create($"Ancestry{Name}Storm")
            .SetGuiPresentation(Category.Feature,
                Gui.Format($"Feature/&Ancestry{Name}AllDescription", Gui.Localize("Rules/&DamageLightningTitle")))
            .SetAncestry(ExtraAncestryType.PathOfTheElements)
            .SetDamageType(DamageTypeLightning)
            .AddToDB();

        var powerStorm = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Storm")
            .SetGuiPresentation(ancestryStorm.GuiPresentation.Title, ancestryStorm.GuiPresentation.Description)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeLightning))
                    .SetImpactEffectParameters(SpellDefinitions.LightningBolt)
                    .Build())
            .AddToDB();

        powerStorm.GuiPresentation.hidden = true;
        ancestryStorm.AddCustomSubFeatures(new CustomBehaviorElementalFury(powerStorm));

        var ancestryBlizzard = FeatureDefinitionAncestryBuilder
            .Create($"Ancestry{Name}Blizzard")
            .SetGuiPresentation(Category.Feature,
                Gui.Format($"Feature/&Ancestry{Name}AllDescription", Gui.Localize("Rules/&DamageColdTitle")))
            .SetAncestry(ExtraAncestryType.PathOfTheElements)
            .SetDamageType(DamageTypeCold)
            .AddToDB();

        var powerBlizzard = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Blizzard")
            .SetGuiPresentation(ancestryBlizzard.GuiPresentation.Title, ancestryBlizzard.GuiPresentation.Description)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeCold))
                    .SetImpactEffectParameters(SpellDefinitions.RayOfFrost)
                    .Build())
            .AddToDB();

        powerBlizzard.GuiPresentation.hidden = true;
        ancestryBlizzard.AddCustomSubFeatures(new CustomBehaviorElementalFury(powerBlizzard));

        var ancestryWildfire = FeatureDefinitionAncestryBuilder
            .Create($"Ancestry{Name}Wildfire")
            .SetGuiPresentation(Category.Feature,
                Gui.Format($"Feature/&Ancestry{Name}AllDescription", Gui.Localize("Rules/&DamageFireTitle")))
            .SetAncestry(ExtraAncestryType.PathOfTheElements)
            .SetDamageType(DamageTypeFire)
            .AddToDB();

        var powerWildfire = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Wildfire")
            .SetGuiPresentation(ancestryWildfire.GuiPresentation.Title, ancestryWildfire.GuiPresentation.Description)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeFire))
                    .SetImpactEffectParameters(SpellDefinitions.FireBolt)
                    .Build())
            .AddToDB();

        powerWildfire.GuiPresentation.hidden = true;
        ancestryWildfire.AddCustomSubFeatures(new CustomBehaviorElementalFury(powerWildfire));

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
            .AddFeatureSet(
                powerElementalBlessingBlizzard,
                powerElementalBlessingStorm,
                powerElementalBlessingWildfire)
            .AddToDB();

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
                    .SetCasterEffectParameters(PowerDomainElementalLightningBlade)
                    .Build())
            .AddCustomSubFeatures(
                new MagicEffectFinishedByMeElementalBurst(PowerDomainElementalLightningBlade),
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
                    .Build())
            .AddCustomSubFeatures(
                new MagicEffectFinishedByMeElementalBurst(PowerDomainElementalIceLance),
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
                    .Build())
            .AddCustomSubFeatures(
                new MagicEffectFinishedByMeElementalBurst(PowerDomainElementalFireBurst),
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
            .AddFeatureSet(
                powerElementalBurstBlizzard,
                powerElementalBurstStorm,
                powerElementalBurstWildfire)
            .AddToDB();

        #endregion

        #region LEVEL 14

        // Storm

        var conditionElementalConduitStorm = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionFlyingAdaptive, $"Condition{Name}{ElementalConduit}Storm")
            .SetSpecialInterruptions(ConditionInterruption.RageStop)
            .AddToDB();

        var powerElementalConduitStorm = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{ElementalConduit}Storm")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnRageStartAutomatic)
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

        var featureElementalConduitWildfire = FeatureDefinitionBuilder
            .Create($"Feature{Name}{ElementalConduit}Wildfire")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureElementalConduitWildfire.AddCustomSubFeatures(
            new ReactToAttackOnMeFinishedElementalConduitWildfire(featureElementalConduitWildfire));

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
            .AddFeatureSet(
                featureElementalConduitBlizzard,
                powerElementalConduitStorm,
                featureElementalConduitWildfire)
            .AddToDB();

        #endregion

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PathOfTheElements, 256))
            .AddFeaturesAtLevel(3, FeatureSetElementalFury, powerBlizzard, powerStorm, powerWildfire)
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

    private sealed class CustomBehaviorElementalFury(FeatureDefinitionPower power)
        : ICharacterBeforeTurnEndListener, IModifyEffectDescription
    {
        public void OnCharacterBeforeTurnEnded(GameLocationCharacter locationCharacter)
        {
            var implementationService = ServiceRepository.GetService<IRulesetImplementationService>();

            if (Gui.Battle == null || implementationService == null)
            {
                return;
            }

            var rulesetCharacter = locationCharacter.RulesetCharacter;

            if (!rulesetCharacter.HasAnyConditionOfType(ConditionRaging))
            {
                return;
            }

            var implementationManagerService =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(power, rulesetCharacter);
            var targets = Gui.Battle.GetContenders(locationCharacter, withinRange: 1);
            var actionParams = new CharacterActionParams(locationCharacter, ActionDefinitions.Id.PowerNoCost)
            {
                ActionModifiers = Enumerable.Repeat(new ActionModifier(), targets.Count).ToList(),
                RulesetEffect = implementationManagerService
                    .MyInstantiateEffectPower(rulesetCharacter, usablePower, false),
                UsablePower = usablePower,
                targetCharacters = targets
            };

            ServiceRepository.GetService<IGameLocationActionService>()?
                .ExecuteAction(actionParams, null, false);
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == power;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var classLevel = character.GetClassLevel(CharacterClassDefinitions.Barbarian);
            var damageForm = effectDescription.FindFirstDamageForm();

            switch (classLevel)
            {
                case < 6:
                    damageForm.diceNumber = 1;
                    damageForm.dieType = DieType.D6;
                    break;
                case < 10:
                    damageForm.diceNumber = 1;
                    damageForm.dieType = DieType.D10;
                    break;
                case < 14:
                    damageForm.diceNumber = 2;
                    damageForm.dieType = DieType.D6;
                    break;
                default:
                    damageForm.diceNumber = 2;
                    damageForm.dieType = DieType.D10;
                    break;
            }

            return effectDescription;
        }
    }

    //
    // Elemental Burst
    //

    private sealed class MagicEffectFinishedByMeElementalBurst(IMagicEffect magicEffect) : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var attacker = action.ActingCharacter;
            var defender = action.ActionParams.TargetCharacters[0];

            EffectHelpers.StartVisualEffect(attacker, defender, magicEffect, EffectHelpers.EffectType.Effect);

            yield break;
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

            if (!rulesetAttacker.HasAnyConditionOfType(ConditionRaging))
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
                var profBonus =
                    AttributeDefinitions.ComputeProficiencyBonus(
                        rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.CharacterLevel));
                var defenderStrModifier =
                    AttributeDefinitions.ComputeAbilityScoreModifier(
                        rulesetDefender.TryGetAttributeValue(AttributeDefinitions.Strength));

                rulesetDefender.RollSavingThrow(0, AttributeDefinitions.Strength, featureDefinition, modifierTrend,
                    advantageTrends, defenderStrModifier, 8 + profBonus + attackerConModifier, false,
                    out var savingOutcome,
                    out _);

                if (savingOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
                {
                    continue;
                }

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

    private sealed class ReactToAttackOnMeFinishedElementalConduitWildfire(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureDefinition) : IPhysicalAttackFinishedOnMe
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
            if (rollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                yield break;
            }

            //do not trigger on my own turn, so won't retaliate on AoO
            if (defender.IsMyTurn())
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (!defender.CanReact() ||
                rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            if (!rulesetDefender.HasAnyConditionOfType(ConditionRaging))
            {
                yield break;
            }

            var actionService =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (actionService == null || battleManager is not { IsBattleInProgress: true })
            {
                yield break;
            }

            var reactionParams =
                new CharacterActionParams(defender, (ActionDefinitions.Id)ExtraActionId.DoNothingReaction);
            var previousReactionCount = actionService.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("ElementalConduitWildfire", reactionParams);

            actionService.AddInterruptRequest(reactionRequest);

            yield return battleManager.WaitForReactions(defender, actionService, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var modifierTrend = rulesetAttacker.actionModifier.savingThrowModifierTrends;
            var advantageTrends = rulesetAttacker.actionModifier.savingThrowAdvantageTrends;
            var attackerConModifier =
                AttributeDefinitions.ComputeAbilityScoreModifier(
                    rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.Constitution));
            var profBonus =
                AttributeDefinitions.ComputeProficiencyBonus(
                    rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.CharacterLevel));
            var defenderDexModifier =
                AttributeDefinitions.ComputeAbilityScoreModifier(
                    rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.Dexterity));

            rulesetAttacker.RollSavingThrow(0, AttributeDefinitions.Dexterity, featureDefinition, modifierTrend,
                advantageTrends, defenderDexModifier, 8 + profBonus + attackerConModifier, false,
                out var savingOutcome,
                out _);

            if (savingOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                yield break;
            }

            var classLevel = rulesetDefender.GetClassLevel(CharacterClassDefinitions.Barbarian);
            var damageForm = new DamageForm
            {
                DamageType = DamageTypeFire, DieType = DieType.D1, DiceNumber = 0, BonusDamage = classLevel
            };

            EffectHelpers.StartVisualEffect(defender, attacker, SpellDefinitions.HellishRebuke);
            RulesetActor.InflictDamage(
                classLevel,
                damageForm,
                DamageTypeFire,
                new RulesetImplementationDefinitions.ApplyFormsParams { targetCharacter = rulesetDefender },
                rulesetDefender,
                false,
                rulesetAttacker.Guid,
                false,
                attackMode.AttackTags,
                new RollInfo(DieType.D1, [], classLevel),
                true,
                out _);
        }
    }
}
