using System;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.GameExtensions;

namespace SolastaUnfinishedBusiness.Definitions;

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

    public static int DamageReduction(
        RulesetImplementationDefinitions.ApplyFormsParams formsParams,
        int damage,
        string damageType)
    {
        var defender = formsParams.targetCharacter;
        var reduction = 0;

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

                if (reduction + tmp > damage)
                {
                    tmp = reduction + tmp - damage;
                    formsParams.sourceTags.Add(prefix + tmp);
                    reduction = damage;
                    break;
                }

                reduction += tmp;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        return reduction;
    }
}
