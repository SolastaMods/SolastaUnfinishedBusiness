namespace SolastaUnfinishedBusiness.CustomDefinitions;

internal sealed class FeatureDefinitionReduceDamage : FeatureDefinition
{
    public RuleDefinitions.AdditionalDamageTriggerCondition TriggerCondition { get; set; }
    public int ReducedDamage { get; set; }
    public string DamageType { get; set; }
    public string NotificationTag { get; set; }
    public CharacterClassDefinition SpellCastingClass { get; set; }
}
