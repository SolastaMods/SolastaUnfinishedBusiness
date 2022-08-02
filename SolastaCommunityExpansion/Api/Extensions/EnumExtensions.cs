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
    None = 0,
    SourceDamage = 1,
    SourceGain = 2,
    AddDice = 3,
    Fixed = 4,
    SourceHalfHitPoints = 5,
    SourceSpellCastingAbility = 6,
    SourceSpellAttack = 7,
    SourceProficiencyBonus = 9000,
    SourceCharacterLevel = 9001,
    SourceClassLevel = 9002
}

public enum ExtraAttributeModifierOperation
{
    Set = 0,
    Additive = 1,
    Multiplicative = 2,
    MultiplyByClassLevel = 3,
    MultiplyByCharacterLevel = 4,
    Force = 5,
    AddAbilityScoreBonus = 6,
    ConditionAmount = 7,
    SurroundingEnemies = 8,
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
