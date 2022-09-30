using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class RoguishOpportunist : AbstractSubclass
{
    internal RoguishOpportunist()
    {
        // Grant advantage when attack enemies whose initiative is lower than your
        // or when perform an attack of opportunity.
        var onComputeAttackModifierOpportunistQuickStrike = FeatureDefinitionOnComputeAttackModifierBuilder
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
                        .Create(ConditionDummy, "ConditionOpportunistDebilitated")
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
        var powerOpportunistDebilitatingStrike = FeatureDefinitionPowerBuilder
            .Create("PowerOpportunistDebilitatingStrike")
            .Configure(
                1,
                RuleDefinitions.UsesDetermination.Fixed,
                AttributeDefinitions.Dexterity,
                RuleDefinitions.ActivationTime.OnSneakAttackHitAuto,
                1,
                RuleDefinitions.RechargeRate.AtWill,
                false,
                false,
                AttributeDefinitions.Dexterity,
                debilitatingStrikeEffectBuilder.Build()
            )
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("RoguishOpportunist")
            .SetGuiPresentation(Category.Subclass, MartialCommander.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(3,
                onComputeAttackModifierOpportunistQuickStrike)
            .AddFeaturesAtLevel(9,
                powerOpportunistDebilitatingStrike)
            //.AddFeaturesAtLevel( 13, thugOvercomeCompetition)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; set; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;

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
}
