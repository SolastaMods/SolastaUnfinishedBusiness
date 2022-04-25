using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.ConditionDefinitions;

namespace SolastaCommunityExpansion.Subclasses.Rogue
{
    internal class Opportunist : AbstractSubclass
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

        private static void QuickStrikeOnAttackDelegate(GameLocationCharacter attacker, GameLocationCharacter defender, ActionModifier attackModifer, RulesetAttackMode attackerAttackMode)
        {
            // melee attack only
            if (attacker == null || defender == null || attackerAttackMode.Ranged)
            {
                return;
            }

            // grant advatage if attacker is performing an opportunity attack or has higher inititative.
            if (attacker.LastInitiative > defender.LastInitiative || attackerAttackMode.ActionType == ActionDefinitions.ActionType.Reaction && attacker.GetActionStatus(ActionDefinitions.Id.AttackOpportunity, ActionDefinitions.ActionScope.Battle) == ActionDefinitions.ActionStatus.Available)
            {
                attackModifer.AttackAdvantageTrends.Add(new RuleDefinitions.TrendInfo(1, RuleDefinitions.FeatureSourceType.CharacterFeature, "QuickStrike", attacker));
            }
        }

        private static bool IsDebilitatingStrikePowerActivate(RulesetCharacter hero)
        {
            // debilitating strike power is always active but the condition should be fired only when sneak attack hit
            return true;
        }

        internal class DebilitatedConditionBuilder : ConditionDefinitionBuilder
        {
            private const string Name = "Debilitated";
            private const string TitleString = "Condition/&DebilitatedConditionTitle";
            private const string DescriptionString = "Condition/&DebilitatedConditioDescription";

            protected DebilitatedConditionBuilder(string name, string guid) : base(ConditionDummy, name, guid)
            {
                Definition.GuiPresentation.Title = TitleString;
                Definition.GuiPresentation.Description = DescriptionString;
            }
            private static ConditionDefinition CreateAndAddToDB(string name, string guid)
            {
                return new DebilitatedConditionBuilder(name, guid).AddToDB();
            }

            internal static readonly ConditionDefinition DebilitatedCondition = CreateAndAddToDB(Name, SubclassNamespace.ToString());
        }

        private static CharacterSubclassDefinition CreateOpportunist()
        {
            var subclassNamespace = new Guid("b217342c-5b1b-46eb-9f2f-86239c3088bf");

            var quickStrike = FeatureDefinitionOnAttackEffectBuilder.Create("RougeSubclassOpportunistQuickStrike", subclassNamespace)
                .SetGuiPresentation("Feature/&OpportunistQuickStrikeTitle","Feature/&OpportunistQuickStrikeDescription")
                .SetOnAttackDelegate(QuickStrikeOnAttackDelegate)
                .AddToDB();

            EffectDescriptionBuilder debilitatingStrikeEffectBuilder = new EffectDescriptionBuilder()
                    .SetDurationData(
                        RuleDefinitions.DurationType.Round,
                        1,
                        RuleDefinitions.TurnOccurenceType.EndOfTurn)
                    .SetTargetingData(
                        RuleDefinitions.Side.Enemy,
                        RuleDefinitions.RangeType.MeleeHit,
                        0, // I think this parameter is irrelevant if range type is meleehit.
                        RuleDefinitions.TargetType.Individuals, // allow multiple effect stack ?
                        0,
                        0,
                        ActionDefinitions.ItemSelectionType.None)
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
                            new List<ConditionDefinition> { ConditionBlinded, ConditionBaned, ConditionBleeding, ConditionStunned })
                        .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.Negates)
                        .CanSaveToCancel(RuleDefinitions.TurnOccurenceType.EndOfTurn)
                        .Build());

            var debilitatingStrikePower = FeatureDefinitionConditionalPowerBuilder.Create("RougeSubclassOpportunistDebilitatingStrikePower", SubclassNamespace).
                Configure(
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
                .SetGuiPresentation("Feature/&OpportunistDebilitatingStrikeTitle", "Feature/&OpportunistDebilitatingStrikeDescription")
                .SetIsActive(IsDebilitatingStrikePowerActivate)
                .AddToDB();

            //debilitatingStrikePower.SetIsActiveDelegate(IsDebilitatingStrikePowerActivate);

            return CharacterSubclassDefinitionBuilder
                .Create("Opportunist", subclassNamespace)
                .SetGuiPresentation(Category.Subclass, MartialCommander.GuiPresentation.SpriteReference)
                .AddFeatureAtLevel(quickStrike, 3)
                .AddFeatureAtLevel(debilitatingStrikePower, 9)
                //.AddFeatureAtLevel(thugOvercomeCompetition, 13)
                .AddToDB();
        }
    }
}
