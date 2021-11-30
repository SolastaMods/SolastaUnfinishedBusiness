using System;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaCommunityExpansion.Features;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Subclasses.Barbarian
{
    internal class PathOfTheLight : AbstractSubclass
    {
        private static readonly Guid SubclassNamespace = new Guid("c2067110-5086-45c0-b0c2-4c140599605c");
        private const string IlluminatedConditionName = "PathOfTheLightIlluminatedCondition";
        private const string IlluminatingStrikeName = "PathOfTheLightIlluminatingStrike";
        private const string IlluminatingBurstName = "PathOfTheLightIlluminatingBurst";

        private static readonly List<ConditionDefinition> InvisibleConditions =
            new List<ConditionDefinition>
            {
                DatabaseHelper.ConditionDefinitions.ConditionInvisibleBase,
                DatabaseHelper.ConditionDefinitions.ConditionInvisible,
                DatabaseHelper.ConditionDefinitions.ConditionInvisibleGreater
            };

        private readonly CharacterSubclassDefinition _subclass;

        private static readonly Dictionary<int, int> LightsProtectionAmountHealedByClassLevel = new Dictionary<int, int>
        {
            {6, 3},
            {7, 3},
            {8, 4},
            {9, 4},
            {10, 5},
            {11, 5},
            {12, 6},
            {13, 6},
            {14, 7},
            {15, 7},
            {16, 8},
            {17, 8},
            {18, 9},
            {19, 9},
            {20, 10}
        };

        internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
        {
            return DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;
        }

        internal override CharacterSubclassDefinition GetSubclass()
        {
            return _subclass;
        }

        internal PathOfTheLight()
        {
            var subclassBuilder = new CharacterSubclassDefinitionBuilder("PathOfTheLight", CreateNamespacedGuid("PathOfTheLight"));

            ConditionDefinition illuminatedCondition = CreateIlluminatedCondition();

            subclassBuilder
                .SetGuiPresentation(CreateSubclassGuiPresentation())
                .AddFeatureAtLevel(CreateIlluminatingStrike(illuminatedCondition), 3)
                .AddFeatureAtLevel(CreatePierceTheDarkness(), 3)
                .AddFeatureAtLevel(CreateLightsProtection(), 6)
                .AddFeatureAtLevel(CreateEyesOfTruth(), 10)
                .AddFeatureAtLevel(CreateIlluminatingStrikeImprovement(), 10)
                .AddFeatureAtLevel(CreateIlluminatingBurst(illuminatedCondition), 14);

            _subclass = subclassBuilder.AddToDB();
        }

        private static string CreateNamespacedGuid(string featureName)
        {
            return GuidHelper.Create(SubclassNamespace, featureName).ToString();
        }

        private static GuiPresentation CreateSubclassGuiPresentation()
        {
            var subclassGuiPresentation = new GuiPresentationBuilder("Subclass/&BarbarianPathOfTheLightDescription", "Subclass/&BarbarianPathOfTheLightTitle");

            subclassGuiPresentation.SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.DomainSun.GuiPresentation.SpriteReference);

            return subclassGuiPresentation.Build();
        }

        private static FeatureDefinition CreateIlluminatingStrike(ConditionDefinition illuminatedCondition)
        {
            var illuminatingStrikeFeatureSet = FeatureDefinitionBuilder<FeatureDefinitionFeatureSet>.Build(
                "PathOfTheLightIlluminatingStrikeFeatureSet",
                CreateNamespacedGuid("PathOfTheLightIlluminatingStrikeFeatureSet"),
                "Subclass/&BarbarianPathOfTheLightIlluminatingStrikeDescription",
                "Subclass/&BarbarianPathOfTheLightIlluminatingStrikeTitle",
                featureSetDefinition =>
                {
                    featureSetDefinition
                        .SetEnumerateInDescription(false)
                        .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                        .SetUniqueChoices(false);

                    featureSetDefinition.SetField("featureSet", new List<FeatureDefinition>());

                    var illuminatingStrikeInitiatorBuilder = new IlluminatingStrikeInitiatorBuilder(
                        "PathOfTheLightIlluminatingStrikeInitiator",
                        CreateNamespacedGuid("PathOfTheLightIlluminatingStrikeInitiator"),
                        "Feature/&NoContentTitle",
                        "Feature/&NoContentTitle",
                        illuminatedCondition);

                    featureSetDefinition.FeatureSet.Add(illuminatingStrikeInitiatorBuilder.AddToDB());
                });

            return illuminatingStrikeFeatureSet;
        }

        private static FeatureDefinition CreateIlluminatingStrikeImprovement()
        {
            // Dummy feature to show in UI

            var illuminatingStrikeImprovement = FeatureDefinitionBuilder<FeatureDefinition>.Build(
                "PathOfTheLightIlluminatingStrikeImprovement",
                CreateNamespacedGuid("PathOfTheLightIlluminatingStrikeImprovement"),
                "Subclass/&BarbarianPathOfTheLightIlluminatingStrikeImprovementDescription",
                "Subclass/&BarbarianPathOfTheLightIlluminatingStrikeImprovementTitle");

            return illuminatingStrikeImprovement;
        }

        private static FeatureDefinition CreatePierceTheDarkness()
        {
            var pierceTheDarkness = FeatureDefinitionBuilder<FeatureDefinitionFeatureSet>.Build(
                "PathOfTheLightPierceTheDarkness",
                CreateNamespacedGuid("PathOfTheLightPierceTheDarkness"),
                "Subclass/&BarbarianPathOfTheLightPierceTheDarknessDescription",
                "Subclass/&BarbarianPathOfTheLightPierceTheDarknessTitle",
                featureSetDefinition =>
                {
                    featureSetDefinition
                        .SetEnumerateInDescription(false)
                        .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                        .SetUniqueChoices(false);

                    featureSetDefinition.SetField("featureSet", new List<FeatureDefinition>());

                    featureSetDefinition.FeatureSet.Add(DatabaseHelper.FeatureDefinitionSenses.SenseSuperiorDarkvision);
                });

            return pierceTheDarkness;
        }

        private static FeatureDefinition CreateLightsProtection()
        {
            var lightsProtection = FeatureDefinitionBuilder<FeatureDefinitionFeatureSet>.Build(
                "PathOfTheLightLightsProtection",
                CreateNamespacedGuid("PathOfTheLightLightsProtection"),
                "Subclass/&BarbarianPathOfTheLightLightsProtectionDescription",
                "Subclass/&BarbarianPathOfTheLightLightsProtectionTitle",
                featureSetDefinition =>
                {
                    featureSetDefinition
                        .SetEnumerateInDescription(false)
                        .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                        .SetUniqueChoices(false);

                    featureSetDefinition.SetField("featureSet", new List<FeatureDefinition>());

                    var conditionalOpportunityAttackImmunity = FeatureDefinitionBuilder<OpportunityAttackImmunityIfAttackerHasCondition>.Build(
                        "PathOfTheLightLightsProtectionOpportunityAttackImmunity",
                        CreateNamespacedGuid("PathOfTheLightLightsProtectionOpportunityAttackImmunity"),
                        "Feature/&NoContentTitle",
                        "Feature/&NoContentTitle",
                        definition =>
                        {
                            definition.ConditionName = IlluminatedConditionName;
                        });

                    featureSetDefinition.FeatureSet.Add(conditionalOpportunityAttackImmunity);
                });

            return lightsProtection;
        }

        private static void ApplyLightsProtectionHealing(ulong sourceGuid)
        {
            var conditionSource = RulesetEntity.GetEntity<RulesetCharacter>(sourceGuid) as RulesetCharacterHero;

            if (conditionSource == null || conditionSource.IsDead)
            {
                return;
            }

            if (!conditionSource.ClassesAndLevels.TryGetValue(DatabaseHelper.CharacterClassDefinitions.Barbarian, out int levelsInClass))
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

        private static FeatureDefinition CreateEyesOfTruth()
        {
            var seeingInvisibleCondition = ConditionDefinitionBuilder.Build(
                "PathOfTheLightEyesOfTruthSeeingInvisible",
                CreateNamespacedGuid("PathOfTheLightEyesOfTruthSeeingInvisible"),
                definition =>
                {
                    var gpb = new GuiPresentationBuilder(
                        "Subclass/&BarbarianPathOfTheLightSeeingInvisibleConditionDescription",
                        "Subclass/&BarbarianPathOfTheLightSeeingInvisibleConditionTitle");

                    gpb.SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionSeeInvisibility.GuiPresentation.SpriteReference);

                    definition
                        .SetGuiPresentation(gpb.Build())
                        .SetDurationType(RuleDefinitions.DurationType.Permanent)
                        .SetConditionType(RuleDefinitions.ConditionType.Beneficial)
                        .SetSilentWhenAdded(true)
                        .SetSilentWhenRemoved(true);

                    definition.Features.Add(DatabaseHelper.FeatureDefinitionSenses.SenseSeeInvisible16);
                });

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

            var seeInvisiblePower = FeatureDefinitionBuilder<FeatureDefinitionPower>.Build(
                "PathOfTheLightEyesOfTruthPower",
                CreateNamespacedGuid("PathOfTheLightEyesOfTruthPower"),
                definition =>
                {
                    var gpb = new GuiPresentationBuilder(
                        "Subclass/&BarbarianPathOfTheLightEyesOfTruthDescription",
                        "Subclass/&BarbarianPathOfTheLightEyesOfTruthTitle");

                    gpb.SetSpriteReference(DatabaseHelper.SpellDefinitions.SeeInvisibility.GuiPresentation.SpriteReference);

                    definition
                        .SetGuiPresentation(gpb.Build())
                        .SetActivationTime(RuleDefinitions.ActivationTime.Permanent)
                        .SetEffectDescription(seeInvisibleEffectBuilder.Build())
                        .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
                        .SetShowCasting(false);
                });

            var eyesOfTruth = FeatureDefinitionBuilder<FeatureDefinitionFeatureSet>.Build(
                "PathOfTheLightEyesOfTruth",
                CreateNamespacedGuid("PathOfTheLightEyesOfTruth"),
                "Subclass/&BarbarianPathOfTheLightEyesOfTruthDescription",
                "Subclass/&BarbarianPathOfTheLightEyesOfTruthTitle",
                featureSetDefinition =>
                {
                    featureSetDefinition
                        .SetEnumerateInDescription(false)
                        .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                        .SetUniqueChoices(false);

                    featureSetDefinition.SetField("featureSet", new List<FeatureDefinition>());

                    featureSetDefinition.FeatureSet.Add(seeInvisiblePower);
                });

            return eyesOfTruth;
        }

        private static FeatureDefinition CreateIlluminatingBurst(ConditionDefinition illuminatedCondition)
        {
            var illuminatingBurstFeatureSet = FeatureDefinitionBuilder<FeatureDefinitionFeatureSet>.Build(
                "PathOfTheLightIlluminatingBurstFeatureSet",
                CreateNamespacedGuid("PathOfTheLightIlluminatingBurstFeatureSet"),
                "Subclass/&BarbarianPathOfTheLightIlluminatingBurstDescription",
                "Subclass/&BarbarianPathOfTheLightIlluminatingBurstTitle",
                featureSetDefinition =>
                {
                    featureSetDefinition
                        .SetEnumerateInDescription(false)
                        .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                        .SetUniqueChoices(false);

                    featureSetDefinition.SetField("featureSet", new List<FeatureDefinition>());

                    ConditionDefinition illuminatingBurstSuppressedCondition = CreateIlluminatingBurstSuppressedCondition();

                    var illuminatingBurstBuilder = new IlluminatingBurstInitiatorBuilder(
                        "PathOfTheLightIlluminatingBurstInitiator",
                        CreateNamespacedGuid("PathOfTheLightIlluminatingBurstInitiator"),
                        "Feature/&NoContentTitle",
                        "Feature/&NoContentTitle",
                        illuminatingBurstSuppressedCondition);

                    featureSetDefinition.FeatureSet.Add(illuminatingBurstBuilder.AddToDB());

                    var illuminatingBurstPowerBuilder = new IlluminatingBurstBuilder(
                        IlluminatingBurstName,
                        CreateNamespacedGuid(IlluminatingBurstName),
                        "Subclass/&BarbarianPathOfTheLightIlluminatingBurstPowerDescription",
                        "Subclass/&BarbarianPathOfTheLightIlluminatingBurstPowerTitle",
                        illuminatedCondition,
                        illuminatingBurstSuppressedCondition);

                    featureSetDefinition.FeatureSet.Add(illuminatingBurstPowerBuilder.AddToDB());

                    featureSetDefinition.FeatureSet.Add(CreateIlluminatingBurstSuppressor(illuminatingBurstSuppressedCondition));
                });

            return illuminatingBurstFeatureSet;
        }

        private static FeatureDefinition CreateIlluminatingBurstSuppressor(ConditionDefinition illuminatingBurstSuppressedCondition)
        {
            var illuminatingBurstSuppressor = FeatureDefinitionBuilder<FeatureDefinitionPower>.Build(
                "PathOfTheLightIlluminatingBurstSuppressor",
                CreateNamespacedGuid("PathOfTheLightIlluminatingBurstSuppressor"),
                definition =>
                {
                    var guiPresentationBuilder = new GuiPresentationBuilder(
                        "Feature/&NoContentTitle",
                        "Feature/&NoContentTitle");

                    var guiPresentation = guiPresentationBuilder.Build();
                    guiPresentation.SetHidden(true);

                    var suppressIlluminatingBurst = new EffectForm
                    {
                        FormType = EffectForm.EffectFormType.Condition,
                        ConditionForm = new ConditionForm
                        {
                            Operation = ConditionForm.ConditionOperation.Add,
                            ConditionDefinition = illuminatingBurstSuppressedCondition
                        }
                    };

                    var effectDescriptionBuilder = new EffectDescriptionBuilder();

                    effectDescriptionBuilder
                        .SetDurationData(RuleDefinitions.DurationType.Permanent, 1, RuleDefinitions.TurnOccurenceType.StartOfTurn)
                        .SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1, RuleDefinitions.TargetType.Self, 1, 0, ActionDefinitions.ItemSelectionType.None)
                        .SetRecurrentEffect(RuleDefinitions.RecurrentEffect.OnActivation | RuleDefinitions.RecurrentEffect.OnTurnStart)
                        .AddEffectForm(suppressIlluminatingBurst);

                    definition
                        .SetGuiPresentation(guiPresentation)
                        .SetActivationTime(RuleDefinitions.ActivationTime.Permanent)
                        .SetEffectDescription(effectDescriptionBuilder.Build())
                        .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill);
                });

            return illuminatingBurstSuppressor;
        }

        private static ConditionDefinition CreateIlluminatedCondition()
        {
            var illuminatedCondition = ConditionDefinitionBuilder<IlluminatedConditionDefinition>.Build(
                IlluminatedConditionName,
                CreateNamespacedGuid(IlluminatedConditionName),
                definition =>
                {
                    var gpb = new GuiPresentationBuilder(
                        "Subclass/&BarbarianPathOfTheLightIlluminatedConditionDescription",
                        "Subclass/&BarbarianPathOfTheLightIlluminatedConditionTitle");

                    gpb.SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionBranded.GuiPresentation.SpriteReference);

                    definition
                        .SetGuiPresentation(gpb.Build())
                        .SetSpecialDuration(true)
                        .SetDurationType(RuleDefinitions.DurationType.Irrelevant)
                        .SetConditionType(RuleDefinitions.ConditionType.Detrimental)
                        .SetAllowMultipleInstances(true)
                        .SetSilentWhenAdded(true)
                        .SetSilentWhenRemoved(false);

                    definition.Features.Add(CreateDisadvantageAgainstNonSource());
                    definition.Features.Add(CreatePreventInvisibility());
                });

            return illuminatedCondition;
        }

        private static AttackDisadvantageAgainstNonSource CreateDisadvantageAgainstNonSource()
        {
            var disadvantageAgainstNonSource = FeatureDefinitionBuilder<AttackDisadvantageAgainstNonSource>.Build(
                "PathOfTheLightIlluminatedDisadvantage",
                CreateNamespacedGuid("PathOfTheLightIlluminatedDisadvantage"),
                "Subclass/&BarbarianPathOfTheLightIlluminatedDisadvantageDescription",
                "Feature/&NoContentTitle",
                definition =>
                {
                    definition.ConditionName = IlluminatedConditionName;
                });

            return disadvantageAgainstNonSource;
        }

        private static FeatureDefinition CreatePreventInvisibility()
        {
            // Prevents a creature from turning invisible by "granting" immunity to invisibility

            var preventInvisibility = FeatureDefinitionBuilder<FeatureDefinitionFeatureSet>.Build(
                "PathOfTheLightIlluminatedPreventInvisibility",
                CreateNamespacedGuid("PathOfTheLightIlluminatedPreventInvisibility"),
                "Subclass/&BarbarianPathOfTheLightIlluminatedPreventInvisibilityDescription",
                "Feature/&NoContentTitle",
                featureSetDefinition =>
                {
                    featureSetDefinition
                        .SetEnumerateInDescription(false)
                        .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                        .SetUniqueChoices(false);

                    featureSetDefinition.SetField("featureSet", new List<FeatureDefinition>());

                    foreach (ConditionDefinition invisibleCondition in InvisibleConditions)
                    {
                        FeatureDefinition preventInvisibilitySubFeature = FeatureDefinitionBuilder<FeatureDefinitionConditionAffinity>.Build(
                            "PathOfTheLightIlluminatedPreventInvisibility" + invisibleCondition.Name,
                            CreateNamespacedGuid("PathOfTheLightIlluminatedPreventInvisibility" + invisibleCondition.Name),
                            "Feature/&NoContentTitle",
                            "Feature/&NoContentTitle",
                            conditionAffinityDefinition =>
                            {
                                conditionAffinityDefinition
                                    .SetConditionAffinityType(RuleDefinitions.ConditionAffinityType.Immunity)
                                    .SetConditionType(invisibleCondition.Name);
                            });

                        featureSetDefinition.FeatureSet.Add(preventInvisibilitySubFeature);
                    }
                });

            return preventInvisibility;
        }

        private static ConditionDefinition CreateIlluminatingBurstSuppressedCondition()
        {
            ConditionDefinition illuminatingBurstSuppressedCondition = ConditionDefinitionBuilder.Build(
                "PathOfTheLightIlluminatingBurstSuppressedCondition",
                CreateNamespacedGuid("PathOfTheLightIlluminatingBurstSuppressedCondition"),
                definition =>
                {
                    var gpb = new GuiPresentationBuilder(
                        "Feature/&NoContentTitle",
                        "Feature/&NoContentTitle");

                    var guiPresentation = gpb.Build();

                    guiPresentation.SetHidden(true);

                    definition
                        .SetGuiPresentation(guiPresentation)
                        .SetDurationType(RuleDefinitions.DurationType.Permanent)
                        .SetConditionType(RuleDefinitions.ConditionType.Neutral)
                        .SetSilentWhenAdded(true)
                        .SetSilentWhenRemoved(true);
                });

            return illuminatingBurstSuppressedCondition;
        }

        private static void HandleAfterIlluminatedConditionRemoved(RulesetActor removedFrom)
        {
            var character = removedFrom as RulesetCharacter;

            if (character == null)
            {
                return;
            }

            // Intentionally *includes* conditions that have Illuminated as their parent (like the Illuminating Burst condition)
            if (!character.HasConditionOfTypeOrSubType(IlluminatedConditionName))
            {
                if (character.PersonalLightSource?.SourceName == IlluminatingStrikeName ||
                    character.PersonalLightSource?.SourceName == IlluminatingBurstName)
                {
                    var visibilityService = ServiceRepository.GetService<IGameLocationVisibilityService>();

                    visibilityService.RemoveCharacterLightSource(GameLocationCharacter.GetFromActor(removedFrom), character.PersonalLightSource);
                    character.PersonalLightSource = null;
                }
            }
        }


        // Helper classes

        private class IlluminatedConditionDefinition : ConditionDefinition, IConditionRemovedOnSourceTurnStart, INotifyConditionRemoval
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

        private class IlluminatedByBurstConditionDefinition : ConditionDefinition, INotifyConditionRemoval
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

        private class IlluminatingStrikeAdditionalDamage : FeatureDefinitionAdditionalDamage, IClassHoldingFeature
        {
            // Allows Illuminating Strike damage to scale with barbarian level
            public CharacterClassDefinition Class => DatabaseHelper.CharacterClassDefinitions.Barbarian;
        }

        private class IlluminatingStrikeFeatureBuilder : BaseDefinitionBuilder<IlluminatingStrikeAdditionalDamage>
        {
            public IlluminatingStrikeFeatureBuilder(string name, string guid, string description, string title, ConditionDefinition illuminatedCondition) : base(name, guid)
            {
                Definition
                    .SetGuiPresentation(CreatePowerGuiPresentation(description, title))
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

                Definition.SetField("conditionOperations", new List<ConditionOperationDescription>());

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

            private static GuiPresentation CreatePowerGuiPresentation(string description, string title)
            {
                var guiPresentationBuilder = new GuiPresentationBuilder(description, title);

                guiPresentationBuilder.SetSpriteReference(DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageDomainLifeDivineStrike.GuiPresentation.SpriteReference);

                return guiPresentationBuilder.Build();
            }

            private static LightSourceForm CreateIlluminatedLightSource()
            {
                EffectForm faerieFireLightSource = DatabaseHelper.SpellDefinitions.FaerieFire.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.LightSource);

                var lightSourceForm = new LightSourceForm();
                lightSourceForm.Copy(faerieFireLightSource.LightSourceForm);

                lightSourceForm
                    .SetBrightRange(4)
                    .SetDimAdditionalRange(4);

                return lightSourceForm;
            }

            private static DiceByRank BuildDiceByRank(int rank, int dice)
            {
                DiceByRank diceByRank = new DiceByRank();
                diceByRank.SetField("rank", rank);
                diceByRank.SetField("diceNumber", dice);
                return diceByRank;
            }
        }

        /// <summary>
        /// Builds the power that enables Illuminating Strike while you're raging.
        /// </summary>
        private class IlluminatingStrikeInitiatorBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
        {
            public IlluminatingStrikeInitiatorBuilder(string name, string guid, string description, string title, ConditionDefinition illuminatedCondition) : base(name, guid)
            {
                Definition
                    .SetGuiPresentation(CreatePowerGuiPresentation(description, title))
                    .SetActivationTime(RuleDefinitions.ActivationTime.OnRageStartAutomatic)
                    .SetEffectDescription(CreatePowerEffect(illuminatedCondition))
                    .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
                    .SetShowCasting(false);
            }

            private static GuiPresentation CreatePowerGuiPresentation(string description, string title)
            {
                var guiPresentationBuilder = new GuiPresentationBuilder(description, title);

                guiPresentationBuilder.SetSpriteReference(DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageDomainLifeDivineStrike.GuiPresentation.SpriteReference);

                var guiPresentation = guiPresentationBuilder.Build();
                guiPresentation.SetHidden(true);

                return guiPresentation;
            }

            private static EffectDescription CreatePowerEffect(ConditionDefinition illuminatedCondition)
            {
                var initiatorCondition = ConditionDefinitionBuilder.Build(
                    "PathOfTheLightIlluminatingStrikeInitiatorCondition",
                    CreateNamespacedGuid("PathOfTheLightIlluminatingStrikeInitiatorCondition"),
                    definition =>
                    {
                        var gpb = new GuiPresentationBuilder("Feature/&NoContentTitle", "Feature/&NoContentTitle");

                        GuiPresentation guiPresentation = gpb.Build();

                        guiPresentation.SetHidden(true);

                        definition
                            .SetGuiPresentation(guiPresentation)
                            .SetDurationType(RuleDefinitions.DurationType.Minute)
                            .SetDurationParameter(1)
                            .SetConditionType(RuleDefinitions.ConditionType.Beneficial)
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

                        definition.SetField("specialInterruptions", new List<RuleDefinitions.ConditionInterruption> { RuleDefinitions.ConditionInterruption.RageStop });
                    });

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

        private class IlluminatingBurstPower : FeatureDefinitionPower, IStartOfTurnRecharge
        {
            public bool IsRechargeSilent => true;
        }

        private class IlluminatingBurstBuilder : BaseDefinitionBuilder<IlluminatingBurstPower>
        {
            public IlluminatingBurstBuilder(string name, string guid, string description, string title, ConditionDefinition illuminatedCondition, ConditionDefinition illuminatingBurstSuppressedCondition) : base(name, guid)
            {
                Definition
                    .SetGuiPresentation(CreatePowerGuiPresentation(description, title))
                    .SetActivationTime(RuleDefinitions.ActivationTime.NoCost)
                    .SetEffectDescription(CreatePowerEffect(illuminatedCondition))
                    .SetRechargeRate(RuleDefinitions.RechargeRate.OneMinute) // Actually recharges at the start of your turn, using IStartOfTurnRecharge
                    .SetFixedUsesPerRecharge(1)
                    .SetUsesDetermination(RuleDefinitions.UsesDetermination.Fixed)
                    .SetCostPerUse(1)
                    .SetShowCasting(false)
                    .SetDisableIfConditionIsOwned(illuminatingBurstSuppressedCondition); // Only enabled on the turn you enter a rage
            }

            private static GuiPresentation CreatePowerGuiPresentation(string description, string title)
            {
                var guiPresentationBuilder = new GuiPresentationBuilder(description, title);

                guiPresentationBuilder.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainSunHeraldOfTheSun.GuiPresentation.SpriteReference);

                return guiPresentationBuilder.Build();
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

                var illuminatedByBurstCondition = ConditionDefinitionBuilder<IlluminatedByBurstConditionDefinition>.Build(
                    "PathOfTheLightIlluminatedByBurstCondition",
                    CreateNamespacedGuid("PathOfTheLightIlluminatedByBurstCondition"),
                    definition =>
                    {
                        var gpb = new GuiPresentationBuilder(
                            "Subclass/&BarbarianPathOfTheLightIlluminatedConditionDescription",
                            "Subclass/&BarbarianPathOfTheLightIlluminatedConditionTitle");

                        gpb.SetSpriteReference(DatabaseHelper.ConditionDefinitions.ConditionBranded.GuiPresentation.SpriteReference);

                        definition
                            .SetGuiPresentation(gpb.Build())
                            .SetDurationType(RuleDefinitions.DurationType.Minute)
                            .SetDurationParameter(1)
                            .SetConditionType(RuleDefinitions.ConditionType.Detrimental)
                            .SetAllowMultipleInstances(true)
                            .SetSilentWhenAdded(true)
                            .SetSilentWhenRemoved(false)
                            .SetParentCondition(illuminatedCondition);
                    });

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

                EffectForm faerieFireLightSource = DatabaseHelper.SpellDefinitions.FaerieFire.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.LightSource);

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
                    .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.GuidingBolt.EffectDescription.EffectParticleParameters)
                    .AddEffectForm(dealDamage)
                    .AddEffectForm(addIlluminatedCondition)
                    .AddEffectForm(addLightSource);

                return effectDescriptionBuilder.Build();
            }
        }

        /// <summary>
        /// Builds the power that enables Illuminating Burst on the turn you enter a rage (by removing the condition disabling it).
        /// </summary>
        private class IlluminatingBurstInitiatorBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
        {
            public IlluminatingBurstInitiatorBuilder(string name, string guid, string description, string title, ConditionDefinition illuminatingBurstSuppressedCondition) : base(name, guid)
            {
                Definition
                    .SetGuiPresentation(CreatePowerGuiPresentation(description, title))
                    .SetActivationTime(RuleDefinitions.ActivationTime.OnRageStartAutomatic)
                    .SetEffectDescription(CreatePowerEffect(illuminatingBurstSuppressedCondition))
                    .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
                    .SetShowCasting(false);
            }

            private static GuiPresentation CreatePowerGuiPresentation(string description, string title)
            {
                var guiPresentationBuilder = new GuiPresentationBuilder(description, title);

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
