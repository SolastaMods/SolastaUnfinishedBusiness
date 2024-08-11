using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public class PatronCelestial : AbstractSubclass
{
    private const string Name = "Celestial";

    public PatronCelestial()
    {
        // LEVEL 01

        // Expanded Spells

        var spellListCelestial = SpellListDefinitionBuilder
            .Create(SpellListDefinitions.SpellListWizard, $"SpellList{Name}")
            .SetGuiPresentationNoContent(true)
            .ClearSpells()
            .SetSpellsAtLevel(1, CureWounds, GuidingBolt)
            .SetSpellsAtLevel(2, FlamingSphere, LesserRestoration)
            .SetSpellsAtLevel(3, Daylight, Revivify)
            .SetSpellsAtLevel(4, GuardianOfFaith, WallOfFire)
            .SetSpellsAtLevel(5, FlameStrike, GreaterRestoration)
            .FinalizeSpells(true, 9)
            .AddToDB();

        var magicAffinityExpandedSpells = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}ExpandedSpells")
            .SetGuiPresentation("MagicAffinityPatronExpandedSpells", Category.Feature)
            .SetExtendedSpellList(spellListCelestial)
            .AddToDB();

        // Bonus Cantrips

        var bonusCantripCelestial = FeatureDefinitionBonusCantripsBuilder
            .Create($"BonusCantrip{Name}")
            .SetGuiPresentation(Category.Feature)
            .SetBonusCantrips(Light, SacredFlame)
            .AddToDB();

        // Healing Light

        var powerHealingLight = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}HealingLight")
            .SetGuiPresentation(Category.Feature, PowerDomainLifePreserveLife)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetHealingForm(HealingComputation.Dice, 0, DieType.D6, 0, false,
                                HealingCap.MaximumHitPoints)
                            .Build())
                    .Build())
            .AddToDB();

        powerHealingLight.AddCustomSubFeatures(
            HasModifiedUses.Marker,
            new ModifyPowerPoolAmount
            {
                PowerPool = powerHealingLight,
                Type = PowerPoolBonusCalculationType.ClassLevel,
                Attribute = WarlockClass
            });

        var healingLightPowers = new List<FeatureDefinitionPower>();

        for (var i = 6; i >= 1; i--)
        {
            // closure
            var j = i;

            var power = FeatureDefinitionPowerSharedPoolBuilder
                .Create($"PowerSharedPool{Name}HealingLight{i}")
                .SetGuiPresentation(
                    $"PowerSharedPool{Name}HealingLightTitle".Formatted(Category.Feature, i),
                    $"PowerSharedPool{Name}HealingLightDescription".Formatted(Category.Feature, i))
                .SetSharedPool(ActivationTime.BonusAction, powerHealingLight, i)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create()
                        .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.IndividualsUnique)
                        .SetEffectForms(
                            EffectFormBuilder
                                .Create()
                                .SetHealingForm(
                                    HealingComputation.Dice, 0, DieType.D6, i, false, HealingCap.MaximumHitPoints)
                                .Build())
                        .SetParticleEffectParameters(PowerDomainLifePreserveLife)
                        .Build())
                .AddCustomSubFeatures(
                    new ValidatorsValidatePowerUse(c =>
                        AttributeDefinitions.ComputeAbilityScoreModifier(
                            c.TryGetAttributeValue(AttributeDefinitions.Charisma)) >= j))
                .AddToDB();

            power.GuiPresentation.hidden = true;

            healingLightPowers.Add(power);
        }

        PowerBundle.RegisterPowerBundle(powerHealingLight, false, healingLightPowers.ToArray());

        var featureSetHealingLight = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}HealingLight")
            .SetGuiPresentation($"Power{Name}HealingLight", Category.Feature)
            .AddFeatureSet()
            .AddFeatureSet(healingLightPowers.OfType<FeatureDefinition>().ToArray())
            .AddFeatureSet(powerHealingLight)
            .AddToDB();

        // LEVEL 06

        // Radiant Soul

        var featureRadiantSoul = FeatureDefinitionBuilder
            .Create($"Feature{Name}RadiantSoul")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(new MagicEffectBeforeHitConfirmedOnEnemyRadiantSoul())
            .AddToDB();

        var featureSetRadiantSoul = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}RadiantSoul")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(featureRadiantSoul, DamageAffinityRadiantResistance)
            .AddToDB();

        // LEVEL 10

        // Celestial Resistance

        const string CelestialResistanceName = $"Power{Name}CelestialResistance";

        RestActivityDefinitionBuilder
            .Create($"RestActivity{Name}CelestialResistanceShortRest")
            .SetGuiPresentation(CelestialResistanceName, Category.Feature)
            .AddCustomSubFeatures(new ValidateRestActivity(false, false))
            .SetRestData(
                RestDefinitions.RestStage.AfterRest,
                RestType.ShortRest,
                RestActivityDefinition.ActivityCondition.CanUsePower,
                PowerBundleContext.UseCustomRestPowerFunctorName,
                CelestialResistanceName)
            .AddToDB();

        RestActivityDefinitionBuilder
            .Create($"RestActivity{Name}CelestialResistanceLongRest")
            .SetGuiPresentation(CelestialResistanceName, Category.Feature)
            .AddCustomSubFeatures(new ValidateRestActivity(false, false))
            .SetRestData(
                RestDefinitions.RestStage.AfterRest,
                RestType.LongRest,
                RestActivityDefinition.ActivityCondition.CanUsePower,
                PowerBundleContext.UseCustomRestPowerFunctorName,
                CelestialResistanceName)
            .AddToDB();

        var powerCelestialResistance = FeatureDefinitionPowerBuilder
            .Create(CelestialResistanceName)
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Rest, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .Build())
            .AddCustomSubFeatures(
                ModifyPowerVisibility.Hidden,
                new PowerOrSpellFinishedByMeCelestialResistance())
            .AddToDB();

        // LEVEL 14

        // Searing Vengeance

        var conditionBlindedBySearingVengeance = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionBlinded, "ConditionBlindedBySearingVengeance")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetParentCondition(ConditionDefinitions.ConditionBlinded)
            .SetFeatures()
            .AddToDB();

        conditionBlindedBySearingVengeance.GuiPresentation.description = "Rules/&ConditionBlindedDescription";

        var powerSearingVengeance = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}SearingVengeance")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.LongRest)
            .SetExplicitAbilityScore(AttributeDefinitions.Charisma)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 0, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeRadiant, 2, DieType.D8)
                            .SetBonusMode(AddBonusMode.AbilityBonus)
                            .Build(),
                        EffectFormBuilder.ConditionForm(conditionBlindedBySearingVengeance))
                    .SetParticleEffectParameters(PowerDomainSunHeraldOfTheSun)
                    .SetCasterEffectParameters(HolyAura.EffectDescription.EffectParticleParameters
                        .effectParticleReference)
                    .Build())
            .AddToDB();

        powerSearingVengeance.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new OnReducedToZeroHpByEnemySearingVengeance(powerSearingVengeance));

        //
        // Main
        //

        Subclass = CharacterSubclassDefinitionBuilder
            .Create($"Patron{Name}")
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PatronCelestial, 256))
            .AddFeaturesAtLevel(1, magicAffinityExpandedSpells, bonusCantripCelestial, featureSetHealingLight)
            .AddFeaturesAtLevel(6, featureSetRadiantSoul)
            .AddFeaturesAtLevel(10, powerCelestialResistance)
            .AddFeaturesAtLevel(14, powerSearingVengeance)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Warlock;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWarlockOtherworldlyPatrons;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Radiant Soul
    //

    private sealed class MagicEffectBeforeHitConfirmedOnEnemyRadiantSoul : IMagicEffectBeforeHitConfirmedOnEnemy
    {
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            if (!firstTarget)
            {
                yield break;
            }

            var effectForm = actualEffectForms
                .FirstOrDefault(x =>
                    x.FormType == EffectForm.EffectFormType.Damage
                    && x.DamageForm.DamageType is DamageTypeFire or DamageTypeRadiant);

            if (effectForm == null)
            {
                yield break;
            }

            var charismaModifier = AttributeDefinitions.ComputeAbilityScoreModifier(
                attacker.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Charisma));

            effectForm.DamageForm.BonusDamage += charismaModifier;
        }
    }

    //
    // Celestial Resistance
    //

    private sealed class PowerOrSpellFinishedByMeCelestialResistance : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var characterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var allies =
                characterService.PartyCharacters.Union(characterService.GuestCharacters);
            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;
            var classLevel = rulesetCharacter.GetClassLevel(CharacterClassDefinitions.Warlock);
            var charismaModifier = AttributeDefinitions.ComputeAbilityScoreModifier(
                rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Charisma));

            foreach (var ally in allies)
            {
                var rulesetAlly = ally.RulesetCharacter;
                var tempHitPoints = (classLevel / (rulesetAlly == rulesetCharacter ? 1 : 2)) + charismaModifier;

                rulesetAlly.ReceiveTemporaryHitPoints(
                    tempHitPoints, DurationType.UntilAnyRest, 0, TurnOccurenceType.StartOfTurn, rulesetCharacter.Guid);
            }

            yield break;
        }
    }

    //
    // Searing Vengeance
    //

    private sealed class OnReducedToZeroHpByEnemySearingVengeance(FeatureDefinitionPower powerSearingVengeance)
        : IOnReducedToZeroHpByEnemy
    {
        public IEnumerator HandleReducedToZeroHpByEnemy(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            var rulesetCharacter = defender.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerSearingVengeance, rulesetCharacter);

            if (Gui.Battle == null ||
                rulesetCharacter.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            var targets = Gui.Battle.GetContenders(defender, withinRange: 5);

            yield return defender.MyReactToUsePower(
                ActionDefinitions.Id.PowerNoCost,
                usablePower,
                targets,
                attacker,
                "SearingVengeance",
                reactionValidated: ReactionValidated);

            yield break;

            void ReactionValidated()
            {
                var hitPoints = rulesetCharacter.MissingHitPoints / 2;

                defender.MyExecuteActionStabilizeAndStandUp(hitPoints);
            }
        }
    }
}
