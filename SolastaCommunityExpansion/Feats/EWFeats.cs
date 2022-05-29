using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using static SolastaModApi.DatabaseHelper;

namespace SolastaCommunityExpansion.Feats;

public static class EWFeats
{
    private static readonly Guid GUID = new("B4ED480F-2D06-4EB1-8732-9A721D80DD1A");

    public static void CreateFeats(List<FeatDefinition> feats)
    {
        feats.Add(FeatDefinitionBuilder
            .Create("FeatSentinel", GUID)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(FeatureDefinitionOnAttackHitEffectBuilder
                .Create("FeatSentinelFeature", GUID)
                .SetGuiPresentationNoContent(true)
                .SetOnAttackHitDelegates(null, (attacker, defender, outcome, actionParams, mode, modifier) =>
                {
                    var character = defender.RulesetCharacter;
                    character.AddConditionOfCategory(AttributeDefinitions.TagCombat,
                        RulesetCondition.CreateActiveCondition(character.Guid,
                            ConditionDefinitions.ConditionRestrained, RuleDefinitions.DurationType.Round,
                            1,
                            RuleDefinitions.TurnOccurenceType.StartOfTurn,
                            character.Guid,
                            character.CurrentFaction.Name
                        ));
                })
                .SetCustomSubFeatures(
                    AttacksOfOpportunity.CanIgnoreDisengage,
                    AttacksOfOpportunity.SentinelFeatMarker//,
                    // new AddEffectToWeaponAttack(new EffectFormBuilder()
                    //     .SetConditionForm(ConditionDefinitions.ConditionRestrained, ConditionForm.ConditionOperation.Add)
                    //     .Build(), WeaponValidators.IsReactionAttack)
                    // new AddEffectToWeaponAttack(new EffectFormBuilder()
                    //     .SetDamageForm(dieType: RuleDefinitions.DieType.D4, diceNumber: 10,
                    //         damageType: RuleDefinitions.DamageTypeRadiant, bonusDamage:5)
                    //     .Build(), WeaponValidators.IsReactionAttack)
                )
                .AddToDB())
            .AddToDB());
    }
}