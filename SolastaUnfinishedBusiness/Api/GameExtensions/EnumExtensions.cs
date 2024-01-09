namespace SolastaUnfinishedBusiness.Api.GameExtensions;

internal enum ExtraReactionContext
{
    Custom = 9000
}

internal enum ExtraAncestryType
{
    WayOfTheDragon = 9000,
    PathOfTheElements = 9001
}

internal enum ExtraSituationalContext
{
    HasBladeMasteryWeaponTypesInHands = 1000,
    HasGreatswordInHands = 1001,
    HasLongswordInHands = 1002,
    HasSpecializedWeaponInHands = 1003,
    HasMeleeWeaponInMainHandWithFreeOffhand = 1004,
    IsNotInBrightLight = 1005,
    IsRagingAndDualWielding = 1006,
    MainWeaponIsMeleeOrUnarmedOrYeomanWithLongbow = 1007,
    NextToWallWithShieldAndMaxMediumArmorAndConsciousAllyNextToTarget = 1008,
    SummonerIsNextToBeast = 1009,
    TargetIsNotEffectSource = 1010,
    WearingNoArmorOrLightArmorWithoutShield = 1011,
    WearingNoArmorOrLightArmorWithTwoHandedQuarterstaff = 1012,
    TargetIsFavoriteEnemy = 1013,
    HasShieldInHands = 1014,
    HasSimpleOrMartialWeaponInHands = 1016,
    IsNotSourceOfCondition = 1017
}

internal enum ExtraCombatAffinityValueDetermination
{
    ConditionAmount = 1000,
    ConditionAmountIfFavoriteEnemy,
    ConditionAmountIfNotFavoriteEnemy
}

#if false
internal enum ExtraEffectFormType
{
    // Damage = EffectForm.EffectFormType.Damage,
    // Healing = EffectForm.EffectFormType.Healing,
    // Condition = EffectForm.EffectFormType.Condition,
    // LightSource = EffectForm.EffectFormType.LightSource,
    // Summon = EffectForm.EffectFormType.Summon,
    // Counter = EffectForm.EffectFormType.Counter,
    // TemporaryHitPoints = EffectForm.EffectFormType.TemporaryHitPoints,
    // Motion = EffectForm.EffectFormType.Motion,
    // SpellSlots = EffectForm.EffectFormType.SpellSlots,
    // Divination = EffectForm.EffectFormType.Divination,
    // ItemProperty = EffectForm.EffectFormType.ItemProperty,
    // Alteration = EffectForm.EffectFormType.Alteration,
    // Topology = EffectForm.EffectFormType.Topology,
    // Revive = EffectForm.EffectFormType.Revive,
    // Kill = EffectForm.EffectFormType.Kill,
    // ShapeChange = EffectForm.EffectFormType.ShapeChange,
    Custom = 9000
}
#endif

#if false
internal enum ExtraRitualCasting
{
    // None = RitualCasting.None,
    // Prepared = RitualCasting.Prepared,
    // Spellbook = RitualCasting.Spellbook,
    Known = 9000
}
#endif

internal enum ExtraMotionType
{
    // PushFromOrigin,
    // DragToOrigin,
    // TeleportToDestination,
    // Levitate,
    // PushFromWall,
    // FallProne,
    // SwapPositions,
    // Telekinesis,
    // RallyKindred,
    // PushRandomDirection,
    CustomSwap = 9000
}

internal enum ExtraPowerAttackHitComputation
{
    // Fixed = PowerAttackHitComputation.Fixed,
    // AbilityScore = PowerAttackHitComputation.AbilityScore,
    SpellAttack = 9000
}

internal enum ExtraOriginOfAmount
{
    // None = ConditionDefinition.OriginOfAmount.None,
    // SourceDamage = ConditionDefinition.OriginOfAmount.SourceDamage,
    // SourceGain = ConditionDefinition.OriginOfAmount.SourceGain,
    // AddDice = ConditionDefinition.OriginOfAmount.AddDice,
    // Fixed = ConditionDefinition.OriginOfAmount.Fixed,
    // SourceHalfHitPoints = ConditionDefinition.OriginOfAmount.SourceHalfHitPoints,
    // SourceSpellCastingAbility = ConditionDefinition.OriginOfAmount.SourceSpellCastingAbility,
    // SourceSpellAttack = ConditionDefinition.OriginOfAmount.SourceSpellAttack,
    SourceProficiencyBonus = 9000,
    SourceCharacterLevel = 9001,
    SourceClassLevel = 9002, //Class name should be in the `additionalDamageType` field of the condition
    SourceAbilityBonus = 9003, //Attribute name should be in the `additionalDamageType` field of the condition
    SourceProficiencyBonusNegative = 9004,

