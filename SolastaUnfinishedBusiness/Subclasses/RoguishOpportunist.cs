using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static RuleDefinitions;

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

        var debilitatingStrikeEffect = EffectDescriptionBuilder
            .Create()
            .SetDurationData(
                DurationType.Round,
                1)
            .SetTargetingData(
                Side.Enemy,
                RangeType.MeleeHit,
                0,
                TargetType.Individuals,
                0,
                0)
            .SetSavingThrowData(
                false,
                AttributeDefinitions.Constitution,
                true,
                EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Dexterity,
                20)
            .SetEffectForms(EffectFormBuilder
                .Create()
                .SetConditionForm(
                    ConditionDefinitionBuilder
                        .Create(ConditionDummy, "ConditionOpportunistDebilitated")
                        .SetOrUpdateGuiPresentation(Category.Condition)
                        .AddToDB(),
                    ConditionForm.ConditionOperation.AddRandom,
                    false,
                    false,
                    ConditionBaned,
                    ConditionBleeding,
                    ConditionDefinitions.ConditionBlinded,
                    ConditionDefinitions.ConditionStunned
                )
                .HasSavingThrow(EffectSavingThrowType.Negates)
                .CanSaveToCancel(TurnOccurenceType.EndOfTurn)
                .Build())
            .Build();

        // Enemies struck by your sneak attack suffered from one of the following condition (Baned, Blinded, Bleed, Stunned)
        // if they fail a CON save against the DC of 8 + your DEX mod + your prof.
        var powerOpportunistDebilitatingStrike = FeatureDefinitionPowerBuilder
            .Create("PowerOpportunistDebilitatingStrike")
            .Configure(
                UsesDetermination.Fixed,
                ActivationTime.OnSneakAttackHitAuto,
                RechargeRate.AtWill,
                debilitatingStrikeEffect)
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

    internal override CharacterSubclassDefinition Subclass { get; }

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

        attackModifier.attackAdvantageTrends.Add(new TrendInfo(1,
            FeatureSourceType.CharacterFeature, "QuickStrike", null));
    }
}
