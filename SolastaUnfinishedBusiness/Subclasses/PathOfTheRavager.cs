using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class PathOfTheRavager : AbstractSubclass
{
    private const string Name = "PathOfTheRavager";

    public PathOfTheRavager()
    {
        // Frenzy

        var additionalDamageBrutalStrike =
            FeatureDefinitionAdditionalDamageBuilder
                .Create($"AdditionalDamage{Name}Frenzy")
                .SetGuiPresentation(Category.Feature)
                .SetNotificationTag("Frenzy")
                .SetDamageDice(DieType.D6, 2)
                .SetAdvancement(AdditionalDamageAdvancement.ClassLevel,
                    DiceByRankBuilder.InterpolateDiceByRankTable(1, 20, (3, 2), (9, 3), (16, 4)))
                .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
                .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
                .AddCustomSubFeatures(
                    // ClassHolder.Barbarian,
                    new ValidateContextInsteadOfRestrictedProperty((_, _, character, _, _, mode, _) => (
                        OperationType.Set,
                        mode is { AbilityScore: AttributeDefinitions.Strength } &&
                        character.HasConditionOfTypeOrSubType(ConditionRaging) &&
                        character.HasConditionOfCategoryAndType(
                            AttributeDefinitions.TagCombat, ConditionReckless))))
                .AddToDB();

        // Intimidating Presence

        var powerIntimidatingPresence = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}IntimidatingPresence")
            .SetGuiPresentation($"FeatureSet{Name}IntimidatingPresence", Category.Feature,
                Sprites.GetSprite("IntimidatingPresence", Resources.PowerDreadfulPresence, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Strength, 8)
                    .SetParticleEffectParameters(PowerBerserkerIntimidatingPresence)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .SetConditionForm(
                                ConditionDefinitions.ConditionFrightened,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        powerIntimidatingPresence.AddCustomSubFeatures(
            new ValidatorsValidatePowerUse(c =>
                c.HasConditionOfTypeOrSubType(ConditionRaging) &&
                c.GetRemainingPowerUses(powerIntimidatingPresence) > 0));

        var powerIntimidatingPresenceRageCost = FeatureDefinitionPowerBuilder
            .Create(powerIntimidatingPresence, $"Power{Name}IntimidatingPresenceRageCost")
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.RagePoints)
            .AddToDB();

        powerIntimidatingPresenceRageCost.AddCustomSubFeatures(
            new ValidatorsValidatePowerUse(c =>
                c.HasConditionOfTypeOrSubType(ConditionRaging) &&
                c.GetRemainingPowerUses(powerIntimidatingPresence) == 0),
            new MagicEffectFinishedByMeIntimidatingPresence(
                powerIntimidatingPresence, powerIntimidatingPresenceRageCost));

        var featureSetIntimidatingPresence = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}IntimidatingPresence")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                powerIntimidatingPresence, powerIntimidatingPresenceRageCost)
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PathOfTheRavager, 256))
            .AddFeaturesAtLevel(3, additionalDamageBrutalStrike)
            .AddFeaturesAtLevel(6, PowerBerserkerMindlessRage)
            .AddFeaturesAtLevel(10, ActionAffinityRetaliation)
            .AddFeaturesAtLevel(14, featureSetIntimidatingPresence)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Barbarian;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class MagicEffectFinishedByMeIntimidatingPresence(
        FeatureDefinitionPower powerLongRest,
        FeatureDefinitionPower powerRageCost) : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterAction action,
            GameLocationCharacter attacker,
            // ReSharper disable once InconsistentNaming
            List<GameLocationCharacter> _targets)
        {
            if (Gui.Battle == null ||
                action is not CharacterActionUsePower characterActionUsePower ||
                (characterActionUsePower.activePower.PowerDefinition != PowerBarbarianRageStart &&
                 characterActionUsePower.activePower.PowerDefinition.OverriddenPower != PowerBarbarianRageStart))
            {
                yield break;
            }

            var rulesetCharacter = attacker.RulesetCharacter;
            var power = rulesetCharacter.GetRemainingPowerUses(powerLongRest) > 0
                ? powerLongRest
                : rulesetCharacter.GetRemainingPowerUses(powerRageCost) > 0
                    ? powerRageCost
                    : null;

            if (!power)
            {
                yield break;
            }

            var usablePower = PowerProvider.Get(power, rulesetCharacter);
            var targets = Gui.Battle
                .GetContenders(attacker, null, true, withinRange: 6);

            yield return attacker.MyReactToUsePower(
                ActionDefinitions.Id.PowerNoCost,
                usablePower,
                targets,
                attacker,
                "IntimidatingPresence",
                reactionValidated: ReactionValidated);

            yield break;

            void ReactionValidated()
            {
                if (power == powerRageCost)
                {
                    rulesetCharacter.SpendRagePoint();
                }
            }
        }
    }
}
