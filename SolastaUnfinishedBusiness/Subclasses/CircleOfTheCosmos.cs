using System.Collections;
using System.Collections.Generic;
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

    private static readonly string[] ConstellationFormConditions =
    [
        $"Condition{Name}Archer", $"Condition{Name}Archer14",
        $"Condition{Name}Chalice", $"Condition{Name}Chalice14",
        $"Condition{Name}Dragon", $"Condition{Name}Dragon10", $"Condition{Name}Dragon14"
    ];

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

        var powerConstellationForm = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ConstellationForm")
            .SetGuiPresentation($"FeatureSet{Name}ConstellationForm", Category.Feature,
                Sprites.GetSprite("ConstellationForm", Resources.PowerConstellationForm, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest, 1, 2)
            .AddToDB();

        powerConstellationForm.AddCustomSubFeatures(
            new MagicEffectFinishedByMeConstellationForm(powerConstellationForm));

        var powerArcherConstellationForm = BuildArcher(ActivationTime.BonusAction, powerConstellationForm);
        var powerChaliceConstellationForm = BuildChalice(ActivationTime.BonusAction, powerConstellationForm);
        var powerDragonConstellationForm = BuildDragon(ActivationTime.BonusAction, powerConstellationForm);
        var powerDisableConstellationForm = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DisableConstellationForm")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("ConstellationForm", Resources.PowerConstellationForm, 256, 128))
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        ConstellationFormConditions
                            .Select(conditionName =>
                                DatabaseRepository
                                    .GetDatabase<ConditionDefinition>()
                                    .GetElement(conditionName))
                            .Select(condition =>
                                EffectFormBuilder.ConditionForm(condition, ConditionForm.ConditionOperation.Remove))
                            .ToArray())
                    .Build())
            .AddCustomSubFeatures(
                new ValidatorsValidatePowerUse(ValidatorsCharacter.HasAnyOfConditions(ConstellationFormConditions)))
            .AddToDB();

        var featureSetConstellationForm = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}ConstellationForm")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(
                powerConstellationForm,
                powerDisableConstellationForm,
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

        var powerCosmosOmenPool = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}CosmosOmenPool")
            .SetGuiPresentationNoContent(true)
            .SetUsesProficiencyBonus(ActivationTime.NoCost)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        var powerWealCosmosOmen = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}WealCosmosOmen")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerCosmosOmenPool)
            .SetShowCasting(false)
            .AddToDB();

        powerWealCosmosOmen.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new TryAlterOutcomeSavingThrowWeal(powerCosmosOmenPool, powerWealCosmosOmen));

        var powerWoeCosmosOmen = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}WoeCosmosOmen")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerCosmosOmenPool)
            .SetShowCasting(false)
            .AddToDB();

        powerWoeCosmosOmen.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new TryAlterOutcomeSavingThrowWoe(powerCosmosOmenPool, powerWoeCosmosOmen));

        var conditionWealCosmosOmen = ConditionDefinitionBuilder
            .Create($"Condition{Name}WealCosmosOmen")
            .SetGuiPresentation($"Power{Name}WealCosmosOmen", Category.Feature, ConditionDefinitions.ConditionGuided)
            .SetPossessive()
            .SetConditionType(ConditionType.Beneficial)
            .SetFeatures(powerWealCosmosOmen)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .AddToDB();

        conditionWealCosmosOmen.GuiPresentation.description = Gui.EmptyContent;

        var conditionWoeCosmosOmen = ConditionDefinitionBuilder
            .Create($"Condition{Name}WoeCosmosOmen")
            .SetGuiPresentation($"Power{Name}WoeCosmosOmen", Category.Feature, ConditionDefinitions.ConditionGuided)
            .SetPossessive()
            .SetConditionType(ConditionType.Beneficial)
            .SetFeatures(powerWoeCosmosOmen)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .AddToDB();

        conditionWoeCosmosOmen.GuiPresentation.description = Gui.EmptyContent;

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
                    .SetParticleEffectParameters(PowerMagebaneSpellCrusher)
                    .SetEffectEffectParameters(new AssetReference())
                    .Build())
            .AddToDB();

        // LEVEL 10

        // Twinkling Stars

        var powerSwitchConstellationForm = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}SwitchConstellationForm")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("SwitchConstellationForm", Resources.PowerSwitchConstellationForm, 256, 128))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.TurnStart)
            .AddCustomSubFeatures(
                new ValidatorsValidatePowerUse(character =>
                {
                    if (Gui.Battle == null || !character.HasAnyConditionOfType(ConstellationFormConditions))
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

        powerSwitchConstellationForm.AddCustomSubFeatures(
            new PowerOrSpellFinishedByMeTwinklingStarsRefund(powerSwitchConstellationForm));

        var powerSwitchConstellationFormAtWill = FeatureDefinitionPowerBuilder
            .Create(powerSwitchConstellationForm, $"Power{Name}SwitchConstellationFormAtWill")
            .SetUsesFixed(ActivationTime.NoCost)
            .AddCustomSubFeatures(
                new ValidatorsValidatePowerUse(c =>
                    Gui.Battle == null && c.HasAnyConditionOfType(ConstellationFormConditions)))
            .AddToDB();

        var powerSwitchConstellationFormArcher = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}SwitchConstellationFormArcher")
            .SetGuiPresentation($"Power{Name}ArcherConstellationForm", Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerSwitchConstellationForm)
            .SetShowCasting(false)
            .AddCustomSubFeatures(new PowerOrSpellFinishedByMeTwinklingStars(powerArcherConstellationForm))
            .AddToDB();

        var powerSwitchConstellationFormChalice = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}SwitchConstellationFormChalice")
            .SetGuiPresentation($"Power{Name}ChaliceConstellationForm", Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerSwitchConstellationForm)
            .SetShowCasting(false)
            .AddCustomSubFeatures(new PowerOrSpellFinishedByMeTwinklingStars(powerChaliceConstellationForm))
            .AddToDB();

        var powerSwitchConstellationFormDragon = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}SwitchConstellationFormDragon")
            .SetGuiPresentation($"Power{Name}DragonConstellationForm", Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerSwitchConstellationForm)
            .SetShowCasting(false)
            .AddCustomSubFeatures(new PowerOrSpellFinishedByMeTwinklingStars(powerDragonConstellationForm))
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
            .SetFeatureSet(powerSwitchConstellationForm, powerSwitchConstellationFormAtWill)
            .AddToDB();

        // LEVEL 14

        // Nova Star

        var featureNovaStar = FeatureDefinitionFeatureSetBuilder
            .Create($"Feature{Name}NovaStar")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.CircleOfTheCosmos, 256))
            .AddFeaturesAtLevel(2, featureSetConstellationMap, featureSetConstellationForm)
            .AddFeaturesAtLevel(6, powerCosmosOmen, powerCosmosOmenPool)
            .AddFeaturesAtLevel(10, featureSetTwinklingStars)
            .AddFeaturesAtLevel(14, featureNovaStar)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => Druid;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceDruidCircle;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static FeatureDefinitionPowerSharedPool BuildArcher(
        ActivationTime activationTime, FeatureDefinitionPower pool)
    {
        var powerArcher = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Archer")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerArcher", Resources.PowerArcher, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetUseSpellAttack()
            .SetExplicitAbilityScore(AttributeDefinitions.Wisdom)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 12, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetBonusMode(AddBonusMode.AbilityBonus)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 0, 10, 10)
                            .SetDamageForm(DamageTypeRadiant, 1, DieType.D8)
                            .Build())
                    .SetCasterEffectParameters(PowerOathOfTirmarGoldenSpeech)
                    .SetImpactEffectParameters(Sunbeam)
                    .Build())
            // required as added through a condition
            .AddCustomSubFeatures(ClassHolder.Druid)
            .AddToDB();

        var powerArcherNoCost = FeatureDefinitionPowerBuilder
            .Create(powerArcher, $"Power{Name}ArcherNoCost")
            .SetUsesFixed(ActivationTime.NoCost)
            .AddToDB();

        var conditionArcherNoCost = ConditionDefinitionBuilder
            .Create($"Condition{Name}ArcherNoCost")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(powerArcherNoCost)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .AddToDB();

        powerArcherNoCost.AddCustomSubFeatures(
            ValidatorsValidatePowerUse.InCombat,
            new PowerOrSpellFinishedByMeArcherNoCost(conditionArcherNoCost));

        var conditionArcher = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionSunbeam, $"Condition{Name}Archer")
            .SetGuiPresentation($"Power{Name}Archer", Category.Feature,
                ConditionDefinitions.ConditionFeatTakeAim)
            .SetPossessive()
            .SetConditionType(ConditionType.Beneficial)
            .SetFeatures(powerArcher)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .AddToDB();

        conditionArcher.GuiPresentation.description = Gui.EmptyContent;

        var conditionArcher14 = ConditionDefinitionBuilder
            .Create(conditionArcher, $"Condition{Name}Archer14")
            .AddFeatures(
                FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistanceTrue,
                FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistanceTrue,
                FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistanceTrue)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .AddToDB();

        // Archer Main

        var lightSourceForm =
            FaerieFire.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.LightSource);

        var powerArcherConstellationForm = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}ArcherConstellationForm")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetSharedPool(activationTime, pool)
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
                                LightSourceType.Basic, 2, 2,
                                lightSourceForm.lightSourceForm.color,
                                lightSourceForm.lightSourceForm.graphicsPrefabReference)
                            .Build())
                    .SetParticleEffectParameters(PowerOathOfJugementWeightOfJustice)
                    .Build())
            .AddToDB();

        powerArcherConstellationForm.AddCustomSubFeatures(
            new ModifyEffectDescriptionConstellationForm(
                powerArcherConstellationForm, conditionArcher, conditionArcher, conditionArcher14));

        return powerArcherConstellationForm;
    }

    private static FeatureDefinitionPowerSharedPool BuildChalice(
        ActivationTime activationTime, FeatureDefinitionPower pool)
    {
        var powerChalice = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Chalice")
            .SetGuiPresentation(Category.Feature, PowerPaladinLayOnHands)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetExplicitAbilityScore(AttributeDefinitions.Wisdom)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetBonusMode(AddBonusMode.AbilityBonus)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 1, 0, 10, 10)
                            .SetHealingForm(HealingComputation.Dice,
                                0, DieType.D8, 1, false, HealingCap.MaximumHitPoints)
                            .Build())
                    .SetParticleEffectParameters(CureWounds)
                    .Build())
            // required as added through a condition
            .AddCustomSubFeatures(ClassHolder.Druid)
            .AddToDB();

        var conditionChaliceHealing = ConditionDefinitionBuilder
            .Create($"Condition{Name}ChaliceHealing")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(powerChalice)
            .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
            .AddToDB();

        var conditionChalice = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionShine, $"Condition{Name}Chalice")
            .SetGuiPresentation($"Power{Name}Chalice", Category.Feature,
                ConditionDefinitions.ConditionBearsEndurance)
            .SetPossessive()
            .SetConditionType(ConditionType.Beneficial)
            .SetFeatures()
            .AddCustomSubFeatures(new MagicEffectFinishedByMeChalice(powerChalice, conditionChaliceHealing))
            .AddToDB();

        var conditionChalice14 = ConditionDefinitionBuilder
            .Create(conditionChalice, $"Condition{Name}Chalice14")
            .AddFeatures(
                FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistanceTrue,
                FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistanceTrue,
                FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistanceTrue)
            .AddCustomSubFeatures(new MagicEffectFinishedByMeChalice(powerChalice, conditionChaliceHealing))
            .AddToDB();

        // Chalice Main

        var lightSourceForm =
            FaerieFire.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.LightSource);

        var powerChaliceConstellationForm = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}ChaliceConstellationForm")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetSharedPool(activationTime, pool)
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
                                LightSourceType.Basic, 2, 2,
                                lightSourceForm.lightSourceForm.color,
                                lightSourceForm.lightSourceForm.graphicsPrefabReference)
                            .Build())
                    .SetCasterEffectParameters(PowerDomainLifePreserveLife)
                    .Build())
            .AddToDB();

        powerChaliceConstellationForm.AddCustomSubFeatures(
            new ModifyEffectDescriptionConstellationForm(
                powerChaliceConstellationForm, conditionChalice, conditionChalice, conditionChalice14));

        return powerChaliceConstellationForm;
    }

    private static FeatureDefinitionPowerSharedPool BuildDragon(
        ActivationTime activationTime, FeatureDefinitionPower pool)
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
            .Create($"Condition{Name}Dragon")
            .SetGuiPresentation($"Power{Name}DragonConstellationForm", Category.Feature,
                ConditionDefinitions.ConditionPactChainPseudodragon)
            .SetPossessive()
            .SetConditionType(ConditionType.Beneficial)
            .SetFeatures(
                dieRollModifierDragonAbility,
                dieRollModifierDragonConcentration)
            .CopyParticleReferences(PowerSorcererDraconicElementalResistance)
            .AddToDB();

        conditionDragon.GuiPresentation.description = Gui.EmptyContent;

        var conditionDragon10 = ConditionDefinitionBuilder
            .Create(conditionDragon, $"Condition{Name}Dragon10")
            .SetParentCondition(ConditionDefinitions.ConditionFlying)
            .SetFeatures(
                FeatureDefinitionMoveModes.MoveModeFly4,
                dieRollModifierDragonAbility,
                dieRollModifierDragonConcentration)
            .AddToDB();

        var conditionDragon14 = ConditionDefinitionBuilder
            .Create(conditionDragon10, $"Condition{Name}Dragon14")
            .SetParentCondition(ConditionDefinitions.ConditionFlying)
            .AddFeatures(
                FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistanceTrue,
                FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistanceTrue,
                FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistanceTrue)
            .AddToDB();

        // Dragon Main

        var lightSourceForm =
            FaerieFire.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.LightSource);

        var powerDragonConstellationForm = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}DragonConstellationForm")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetSharedPool(activationTime, pool)
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
                                LightSourceType.Basic, 2, 2,
                                lightSourceForm.lightSourceForm.color,
                                lightSourceForm.lightSourceForm.graphicsPrefabReference)
                            .Build())
                    .SetCasterEffectParameters(PowerDomainLawForceOfLaw)
                    .Build())
            .AddToDB();

        powerDragonConstellationForm.AddCustomSubFeatures(
            new ModifyEffectDescriptionConstellationForm(
                powerDragonConstellationForm, conditionDragon, conditionDragon10, conditionDragon14));

        return powerDragonConstellationForm;
    }

    //
    // Constellation Form
    //

    private sealed class MagicEffectFinishedByMeConstellationForm(FeatureDefinitionPower pool)
        : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterAction action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            if (action.ActionParams.activeEffect is not RulesetEffectPower rulesetEffectPower)
            {
                yield break;
            }

            RulesetUsablePower usablePower;
            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetEffectPower.PowerDefinition == PowerDruidWildShape)
            {
                usablePower = PowerProvider.Get(pool, rulesetAttacker);
                rulesetAttacker.UsePower(usablePower);

                // this is required as MulticlassWildshapeContext.UpdateUsablePowers get called before
                var rulesetMonster = ServiceRepository.GetService<IGameLocationCharacterService>().PartyCharacters
                    .FirstOrDefault(x => x.RulesetCharacter.OriginalFormCharacter == rulesetAttacker)?.RulesetCharacter;

                if (rulesetMonster == null)
                {
                    yield break;
                }

                usablePower = PowerProvider.Get(pool, rulesetMonster);
                rulesetMonster.UsePower(usablePower);
            }
            else if (rulesetEffectPower.PowerDefinition is FeatureDefinitionPowerSharedPool powerSharedPool &&
                     powerSharedPool.SharedPool == pool)
            {
                usablePower = PowerProvider.Get(PowerDruidWildShape, rulesetAttacker);
                rulesetAttacker.UsePower(usablePower);
            }
        }
    }

    private sealed class ModifyEffectDescriptionConstellationForm(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerConstellationForm,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionConstellationForm,
        ConditionDefinition conditionConstellationForm10,
        ConditionDefinition conditionConstellationForm14) : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerConstellationForm;
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
                x.ConditionForm.ConditionDefinition == conditionConstellationForm);

            conditionForm!.ConditionForm.conditionDefinition = levels < 14
                ? conditionConstellationForm10
                : conditionConstellationForm14;

            return effectDescription;
        }
    }

    //
    // Archer
    //

    private sealed class PowerOrSpellFinishedByMeArcherNoCost(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionArcherNoCost) : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;

            // no need to check for source here as these are all self conditions
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

    private sealed class MagicEffectFinishedByMeChalice(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerChalice,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionChaliceHealing) : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterAction action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetEffect = action.ActionParams.RulesetEffect;

            // no need to check for source here as these are all self conditions
            if (rulesetEffect.SourceDefinition == powerChalice &&
                rulesetAttacker.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionChaliceHealing.Name, out var activeCondition))
            {
                rulesetAttacker.RemoveCondition(activeCondition);

                yield break;
            }

            if (rulesetEffect is not RulesetEffectSpell ||
                rulesetEffect.EffectDescription.EffectForms.All(x =>
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
    // Weal
    //

    private sealed class TryAlterOutcomeSavingThrowWeal(
        FeatureDefinitionPower powerPool,
        FeatureDefinitionPower powerWeal)
        : ITryAlterOutcomeAttack, ITryAlterOutcomeAttributeCheck, ITryAlterOutcomeSavingThrow
    {
        private const DieType DieType = RuleDefinitions.DieType.D6;
        private static readonly int MaxDieTypeValue = DiceMaxValue[(int)DieType];

        public int HandlerPriority => -10;

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            var rulesetHelper = helper.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerPool, rulesetHelper);

            if (action.AttackRollOutcome != RollOutcome.Failure ||
                action.AttackSuccessDelta + MaxDieTypeValue < 0 ||
                rulesetHelper.GetRemainingUsesOfPower(usablePower) == 0 ||
                !helper.CanReact() ||
                attacker.IsOppositeSide(helper.Side) ||
                !helper.IsWithinRange(attacker, 6) ||
                !helper.CanPerceiveTarget(attacker))
            {
                yield break;
            }

            // any reaction within an attack flow must use the attacker as waiter
            yield return helper.MyReactToSpendPower(
                usablePower,
                attacker,
                "WealCosmosOmenAttack",
                "SpendPowerWealCosmosOmenAttackDescription".Formatted(
                    Category.Reaction, attacker.Name, defender.Name, helper.Name),
                ReactionValidated,
                battleManager);

            yield break;

            void ReactionValidated()
            {
                helper.SpendActionType(ActionDefinitions.ActionType.Reaction);

                var dieRoll = rulesetHelper.RollDie(DieType, RollContext.None, false, AdvantageType.None, out _, out _);

                attackModifier.AttacktoHitTrends.Add(
                    new TrendInfo(dieRoll, FeatureSourceType.Power, powerWeal.Name, powerWeal)
                    {
                        dieType = DieType, dieFlag = TrendInfoDieFlag.None
                    });

                action.AttackSuccessDelta += dieRoll;
                attackModifier.AttackRollModifier += dieRoll;

                if (action.AttackSuccessDelta >= 0)
                {
                    action.AttackRollOutcome = RollOutcome.Success;
                }

                var rulesetCharacter = helper.RulesetCharacter;

                rulesetCharacter.LogCharacterUsedPower(
                    powerWeal,
                    "Feedback/&CosmosOmenAttackToHitRoll",
                    extra:
                    [
                        (ConsoleStyleDuplet.ParameterType.Positive, dieRoll.ToString())
                    ]);
            }
        }

        public IEnumerator OnTryAlterAttributeCheck(
            GameLocationBattleManager battleManager,
            AbilityCheckData abilityCheckData,
            GameLocationCharacter defender,
            GameLocationCharacter helper)
        {
            var rulesetHelper = helper.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerPool, rulesetHelper);

            if (abilityCheckData.AbilityCheckRoll == 0 ||
                abilityCheckData.AbilityCheckRollOutcome != RollOutcome.Failure ||
                abilityCheckData.AbilityCheckSuccessDelta + MaxDieTypeValue < 0 ||
                rulesetHelper.GetRemainingUsesOfPower(usablePower) == 0 ||
                !helper.CanReact() ||
                defender.IsOppositeSide(helper.Side) ||
                !helper.IsWithinRange(defender, 6) ||
                !helper.CanPerceiveTarget(defender))
            {
                yield break;
            }

            // any reaction within an attribute check flow must use the yielder as waiter
            yield return helper.MyReactToSpendPower(
                usablePower,
                helper,
                "WealCosmosOmenCheck",
                "SpendPowerWealCosmosOmenCheckDescription".Formatted(
                    Category.Reaction, defender.Name, helper.Name),
                ReactionValidated,
                battleManager);

            yield break;

            void ReactionValidated()
            {
                helper.SpendActionType(ActionDefinitions.ActionType.Reaction);

                var dieRoll = rulesetHelper.RollDie(DieType, RollContext.None, false, AdvantageType.None, out _, out _);
                var abilityCheckModifier = abilityCheckData.AbilityCheckActionModifier;

                abilityCheckModifier.AbilityCheckModifierTrends.Add(
                    new TrendInfo(dieRoll, FeatureSourceType.Power, powerWeal.Name, powerWeal)
                    {
                        dieType = DieType.D4, dieFlag = TrendInfoDieFlag.None
                    });
                abilityCheckModifier.AbilityCheckModifier += dieRoll;
                abilityCheckData.AbilityCheckSuccessDelta += dieRoll;

                (ConsoleStyleDuplet.ParameterType, string) extra;

                if (abilityCheckData.AbilityCheckSuccessDelta >= 0)
                {
                    abilityCheckData.AbilityCheckRollOutcome = RollOutcome.Success;
                    extra = (ConsoleStyleDuplet.ParameterType.Positive, "Feedback/&RollCheckSuccessTitle");
                }
                else
                {
                    abilityCheckData.AbilityCheckRollOutcome = RollOutcome.Failure;
                    extra = (ConsoleStyleDuplet.ParameterType.Negative, "Feedback/&RollCheckFailureTitle");
                }

                helper.RulesetCharacter.LogCharacterUsedPower(
                    powerWeal,
                    "Feedback/&CosmosOmenCheckToHitRoll",
                    extra:
                    [
                        (ConsoleStyleDuplet.ParameterType.Positive, dieRoll.ToString()),
                        extra
                    ]);
            }
        }

        public IEnumerator OnTryAlterOutcomeSavingThrow(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            SavingThrowData savingThrowData,
            bool hasHitVisual)
        {
            var rulesetHelper = helper.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerPool, rulesetHelper);

            if (savingThrowData.SaveOutcomeDelta + MaxDieTypeValue < 0 ||
                !helper.CanReact() ||
                helper.IsOppositeSide(defender.Side) ||
                !helper.IsWithinRange(defender, 6) ||
                !helper.CanPerceiveTarget(defender) ||
                rulesetHelper.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            // any reaction within a saving flow must use the yielder as waiter
            yield return helper.MyReactToSpendPower(
                usablePower,
                helper,
                "WealCosmosOmenSaving",
                "SpendPowerWealCosmosOmenSavingDescription".Formatted(
                    Category.Reaction, defender.Name, attacker?.Name ?? ReactionRequestCustom.EnvTitle,
                    savingThrowData.Title),
                ReactionValidated,
                battleManager);

            yield break;

            void ReactionValidated()
            {
                helper.SpendActionType(ActionDefinitions.ActionType.Reaction);

                var dieRoll = rulesetHelper.RollDie(DieType, RollContext.None, false, AdvantageType.None, out _, out _);

                savingThrowData.SaveOutcomeDelta += dieRoll;

                (ConsoleStyleDuplet.ParameterType, string) extra;

                if (savingThrowData.SaveOutcomeDelta >= 0)
                {
                    savingThrowData.SaveOutcome = RollOutcome.Success;
                    extra = (ConsoleStyleDuplet.ParameterType.Positive, "Feedback/&RollCheckSuccessTitle");
                }
                else
                {
                    savingThrowData.SaveOutcome = RollOutcome.Failure;
                    extra = (ConsoleStyleDuplet.ParameterType.Negative, "Feedback/&RollCheckFailureTitle");
                }

                helper.RulesetCharacter.LogCharacterUsedPower(
                    powerWeal,
                    "Feedback/&CosmosOmenSavingToHitRoll",
                    extra:
                    [
                        (ConsoleStyleDuplet.ParameterType.Positive, dieRoll.ToString()),
                        extra
                    ]);
            }
        }
    }

    //
    // Woe
    //

    private sealed class TryAlterOutcomeSavingThrowWoe(
        FeatureDefinitionPower powerPool,
        FeatureDefinitionPower powerWoe)
        : ITryAlterOutcomeAttack, ITryAlterOutcomeAttributeCheck, ITryAlterOutcomeSavingThrow
    {
        private const DieType DieType = RuleDefinitions.DieType.D6;
        private static readonly int MaxDieTypeValue = DiceMaxValue[(int)DieType];

        public int HandlerPriority => -10;

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            var rulesetHelper = helper.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerPool, rulesetHelper);

            if (action.AttackRollOutcome != RollOutcome.Success ||
                action.AttackSuccessDelta - MaxDieTypeValue >= 0 ||
                !helper.CanReact() ||
                !helper.IsOppositeSide(attacker.Side) ||
                !helper.IsWithinRange(attacker, 6) ||
                !helper.CanPerceiveTarget(attacker) ||
                rulesetHelper.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            // any reaction within an attack flow must use the attacker as waiter
            yield return helper.MyReactToSpendPower(
                usablePower,
                attacker,
                "WoeCosmosOmenAttack",
                "SpendPowerWoeCosmosOmenAttackDescription".Formatted(
                    Category.Reaction, attacker.Name, defender.Name, helper.Name),
                ReactionValidated,
                battleManager);

            yield break;

            void ReactionValidated()
            {
                helper.SpendActionType(ActionDefinitions.ActionType.Reaction);

                var dieRoll =
                    -rulesetHelper.RollDie(DieType, RollContext.None, false, AdvantageType.None, out _, out _);

                attackModifier.AttacktoHitTrends.Add(
                    new TrendInfo(dieRoll, FeatureSourceType.Power, powerWoe.Name, powerWoe)
                    {
                        dieType = DieType, dieFlag = TrendInfoDieFlag.None
                    });

                action.AttackSuccessDelta += dieRoll;
                attackModifier.AttackRollModifier += dieRoll;

                if (action.AttackSuccessDelta < 0)
                {
                    action.AttackRollOutcome = RollOutcome.Failure;
                }

                var rulesetCharacter = helper.RulesetCharacter;

                rulesetCharacter.LogCharacterUsedPower(
                    powerWoe,
                    "Feedback/&CosmosOmenAttackToHitRoll",
                    extra:
                    [
                        (ConsoleStyleDuplet.ParameterType.Negative, dieRoll.ToString())
                    ]);
            }
        }

        public IEnumerator OnTryAlterAttributeCheck(
            GameLocationBattleManager battleManager,
            AbilityCheckData abilityCheckData,
            GameLocationCharacter defender,
            GameLocationCharacter helper)
        {
            var rulesetHelper = helper.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerPool, rulesetHelper);

            if (abilityCheckData.AbilityCheckRoll == 0 ||
                abilityCheckData.AbilityCheckRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess) ||
                abilityCheckData.AbilityCheckSuccessDelta - MaxDieTypeValue >= 0 ||
                !helper.CanReact() ||
                !helper.IsOppositeSide(defender.Side) ||
                !helper.IsWithinRange(defender, 6) ||
                !helper.CanPerceiveTarget(defender) ||
                rulesetHelper.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            // any reaction within an attribute check flow must use the yielder as waiter
            yield return helper.MyReactToSpendPower(
                usablePower,
                helper,
                "WoeCosmosOmenCheck",
                "SpendPowerWoeCosmosOmenCheckDescription".Formatted(
                    Category.Reaction, defender.Name, helper.Name),
                ReactionValidated,
                battleManager);

            yield break;

            void ReactionValidated()
            {
                helper.SpendActionType(ActionDefinitions.ActionType.Reaction);

                var dieRoll =
                    -rulesetHelper.RollDie(DieType, RollContext.None, false, AdvantageType.None, out _, out _);
                var abilityCheckModifier = abilityCheckData.AbilityCheckActionModifier;

                abilityCheckModifier.AbilityCheckModifierTrends.Add(
                    new TrendInfo(dieRoll, FeatureSourceType.Power, powerWoe.Name, powerWoe)
                    {
                        dieType = DieType.D4, dieFlag = TrendInfoDieFlag.None
                    });
                abilityCheckModifier.AbilityCheckModifier += dieRoll;
                abilityCheckData.AbilityCheckSuccessDelta += dieRoll;

                (ConsoleStyleDuplet.ParameterType, string) extra;

                if (abilityCheckData.AbilityCheckSuccessDelta < 0)
                {
                    abilityCheckData.AbilityCheckRollOutcome = RollOutcome.Failure;
                    extra = (ConsoleStyleDuplet.ParameterType.Negative, "Feedback/&RollCheckFailureTitle");
                }
                else
                {
                    abilityCheckData.AbilityCheckRollOutcome = RollOutcome.Success;
                    extra = (ConsoleStyleDuplet.ParameterType.Positive, "Feedback/&RollCheckSuccessTitle");
                }

                helper.RulesetCharacter.LogCharacterUsedPower(
                    powerWoe,
                    "Feedback/&CosmosOmenCheckToHitRoll",
                    extra:
                    [
                        (ConsoleStyleDuplet.ParameterType.Positive, dieRoll.ToString()),
                        extra
                    ]);
            }
        }

        public IEnumerator OnTryAlterOutcomeSavingThrow(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            SavingThrowData savingThrowData,
            bool hasHitVisual)
        {
            var rulesetHelper = helper.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerPool, rulesetHelper);

            if (savingThrowData.SaveOutcome != RollOutcome.Success ||
                savingThrowData.SaveOutcomeDelta - MaxDieTypeValue >= 0 ||
                !helper.CanReact() ||
                !helper.IsOppositeSide(defender.Side) ||
                !helper.IsWithinRange(defender, 6) ||
                !helper.CanPerceiveTarget(defender) ||
                rulesetHelper.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            // any reaction within a saving flow must use the yielder as waiter
            yield return helper.MyReactToSpendPower(
                usablePower,
                helper,
                "WoeCosmosOmenSaving",
                "SpendPowerWoeCosmosOmenSavingDescription".Formatted(
                    Category.Reaction, defender.Name, attacker?.Name ?? ReactionRequestCustom.EnvTitle,
                    savingThrowData.Title),
                ReactionValidated,
                battleManager);

            yield break;

            void ReactionValidated()
            {
                helper.SpendActionType(ActionDefinitions.ActionType.Reaction);

                var dieRoll =
                    -rulesetHelper.RollDie(DieType, RollContext.None, false, AdvantageType.None, out _, out _);

                savingThrowData.SaveOutcomeDelta += dieRoll;

                (ConsoleStyleDuplet.ParameterType, string) extra;

                if (savingThrowData.SaveOutcomeDelta < 0)
                {
                    savingThrowData.SaveOutcome = RollOutcome.Failure;
                    extra = (ConsoleStyleDuplet.ParameterType.Negative, "Feedback/&RollCheckFailureTitle");
                }
                else
                {
                    savingThrowData.SaveOutcome = RollOutcome.Success;
                    extra = (ConsoleStyleDuplet.ParameterType.Positive, "Feedback/&RollCheckSuccessTitle");
                }

                helper.RulesetCharacter.LogCharacterUsedPower(
                    powerWoe,
                    "Feedback/&CosmosOmenSavingToHitRoll",
                    extra:
                    [
                        (ConsoleStyleDuplet.ParameterType.Positive, dieRoll.ToString()),
                        extra
                    ]);
            }
        }
    }

    //
    // Twinkling Stars
    //

    private sealed class PowerOrSpellFinishedByMeTwinklingStarsRefund(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower magicEffect) : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var usablePower = PowerProvider.Get(magicEffect, rulesetCharacter);

            usablePower.Recharge();

            yield break;
        }
    }

    private sealed class PowerOrSpellFinishedByMeTwinklingStars(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower magicEffect) : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;

            var activeCondition = rulesetCharacter.ConditionsByCategory
                .SelectMany(x => x.Value)
                .FirstOrDefault(x =>
                    ConstellationFormConditions.Contains(x.Name));

            if (activeCondition == null)
            {
                yield break;
            }

            var remainingRounds = activeCondition.RemainingRounds;

            // no need to check for source here as these are all self conditions
            rulesetCharacter.RemoveCondition(activeCondition);

            var implementationService = ServiceRepository.GetService<IRulesetImplementationService>();
            var usablePower = PowerProvider.Get(magicEffect, rulesetCharacter);
            var effectPower = implementationService.InstantiateEffectPower(rulesetCharacter, usablePower, false);

            effectPower.remainingRounds = remainingRounds;

            //TODO: double check if spend power works here
            actingCharacter.MyExecuteActionSpendPower(usablePower, actingCharacter);
        }
    }
}
