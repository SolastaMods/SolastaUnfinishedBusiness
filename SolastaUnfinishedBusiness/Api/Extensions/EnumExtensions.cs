using JetBrains.Annotations;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Api.Extensions;

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
    None = RitualCasting.None,
    Prepared = RitualCasting.Prepared,
    Spellbook = RitualCasting.Spellbook,
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

#if false
public enum ExtraTurnOccurenceType
{
    StartOfTurn = TurnOccurenceType.StartOfTurn,
    EndOfTurn = TurnOccurenceType.EndOfTurn,
    EndOfTurnNoPerceptionOfSource = TurnOccurenceType.EndOfTurnNoPerceptionOfSource,
    StartOfTurnWithPerceptionOfSource = 9000,
    OnMoveEnd = 9001
}
#endif

public enum ExtraAdditionalDamageAdvancement
{
    None = AdditionalDamageAdvancement.None,
    ClassLevel = AdditionalDamageAdvancement.ClassLevel,
    SlotLevel = AdditionalDamageAdvancement.SlotLevel,
    CharacterLevel = 9000
}

public enum ExtraAdvancementDuration
{
    None = AdvancementDuration.None,

    // ReSharper disable once InconsistentNaming
    Hours_1_8_24 = AdvancementDuration.Hours_1_8_24,

    // ReSharper disable once InconsistentNaming
    Minutes_1_10_480_1440_Infinite = AdvancementDuration.Minutes_1_10_480_1440_Infinite,
    DominateBeast = 9000,
    DominatePerson = 9001,
    DominateMonster = 9002
}

internal static class EnumImplementation
{
    internal static bool ComputeExtraAdvancementDuration([NotNull] EffectDescription effect, int slotLevel,
        ref int result)
    {
        //
        // BUGFIX: dominate spells
        //

        if (effect.EffectAdvancement.AlteredDuration >= 0)
        {
            // use standard calculation
            return true;
        }

        var alteredDuration = (ExtraAdvancementDuration)effect.EffectAdvancement.AlteredDuration;

        var duration = alteredDuration switch
        {
            // TA DominateBeast and DominatePerson use AdvancementDuration.Minutes_1_10_480_1440_Infinite
            // which is only computed correctly for BestowCurse.

            ExtraAdvancementDuration.DominateBeast => slotLevel switch
            {
                <= 4 => ComputeRoundsDuration(DurationType.Minute, 1),
                5 => ComputeRoundsDuration(DurationType.Minute, 10),
                6 => ComputeRoundsDuration(DurationType.Hour, 1),
                _ => ComputeRoundsDuration(DurationType.Hour, 8)
            },
            ExtraAdvancementDuration.DominatePerson => slotLevel switch
            {
                <= 5 => ComputeRoundsDuration(DurationType.Minute, 1),
                6 => ComputeRoundsDuration(DurationType.Minute, 10),
                7 => ComputeRoundsDuration(DurationType.Hour, 1),
                _ => ComputeRoundsDuration(DurationType.Hour, 8)
            },
            ExtraAdvancementDuration.DominateMonster => slotLevel switch // currently a DubHerder CE specific spell
            {
                <= 8 => ComputeRoundsDuration(DurationType.Hour, 1),
                _ => ComputeRoundsDuration(DurationType.Hour, 8)
            },
            _ => -1
        };

        if (duration == -1)
        {
            return true;
        }

        result = duration;

        return false;
    }
}
