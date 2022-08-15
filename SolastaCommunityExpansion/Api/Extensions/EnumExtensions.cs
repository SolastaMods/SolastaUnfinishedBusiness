namespace SolastaCommunityExpansion.Api.Extensions;

public enum ExtraEffectFormType
{
    Damage = EffectForm.EffectFormType.Damage,
    Healing = EffectForm.EffectFormType.Healing,
    Condition = EffectForm.EffectFormType.Condition,
    LightSource = EffectForm.EffectFormType.LightSource,
    Summon = EffectForm.EffectFormType.Summon,
    Counter = EffectForm.EffectFormType.Counter,
    TemporaryHitPoints = EffectForm.EffectFormType.TemporaryHitPoints,
    Motion = EffectForm.EffectFormType.Motion,
    SpellSlots = EffectForm.EffectFormType.SpellSlots,
    Divination = EffectForm.EffectFormType.Divination,
    ItemProperty = EffectForm.EffectFormType.ItemProperty,
    Alteration = EffectForm.EffectFormType.Alteration,
    Topology = EffectForm.EffectFormType.Topology,
    Revive = EffectForm.EffectFormType.Revive,
    Kill = EffectForm.EffectFormType.Kill,
    ShapeChange = EffectForm.EffectFormType.ShapeChange,
    Custom = 9000
}

public enum ExtraRitualCasting
{
    None = RuleDefinitions.RitualCasting.None,
    Prepared = RuleDefinitions.RitualCasting.Prepared,
    Spellbook = RuleDefinitions.RitualCasting.Spellbook,
    Known = 9000
}

public enum ExtraOriginOfAmount
{
    None = ConditionDefinition.OriginOfAmount.None,
    SourceDamage = ConditionDefinition.OriginOfAmount.SourceDamage,
    SourceGain = ConditionDefinition.OriginOfAmount.SourceGain,
    AddDice = ConditionDefinition.OriginOfAmount.AddDice,
    Fixed = ConditionDefinition.OriginOfAmount.Fixed,
    SourceHalfHitPoints = ConditionDefinition.OriginOfAmount.SourceHalfHitPoints,
    SourceSpellCastingAbility = ConditionDefinition.OriginOfAmount.SourceSpellCastingAbility,
    SourceSpellAttack = ConditionDefinition.OriginOfAmount.SourceSpellAttack,
    SourceProficiencyBonus = 9000,
    SourceCharacterLevel = 9001,
    SourceClassLevel = 9002
}

public enum ExtraAttributeModifierOperation
{
    Set = FeatureDefinitionAttributeModifier.AttributeModifierOperation.Set,
    Additive = FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
    Multiplicative = FeatureDefinitionAttributeModifier.AttributeModifierOperation.Multiplicative,
    MultiplyByClassLevel = FeatureDefinitionAttributeModifier.AttributeModifierOperation.MultiplyByClassLevel,
    MultiplyByCharacterLevel = FeatureDefinitionAttributeModifier.AttributeModifierOperation.MultiplyByCharacterLevel,
    Force = FeatureDefinitionAttributeModifier.AttributeModifierOperation.Force,
    AddAbilityScoreBonus = FeatureDefinitionAttributeModifier.AttributeModifierOperation.AddAbilityScoreBonus,
    ConditionAmount = FeatureDefinitionAttributeModifier.AttributeModifierOperation.ConditionAmount,
    SurroundingEnemies = FeatureDefinitionAttributeModifier.AttributeModifierOperation.SurroundingEnemies,
    AdditiveAtEnd = 9000
}

public enum ExtraTurnOccurenceType
{
    StartOfTurn = RuleDefinitions.TurnOccurenceType.StartOfTurn,
    EndOfTurn = RuleDefinitions.TurnOccurenceType.EndOfTurn,
    EndOfTurnNoPerceptionOfSource = RuleDefinitions.TurnOccurenceType.EndOfTurnNoPerceptionOfSource,
    StartOfTurnWithPerceptionOfSource = 9000,
    OnMoveEnd = 9001
}

public enum ExtraAdditionalDamageAdvancement
{
    None = RuleDefinitions.AdditionalDamageAdvancement.None,
    ClassLevel = RuleDefinitions.AdditionalDamageAdvancement.ClassLevel,
    SlotLevel = RuleDefinitions.AdditionalDamageAdvancement.SlotLevel,
    CharacterLevel = 9000
}

public enum ExtraAdvancementDuration
{
    None = RuleDefinitions.AdvancementDuration.None,

    // ReSharper disable once InconsistentNaming
    Hours_1_8_24 = RuleDefinitions.AdvancementDuration.Hours_1_8_24,

    // ReSharper disable once InconsistentNaming
    Minutes_1_10_480_1440_Infinite = RuleDefinitions.AdvancementDuration.Minutes_1_10_480_1440_Infinite,
    DominateBeast = 9000,
    DominatePerson = 9001,
    DominateMonster = 9002
}
