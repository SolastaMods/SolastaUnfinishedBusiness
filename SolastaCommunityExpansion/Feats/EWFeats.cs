using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.Models;
using static RuleDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper;

namespace SolastaCommunityExpansion.Feats;

public static class EwFeats
{
    public const string SentinelFeat = "FeatSentinel";
    private const string PolearmExpertFeat = "FeatPolearmExpert";
    private const string RangedExpertFeat = "FeatRangedExpert";
    private const string RecklessAttackFeat = "FeatRecklessAttack";
    private static readonly Guid Guid = new("B4ED480F-2D06-4EB1-8732-9A721D80DD1A");

    public static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        feats.Add(BuildSentinel());
        feats.Add(BuildPolearmExpert());
        feats.Add(BuildRangedExpert());
        feats.Add(BuildRecklessAttack());
    }

    private static FeatDefinition BuildSentinel()
    {
        var restrained = ConditionDefinitions.ConditionRestrained;

        var stopMovementCondition = ConditionDefinitionBuilder
            .Create("SentinelStopMovementCondition", Guid)
            .SetGuiPresentation(Category.Condition, Gui.NoLocalization, restrained.GuiPresentation.SpriteReference)
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(
                FeatureDefinitionMovementAffinitys.MovementAffinityConditionRestrained,
                FeatureDefinitionActionAffinitys.ActionAffinityConditionRestrained
            )
            .AddToDB();

        return FeatDefinitionBuilder
            .Create(SentinelFeat, Guid)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(FeatureDefinitionOnAttackHitEffectBuilder
                .Create("FeatSentinelFeature", Guid)
                .SetGuiPresentationNoContent(true)
                .SetOnAttackHitDelegates(null, (attacker, defender, outcome, _, mode, _) =>
                {
                    if (outcome != RollOutcome.Success && outcome != RollOutcome.CriticalSuccess)
                    {
                        return;
                    }

                    if (mode is not {ActionType: ActionDefinitions.ActionType.Reaction})
                    {
                        return;
                    }

                    if (mode.AttackTags.Contains(AttacksOfOpportunity.NotAoOTag))
                    {
                        return;
                    }

                    var character = defender.RulesetCharacter;

                    character.AddConditionOfCategory(AttributeDefinitions.TagCombat,
                        RulesetCondition.CreateActiveCondition(character.Guid,
                            stopMovementCondition,
                            DurationType.Round,
                            1,
                            TurnOccurenceType.StartOfTurn,
                            attacker.Guid,
                            string.Empty
                        ));
                })
                .SetCustomSubFeatures(
                    AttacksOfOpportunity.CanIgnoreDisengage,
                    AttacksOfOpportunity.SentinelFeatMarker
                )
                .AddToDB())
            .AddToDB();
    }

    private static FeatDefinition BuildPolearmExpert()
    {
        return FeatDefinitionBuilder
            .Create(PolearmExpertFeat, Guid)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(FeatureDefinitionBuilder
                .Create("PolearmFeatFeature", Guid)
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(
                    new CanmakeAoOOnReachEntered(CharacterValidators.HasPolearm),
                    new AddPolearmFollowupAttack()
                )
                .AddToDB())
            .AddToDB();
    }

    private static FeatDefinition BuildRangedExpert()
    {
        return FeatDefinitionBuilder
            .Create(RangedExpertFeat, Guid)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(FeatureDefinitionBuilder
                .Create("FeatRangedExpertFeature", Guid)
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(
                    new RangedAttackInMeleeDisadvantageRemover(),
                    new AddExtraRangedAttack(IsOneHandedRanged, ActionDefinitions.ActionType.Bonus,
                        CharacterValidators.HasAttacked)
                )
                .AddToDB())
            .AddToDB();
    }

    private static FeatDefinition BuildRecklessAttack()
    {
        return FeatDefinitionWithPrerequisitesBuilder
            .Create(RecklessAttackFeat, Guid)
            .SetGuiPresentation("RecklessAttack", Category.Action)
            .SetFeatures(FeatureDefinitionActionAffinitys.ActionAffinityBarbarianRecklessAttack)
            .SetValidators(FeatsValidations.ValidateNotClass(CharacterClassDefinitions.Barbarian))
            .AddToDB();
    }

    private static bool IsOneHandedRanged(RulesetAttackMode mode, RulesetItem weapon, RulesetCharacter character)
    {
        return WeaponValidators.IsRanged(weapon) && WeaponValidators.IsOneHanded(weapon);
    }
}
