using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class CircleOfTheCosmos : AbstractSubclass
{
    private const string Name = "CircleOfTheCosmos";

    public CircleOfTheCosmos()
    {
        // LEVEL 02

        // Constellation Map

        var autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}ConstellationMap")
            .SetGuiPresentationNoContent(true)
            .SetAutoTag("Circle")
            .SetPreparedSpellGroups(BuildSpellGroup(2, GuidingBolt))
            .SetSpellcastingClass(Druid)
            .AddToDB();

        var bonusCantrips = FeatureDefinitionBonusCantripsBuilder
            .Create($"BonusCantrips{Name}ConstellationMap")
            .SetGuiPresentationNoContent(true)
            .SetBonusCantrips(Guidance)
            .AddToDB();

        var powerGuidingBolt = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}GuidingBolt")
            .SetGuiPresentation(GuidingBolt.GuiPresentation)
            .SetUsesProficiencyBonus(ActivationTime.Action)
            .SetUseSpellAttack()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(GuidingBolt)
                    .Build())
            .AddToDB();

        var featureSetConstellationMap = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}ConstellationMap")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(autoPreparedSpells, bonusCantrips, powerGuidingBolt)
            .AddToDB();

        // Constellation Form

        var powerConstellationForm = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}ConstellationForm")
            .SetGuiPresentation($"FeatureSet{Name}ConstellationForm", Category.Feature,
                Sprites.GetSprite("ConstellationForm", Resources.PowerConstellationForm, 256, 128))
            .SetSharedPool(ActivationTime.BonusAction, PowerDruidWildShape)
            .AddToDB();

        var powerArcherConstellationForm = BuildArcher(ActivationTime.BonusAction);
        var powerChaliceConstellationForm = BuildChalice(ActivationTime.BonusAction);
        var powerDragonConstellationForm = BuildDragon(ActivationTime.BonusAction);

        var featureSetConstellationForm = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}ConstellationForm")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(
                powerConstellationForm,
                powerArcherConstellationForm,
                powerChaliceConstellationForm,
                powerDragonConstellationForm)
            .AddToDB();

        PowerBundle.RegisterPowerBundle(
            powerConstellationForm,
            false,
            powerArcherConstellationForm,
            powerChaliceConstellationForm,
            powerDragonConstellationForm);
        ForceGlobalUniqueEffects.AddToGroup(
            ForceGlobalUniqueEffects.Group.ConstellationForm,
            powerArcherConstellationForm, powerChaliceConstellationForm, powerDragonConstellationForm);

        // LEVEL 06

        // Cosmos Omen

        var powerWealCosmosOmen = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}WealCosmosOmen")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Reaction)
            .SetReactionContext(ExtraReactionContext.Custom)
            .AddToDB();

        powerWealCosmosOmen.AddCustomSubFeatures(
            new TryAlterOutcomeSavingThrowWeal(powerWealCosmosOmen));

        var conditionWealCosmosOmen = ConditionDefinitionBuilder
            .Create($"Condition{Name}WealCosmosOmen")
            .SetGuiPresentation($"Power{Name}WealCosmosOmen", Category.Feature, ConditionDefinitions.ConditionGuided)
            .SetPossessive()
            .SetConditionType(ConditionType.Beneficial)
            .SetFeatures(powerWealCosmosOmen)
            .AddCustomSubFeatures(new AddUsablePowersFromCondition())
            .AddToDB();

        conditionWealCosmosOmen.GuiPresentation.description = Gui.NoLocalization;

        var powerWoeCosmosOmen = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}WoeCosmosOmen")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Reaction)
            .SetReactionContext(ExtraReactionContext.Custom)
            .AddToDB();

        powerWoeCosmosOmen.AddCustomSubFeatures(
            new TryAlterOutcomeSavingThrowWoe(powerWoeCosmosOmen));

        var conditionWoeCosmosOmen = ConditionDefinitionBuilder
            .Create($"Condition{Name}WoeCosmosOmen")
            .SetGuiPresentation($"Power{Name}WoeCosmosOmen", Category.Feature, ConditionDefinitions.ConditionGuided)
            .SetPossessive()
            .SetConditionType(ConditionType.Beneficial)
            .SetFeatures(powerWoeCosmosOmen)
            .AddCustomSubFeatures(new AddUsablePowersFromCondition())
            .AddToDB();

        conditionWoeCosmosOmen.GuiPresentation.description = Gui.NoLocalization;

        var powerCosmosOmen = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}CosmosOmen")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("CosmosOmen", Resources.PowerCosmosOmen, 256, 128))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.UntilLongRest)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitions.ConditionShine, // only a placeholder
                                ConditionForm.ConditionOperation.AddRandom,
                                false, false,
                                conditionWealCosmosOmen,
                                conditionWoeCosmosOmen)
                            .Build())
                    .Build())
            .AddToDB();

        // LEVEL 10

        // Twinkling Stars

        var powerSwitchConstellationForm = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}SwitchConstellationForm")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("SwitchConstellationForm", Resources.PowerSwitchConstellationForm, 256, 128))
            .SetUsesFixed(ActivationTime.NoCost)
            .AddCustomSubFeatures(
                new ValidatorsValidatePowerUse(character =>
                {
                    if (Gui.Battle == null)
                    {
                        return false;
                    }

                    if (!character.HasAnyConditionOfType(
                            $"Condition{Name}Archer",
                            $"Condition{Name}Chalice",
                            $"Condition{Name}Dragon"))
                    {
                        return false;
                    }

                    var glc = GameLocationCharacter.GetFromActor(character);

                    if (glc == null || glc.HasAttackedSinceLastTurn || glc.UsedTacticalMoves > 0)
                    {
                        return false;
                    }

                    var hasMainAction = glc.GetActionTypeStatus(ActionDefinitions.ActionType.Main) ==
                                        ActionDefinitions.ActionStatus.Available;

                    var hasBonusAction = glc.GetActionTypeStatus(ActionDefinitions.ActionType.Bonus) ==
                                         ActionDefinitions.ActionStatus.Available;

                    return hasMainAction && hasBonusAction;
                }))
            .AddToDB();

        var powerSwitchConstellationFormArcher = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}SwitchConstellationFormArcher")
            .SetGuiPresentation($"Power{Name}ArcherConstellationForm", Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerSwitchConstellationForm)
            .AddCustomSubFeatures(new MagicEffectFinishedByMeTwinklingStars($"Condition{Name}Archer"))
            .AddToDB();

        var powerSwitchConstellationFormChalice = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}SwitchConstellationFormChalice")
            .SetGuiPresentation($"Power{Name}ChaliceConstellationForm", Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerSwitchConstellationForm)
            .AddCustomSubFeatures(new MagicEffectFinishedByMeTwinklingStars($"Condition{Name}Chalice"))
            .AddToDB();

        var powerSwitchConstellationFormDragon = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}SwitchConstellationFormDragon")
            .SetGuiPresentation($"Power{Name}DragonConstellationForm", Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerSwitchConstellationForm)
            .AddCustomSubFeatures(new MagicEffectFinishedByMeTwinklingStars($"Condition{Name}Dragon"))
            .AddToDB();

        PowerBundle.RegisterPowerBundle(
            powerSwitchConstellationForm,
            false,
            powerSwitchConstellationFormArcher,
            powerSwitchConstellationFormChalice,
            powerSwitchConstellationFormDragon);

        var featureSetTwinklingStars = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}TwinklingStars")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(powerSwitchConstellationForm)
            .AddToDB();

        // LEVEL 14

        // Nova Star

        var featureSetNovaStar = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}NovaStar")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(
                FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistance)
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PatronEldritchSurge, 256))
            .AddFeaturesAtLevel(2, featureSetConstellationMap, featureSetConstellationForm)
            .AddFeaturesAtLevel(6, powerCosmosOmen)
            .AddFeaturesAtLevel(10, featureSetTwinklingStars)
            .AddFeaturesAtLevel(14, featureSetNovaStar)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => Druid;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceDruidCircle;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static FeatureDefinitionPowerSharedPool BuildArcher(ActivationTime activationTime)
    {
        var sprite = Sprites.GetSprite("ConstellationFormArcher", Resources.PowerArcher, 256, 128);

        // Archer No Cost

        var powerArcherNoCost = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ArcherNoCost")
            .SetGuiPresentation($"Power{Name}Archer", Category.Feature, sprite)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetUseSpellAttack()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 12, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetBonusMode(AddBonusMode.Proficiency)
                            .SetDamageForm(DamageTypeRadiant, 1, DieType.D8)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 2, 20, (10, 1))
                            .Build())
                    .SetParticleEffectParameters(PowerTraditionLightBlindingFlash)
                    .Build())
            .AddToDB();

        var conditionArcherNoCost = ConditionDefinitionBuilder
            .Create($"Condition{Name}ArcherNoCost")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(powerArcherNoCost)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddCustomSubFeatures(new AddUsablePowersFromCondition())
            .AddToDB();

        powerArcherNoCost.EffectDescription.effectParticleParameters.effectParticleReference = new AssetReference();
        powerArcherNoCost.AddCustomSubFeatures(
            ValidatorsValidatePowerUse.InCombat,
            new MagicEffectFinishedByMeArcherNoCost(conditionArcherNoCost));

        // Archer Bonus Action

        var powerArcher = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Archer")
            .SetGuiPresentation(Category.Feature, sprite)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetUseSpellAttack()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 12, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetBonusMode(AddBonusMode.Proficiency)
                            .SetDamageForm(DamageTypeRadiant, 1, DieType.D8)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 2, 20, (10, 1))
                            .Build())
                    .SetParticleEffectParameters(PowerTraditionLightBlindingFlash)
                    .Build())
            .AddToDB();

        powerArcher.EffectDescription.effectParticleParameters.effectParticleReference = new AssetReference();

        var conditionArcher = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionShine, $"Condition{Name}Archer")
            .SetGuiPresentation($"Power{Name}Archer", Category.Feature,
                ConditionDefinitions.ConditionLightSensitiveSorakSaboteur)
            .SetPossessive()
            .SetConditionType(ConditionType.Beneficial)
            .SetFeatures(powerArcher)
            .AddCustomSubFeatures(new AddUsablePowersFromCondition())
            .AddToDB();

        conditionArcher.GuiPresentation.description = Gui.NoLocalization;

        // Archer Main

        var lightSourceForm =
            FaerieFire.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.LightSource);

        var powerArcherConstellationForm = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}ArcherConstellationForm")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetSharedPool(activationTime, PowerDruidWildShape)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 10)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionArcher),
                        EffectFormBuilder.ConditionForm(conditionArcherNoCost),
                        EffectFormBuilder
                            .Create()
                            .SetLightSourceForm(
                                LightSourceType.Basic, 10, 10,
                                lightSourceForm.lightSourceForm.color,
                                lightSourceForm.lightSourceForm.graphicsPrefabReference)
                            .Build())
                    .SetParticleEffectParameters(PowerOathOfJugementWeightOfJustice)
                    .Build())
            .AddToDB();

        return powerArcherConstellationForm;
    }

    private static FeatureDefinitionPowerSharedPool BuildChalice(ActivationTime activationTime)
    {
        // Chalice Bonus Action

        var powerChalice = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Chalice")
            .SetGuiPresentation(Category.Feature, PowerPaladinLayOnHands)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetUseSpellAttack()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetBonusMode(AddBonusMode.AbilityBonus)
                            .SetHealingForm(HealingComputation.Dice,
                                0, DieType.D8, 1, false, HealingCap.MaximumHitPoints)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 2, 20, (10, 1))
                            .Build())
                    .SetParticleEffectParameters(HealingWord)
                    .Build())
            .AddToDB();

        powerChalice.EffectDescription.effectParticleParameters.effectParticleReference = new AssetReference();

        var conditionChaliceHealing = ConditionDefinitionBuilder
            .Create($"Condition{Name}ChaliceHealing")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(powerChalice)
            .AddCustomSubFeatures(new AddUsablePowersFromCondition())
            .AddToDB();

        var conditionChalice = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionShine, $"Condition{Name}Chalice")
            .SetGuiPresentation($"Power{Name}Chalice", Category.Feature,
                ConditionDefinitions.ConditionLightSensitiveSorakSaboteur)
            .SetPossessive()
            .SetConditionType(ConditionType.Beneficial)
            .AddCustomSubFeatures(new MagicEffectFinishedByMeAnyChalice(powerChalice, conditionChaliceHealing))
            .AddToDB();

        // Chalice Main

        var lightSourceForm =
            FaerieFire.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.LightSource);

        var powerChaliceConstellationForm = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}ChaliceConstellationForm")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetSharedPool(activationTime, PowerDruidWildShape)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 10)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionChalice),
                        EffectFormBuilder
                            .Create()
                            .SetLightSourceForm(
                                LightSourceType.Basic, 10, 10,
                                lightSourceForm.lightSourceForm.color,
                                lightSourceForm.lightSourceForm.graphicsPrefabReference)
                            .Build())
                    .Build())
            .AddToDB();

        return powerChaliceConstellationForm;
    }

    private static FeatureDefinitionPowerSharedPool BuildDragon(ActivationTime activationTime)
    {
        var dieRollModifierDragonAbility = FeatureDefinitionDieRollModifierBuilder
            .Create($"DieRollModifier{Name}DragonAbility")
            .SetGuiPresentation($"Power{Name}DragonConstellationForm", Category.Feature,
                $"Feature/&DieRollModifier{Name}DragonAbilityDescription")
            .SetModifiers(
                RollContext.AbilityCheck,
                0,
                10,
                0,
                "Feedback/&DieRollModifierCircleOfTheCosmosDragonReroll",
                // Intelligence Checks
                SkillDefinitions.Arcana,
                SkillDefinitions.History,
                SkillDefinitions.Investigation,
                SkillDefinitions.Nature,
                SkillDefinitions.Religion,
                // Wisdom Checks
                SkillDefinitions.AnimalHandling,
                SkillDefinitions.Insight,
                SkillDefinitions.Medecine,
                SkillDefinitions.Perception,
                SkillDefinitions.Survival)
            .AddToDB();

        var dieRollModifierDragonConcentration = FeatureDefinitionDieRollModifierBuilder
            .Create($"DieRollModifier{Name}DragonConcentration")
            .SetGuiPresentation($"Power{Name}DragonConstellationForm", Category.Feature,
                $"Feature/&DieRollModifier{Name}DragonConcentrationDescription")
            .SetModifiers(
                RollContext.ConcentrationCheck,
                0,
                10,
                0,
                "Feedback/&DieRollModifierCircleOfTheCosmosDragonReroll")
            .AddToDB();

        var conditionDragon = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionShine, $"Condition{Name}Dragon")
            .SetGuiPresentation($"Power{Name}DragonConstellationForm", Category.Feature,
                ConditionDefinitions.ConditionLightSensitiveSorakSaboteur)
            .SetPossessive()
            .SetConditionType(ConditionType.Beneficial)
            .SetFeatures(
                dieRollModifierDragonAbility,
                dieRollModifierDragonConcentration)
            .AddToDB();

        conditionDragon.GuiPresentation.description = Gui.NoLocalization;

        var conditionDragonHigher = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionShine, $"Condition{Name}DragonHigher")
            .SetGuiPresentation($"Power{Name}DragonConstellationForm", Category.Feature,
                ConditionDefinitions.ConditionLightSensitiveSorakSaboteur)
            .SetParentCondition(ConditionDefinitions.ConditionFlying)
            .SetPossessive()
            .SetConditionType(ConditionType.Beneficial)
            .SetFeatures(
                FeatureDefinitionMoveModes.MoveModeFly4,
                dieRollModifierDragonAbility,
                dieRollModifierDragonConcentration)
            .AddToDB();

        // there is indeed a typo on tag
        // ReSharper disable once StringLiteralTypo
        conditionDragonHigher.ConditionTags.SetRange("Verticality");
        conditionDragonHigher.GuiPresentation.description = Gui.NoLocalization;

        // Dragon Main

        var lightSourceForm =
            FaerieFire.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.LightSource);

        var powerDragonConstellationForm = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}DragonConstellationForm")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetSharedPool(activationTime, PowerDruidWildShape)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 10)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionDragon),
                        EffectFormBuilder
                            .Create()
                            .SetLightSourceForm(
                                LightSourceType.Basic, 10, 10,
                                lightSourceForm.lightSourceForm.color,
                                lightSourceForm.lightSourceForm.graphicsPrefabReference)
                            .Build())
                    .Build())
            .AddToDB();

        powerDragonConstellationForm.AddCustomSubFeatures(
            new ModifyEffectDescriptionDragon(powerDragonConstellationForm, conditionDragon, conditionDragonHigher));

        return powerDragonConstellationForm;
    }

    //
    // Archer No Cost
    //

    private sealed class MagicEffectFinishedByMeArcherNoCost(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionArcherNoCost) : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;

            if (rulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionArcherNoCost.Name, out var activeCondition))
            {
                rulesetCharacter.RemoveCondition(activeCondition);
            }

            yield break;
        }
    }

    //
    // Chalice
    //

    private sealed class MagicEffectFinishedByMeAnyChalice(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerChalice,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionChaliceHealing) : IMagicEffectFinishedByMeAny
    {
        public IEnumerator OnMagicEffectFinishedByMeAny(
            CharacterActionMagicEffect action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetEffect = action.ActionParams.RulesetEffect;

            if (rulesetEffect.SourceDefinition == powerChalice &&
                rulesetAttacker.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionChaliceHealing.Name, out var activeCondition))
            {
                rulesetAttacker.RemoveCondition(activeCondition);

                yield break;
            }

            if (rulesetEffect.EffectDescription.EffectForms.All(x =>
                    x.FormType != EffectForm.EffectFormType.Healing))
            {
                yield break;
            }

            rulesetAttacker.InflictCondition(
                conditionChaliceHealing.Name,
                DurationType.Permanent,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionChaliceHealing.Name,
                0,
                0,
                0);
        }
    }

    //
    // Dragon
    //

    private sealed class ModifyEffectDescriptionDragon(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerDragon,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionDragon,
        ConditionDefinition conditionDragonHigher) : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerDragon;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var levels = character.GetClassLevel(Druid);

            if (levels < 10)
            {
                return effectDescription;
            }

            var conditionForm = effectDescription.EffectForms.FirstOrDefault(x =>
                x.FormType == EffectForm.EffectFormType.Condition &&
                x.ConditionForm.ConditionDefinition == conditionDragon);

            if (conditionForm != null)
            {
                conditionForm.ConditionForm.conditionDefinition = conditionDragonHigher;
            }

            return effectDescription;
        }
    }

    //
    // Weal
    //

    private sealed class TryAlterOutcomeSavingThrowWeal(
        FeatureDefinitionPower powerWeal) : ITryAlterOutcomeAttack, ITryAlterOutcomeSavingThrow
    {
        private const DieType DieType = RuleDefinitions.DieType.D6;
        private static readonly int MaxDieTypeValue = DiceMaxValue[(int)DieType];

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager battle,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier attackModifier)
        {
            var gameLocationActionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (gameLocationActionManager == null)
            {
                yield break;
            }

            if (action.AttackRollOutcome is not (RollOutcome.Failure or RollOutcome.CriticalFailure))
            {
                yield break;
            }

            if (!ShouldAttackTrigger(action, attacker, helper))
            {
                yield break;
            }

            var rulesetCharacter = helper.RulesetCharacter;

            var reactionParams =
                new CharacterActionParams(helper, (ActionDefinitions.Id)ExtraActionId.DoNothingReaction)
                {
                    StringParameter = Gui.Format("Reaction/&CustomReactionWealCosmosOmenDescription",
                        attacker.Name, defender.Name, helper.Name)
                };

            var previousReactionCount = gameLocationActionManager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("WealCosmosOmen", reactionParams);

            gameLocationActionManager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(helper, gameLocationActionManager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var dieRoll = RollDie(DieType, AdvantageType.None, out _, out _);

            attackModifier.AttacktoHitTrends.Add(
                new TrendInfo(dieRoll, FeatureSourceType.Power, powerWeal.Name, powerWeal)
                {
                    dieType = DieType, dieFlag = TrendInfoDieFlag.None
                });

            action.AttackSuccessDelta += dieRoll;
            attackModifier.attackRollModifier += dieRoll;

            var success = action.AttackSuccessDelta >= 0;

            if (success)
            {
                action.AttackRollOutcome = RollOutcome.Success;
            }

            rulesetCharacter.LogCharacterUsedPower(
                powerWeal,
                "Feedback/&CosmosOmenToHitRoll",
                extra:
                [
                    (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(DieType)),
                    (ConsoleStyleDuplet.ParameterType.Positive, dieRoll.ToString())
                ]);
        }

        public IEnumerator OnTryAlterOutcomeSavingThrow(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier saveModifier,
            bool hasHitVisual,
            bool hasBorrowedLuck)
        {
            var gameLocationActionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (gameLocationActionManager == null)
            {
                yield break;
            }

            if (!action.RolledSaveThrow || action.SaveOutcome == RollOutcome.Success)
            {
                yield break;
            }

            if (!ShouldSavingTrigger(action, attacker, helper))
            {
                yield break;
            }

            var implementationManagerService =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var rulesetHelper = helper.RulesetCharacter;

            var usablePower = PowerProvider.Get(powerWeal, rulesetHelper);
            var reactionParams = new CharacterActionParams(helper, ActionDefinitions.Id.SpendPower)
            {
                StringParameter = "WealCosmosOmen",
                StringParameter2 = Gui.Format("Reaction/&SpendPowerWealCosmosOmenReactDescription",
                    defender.Name, attacker.Name, helper.Name),
                RulesetEffect = implementationManagerService
                    //CHECK: no need for AddAsActivePowerToSource
                    .MyInstantiateEffectPower(rulesetHelper, usablePower, false),
                UsablePower = usablePower
            };
            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var count = actionService.PendingReactionRequestGroups.Count;

            actionService.ReactToSpendPower(reactionParams);

            yield return battleManager.WaitForReactions(helper, actionService, count);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            TryModifyRoll(action, helper, saveModifier);
        }

        private static bool ShouldSavingTrigger(
            CharacterAction action,
            GameLocationCharacter defender,
            GameLocationCharacter helper)
        {
            return helper.CanReact() &&
                   defender.Side == helper.Side &&
                   helper.IsWithinRange(defender, 6) &&
                   action.SaveOutcomeDelta + MaxDieTypeValue >= 0;
        }

        private static bool ShouldAttackTrigger(
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter helper)
        {
            return helper.CanReact() &&
                   attacker.Side == helper.Side &&
                   helper.IsWithinRange(attacker, 6) &&
                   action.AttackSuccessDelta + MaxDieTypeValue >= 0;
        }

        private void TryModifyRoll(
            CharacterAction action,
            // ReSharper disable once SuggestBaseTypeForParameter
            GameLocationCharacter helper,
            ActionModifier saveModifier)
        {
            var dieRoll = helper.RulesetCharacter
                .RollDie(DieType, RollContext.None, false, AdvantageType.None, out _, out _);
            var saveDc = action.GetSaveDC() + saveModifier.SaveDCModifier;
            var rolled = saveDc + action.saveOutcomeDelta + dieRoll;
            var success = rolled >= saveDc;

            saveModifier.SavingThrowModifierTrends?.Add(
                new TrendInfo(dieRoll, FeatureSourceType.Power, powerWeal.Name, powerWeal)
                {
                    dieType = DieType, dieFlag = TrendInfoDieFlag.None
                });

            const string TEXT = "Feedback/&CharacterGivesBonusToSaveWithDCFormat";
            string result;
            ConsoleStyleDuplet.ParameterType resultType;

            if (success)
            {
                result = GameConsole.SaveSuccessOutcome;
                resultType = ConsoleStyleDuplet.ParameterType.SuccessfulRoll;
                action.saveOutcome = RollOutcome.Success;
                action.saveOutcomeDelta += dieRoll;
            }
            else
            {
                result = GameConsole.SaveFailureOutcome;
                resultType = ConsoleStyleDuplet.ParameterType.FailedRoll;
            }

            var console = Gui.Game.GameConsole;
            var entry = new GameConsoleEntry(TEXT, console.consoleTableDefinition) { Indent = true };

            console.AddCharacterEntry(helper.RulesetCharacter, entry);

            entry.AddParameter(ConsoleStyleDuplet.ParameterType.Positive, $"+{dieRoll}");
            entry.AddParameter(resultType, Gui.Format(result, rolled.ToString()));
            entry.AddParameter(ConsoleStyleDuplet.ParameterType.AbilityInfo, saveDc.ToString());

            console.AddEntry(entry);

            action.RolledSaveThrow = true;
        }
    }

    //
    // Woe
    //

    private sealed class TryAlterOutcomeSavingThrowWoe(
        FeatureDefinitionPower powerWoe) : ITryAlterOutcomeAttack, ITryAlterOutcomeSavingThrow
    {
        private const DieType DieType = RuleDefinitions.DieType.D6;
        private static readonly int MaxDieTypeValue = DiceMaxValue[(int)DieType];

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager battle,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier attackModifier)
        {
            var gameLocationActionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (gameLocationActionManager == null)
            {
                yield break;
            }

            if (action.AttackRollOutcome is not RollOutcome.Success)
            {
                yield break;
            }

            if (!ShouldAttackTrigger(action, attacker, helper))
            {
                yield break;
            }

            var rulesetCharacter = helper.RulesetCharacter;

            var reactionParams =
                new CharacterActionParams(helper, (ActionDefinitions.Id)ExtraActionId.DoNothingReaction)
                {
                    StringParameter = Gui.Format("Reaction/&CustomReactionWoeCosmosOmenDescription",
                        attacker.Name, defender.Name, helper.Name)
                };

            var previousReactionCount = gameLocationActionManager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("WoeCosmosOmen", reactionParams);

            gameLocationActionManager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(helper, gameLocationActionManager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var dieRoll = -RollDie(DieType, AdvantageType.None, out _, out _);

            attackModifier.AttacktoHitTrends.Add(
                new TrendInfo(dieRoll, FeatureSourceType.Power, powerWoe.Name, powerWoe)
                {
                    dieType = DieType, dieFlag = TrendInfoDieFlag.None
                });

            action.AttackSuccessDelta += dieRoll;
            attackModifier.attackRollModifier += dieRoll;

            var failure = action.AttackSuccessDelta < 0;

            if (failure)
            {
                action.AttackRollOutcome = RollOutcome.Failure;
            }

            rulesetCharacter.LogCharacterUsedPower(
                powerWoe,
                "Feedback/&CosmosOmenToHitRoll",
                extra:
                [
                    (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(DieType)),
                    (ConsoleStyleDuplet.ParameterType.Negative, dieRoll.ToString())
                ]);
        }

        public IEnumerator OnTryAlterOutcomeSavingThrow(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier saveModifier,
            bool hasHitVisual,
            bool hasBorrowedLuck)
        {
            var gameLocationActionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (gameLocationActionManager == null)
            {
                yield break;
            }

            if (!action.RolledSaveThrow || action.SaveOutcome == RollOutcome.Success)
            {
                yield break;
            }

            if (!ShouldSavingTrigger(action, defender, helper))
            {
                yield break;
            }

            var implementationManagerService =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var rulesetHelper = helper.RulesetCharacter;

            var usablePower = PowerProvider.Get(powerWoe, rulesetHelper);
            var reactionParams = new CharacterActionParams(helper, ActionDefinitions.Id.SpendPower)
            {
                StringParameter = "WoeCosmosOmen",
                // StringParameter2 = Gui.Format("Reaction/&SpendPowerWoeCosmosOmenReactDescription",
                //     defender.Name, attacker.Name, helper.Name),
                RulesetEffect = implementationManagerService
                    //CHECK: no need for AddAsActivePowerToSource
                    .MyInstantiateEffectPower(rulesetHelper, usablePower, false),
                UsablePower = usablePower
            };
            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var count = actionService.PendingReactionRequestGroups.Count;

            actionService.ReactToSpendPower(reactionParams);

            yield return battleManager.WaitForReactions(helper, actionService, count);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            TryModifyRoll(action, helper, saveModifier);
        }

        private static bool ShouldSavingTrigger(
            CharacterAction action,
            GameLocationCharacter defender,
            GameLocationCharacter helper)
        {
            return helper.CanReact() &&
                   defender.IsOppositeSide(helper.Side) &&
                   helper.IsWithinRange(defender, 6) &&
                   action.SaveOutcomeDelta - MaxDieTypeValue < 0;
        }

        private static bool ShouldAttackTrigger(
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter helper)
        {
            return helper.CanReact() &&
                   attacker.IsOppositeSide(helper.Side) &&
                   helper.IsWithinRange(attacker, 6) &&
                   action.AttackSuccessDelta - MaxDieTypeValue < 0;
        }

        private void TryModifyRoll(
            CharacterAction action,
            // ReSharper disable once SuggestBaseTypeForParameter
            GameLocationCharacter helper,
            ActionModifier saveModifier)
        {
            var dieRoll = -helper.RulesetCharacter
                .RollDie(DieType, RollContext.None, false, AdvantageType.None, out _, out _);
            var saveDc = action.GetSaveDC() + saveModifier.SaveDCModifier;
            var rolled = saveDc + action.saveOutcomeDelta + dieRoll;
            var failure = rolled < saveDc;

            saveModifier.SavingThrowModifierTrends.Add(
                new TrendInfo(dieRoll, FeatureSourceType.Power, powerWoe.Name, powerWoe)
                {
                    dieType = DieType, dieFlag = TrendInfoDieFlag.None
                });

            const string TEXT = "Feedback/&CharacterGivesBonusToSaveWithDCFormat";
            string result;
            ConsoleStyleDuplet.ParameterType resultType;

            if (failure)
            {
                result = GameConsole.SaveFailureOutcome;
                resultType = ConsoleStyleDuplet.ParameterType.FailedRoll;
                action.saveOutcome = RollOutcome.Failure;
                action.saveOutcomeDelta += dieRoll;
            }
            else
            {
                result = GameConsole.SaveSuccessOutcome;
                resultType = ConsoleStyleDuplet.ParameterType.SuccessfulRoll;
            }

            var console = Gui.Game.GameConsole;
            var entry = new GameConsoleEntry(TEXT, console.consoleTableDefinition) { Indent = true };

            console.AddCharacterEntry(helper.RulesetCharacter, entry);

            entry.AddParameter(ConsoleStyleDuplet.ParameterType.Positive, $"+{dieRoll}");
            entry.AddParameter(resultType, Gui.Format(result, rolled.ToString()));
            entry.AddParameter(ConsoleStyleDuplet.ParameterType.AbilityInfo, saveDc.ToString());

            console.AddEntry(entry);

            action.RolledSaveThrow = true;
        }
    }

    //
    // Twinkling Stars
    //

    private sealed class MagicEffectFinishedByMeTwinklingStars(string conditionName) : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;

            var activeCondition = rulesetCharacter.AllConditions.FirstOrDefault(x =>
                x.Name is $"Condition{Name}Archer" or $"Condition{Name}Chalice" or $"Condition{Name}Dragon");

            if (activeCondition == null)
            {
                yield break;
            }

            var durationType = activeCondition.DurationType;
            var rounds = activeCondition.RemainingRounds;
            var endOccurence = activeCondition.EndOccurence;

            rulesetCharacter.RemoveCondition(activeCondition);
            rulesetCharacter.InflictCondition(
                conditionName,
                durationType,
                rounds,
                endOccurence,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                conditionName,
                0,
                0,
                0);
        }
    }
}
