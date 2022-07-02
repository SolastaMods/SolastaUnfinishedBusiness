using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.ConditionDefinitions;

namespace SolastaCommunityExpansion.Subclasses.Rogue;

internal sealed class Opportunist : AbstractSubclass
{
    private static readonly Guid SubclassNamespace = new("2db81c3c-a9e2-4829-b27b-47af3ba42c76");

    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return CreateOpportunist();
    }

    private static void QuickStrikeOnAttackDelegate([CanBeNull] GameLocationCharacter attacker,
        [CanBeNull] GameLocationCharacter defender,
        ActionModifier attackModifier, RulesetAttackMode attackerAttackMode)
    {
        // melee attack only
        if (attacker == null || defender == null)
        {
            return;
        }

        // grant advantage if attacker is performing an opportunity attack or has higher initiative.
        if (attacker.LastInitiative > defender.LastInitiative ||
            (attackerAttackMode.ActionType == ActionDefinitions.ActionType.Reaction &&
             attacker.GetActionStatus(ActionDefinitions.Id.AttackOpportunity,
                 ActionDefinitions.ActionScope.Battle) == ActionDefinitions.ActionStatus.Available))
        {
            attackModifier.AttackAdvantageTrends.Add(new RuleDefinitions.TrendInfo(1,
                RuleDefinitions.FeatureSourceType.CharacterFeature, "QuickStrike", attacker));
        }
    }

    private static CharacterSubclassDefinition CreateOpportunist()
    {
        var subclassNamespace = new Guid("b217342c-5b1b-46eb-9f2f-86239c3088bf");

        // Grant advantage when attack enemies whose initiative is lower than your
        // or when perform an attack of opportunity.
        var quickStrike = FeatureDefinitionOnAttackEffectBuilder
            .Create("RoguishOppotunistQuickStrike", subclassNamespace)
            .SetGuiPresentation("OpportunistQuickStrike", Category.Feature)
            .SetOnAttackDelegates(QuickStrikeOnAttackDelegate, null)
            .AddToDB();

        var debilitatingStrikeEffectBuilder = new EffectDescriptionBuilder()
            .SetDurationData(
                RuleDefinitions.DurationType.Round,
                1,
                RuleDefinitions.TurnOccurenceType.EndOfTurn)
            .SetTargetingData(
                RuleDefinitions.Side.Enemy,
                RuleDefinitions.RangeType.MeleeHit,
                0, // I think this parameter is irrelevant if range type is melee hit.
                RuleDefinitions.TargetType.Individuals, // allow multiple effect stack ?
                0,
                0)
            .SetSavingThrowData(
                true,
                false,
                SmartAttributeDefinitions.Constitution.name,
                true,
                RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                SmartAttributeDefinitions.Dexterity.name,
                20,
                false,
                new List<SaveAffinityBySenseDescription>())
            .AddEffectForm(new EffectFormBuilder()
                .SetConditionForm(
                    DebilitatedConditionBuilder.DebilitatedCondition,
                    ConditionForm.ConditionOperation.AddRandom,
                    false,
                    false,
                    new List<ConditionDefinition>
                    {
                        ConditionBlinded, ConditionBaned, ConditionBleeding, ConditionStunned
                    })
                .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.Negates)
                .CanSaveToCancel(RuleDefinitions.TurnOccurenceType.EndOfTurn)
                .Build());

        // Enemies struck by your sneak attack suffered from one of the following condition (Baned, Blinded, Bleed, Stunned)
        // if they fail a CON save against the DC of 8 + your DEX mod + your prof.
        var debilitatingStrikePower = FeatureDefinitionConditionalPowerBuilder
            .Create("RoguishOpportunistDebilitatingStrikePower", SubclassNamespace)
            .Configure(
                1,
                RuleDefinitions.UsesDetermination.Fixed,
                AttributeDefinitions.Dexterity,
                RuleDefinitions.ActivationTime.OnSneakAttackHit,
                1,
                RuleDefinitions.RechargeRate.AtWill,
                false,
                false,
                AttributeDefinitions.Dexterity,
                debilitatingStrikeEffectBuilder.Build()
            )
            .SetGuiPresentation("OpportunistDebilitatingStrike", Category.Feature)
            .AddToDB();

        return CharacterSubclassDefinitionBuilder
            .Create("Opportunist", subclassNamespace)
            .SetGuiPresentation(Category.Subclass, MartialCommander.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(quickStrike, 3)
            .AddFeatureAtLevel(debilitatingStrikePower, 9)
            //.AddFeatureAtLevel(thugOvercomeCompetition, 13)
            .AddToDB();
    }

    private sealed class DebilitatedConditionBuilder : ConditionDefinitionBuilder
    {
        private const string Name = "Debilitated";
        private const string TitleString = "Condition/&DebilitatedConditionTitle";
        private const string DescriptionString = "Condition/&DebilitatedConditionDescription";

        internal static readonly ConditionDefinition DebilitatedCondition =
            CreateAndAddToDB(Name, SubclassNamespace.ToString());

        private DebilitatedConditionBuilder(string name, string guid) : base(ConditionDummy, name, guid)
        {
            Definition.GuiPresentation.Title = TitleString;
            Definition.GuiPresentation.Description = DescriptionString;
        }

        private static ConditionDefinition CreateAndAddToDB(string name, string guid)
        {
            return new DebilitatedConditionBuilder(name, guid).AddToDB();
        }
    }
}
