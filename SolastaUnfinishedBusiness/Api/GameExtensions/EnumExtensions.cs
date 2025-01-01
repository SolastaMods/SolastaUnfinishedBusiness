namespace SolastaUnfinishedBusiness.Api.GameExtensions;

internal enum ExtraActionId
{
    ArcaneArcherToggle = 9000,
    AmazingDisplayToggle,
    AudaciousWhirlToggle,
    BalefulScionToggle,
    BondOfTheTalismanTeleport,
    BrutalStrikeToggle,
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
    CombatRageStart,
    CombatWildShape,
    CompellingStrikeToggle,
    CoordinatedAssaultToggle,
    CrystalDefenseOff,
    CrystalDefenseOn,
    CunningStrikeToggle,
    DestructiveWrathToggle,
    DoNothingFree,
    DoNothingReaction,
    DragonHideToggle,
    DyingLightToggle,
    EldritchVersatilityBonus,
    EldritchVersatilityMain,
    EldritchVersatilityNoCost,
    FarStep,
    FeatCrusherToggle,
    FlightResume,
    FlightSuspend,
    ForcePoweredStrikeToggle,
    GloomBladeToggle,
    HailOfArrows,
    HailOfBladesToggle,
    ImpishWrathToggle,
    InventorInfusion,
    MasterfulWhirlToggle,
    MindSculptToggle,
    OrcishFuryToggle,
    PaladinSmiteToggle,
    PowerSurgeToggle,
    PressTheAdvantageToggle,
    ProxyDawn,
    ProxyHoundWeapon,
    ProxyPactWeapon,
    ProxyPactWeaponFree,
    ProxyPetalStorm,
    PushedCustom,
    QuiveringPalmToggle,
    SupremeWillToggle,
    TacticianGambitBonus,
    TacticianGambitMain,
    TacticianGambitNoCost,
    ThunderousStrikeToggle,
    TidesOfChaosRecharge,
    UseHeroicInspiration,
    WildlingFeralAgility,
    WildSurgeBolt,
    WildSurgeBoltFree,
    WildSurgeSummon,
    WildSurgeSummonFree,
    WildSurgeTeleport,
    WildSurgeTeleportFree,
    WildSurgeReroll,
    CastQuickened,
    ZenShotToggle,
    Grapple,
    GrappleBonus,
    DisableGrapple,
    GrappleOnUnarmedToggle,
    CleavingAttackToggle,
    PowerAttackToggle,
    DeadEyeToggle,
    OverChannelToggle,
    GravityWellToggle,
    GrappleNoCost,
    ProxyDarkness,
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
    TargetIsWithin10Ft,
    SourceOrTargetAreNotBright,
    SourceIsSneakingAttack
}

internal enum ExtraAdditionalDamageValueDetermination
{
    CustomModifier = 9001,
    FlatWithProgression
}

internal enum ExtraAncestryType
{
    PathOfTheElements = 9000,
    WayOfTheDragon,
    CollegeOfAudacityDefensiveWhirl
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
    AfterWasAttackedNotBySource,
    AttacksWithWeaponOrUnarmed,
    SourceRageStop,
    UsesBonusAction,
    AttacksWithMeleeAndDamages,
    SpendPower,
    SpendPowerExecuted
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
    CustomSwap = 9000,
    PushDown = 9001
}

internal enum ExtraEffectType
{
    RecoverSorceryHalfLevelDown = 9000
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
    SourceClassLevel = 9002, // class name should be in the `additionalDamageType` field of the condition
    SourceCopyAttributeFromSummoner, // attribute name should be in the `additionalDamageType` field of the condition
    SourceGambitDieRoll,
    SourceProficiencyAndAbilityBonus,
    SourceProficiencyBonus
}

internal enum ExtraSituationalContext
{
    HasBladeMasteryWeaponTypesInHands = 9000,
    HasGreatswordInHands = 9001,
    HasLongswordInHands = 9002,
    HasMeleeWeaponInMainHandAndFreeOffhand = 9003,
    HasSimpleOrMartialWeaponInHands = 9005,
    IsNotInBrightLight = 9007,
    IsRagingAndDualWielding = 9008,
    AttackerWithMeleeOrUnarmedAndTargetWithinReachOrYeomanWithLongbow = 9009,
    NextToWallWithShieldAndMaxMediumArmorAndConsciousAllyNextToTarget = 9010,
    TargetIsFavoriteEnemy = 9012,
    IsNotConditionSource = 9013,
    WearingNoArmorOrLightArmorWithoutShield = 9014,
    WearingNoArmorOrLightArmorWithTwoHandedQuarterstaff = 9015,
    IsNotConditionSourceNotRanged = 9016,
    IsConcentratingOnSpell = 9017,
    IsConditionSource = 9018,
    HasMonkMeleeWeaponInMainHand = 9019
}

internal enum ExtraTurnOccurenceType
{
    // StartOfTurn = RuleDefinitions.TurnOccurenceType.StartOfTurn,
    // EndOfTurn = RuleDefinitions.TurnOccurenceType.EndOfTurn,
    // EndOfTurnNoPerceptionOfSource = RuleDefinitions.TurnOccurenceType.EndOfTurnNoPerceptionOfSource,
    // EndOfSourceTurn = RuleDefinitions.TurnOccurenceType.EndOfSourceTurn,
    StartOfSourceTurn = 9000
}