    SourceCopyAttributeFromSummoner =
        9005, //Attribute name should be in the `additionalDamageType` field of the condition
    SourceProficiencyAndAbilityBonus = 9006,
    SourceGambitDieRoll = 9007
}

internal enum ExtraAdditionalDamageTriggerCondition
{
    // AdvantageOrNearbyAlly = AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly,
    // SpendSpellSlot = AdditionalDamageTriggerCondition.SpendSpellSlot,
    // SpecificCharacterFamily = AdditionalDamageTriggerCondition.SpecificCharacterFamily,
    // TargetHasConditionCreatedByMe = AdditionalDamageTriggerCondition.TargetHasConditionCreatedByMe,
    // AlwaysActive = AdditionalDamageTriggerCondition.AlwaysActive,
    // TargetHasCondition = AdditionalDamageTriggerCondition.TargetHasCondition,
    // TargetIsWounded = AdditionalDamageTriggerCondition.TargetIsWounded,
    // TargetHasSenseType = AdditionalDamageTriggerCondition.TargetHasSenseType,
    // TargetHasCreatureTag = AdditionalDamageTriggerCondition.TargetHasCreatureTag,
    // RangeAttackFromHigherGround = AdditionalDamageTriggerCondition.RangeAttackFromHigherGround,
    // EvocationSpellDamage = AdditionalDamageTriggerCondition.EvocationSpellDamage,
    // TargetDoesNotHaveCondition = AdditionalDamageTriggerCondition.TargetDoesNotHaveCondition,
    // SpellDamageMatchesSourceAncestry = AdditionalDamageTriggerCondition.SpellDamageMatchesSourceAncestry,
    // CriticalHit = AdditionalDamageTriggerCondition.CriticalHit,
    // RagingAndTargetIsSpellcaster = AdditionalDamageTriggerCondition.RagingAndTargetIsSpellcaster,
    // Raging = AdditionalDamageTriggerCondition.Raging,
    // SpellDamagesTarget = AdditionalDamageTriggerCondition.SpellDamagesTarget,
    // NotWearingHeavyArmor = AdditionalDamageTriggerCondition.NotWearingHeavyArmor,

    TargetWithin10Ft = 9000,
    TargetIsDuelingWithYou = 9002,
    FlurryOfBlows = 9003
}

public enum ExtraConditionInterruption
{
    //Interrupts after attack was made against this target, unlike `ConditionInterruption.Attacked` that triggers at the very start
    AfterWasAttacked = 9000,
    AttacksWithWeaponOrUnarmed = 9001,
    AttackedNotBySource = 9002,
    UsesBonusAction = 9003
}

#if false
internal enum ExtraTurnOccurenceType
{
    StartOfTurn = TurnOccurenceType.StartOfTurn,
    EndOfTurn = TurnOccurenceType.EndOfTurn,
    EndOfTurnNoPerceptionOfSource = TurnOccurenceType.EndOfTurnNoPerceptionOfSource,
    StartOfTurnWithPerceptionOfSource = 9000,
    OnMoveEnd = 9001
}
#endif

internal enum ExtraAdditionalDamageAdvancement
{
    // None = AdditionalDamageAdvancement.None,
    // ClassLevel = AdditionalDamageAdvancement.ClassLevel,
    // SlotLevel = AdditionalDamageAdvancement.SlotLevel,
    CharacterLevel = 9000,
    ConditionAmount
}

internal enum ExtraAdditionalDamageValueDetermination
{
    FlatWithProgression = 1000,
    CustomModifier = 1001,
    CharacterLevel = 1002
}

#if false
internal enum ExtraAdvancementDuration
{
    // None = AdvancementDuration.None,
    // Hours_1_8_24 = AdvancementDuration.Hours_1_8_24,
    // Minutes_1_10_480_1440_Infinite = AdvancementDuration.Minutes_1_10_480_1440_Infinite,
    // ReSharper disable once InconsistentNaming
    Minutes_1_10_480_1440_Level2 = -9000,

    // ReSharper disable once InconsistentNaming
    Minutes_1_10_480_1440_Level3 = -9001,

    // ReSharper disable once InconsistentNaming
    Minutes_1_10_480_1440_Level4 = -9002,

    // ReSharper disable once InconsistentNaming
    Minutes_1_10_480_1440_Level5 = -9003
}
#endif

