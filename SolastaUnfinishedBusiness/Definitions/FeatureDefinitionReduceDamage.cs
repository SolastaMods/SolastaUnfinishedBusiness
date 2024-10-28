using System;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors.Specific;

// ReSharper disable once CheckNamespace
internal delegate int ReducedDamageHandler(
    GameLocationCharacter attacker,
    GameLocationCharacter defender);

internal sealed class FeatureDefinitionReduceDamage : FeatureDefinition
{
    public RuleDefinitions.AdditionalDamageTriggerCondition TriggerCondition { get; set; }
    public ReducedDamageHandler ReducedDamage { get; set; }
    public List<string> DamageTypes { get; set; }
    public string NotificationTag { get; set; }
    public CharacterClassDefinition SpellCastingClass { get; set; }

    // for powers that reduce based on a point count
    public FeatureDefinitionPower FeedbackPower { get; set; }

    // for feedback powers caused by an external effect
    public ConditionDefinition SourceCondition { get; set; }

    public static int DamageReduction(
        RulesetImplementationDefinitions.ApplyFormsParams formsParams,
        int damage,
        string damageType)
    {
        var defender = formsParams.targetCharacter;
        var reduction = 0;

        if (defender is not { Side: RuleDefinitions.Side.Ally })
        {
            return reduction;
        }

        foreach (var feature in defender.GetFeaturesByType<FeatureDefinitionReduceDamage>())
        {
            if (feature.DamageTypes != null
                && feature.DamageTypes.Count != 0
                && !feature.DamageTypes.Contains(damageType))
            {
                continue;
            }

            var prefix = $"{feature.Name}:{defender.Guid}:";
            var k = formsParams.sourceTags.FindIndex(x => x.StartsWith(prefix));

            if (k < 0)
            {
                continue;
            }

            var tag = formsParams.sourceTags[k];

            formsParams.sourceTags.RemoveAt(k);

            try
            {
                var tmp = int.Parse(tag.Split(':')[2]);
                var tmp2 = tmp;

                if (reduction + tmp > damage)
                {
                    tmp = reduction + tmp - damage;
                    formsParams.sourceTags.Add(prefix + tmp);
                    reduction = damage;
                }
                else
                {
                    reduction += tmp;
                    tmp = 0;
                }

                // need double if check here to avoid issues with unity objects lifecycle
                if (feature.FeedbackPower)
                {
                    if (defender is RulesetCharacter sourceRulesetCharacter)
                    {
                        sourceRulesetCharacter = sourceRulesetCharacter.TryGetConditionOfCategoryAndType(
                            AttributeDefinitions.TagEffect,
                            feature.SourceCondition?.Name,
                            out var activeCondition)
                            ? EffectHelpers.GetCharacterByGuid(activeCondition.SourceGuid)
                            : sourceRulesetCharacter;

                        sourceRulesetCharacter.UpdateUsageForPower(feature.FeedbackPower, tmp2 - tmp);
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        return reduction;
    }
}
