using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using static SolastaCommunityExpansion.Api.DatabaseHelper;

namespace SolastaCommunityExpansion.Subclasses.Paladin;

internal sealed class OathOfRetribution : AbstractSubclass
{
    private static readonly Guid SubclassNamespace = new("f5efd735-ff95-4256-ad17-dde585aeb5f3");
    private readonly CharacterSubclassDefinition Subclass;

    internal OathOfRetribution()
    {
        var paladinOathOfRetributionAutoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create("PaladinOathOfRetributionAutoPreparedSpells", SubclassNamespace)
            .SetGuiPresentation(Category.Feature)
            .SetCastingClass(CharacterClassDefinitions.Paladin)
            .SetPreparedSpellGroups(
                AutoPreparedSpellsGroupBuilder.BuildSpellGroup(3, SpellDefinitions.Bane, SpellDefinitions.HuntersMark),
                AutoPreparedSpellsGroupBuilder.BuildSpellGroup(5, SpellDefinitions.HoldPerson,
                    SpellDefinitions.MistyStep),
                AutoPreparedSpellsGroupBuilder.BuildSpellGroup(9, SpellDefinitions.Haste,
                    SpellDefinitions.ProtectionFromEnergy))
            .AddToDB();

        var conditionFrightenedZealousAccusation = ConditionDefinitionBuilder
            .Create("ConditionFrightenedZealousAccusation", SubclassNamespace)
            .SetGuiPresentation(Category.Condition,
                ConditionDefinitions.ConditionFrightened.GuiPresentation.SpriteReference)
            .Configure(RuleDefinitions.DurationType.Minute, 1, false,
                FeatureDefinitionCombatAffinitys.CombatAffinityFrightened,
                FeatureDefinitionMovementAffinitys.MovementAffinityConditionRestrained)
            .SetConditionType(RuleDefinitions.ConditionType.Detrimental)
            .SetSpecialInterruptions(
                RuleDefinitions.ConditionInterruption.Damaged,
                RuleDefinitions.ConditionInterruption.DamagedByFriendly
            )
            .AddToDB();

        var effectParticleParametersRetributionZealousAccusation = new EffectParticleParameters();

        effectParticleParametersRetributionZealousAccusation
            .Copy(SpellDefinitions.Fear.EffectDescription.EffectParticleParameters);

        var powerOathOfRetributionZealousAccusation = FeatureDefinitionPowerBuilder
            .Create("PowerOathOfRetributionZealousAccusation", SubclassNamespace)
            .SetGuiPresentation(Category.Power, FeatureDefinitionPowers.PowerDomainLawHolyRetribution
                .GuiPresentation.SpriteReference)
            .Configure(
                1,
                RuleDefinitions.UsesDetermination.Fixed,
                AttributeDefinitions.Charisma,
                RuleDefinitions.ActivationTime.Action,
                1,
                RuleDefinitions.RechargeRate.ChannelDivinity,
                false,
                false,
                AttributeDefinitions.Charisma,
                new EffectDescription
                {
                    targetSide = RuleDefinitions.Side.Enemy,
                    createdByCharacter = true,
                    rangeType = RuleDefinitions.RangeType.Distance,
                    rangeParameter = 12,
                    targetType = RuleDefinitions.TargetType.Individuals,
                    itemSelectionType = ActionDefinitions.ItemSelectionType.None,
                    canBePlacedOnCharacter = true,
                    durationType = RuleDefinitions.DurationType.Minute,
                    durationParameter = 1,
                    endOfEffect = RuleDefinitions.TurnOccurenceType.EndOfTurn,
                    hasSavingThrow = true,
                    savingThrowAbility = AttributeDefinitions.Wisdom,
                    savingThrowDifficultyAbility = AttributeDefinitions.Charisma,
                    fixedSavingThrowDifficultyClass = 15,
                    savingThrowAffinitiesBySense =
                        new List<SaveAffinityBySenseDescription>
                        {
                            new()
                            {
                                senseType = SenseMode.Type.Darkvision,
                                advantageType = RuleDefinitions.AdvantageType.Disadvantage
                            },
                            new()
                            {
                                senseType = SenseMode.Type.SuperiorDarkvision,
                                advantageType = RuleDefinitions.AdvantageType.Disadvantage
                            }
                        },
                    effectForms = new List<EffectForm>
                    {
                        new EffectFormBuilder()
                            .CreatedByCharacter()
                            .SetConditionForm(conditionFrightenedZealousAccusation,
                                ConditionForm.ConditionOperation.Add)
                            .Build()
                    },
                    effectAdvancement = new EffectAdvancement(),
                    effectParticleParameters = effectParticleParametersRetributionZealousAccusation
                }
            )
            .SetUniqueInstance(true)
            .AddToDB();

        var conditionTrueStrikeZealousCondemnation = ConditionDefinitionBuilder
            .Create("ConditionTrueStrikeZealousCondemnation", SubclassNamespace)
            .SetGuiPresentation(Category.Condition,
                ConditionDefinitions.ConditionGuided.GuiPresentation.SpriteReference)
            .Configure(RuleDefinitions.DurationType.Minute, 1, false,
                FeatureDefinitionCombatAffinitys.CombatAffinityTrueStrike)
            .SetConditionType(RuleDefinitions.ConditionType.Detrimental)
            .SetSpecialInterruptions(
                RuleDefinitions.ConditionInterruption.None
            )
            .AddToDB();

        var effectParticleParametersRetributionZealousCondemnation = new EffectParticleParameters();

        effectParticleParametersRetributionZealousCondemnation
            .Copy(SpellDefinitions.TrueStrike.EffectDescription.EffectParticleParameters);

        var powerOathOfRetributionZealousCondemnation = FeatureDefinitionPowerBuilder
            .Create("PowerOathOfRetributionZealousCondemnation", SubclassNamespace)
            .SetGuiPresentation(Category.Power, FeatureDefinitionPowers.PowerOathOfTirmarSmiteTheHidden
                .GuiPresentation.SpriteReference)
            .Configure(
                1,
                RuleDefinitions.UsesDetermination.Fixed,
                AttributeDefinitions.Charisma,
                RuleDefinitions.ActivationTime.BonusAction,
                1,
                RuleDefinitions.RechargeRate.ChannelDivinity,
                false,
                false,
                AttributeDefinitions.Charisma,
                new EffectDescription
                {
                    targetSide = RuleDefinitions.Side.Enemy,
                    createdByCharacter = true,
                    rangeType = RuleDefinitions.RangeType.Distance,
                    rangeParameter = 2,
                    targetType = RuleDefinitions.TargetType.Individuals,
                    itemSelectionType = ActionDefinitions.ItemSelectionType.None,
                    canBePlacedOnCharacter = true,
                    durationType = RuleDefinitions.DurationType.Minute,
                    durationParameter = 1,
                    endOfEffect = RuleDefinitions.TurnOccurenceType.EndOfTurn,
                    effectForms = new List<EffectForm>
                    {
                        new EffectFormBuilder()
                            .CreatedByCharacter()
                            .SetConditionForm(conditionTrueStrikeZealousCondemnation,
                                ConditionForm.ConditionOperation.Add)
                            .Build()
                    },
                    effectAdvancement = new EffectAdvancement(),
                    effectParticleParameters = effectParticleParametersRetributionZealousCondemnation
                }
            )
            .SetUniqueInstance(true)
            .AddToDB();

        var conditionBonusRushTenaciousPursuit = ConditionDefinitionBuilder
            .Create("ConditionBonusRushTenaciousPursuit", SubclassNamespace)
            .SetGuiPresentation(Category.Condition,
                ConditionDefinitions.ConditionGuided.GuiPresentation.SpriteReference)
            .Configure(RuleDefinitions.DurationType.Round, 1, false,
                FeatureDefinitionCombatAffinitys.CombatAffinityDisengaging,
                FeatureDefinitionActionAffinitys.ActionAffinityExpeditiousRetreat,
                FeatureDefinitionAdditionalActions.AdditionalActionExpeditiousRetreat)
            .SetTurnOccurence(RuleDefinitions.TurnOccurenceType.StartOfTurn)
            .SetSpecialInterruptions(
                RuleDefinitions.ConditionInterruption.None
            )
            .AddToDB();

        var effectParticleParametersRetributionTenaciousPursuit = new EffectParticleParameters();

        effectParticleParametersRetributionTenaciousPursuit
            .Copy(FeatureDefinitionPowers.PowerDomainLawHolyRetribution.EffectDescription.EffectParticleParameters);

        var powerOathOfRetributionTenaciousPursuit = FeatureDefinitionPowerBuilder
            .Create("PowerOathOfRetributionTenaciousPursuit", SubclassNamespace)
            .SetGuiPresentation(Category.Power,
                FeatureDefinitionPowers.PowerDomainBattleDecisiveStrike.GuiPresentation.SpriteReference)
            .Configure(
                5,
                RuleDefinitions.UsesDetermination.Fixed,
                AttributeDefinitions.Charisma,
                RuleDefinitions.ActivationTime.OnAttackHit,
                1,
                RuleDefinitions.RechargeRate.LongRest,
                false,
                false,
                AttributeDefinitions.Charisma,
                new EffectDescription
                {
                    targetType = RuleDefinitions.TargetType.Self,
                    itemSelectionType = ActionDefinitions.ItemSelectionType.Equiped,
                    targetSide = RuleDefinitions.Side.Ally,
                    createdByCharacter = true,
                    rangeType = RuleDefinitions.RangeType.Self,
                    canBePlacedOnCharacter = true,
                    durationType = RuleDefinitions.DurationType.Round,
                    durationParameter = 1,
                    endOfEffect = RuleDefinitions.TurnOccurenceType.StartOfTurn,
                    effectAdvancement = new EffectAdvancement {incrementMultiplier = 1},
                    effectForms = new List<EffectForm>
                    {
                        new EffectFormBuilder()
                            .CreatedByCharacter()
                            .SetConditionForm(conditionBonusRushTenaciousPursuit,
                                ConditionForm.ConditionOperation.Add, true, false)
                            .Build()
                    },
                    effectParticleParameters = effectParticleParametersRetributionTenaciousPursuit
                }
            )
            .SetShowCasting(true)
            .SetUniqueInstance(true)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("PaladinOathOfRetribution", SubclassNamespace)
            .SetGuiPresentation(Category.Subclass,
                CharacterSubclassDefinitions.DomainBattle.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(3,
                paladinOathOfRetributionAutoPreparedSpells,
                powerOathOfRetributionZealousAccusation,
                powerOathOfRetributionZealousCondemnation)
            .AddFeaturesAtLevel(7,
                powerOathOfRetributionTenaciousPursuit)
            .AddToDB();
    }

    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoicePaladinSacredOaths;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass;
    }
}
