using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class RoguishOpportunist : AbstractSubclass
{
    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return CreateOpportunist();
    }

    private static void QuickStrikeOnComputeAttackModifier(
        RulesetCharacter myself,
        RulesetCharacter defender,
        RulesetAttackMode attackMode,
        ref ActionModifier attackModifier)
    {
        if (attackMode == null || defender == null)
        {
            return;
        }

        var hero = GameLocationCharacter.GetFromActor(myself);
        var target = GameLocationCharacter.GetFromActor(defender);

        // grant advantage if attacker is performing an opportunity attack or has higher initiative.
        if (hero.LastInitiative <= target.LastInitiative &&
            attackMode.actionType != ActionDefinitions.ActionType.Reaction)
        {
            return;
        }

        attackModifier.attackAdvantageTrends.Add(new RuleDefinitions.TrendInfo(1,
            RuleDefinitions.FeatureSourceType.CharacterFeature, "QuickStrike", null));
    }

    private static CharacterSubclassDefinition CreateOpportunist()
    {
        // Grant advantage when attack enemies whose initiative is lower than your
        // or when perform an attack of opportunity.
        var quickStrike = FeatureDefinitionOnComputeAttackModifierBuilder
            .Create("OnComputeAttackModifierOpportunistQuickStrike")
            .SetGuiPresentation(Category.Feature)
            .SetOnComputeAttackModifierDelegate(QuickStrikeOnComputeAttackModifier)
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
                    ConditionDefinitionBuilder
                        .Create("ConditionOpportunistDebilitated")
                        .SetOrUpdateGuiPresentation(Category.Condition)
                        .AddToDB(),
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
        var debilitatingStrikePower = FeatureDefinitionPowerBuilder
            .Create("PowerOpportunistDebilitatingStrike")
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
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        return CharacterSubclassDefinitionBuilder
            .Create("RoguishOpportunist")
            .SetGuiPresentation(Category.Subclass, MartialCommander.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(quickStrike, 3)
            .AddFeatureAtLevel(debilitatingStrikePower, 9)
            //.AddFeatureAtLevel(thugOvercomeCompetition, 13)
            .AddToDB();
    }
}
