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

    internal static ConditionDefinition Condition => _condition ??= BuildCondition();

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
            .SetGuiPresentationNoContent()
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeCold, bonusDamage: TempHpPerLevel)
                            .Build())
                    .SetEffectAdvancement(EffectIncrementMethod.PerAdditionalSlotLevel, TempHpPerLevel)
                    .Build())
            .SetUniqueInstance()
            .AddToDB();

        var feature1 = FeatureDefinitionBuilder
            .Create("FeatureSkinOfRetributionHp")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(SkinProvider.Mark)
            .AddToDB();

        FeatureDefinition feature2 = FeatureDefinitionDamageAffinityBuilder
            .Create("DamageSkinOfRetribution")
            .SetGuiPresentationNoContent()
            .SetDamageAffinityType(DamageAffinityType.None)
            .SetRetaliate(powerSkinOfRetribution, 1, true)
            .AddToDB();


        FeatureDefinition[] features = { feature1, feature2 };

        return features;
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
        RulesetCharacter attacker,
        RulesetActor target)
    {
        var conditions = GetConditions(target as RulesetCharacter);

        foreach (var condition in conditions)
        {
            if (((RulesetCharacter)target).temporaryHitPoints == 0)
            {
                target.RemoveCondition(condition);
            }
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
