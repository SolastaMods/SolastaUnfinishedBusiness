using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

internal sealed class FeatureDefinitionReduceDamage : FeatureDefinition
{
    public RuleDefinitions.AdditionalDamageTriggerCondition TriggerCondition { get; set; }
    public int ReducedDamage { get; set; }
    public List<string> DamageTypes { get; set; }
    public string NotificationTag { get; set; }
    public CharacterClassDefinition SpellCastingClass { get; set; }
}