internal enum ExtraActionId
{
    CastInvocationBonus = 9000,
    CastInvocationNoCost = 9001,
    CastPlaneMagicMain = 9002,
    CastPlaneMagicBonus = 9003,
    InventorInfusion = 9004,
    TacticianGambitMain = 9005,
    TacticianGambitBonus = 9006,
    TacticianGambitNoCost = 9007,
    PushedCustom = 9008,
    FarStep = 9009,
    BondOfTheTalismanTeleport = 9010,
    DoNothingFree = 9011,
    DoNothingReaction = 9012,
    MonkKiPointsToggle = 9013,
    PaladinSmiteToggle = 9014,
    AudaciousWhirlToggle = 9015,
    CombatWildShape = 9016,
    FeatCrusherToggle = 9017,
    CannonFlamethrower = 9018,
    CannonForceBallista = 9019,
    CannonProtector = 9020,
    CombatRageStart = 9021,
    CastSpellMasteryMain = 9022,
    CastSignatureSpellsMain = 9023,
    QuiveringPalmToggle = 9024,
    CannonFlamethrowerBonus = 9025,
    CannonForceBallistaBonus = 9026,
    CannonProtectorBonus = 9027,
    TempestFury = 9028,
    MasterfulWhirlToggle = 9029,
    UseHeroicInspiration = 9030,
    FlightSuspend = 9031,
    FlightResume = 9032,
    ArcaneArcherToggle = 9033,
    CunningStrikeToggle = 9034,
    Withdraw = 9035,
    HailOfBladesToggle = 9036,
    EldritchVersatilityMain = 9037,
    EldritchVersatilityBonus = 9038,
    EldritchVersatilityNoCost = 9039,
    MindSculptToggle = 9040,
    SupremeWillToggle = 9041,
    CrystalDefenseOn = 9042,
    CrystalDefenseOff = 9043,
    WildlingFeralAgility = 9044,
    ImpishWrathToggle = 9045,
    HailOfArrows = 9046,
    CompellingStrikeToggle = 9047,
    PressTheAdvantageToggle = 9048,
    CoordinatedAssaultToggle = 9049,
    DyingLightToggle = 9050,
    PrioritizeAction = 10000
}

#if false
internal static class EnumImplementation
{
    internal static bool ComputeExtraAdvancementDuration(
        [NotNull] EffectDescription effect,
        int slotLevel,
        ref int result,
        ref DurationType durationType)
    {
        if (effect.EffectAdvancement.AlteredDuration >= 0)
        {
            return true;
        }

        var alteredDuration = (ExtraAdvancementDuration)effect.EffectAdvancement.AlteredDuration;

        var duration = alteredDuration switch
        {
            ExtraAdvancementDuration.Minutes_1_10_480_1440_Level2 => slotLevel switch
            {
                <= 2 => ComputeRoundsDuration(DurationType.Minute, 1),
                3 => ComputeRoundsDuration(DurationType.Minute, 10),
                4 => ComputeRoundsDuration(DurationType.Hour, 1),
                _ => ComputeRoundsDuration(DurationType.Hour, 8)
            },
            ExtraAdvancementDuration.Minutes_1_10_480_1440_Level3 => slotLevel switch
            {
                <= 3 => ComputeRoundsDuration(DurationType.Minute, 1),
                4 => ComputeRoundsDuration(DurationType.Minute, 10),
                5 => ComputeRoundsDuration(DurationType.Hour, 1),
                _ => ComputeRoundsDuration(DurationType.Hour, 8)
            },
            ExtraAdvancementDuration.Minutes_1_10_480_1440_Level4 => slotLevel switch
            {
                <= 4 => ComputeRoundsDuration(DurationType.Minute, 1),
                5 => ComputeRoundsDuration(DurationType.Minute, 10),
                6 => ComputeRoundsDuration(DurationType.Hour, 1),
                _ => ComputeRoundsDuration(DurationType.Hour, 8)
            },
            ExtraAdvancementDuration.Minutes_1_10_480_1440_Level5 => slotLevel switch
            {
                <= 5 => ComputeRoundsDuration(DurationType.Minute, 1),
                6 => ComputeRoundsDuration(DurationType.Minute, 10),
                7 => ComputeRoundsDuration(DurationType.Hour, 1),
                _ => ComputeRoundsDuration(DurationType.Hour, 8)
            },
            _ => -1
        };

        if (duration == -1)
        {
            return true;
        }

        result = duration;
        durationType = DurationType.Round;

        return false;
    }
}
#endif
