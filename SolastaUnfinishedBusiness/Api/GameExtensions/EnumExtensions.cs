namespace SolastaUnfinishedBusiness.Api.GameExtensions;

internal enum ExtraActionId
{
    ArcaneArcherToggle = 9000,
    AudaciousWhirlToggle,
    BondOfTheTalismanTeleport,
    CannonFlamethrower,
    CannonFlamethrowerBonus,
    CannonForceBallista,
    CannonForceBallistaBonus,
    CannonProtector,
    CannonProtectorBonus,
    CastInvocationBonus,
    CastInvocationNoCost,
    CastPlaneMagicBonus,
    CastPlaneMagicMain,
    CastSignatureSpellsMain,
    CastSpellMasteryMain,
    CombatRageStart,
    CombatWildShape,
    CompellingStrikeToggle,
    CoordinatedAssaultToggle,
    CrystalDefenseOff,
    CrystalDefenseOn,
    CunningStrikeToggle,
    DoNothingFree,
    DoNothingReaction,
    DyingLightToggle,
    EldritchVersatilityBonus,
    EldritchVersatilityMain,
    EldritchVersatilityNoCost,
    FarStep,
    FeatCrusherToggle,
    FlightResume,
    FlightSuspend,
    HailOfArrows,
    HailOfBladesToggle,
    ImpishWrathToggle,
    InventorInfusion,
    MasterfulWhirlToggle,
    MindSculptToggle,
    PaladinSmiteToggle,
    PressTheAdvantageToggle,
    PushedCustom,
    QuiveringPalmToggle,
    SupremeWillToggle,
    TacticianGambitBonus,
    TacticianGambitMain,
    TacticianGambitNoCost,
    TempestFury,
    UseHeroicInspiration,
    WildlingFeralAgility,
    Withdraw,
    PrioritizeAction = 10000
}

internal enum ExtraAdditionalDamageAdvancement
{
    CharacterLevel = 9000,
    ConditionAmount
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
    FlurryOfBlows = 9000,
    TargetIsDuelingWithYou,
    TargetWithin10Ft
}

internal enum ExtraAdditionalDamageValueDetermination
{
    CharacterLevel = 9000,
    CustomModifier,
    FlatWithProgression
}

internal enum ExtraAncestryType
{
    PathOfTheElements = 9000,
    WayOfTheDragon
}

internal enum ExtraCombatAffinityValueDetermination
{
    ConditionAmount = 9000,
    ConditionAmountIfFavoriteEnemy,
    ConditionAmountIfNotFavoriteEnemy
}

public enum ExtraConditionInterruption
{
    AfterWasAttacked = 9000,
    AttackedNotBySource,
    AttacksWithWeaponOrUnarmed,
    SourceRageStop,
    UsesBonusAction
}

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
    SourceAbilityBonus = 9000, // attribute name should be in the `additionalDamageType` field of the condition
    SourceCharacterLevel,
    SourceClassLevel, // class name should be in the `additionalDamageType` field of the condition
    SourceCopyAttributeFromSummoner, // attribute name should be in the `additionalDamageType` field of the condition
    SourceGambitDieRoll,
    SourceProficiencyAndAbilityBonus,
    SourceProficiencyBonus,
    SourceProficiencyBonusNegative
}

internal enum ExtraReactionContext
{
    Custom = 9000
}

internal enum ExtraSituationalContext
{
    HasBladeMasteryWeaponTypesInHands = 9000,
    HasGreatswordInHands = 9001,
    HasLongswordInHands = 9002,
    HasMeleeWeaponInMainHandWithFreeOffhand = 9003,
    HasShieldInHands = 9004,
    HasSimpleOrMartialWeaponInHands = 9005,
    HasSpecializedWeaponInHands = 9006,
    IsNotInBrightLight = 9007,
    IsRagingAndDualWielding = 9008,
    MainWeaponIsMeleeOrUnarmedOrYeomanWithLongbow = 9009,
    NextToWallWithShieldAndMaxMediumArmorAndConsciousAllyNextToTarget = 9010,
    SummonerIsNextToBeast = 9011,
    TargetIsFavoriteEnemy = 9012,
    IsNotConditionSource = 9013,
    WearingNoArmorOrLightArmorWithoutShield = 9014,
    WearingNoArmorOrLightArmorWithTwoHandedQuarterstaff = 9015,
    IsNotConditionSourceWithSimpleOrMartialWeaponInHands = 9016
}

internal enum ExtraTurnOccurenceType
{
    // StartOfTurn = RuleDefinitions.TurnOccurenceType.StartOfTurn,
    // EndOfTurn = RuleDefinitions.TurnOccurenceType.EndOfTurn,
    // EndOfTurnNoPerceptionOfSource = RuleDefinitions.TurnOccurenceType.EndOfTurnNoPerceptionOfSource,
    // EndOfSourceTurn = RuleDefinitions.TurnOccurenceType.EndOfSourceTurn,
    StartOfSourceTurn = 9000
}
