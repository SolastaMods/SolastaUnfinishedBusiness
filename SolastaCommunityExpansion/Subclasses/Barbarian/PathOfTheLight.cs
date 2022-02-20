using System;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.ConditionDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaCommunityExpansion.Subclasses.Barbarian
{
    internal class PathOfTheLight : AbstractSubclass
    {
        private static readonly Guid SubclassNamespace = new("c2067110-5086-45c0-b0c2-4c140599605c");
        private const string IlluminatedConditionName = "PathOfTheLightIlluminatedCondition";
        private const string IlluminatingStrikeName = "PathOfTheLightIlluminatingStrike";
        private const string IlluminatingBurstName = "PathOfTheLightIlluminatingBurst";

        private static readonly List<ConditionDefinition> InvisibleConditions =
            new()
            {
                ConditionInvisibleBase,
                ConditionInvisible,
                ConditionInvisibleGreater
            };

        private static readonly Dictionary<int, int> LightsProtectionAmountHealedByClassLevel = new()
        {
            { 6, 3 },
            { 7, 3 },
            { 8, 4 },
            { 9, 4 },
            { 10, 5 },
            { 11, 5 },
            { 12, 6 },
            { 13, 6 },
            { 14, 7 },
            { 15, 7 },
            { 16, 8 },
            { 17, 8 },
            { 18, 9 },
            { 19, 9 },
            { 20, 10 }
        };

        internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
        {
            return FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;
        }

        internal override CharacterSubclassDefinition GetSubclass()
        {
            return Subclass;
        }

        private CharacterSubclassDefinition Subclass { get; } = CharacterSubclassDefinitionBuilder
            .Create("PathOfTheLight", SubclassNamespace)
            .SetGuiPresentation("BarbarianPathOfTheLight", Category.Subclass, DomainSun.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(IlluminatingStrike, 3)
            .AddFeatureAtLevel(PierceTheDarkness, 3)
            .AddFeatureAtLevel(LightsProtection, 6)
            .AddFeatureAtLevel(EyesOfTruth, 10)
            .AddFeatureAtLevel(IlluminatingStrikeImprovement, 10)
            .AddFeatureAtLevel(IlluminatingBurst, 14)
            .AddToDB();

        private static string CreateNamespacedGuid(string featureName)
        {
            return GuidHelper.Create(SubclassNamespace, featureName).ToString();
        }

        // TODO: convert to lazy loading?
        private static IlluminatedConditionDefinition IlluminatedCondition { get; } = IlluminatedConditionDefinitionBuilder
            .Create(IlluminatedConditionName, SubclassNamespace)
            .SetGuiPresentation("BarbarianPathOfTheLightIlluminatedCondition", Category.Subclass, ConditionBranded.GuiPresentation.SpriteReference)
            .Configure(
                definition =>
                {
                    definition
                        .SetAllowMultipleInstances(true)
                        .SetConditionType(RuleDefinitions.ConditionType.Detrimental)
                        .SetDurationType(RuleDefinitions.DurationType.Irrelevant)
                        .SetSilentWhenAdded(true)
                        .SetSilentWhenRemoved(false)
                        .SetSpecialDuration(true);

                    definition.Features.Add(DisadvantageAgainstNonSource);
                    definition.Features.Add(PreventInvisibility);
                })
            .AddToDB();

        // TODO: convert to lazy loading?
        private static FeatureDefinition IlluminatingStrike { get; } = FeatureDefinitionFeatureSetBuilder
            .Create("PathOfTheLightIlluminatingStrikeFeatureSet", SubclassNamespace)
            .SetGuiPresentation("BarbarianPathOfTheLightIlluminatingStrike", Category.Subclass)
            .Configure(
                featureSetDefinition =>
                {
                    featureSetDefinition
                        .SetEnumerateInDescription(false)
                        .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                        .SetUniqueChoices(false);

                    var illuminatingStrikeInitiatorBuilder = new IlluminatingStrikeInitiatorBuilder(
                        "PathOfTheLightIlluminatingStrikeInitiator",
                        CreateNamespacedGuid("PathOfTheLightIlluminatingStrikeInitiator"),
                        "Feature/&NoContentTitle",
                        "Feature/&NoContentTitle",
                        IlluminatedCondition);

                    featureSetDefinition.FeatureSet.Add(illuminatingStrikeInitiatorBuilder.AddToDB());
                })
            .AddToDB();

        // Dummy feature to show in UI
        // TODO: convert to lazy loading?
        private static FeatureDefinition IlluminatingStrikeImprovement { get; } = FeatureDefinitionBuilder
            .Create("PathOfTheLightIlluminatingStrikeImprovement", SubclassNamespace)
            .SetGuiPresentation("BarbarianPathOfTheLightIlluminatingStrikeImprovement", Category.Subclass)
            .AddToDB();

        // TODO: convert to lazy loading?
        private static FeatureDefinition PierceTheDarkness { get; } = FeatureDefinitionFeatureSetBuilder
            .Create("PathOfTheLightPierceTheDarkness", SubclassNamespace)
            .SetGuiPresentation("BarbarianPathOfTheLightPierceTheDarkness", Category.Subclass)
            .Configure(
                featureSetDefinition =>
                {
                    featureSetDefinition
                        .SetEnumerateInDescription(false)
                        .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                        .SetUniqueChoices(false);

                    featureSetDefinition.FeatureSet.Add(FeatureDefinitionSenses.SenseSuperiorDarkvision);
                })
            .AddToDB();

        // TODO: convert to lazy loading?
        private static FeatureDefinition LightsProtection { get; } = FeatureDefinitionFeatureSetBuilder
            .Create("PathOfTheLightLightsProtection", SubclassNamespace)
            .SetGuiPresentation("BarbarianPathOfTheLightLightsProtection", Category.Subclass)
            .Configure(
                definition =>
                {
                    definition
                        .SetEnumerateInDescription(false)
                        .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                        .SetUniqueChoices(false);

                    var conditionalOpportunityAttackImmunity = FeatureDefinitionOpportunityAttackImmunityIfAttackerHasConditionBuilder
                        .Create("PathOfTheLightLightsProtectionOpportunityAttackImmunity", SubclassNamespace)
                        .SetGuiPresentationNoContent()
                        .SetConditionName(IlluminatedConditionName)
                        .AddToDB();

                    definition.FeatureSet.Add(conditionalOpportunityAttackImmunity);
                })
            .AddToDB();

        private static void ApplyLightsProtectionHealing(ulong sourceGuid)
        {
            if (RulesetEntity.GetEntity<RulesetCharacter>(sourceGuid) is not RulesetCharacterHero conditionSource || conditionSource.IsDead)
            {
                return;
            }

            if (!conditionSource.ClassesAndLevels.TryGetValue(CharacterClassDefinitions.Barbarian, out int levelsInClass))
            {
                // Character doesn't have levels in class
                return;
            }

            if (!LightsProtectionAmountHealedByClassLevel.TryGetValue(levelsInClass, out int amountHealed))
            {
                // Character doesn't heal at the current level
                return;
            }

            if (amountHealed > 0)
            {
                conditionSource.ReceiveHealing(amountHealed, notify: true, sourceGuid);
            }
        }

        // TODO: convert to lazy loading?
        private static FeatureDefinition EyesOfTruth { get; } = CreateEyesOfTruth();

        private static FeatureDefinition CreateEyesOfTruth()
        {
            var seeingInvisibleCondition = ConditionDefinitionBuilder
                .Create("PathOfTheLightEyesOfTruthSeeingInvisible", SubclassNamespace)
                .SetGuiPresentation("BarbarianPathOfTheLightSeeingInvisibleCondition", Category.Subclass, ConditionSeeInvisibility.GuiPresentation.SpriteReference)
                .Configure(
                    definition =>
                    {
                        definition
                            .SetAllowMultipleInstances(false)
                            .SetConditionType(RuleDefinitions.ConditionType.Beneficial)
                            .SetDurationType(RuleDefinitions.DurationType.Permanent)
                            .SetSilentWhenAdded(true)
                            .SetSilentWhenRemoved(true);

                        definition.Features.Add(FeatureDefinitionSenses.SenseSeeInvisible16);
                    })
                .AddToDB();

            var seeInvisibleEffectBuilder = new EffectDescriptionBuilder();

            var seeInvisibleConditionForm = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Condition,
                ConditionForm = new ConditionForm
                {
                    Operation = ConditionForm.ConditionOperation.Add,
                    ConditionDefinition = seeingInvisibleCondition
                }
            };

            seeInvisibleEffectBuilder
                .SetDurationData(RuleDefinitions.DurationType.Permanent, 1, RuleDefinitions.TurnOccurenceType.StartOfTurn)
                .SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1, RuleDefinitions.TargetType.Self, 1, 0, ActionDefinitions.ItemSelectionType.None)
                .AddEffectForm(seeInvisibleConditionForm);

            var seeInvisiblePower = FeatureDefinitionPowerBuilder
                .Create("PathOfTheLightEyesOfTruthPower", SubclassNamespace)
                .SetGuiPresentation("BarbarianPathOfTheLightEyesOfTruth", Category.Subclass, SpellDefinitions.SeeInvisibility.GuiPresentation.SpriteReference)
                .SetShowCasting(false)
                .SetEffectDescription(seeInvisibleEffectBuilder.Build())
                .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
                .Configure(
                    definition =>
                    {
                        // TODO: builder has version of this which also sets cost per use
                        definition
                            .SetActivationTime(RuleDefinitions.ActivationTime.Permanent);
                    })
                .AddToDB();

            return FeatureDefinitionFeatureSetBuilder
                .Create("PathOfTheLightEyesOfTruth", SubclassNamespace)
                .SetGuiPresentation("BarbarianPathOfTheLightEyesOfTruth", Category.Subclass)
                .Configure(
                    definition =>
                    {
                        definition
                            .SetEnumerateInDescription(false)
                            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                            .SetUniqueChoices(false);

                        definition.FeatureSet.Add(seeInvisiblePower);
                    })
                .AddToDB();
        }

        // TODO: convert to lazy loading?
        private static FeatureDefinition IlluminatingBurst { get; } = FeatureDefinitionFeatureSetBuilder
            .Create("PathOfTheLightIlluminatingBurstFeatureSet", SubclassNamespace)
            .SetGuiPresentation("BarbarianPathOfTheLightIlluminatingBurst", Category.Subclass)
            .Configure(
                definition =>
                {
                    definition
                        .SetEnumerateInDescription(false)
                        .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                        .SetUniqueChoices(false);

                    var illuminatingBurstBuilder = new IlluminatingBurstInitiatorBuilder(
                        "PathOfTheLightIlluminatingBurstInitiator",
                        CreateNamespacedGuid("PathOfTheLightIlluminatingBurstInitiator"),
                        "Feature/&NoContentTitle",
                        "Feature/&NoContentTitle",
                        IlluminatingBurstSuppressedCondition);

                    definition.FeatureSet.Add(illuminatingBurstBuilder.AddToDB());

                    var illuminatingBurstPowerBuilder = new IlluminatingBurstPowerBuilder(
                        IlluminatingBurstName,
                        CreateNamespacedGuid(IlluminatingBurstName),
                        "Subclass/&BarbarianPathOfTheLightIlluminatingBurstPowerTitle",
                        "Subclass/&BarbarianPathOfTheLightIlluminatingBurstPowerDescription",
                        IlluminatedCondition,
                        IlluminatingBurstSuppressedCondition);

                    definition.FeatureSet.Add(illuminatingBurstPowerBuilder.AddToDB());

                    definition.FeatureSet.Add(CreateIlluminatingBurstSuppressor());
                })
            .AddToDB();

        private static FeatureDefinition CreateIlluminatingBurstSuppressor()
        {
            return FeatureDefinitionPowerBuilder
                .Create("PathOfTheLightIlluminatingBurstSuppressor", SubclassNamespace)
                .SetGuiPresentationNoContent(true)
                .Configure(
                    definition =>
                    {
                        // TODO: move into builder
                        var suppressIlluminatingBurst = new EffectForm
                        {
                            FormType = EffectForm.EffectFormType.Condition,
                            ConditionForm = new ConditionForm
                            {
                                Operation = ConditionForm.ConditionOperation.Add,
                                ConditionDefinition = IlluminatingBurstSuppressedCondition
                            }
                        };

                        var effectDescriptionBuilder = new EffectDescriptionBuilder();

                        effectDescriptionBuilder
                            .SetDurationData(RuleDefinitions.DurationType.Permanent, 1, RuleDefinitions.TurnOccurenceType.StartOfTurn)
                            .SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1, RuleDefinitions.TargetType.Self, 1, 0, ActionDefinitions.ItemSelectionType.None)
                            .SetRecurrentEffect(RuleDefinitions.RecurrentEffect.OnActivation | RuleDefinitions.RecurrentEffect.OnTurnStart)
                            .AddEffectForm(suppressIlluminatingBurst);

                        definition
                            .SetActivationTime(RuleDefinitions.ActivationTime.Permanent)
                            .SetEffectDescription(effectDescriptionBuilder.Build())
                            .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill);
                    })
                .AddToDB();
        }

        // TODO: convert to lazy loading?
        private static FeatureDefinitionAttackDisadvantageAgainstNonSource DisadvantageAgainstNonSource { get; } =
            FeatureDefinitionAttackDisadvantageAgainstNonSourceBuilder
                .Create("PathOfTheLightIlluminatedDisadvantage", SubclassNamespace)
                .SetGuiPresentation("Feature/&NoContentTitle", "Subclass/&BarbarianPathOfTheLightIlluminatedDisadvantageDescription")
                .SetConditionName(IlluminatedConditionName)
                .AddToDB();

        // Prevents a creature from turning invisible by "granting" immunity to invisibility
        // TODO: convert to lazy loading?
        private static FeatureDefinition PreventInvisibility { get; } = FeatureDefinitionFeatureSetBuilder
            .Create("PathOfTheLightIlluminatedPreventInvisibility", SubclassNamespace)
            .SetGuiPresentation("Feature/&NoContentTitle", "Subclass/&BarbarianPathOfTheLightIlluminatedPreventInvisibilityDescription")
            .Configure(
                featureSetDefinition =>
                {
                    featureSetDefinition
                        .SetEnumerateInDescription(false)
                        .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                        .SetUniqueChoices(false);

                    foreach (var invisibleConditionName in InvisibleConditions.Select(ic => ic.Name))
                    {
                        var preventInvisibilitySubFeature = FeatureDefinitionConditionAffinityBuilder
                            .Create("PathOfTheLightIlluminatedPreventInvisibility" + invisibleConditionName, SubclassNamespace)
                            .SetGuiPresentationNoContent()
                            .Configure(
                                conditionAffinityDefinition =>
                                {
                                    // TODO: move into builder
                                    conditionAffinityDefinition
                                        .SetConditionAffinityType(RuleDefinitions.ConditionAffinityType.Immunity)
                                        .SetConditionType(invisibleConditionName);
                                })
                            .AddToDB();

                        featureSetDefinition.FeatureSet.Add(preventInvisibilitySubFeature);
                    }
                })
            .AddToDB();

        // TODO: convert to lazy loading?
        private static ConditionDefinition IlluminatingBurstSuppressedCondition { get; } = ConditionDefinitionBuilder
            .Create("PathOfTheLightIlluminatingBurstSuppressedCondition", SubclassNamespace)
            .SetGuiPresentationNoContent(true)
            .Configure(
                definition =>
                {
                    definition
                        .SetAllowMultipleInstances(false)
                        .SetConditionType(RuleDefinitions.ConditionType.Neutral)
                        .SetDurationType(RuleDefinitions.DurationType.Permanent)
                        .SetSilentWhenAdded(true)
                        .SetSilentWhenRemoved(true);
                })
            .AddToDB();

        private static void HandleAfterIlluminatedConditionRemoved(RulesetActor removedFrom)
        {
            if (removedFrom is not RulesetCharacter character)
            {
                return;
            }

            // Intentionally *includes* conditions that have Illuminated as their parent (like the Illuminating Burst condition)
            if (!character.HasConditionOfTypeOrSubType(IlluminatedConditionName)
                && (character.PersonalLightSource?.SourceName == IlluminatingStrikeName || character.PersonalLightSource?.SourceName == IlluminatingBurstName))
            {
                var visibilityService = ServiceRepository.GetService<IGameLocationVisibilityService>();

                visibilityService.RemoveCharacterLightSource(GameLocationCharacter.GetFromActor(removedFrom), character.PersonalLightSource);
                character.PersonalLightSource = null;
            }
        }

        // Helper classes

        private sealed class IlluminatedConditionDefinition : ConditionDefinition, IConditionRemovedOnSourceTurnStart, INotifyConditionRemoval
        {
            public void AfterConditionRemoved(RulesetActor removedFrom, RulesetCondition rulesetCondition)
            {
                HandleAfterIlluminatedConditionRemoved(removedFrom);
            }

            public void BeforeDyingWithCondition(RulesetActor rulesetActor, RulesetCondition rulesetCondition)
            {
                ApplyLightsProtectionHealing(rulesetCondition.SourceGuid);
            }
        }

        private sealed class IlluminatedConditionDefinitionBuilder
            : ConditionDefinitionBuilder<IlluminatedConditionDefinition, IlluminatedConditionDefinitionBuilder>
        {
            private IlluminatedConditionDefinitionBuilder(string name, Guid guidNamespace) : base(name, guidNamespace) { }

            public static IlluminatedConditionDefinitionBuilder Create(string name, Guid guidNamespace)
            {
                return new IlluminatedConditionDefinitionBuilder(name, guidNamespace);
            }
        }

        private sealed class IlluminatedByBurstConditionDefinition : ConditionDefinition, INotifyConditionRemoval
        {
            public void AfterConditionRemoved(RulesetActor removedFrom, RulesetCondition rulesetCondition)
            {
                HandleAfterIlluminatedConditionRemoved(removedFrom);
            }

            public void BeforeDyingWithCondition(RulesetActor rulesetActor, RulesetCondition rulesetCondition)
            {
                ApplyLightsProtectionHealing(rulesetCondition.SourceGuid);
            }
        }

        private sealed class IlluminatedByBurstConditionDefinitionBuilder
            : ConditionDefinitionBuilder<IlluminatedByBurstConditionDefinition, IlluminatedByBurstConditionDefinitionBuilder>
        {
            private IlluminatedByBurstConditionDefinitionBuilder(string name, Guid guidNamespace) : base(name, guidNamespace) { }

            public static IlluminatedByBurstConditionDefinitionBuilder Create(string name, Guid guidNamespace)
            {
                return new IlluminatedByBurstConditionDefinitionBuilder(name, guidNamespace);
            }
        }

        private sealed class IlluminatingStrikeAdditionalDamage : FeatureDefinitionAdditionalDamage, IClassHoldingFeature
        {
            // Allows Illuminating Strike damage to scale with barbarian level
            public CharacterClassDefinition Class => CharacterClassDefinitions.Barbarian;
        }

        private sealed class IlluminatingStrikeFeatureBuilder : FeatureDefinitionAdditionalDamageBuilder<IlluminatingStrikeAdditionalDamage, IlluminatingStrikeFeatureBuilder>
        {
            public IlluminatingStrikeFeatureBuilder(string name, string guid, string title, string description, ConditionDefinition illuminatedCondition) : base(name, guid)
            {
                Definition
                    .SetGuiPresentation(title, description, AdditionalDamageDomainLifeDivineStrike.GuiPresentation.SpriteReference)
                    .SetAdditionalDamageType(RuleDefinitions.AdditionalDamageType.Specific)
                    .SetSpecificDamageType("DamageRadiant")
                    .SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive)
                    .SetDamageValueDetermination(RuleDefinitions.AdditionalDamageValueDetermination.Die)
                    .SetDamageDiceNumber(1)
                    .SetDamageDieType(RuleDefinitions.DieType.D6)
                    .SetDamageSaveAffinity(RuleDefinitions.EffectSavingThrowType.None)
                    .SetDamageAdvancement(RuleDefinitions.AdditionalDamageAdvancement.ClassLevel)
                    .SetLimitedUsage(RuleDefinitions.FeatureLimitedUsage.OnceInMyturn)
                    .SetNotificationTag("BarbarianPathOfTheLightIlluminatingStrike")
                    .SetRequiredProperty(RuleDefinitions.AdditionalDamageRequiredProperty.None)
                    .SetAddLightSource(true)
                    .SetLightSourceForm(CreateIlluminatedLightSource());

                Definition.DiceByRankTable.AddRange(new[]
                {
                    BuildDiceByRank(3, 1),
                    BuildDiceByRank(4, 1),
                    BuildDiceByRank(5, 1),
                    BuildDiceByRank(6, 1),
                    BuildDiceByRank(7, 1),
                    BuildDiceByRank(8, 1),
                    BuildDiceByRank(9, 1),
                    BuildDiceByRank(10, 2),
                    BuildDiceByRank(11, 2),
                    BuildDiceByRank(12, 2),
                    BuildDiceByRank(13, 2),
                    BuildDiceByRank(14, 2),
                    BuildDiceByRank(15, 2),
                    BuildDiceByRank(16, 2),
                    BuildDiceByRank(17, 2),
                    BuildDiceByRank(18, 2),
                    BuildDiceByRank(19, 2),
                    BuildDiceByRank(20, 2)
                });

                Definition.ConditionOperations.Add(
                    new ConditionOperationDescription
                    {
                        Operation = ConditionOperationDescription.ConditionOperation.Add,
                        ConditionDefinition = illuminatedCondition
                    });

                foreach (ConditionDefinition invisibleCondition in InvisibleConditions)
                {
                    Definition.ConditionOperations.Add(
                        new ConditionOperationDescription
                        {
                            Operation = ConditionOperationDescription.ConditionOperation.Remove,
                            ConditionDefinition = invisibleCondition
                        });
                }
            }

            private static LightSourceForm CreateIlluminatedLightSource()
            {
                EffectForm faerieFireLightSource = SpellDefinitions.FaerieFire.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.LightSource);

                var lightSourceForm = new LightSourceForm();
                lightSourceForm.Copy(faerieFireLightSource.LightSourceForm);

                lightSourceForm
                    .SetBrightRange(4)
                    .SetDimAdditionalRange(4);

                return lightSourceForm;
            }

            // Common helper: factor out
            private static DiceByRank BuildDiceByRank(int rank, int dice)
            {
                DiceByRank diceByRank = new DiceByRank();
                diceByRank.SetRank(rank);
                diceByRank.SetDiceNumber(dice);
                return diceByRank;
            }
        }

        /// <summary>
        /// Builds the power that enables Illuminating Strike while you're raging.
        /// </summary>
        private sealed class IlluminatingStrikeInitiatorBuilder : FeatureDefinitionPowerBuilder
        {
            public IlluminatingStrikeInitiatorBuilder(string name, string guid, string title, string description, ConditionDefinition illuminatedCondition) : base(name, guid)
            {
                Definition
                    .SetGuiPresentation(title, description, AdditionalDamageDomainLifeDivineStrike.GuiPresentation.SpriteReference, true)
                    .SetActivationTime(RuleDefinitions.ActivationTime.OnRageStartAutomatic)
                    .SetEffectDescription(CreatePowerEffect(illuminatedCondition))
                    .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
                    .SetShowCasting(false);
            }

            private static EffectDescription CreatePowerEffect(ConditionDefinition illuminatedCondition)
            {
                var initiatorCondition = ConditionDefinitionBuilder
                    .Create("PathOfTheLightIlluminatingStrikeInitiatorCondition", SubclassNamespace)
                    .SetGuiPresentationNoContent(true)
                    .Configure(
                        definition =>
                        {
                            definition
                                .SetAllowMultipleInstances(false)
                                .SetConditionType(RuleDefinitions.ConditionType.Beneficial)
                                .SetDurationType(RuleDefinitions.DurationType.Minute)
                                .SetDurationParameter(1)
                                .SetTerminateWhenRemoved(true)
                                .SetSilentWhenAdded(true)
                                .SetSilentWhenRemoved(true);

                            var illuminatingStrikeFeature = new IlluminatingStrikeFeatureBuilder(
                                IlluminatingStrikeName,
                                CreateNamespacedGuid(IlluminatingStrikeName),
                                "Feature/&NoContentTitle",
                                "Feature/&NoContentTitle",
                                illuminatedCondition);

                            definition.Features.Add(illuminatingStrikeFeature.AddToDB());

                            definition.SpecialInterruptions.SetRange(RuleDefinitions.ConditionInterruption.RageStop);
                        })
                    .AddToDB();

                var enableIlluminatingStrike = new EffectForm
                {
                    FormType = EffectForm.EffectFormType.Condition,
                    ConditionForm = new ConditionForm
                    {
                        Operation = ConditionForm.ConditionOperation.Add,
                        ConditionDefinition = initiatorCondition
                    }
                };

                var effectDescriptionBuilder = new EffectDescriptionBuilder();

                effectDescriptionBuilder
                    .SetDurationData(RuleDefinitions.DurationType.Minute, 1, RuleDefinitions.TurnOccurenceType.StartOfTurn)
                    .AddEffectForm(enableIlluminatingStrike);

                return effectDescriptionBuilder.Build();
            }
        }

        private sealed class IlluminatingBurstPower : FeatureDefinitionPower, IStartOfTurnRecharge
        {
            public bool IsRechargeSilent => true;
        }

        private sealed class IlluminatingBurstPowerBuilder : FeatureDefinitionPowerBuilder<IlluminatingBurstPower, IlluminatingBurstPowerBuilder>
        {
            public IlluminatingBurstPowerBuilder(string name, string guid, string title, string description, ConditionDefinition illuminatedCondition, ConditionDefinition illuminatingBurstSuppressedCondition) : base(name, guid)
            {
                Definition
                    .SetGuiPresentation(title, description, PowerDomainSunHeraldOfTheSun.GuiPresentation.SpriteReference)
                    .SetActivationTime(RuleDefinitions.ActivationTime.NoCost)
                    .SetEffectDescription(CreatePowerEffect(illuminatedCondition))
                    .SetRechargeRate(RuleDefinitions.RechargeRate.OneMinute) // Actually recharges at the start of your turn, using IStartOfTurnRecharge
                    .SetFixedUsesPerRecharge(1)
                    .SetUsesDetermination(RuleDefinitions.UsesDetermination.Fixed)
                    .SetCostPerUse(1)
                    .SetShowCasting(false)
                    .SetDisableIfConditionIsOwned(illuminatingBurstSuppressedCondition); // Only enabled on the turn you enter a rage
            }

            private static EffectDescription CreatePowerEffect(ConditionDefinition illuminatedCondition)
            {
                var effectDescriptionBuilder = new EffectDescriptionBuilder();

                var dealDamage = new EffectForm
                {
                    FormType = EffectForm.EffectFormType.Damage,
                    DamageForm = new DamageForm
                    {
                        DamageType = "DamageRadiant",
                        DiceNumber = 4,
                        DieType = RuleDefinitions.DieType.D6
                    },
                    SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates
                };

                var illuminatedByBurstCondition = IlluminatedByBurstConditionDefinitionBuilder
                    .Create("PathOfTheLightIlluminatedByBurstCondition", SubclassNamespace)
                    .SetGuiPresentation("BarbarianPathOfTheLightIlluminatedCondition", Category.Subclass, ConditionBranded.GuiPresentation.SpriteReference)
                    .Configure(definition =>
                    {
                        definition
                            .SetAllowMultipleInstances(true)
                            .SetConditionType(RuleDefinitions.ConditionType.Detrimental)
                            .SetDurationType(RuleDefinitions.DurationType.Minute)
                            .SetDurationParameter(1)
                            .SetParentCondition(illuminatedCondition)
                            .SetSilentWhenAdded(true)
                            .SetSilentWhenRemoved(false);
                    })
                    .AddToDB();

                var addIlluminatedCondition = new EffectForm
                {
                    FormType = EffectForm.EffectFormType.Condition,
                    ConditionForm = new ConditionForm
                    {
                        Operation = ConditionForm.ConditionOperation.Add,
                        ConditionDefinition = illuminatedByBurstCondition
                    },
                    CanSaveToCancel = true,
                    SaveOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn,
                    SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates
                };

                EffectForm faerieFireLightSource = SpellDefinitions.FaerieFire.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.LightSource);

                var lightSourceForm = new LightSourceForm();
                lightSourceForm.Copy(faerieFireLightSource.LightSourceForm);

                lightSourceForm
                    .SetBrightRange(4)
                    .SetDimAdditionalRange(4);

                var addLightSource = new EffectForm
                {
                    FormType = EffectForm.EffectFormType.LightSource,
                    SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates
                };

                addLightSource.SetLightSourceForm(lightSourceForm);

                effectDescriptionBuilder
                    .SetSavingThrowData(
                        hasSavingThrow: true,
                        disableSavingThrowOnAllies: false,
                        savingThrowAbility: "Constitution",
                        ignoreCover: false,
                        RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                        savingThrowDifficultyAbility: "Constitution",
                        fixedSavingThrowDifficultyClass: 10,
                        advantageForEnemies: false,
                        new List<SaveAffinityBySenseDescription>())
                    .SetDurationData(
                        RuleDefinitions.DurationType.Minute,
                        durationParameter: 1,
                        RuleDefinitions.TurnOccurenceType.EndOfTurn)
                    .SetTargetingData(
                        RuleDefinitions.Side.Enemy,
                        RuleDefinitions.RangeType.Distance,
                        rangeParameter: 6,
                        RuleDefinitions.TargetType.IndividualsUnique,
                        targetParameter: 3,
                        targetParameter2: 0,
                        ActionDefinitions.ItemSelectionType.None)
                    .SetSpeed(
                        RuleDefinitions.SpeedType.CellsPerSeconds,
                        speedParameter: 9.5f)
                    .SetParticleEffectParameters(SpellDefinitions.GuidingBolt.EffectDescription.EffectParticleParameters)
                    .AddEffectForm(dealDamage)
                    .AddEffectForm(addIlluminatedCondition)
                    .AddEffectForm(addLightSource);

                return effectDescriptionBuilder.Build();
            }
        }

        /// <summary>
        /// Builds the power that enables Illuminating Burst on the turn you enter a rage (by removing the condition disabling it).
        /// </summary>
        private sealed class IlluminatingBurstInitiatorBuilder : FeatureDefinitionPowerBuilder
        {
            public IlluminatingBurstInitiatorBuilder(string name, string guid, string title, string description, ConditionDefinition illuminatingBurstSuppressedCondition) : base(name, guid)
            {
                Definition
                    .SetGuiPresentation(CreatePowerGuiPresentation(title, description))
                    .SetActivationTime(RuleDefinitions.ActivationTime.OnRageStartAutomatic)
                    .SetEffectDescription(CreatePowerEffect(illuminatingBurstSuppressedCondition))
                    .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
                    .SetShowCasting(false);
            }

            private static GuiPresentation CreatePowerGuiPresentation(string title, string description)
            {
                var guiPresentationBuilder = new GuiPresentationBuilder(title, description);

                var guiPresentation = guiPresentationBuilder.Build();
                guiPresentation.SetHidden(true);

                return guiPresentation;
            }

            private static EffectDescription CreatePowerEffect(ConditionDefinition illuminatingBurstSuppressedCondition)
            {
                var enableIlluminatingBurst = new EffectForm
                {
                    FormType = EffectForm.EffectFormType.Condition,
                    ConditionForm = new ConditionForm
                    {
                        Operation = ConditionForm.ConditionOperation.Remove,
                        ConditionDefinition = illuminatingBurstSuppressedCondition
                    }
                };

                var effectDescriptionBuilder = new EffectDescriptionBuilder();

                effectDescriptionBuilder
                    .SetDurationData(RuleDefinitions.DurationType.Round, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn)
                    .AddEffectForm(enableIlluminatingBurst);

                return effectDescriptionBuilder.Build();
            }
        }
    }
}
