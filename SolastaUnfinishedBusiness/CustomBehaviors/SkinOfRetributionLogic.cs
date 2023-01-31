using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

public class SkinOfRetributionLogic
{
    private const string ConditionName = "ConditionSkinOfRetribution";
    public const int TempHpPerLevel = 5;

    private static ConditionDefinition _condition;

    private SkinOfRetributionLogic() { }

    private static ConditionDefinition Condition => _condition ??= BuildCondition();

    private static SkinOfRetributionLogic Marker { get; } = new();

    private static ConditionDefinition BuildCondition()
    {
        var spriteReference = Sprites.GetSprite("ConditionMirrorImage", Resources.ConditionMirrorImage, 32);

        return ConditionDefinitionBuilder
            .Create(ConditionName)
            .SetGuiPresentation(Category.Spell, spriteReference)
            .SetCustomSubFeatures(Marker)
            .SetSilent(Silent.WhenAdded)
            .SetPossessive()
            .SetFeatures(BuildFeatures())
            .AddToDB();
    }

    private static FeatureDefinition[] BuildFeatures()
    {
        var powerSkinOfRetribution = FeatureDefinitionPowerBuilder
            .Create("PowerSkinOfRetribution")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeCold, TempHpPerLevel)
                            .Build())
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, additionalDicePerIncrement:TempHpPerLevel)
                    .Build())
            .SetUniqueInstance()
            .AddToDB();

        var featureSkinOfRetributionHp = FeatureDefinitionBuilder
            .Create("FeatureSkinOfRetributionHp")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(SkinProvider.Mark)
            .AddToDB();

        var damageSkinOfRetribution = FeatureDefinitionDamageAffinityBuilder
            .Create("DamageAffinitySkinOfRetribution")
            .SetGuiPresentationNoContent(true)
            .SetDamageAffinityType(DamageAffinityType.None)
            .SetRetaliate(powerSkinOfRetribution, 1, true)
            .AddToDB();

        return new[] { featureSkinOfRetributionHp, damageSkinOfRetribution };
    }

    private static List<RulesetCondition> GetConditions(RulesetActor character)
    {
        var conditions = new List<RulesetCondition>();

        if (character == null)
        {
            return conditions;
        }

        character.GetAllConditions(conditions);

        return conditions.FindAll(c =>
                c.ConditionDefinition.HasSubFeatureOfType<SkinOfRetributionLogic>())
            .ToList();
    }

    internal static void AttackRollPostfix(
        RulesetActor target)
    {
        var conditions = GetConditions(target as RulesetCharacter);

        foreach (var condition in conditions
                     .Where(_ => ((RulesetCharacter)target).temporaryHitPoints == 0))
        {
            target.RemoveCondition(condition);
        }
    }


    internal class SkinProvider : ICustomConditionFeature
    {
        private SkinProvider()
        {
        }

        public static ICustomConditionFeature Mark { get; } = new SkinProvider();

        public void ApplyFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            var condition = RulesetCondition.CreateActiveCondition(
                target.Guid, Condition, DurationType.Hour, 1,
                TurnOccurenceType.EndOfTurn, target.Guid, target.CurrentFaction.Name);

            target.AddConditionOfCategory(AttributeDefinitions.TagEffect, condition);
        }

        public void RemoveFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            var conditions = GetConditions(target);

            foreach (var condition in conditions)
            {
                target.RemoveCondition(condition);
            }
        }
    }
}
