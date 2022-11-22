using System;
using System.Collections;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using TA;
using UnityEngine.AddressableAssets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class MartialTactician : AbstractSubclass
{
    private static readonly LimitEffectInstances GambitLimiter = new("Gambit", _ => 1);

    private static readonly DamageDieProvider UpgradeDice = (character, _) => GetGambitDieSize(character);

    private static int _gambitPoolIncreases;

    internal MartialTactician()
    {
        BuildGambitPool();

        GambitDieDamage = BuildGambitDieDamage("");

        //make sure that if we add any custom sub-features to base one we add them to this one too
        GambitDieDamageOnce = BuildGambitDieDamage("Once", FeatureLimitedUsage.OncePerTurn);

        var learn2Gambits = BuildLearn(2);
        var learn3Gambits = BuildLearn(3);
        var unlearn = BuildUnlearn();

        EverVigilant = BuildEverVigilant();
        Subclass = CharacterSubclassDefinitionBuilder
            .Create("MartialTactician")
            .SetGuiPresentation(Category.Subclass, RoguishShadowCaster)
            .AddFeaturesAtLevel(3, BuildSharpMind(), GambitPool, learn3Gambits, EverVigilant)
            .AddFeaturesAtLevel(5, BuildGambitDieSize(DieType.D8))
            .AddFeaturesAtLevel(7, BuildGambitPoolIncrease(), learn2Gambits, unlearn, BuildSharedVigilance())
            .AddFeaturesAtLevel(10, BuildAdaptiveStrategy(), BuildTacticalSurge(), BuildGambitDieSize(DieType.D10))
            .AddFeaturesAtLevel(15, BuildGambitPoolIncrease(), learn2Gambits, unlearn, BuildGambitDieSize(DieType.D12))
            .AddToDB();

        BuildGambits();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;

    private static FeatureDefinitionPower GambitPool { get; set; }
    private static FeatureDefinitionAdditionalDamage GambitDieDamage { get; set; }
    private static FeatureDefinitionAdditionalDamage GambitDieDamageOnce { get; set; }
    private static FeatureDefinition EverVigilant { get; set; }

    private static void BuildGambitPool()
    {
        GambitPool = FeatureDefinitionPowerBuilder
            .Create("PowerPoolTacticianGambit")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(IsPowerPool.Marker)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest, 1, 4)
            .AddToDB();
    }

    private static DieType GetGambitDieSize(RulesetCharacter character)
    {
        var level = character.GetClassLevel(CharacterClassDefinitions.Fighter);

        return level switch
        {
            >= 15 => DieType.D12,
            >= 10 => DieType.D10,
            >= 5 => DieType.D8,
            _ => DieType.D6
        };
    }

    private static FeatureDefinition BuildSharpMind()
    {
        return FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetTacticianSharpMind")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionPointPoolBuilder
                    .Create("PointPoolTacticianSharpMindSkill")
                    .SetGuiPresentationNoContent()
                    .SetPool(HeroDefinitions.PointsPoolType.Skill, 1)
                    .AddToDB(),
                FeatureDefinitionPointPoolBuilder
                    .Create("PointPoolTacticianSharpMindExpertise")
                    .SetGuiPresentationNoContent()
                    .SetPool(HeroDefinitions.PointsPoolType.Expertise, 1)
                    .AddToDB()
            )
            .AddToDB();
    }

    private static FeatureDefinition BuildEverVigilant()
    {
        return FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierTacticianEverVigilant")
            .SetGuiPresentation(Category.Feature)
            .SetModifierAbilityScore(AttributeDefinitions.Initiative, AttributeDefinitions.Intelligence)
            .AddToDB();
    }

    private static FeatureDefinition BuildSharedVigilance()
    {
        return FeatureDefinitionPowerBuilder
            .Create("PowerTacticianSharedVigilance")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .SetUsesFixed(ActivationTime.PermanentUnlessIncapacitated)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Sphere, 6)
                .ExcludeCaster()
                .SetRecurrentEffect(
                    RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(ConditionDefinitionBuilder
                        .Create("ConditionTacticianSharedVigilance")
                        .SetGuiPresentationNoContent(true)
                        .SetSilent(Silent.WhenAddedOrRemoved)
                        .SetAmountOrigin(ExtraOriginOfAmount.SourceAbilityBonus, AttributeDefinitions.Intelligence)
                        .SetFeatures(FeatureDefinitionAttributeModifierBuilder
                            .Create("AttributeModifierTacticianSharedVigilance")
                            .SetGuiPresentation(EverVigilant.GuiPresentation)
                            .SetAddConditionAmount(AttributeDefinitions.Initiative)
                            .AddToDB())
                        .AddToDB(), ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .AddToDB();
    }

    private static FeatureDefinition BuildGambitPoolIncrease()
    {
        return FeatureDefinitionPowerUseModifierBuilder
            .Create($"PowerUseModifierTacticianGambitPool{_gambitPoolIncreases++:D2}")
            .SetGuiPresentation("PowerUseModifierTacticianGambitPool", Category.Feature)
            .SetFixedValue(GambitPool, 1)
            .AddToDB();
    }

    private static FeatureDefinition BuildAdaptiveStrategy()
    {
        var feature = FeatureDefinitionBuilder
            .Create("FeatureAdaptiveStrategy")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        feature.SetCustomSubFeatures(new RefundPowerUseAfterCrit(GambitPool, feature));

        return feature;
    }

    private static FeatureDefinitionCustomInvocationPool BuildLearn(int points)
    {
        return CustomInvocationPoolDefinitionBuilder
            .Create($"InvocationPoolGambitLearn{points}")
            .SetGuiPresentation(Category.Feature)
            .Setup(InvocationPoolTypeCustom.Pools.Gambit, points)
            .AddToDB();
    }

    private static FeatureDefinitionCustomInvocationPool BuildUnlearn()
    {
        return CustomInvocationPoolDefinitionBuilder
            .Create("InvocationPoolGambitUnlearn")
            .SetGuiPresentationNoContent(true)
            .Setup(InvocationPoolTypeCustom.Pools.Gambit, 1, true)
            .AddToDB();
    }

    private static FeatureDefinition BuildGambitDieSize(DieType size)
    {
        //doesn't do anything, just to display to player dice size progression on level up
        return FeatureDefinitionBuilder
            .Create($"FeatureTacticianGambitDieSize{size}")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();
    }

    private static FeatureDefinitionAdditionalDamage BuildGambitDieDamage(string name,
        FeatureLimitedUsage limit = FeatureLimitedUsage.None)
    {
        return FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamageGambitDie{name}")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(UpgradeDice)
            .SetDamageDice(DieType.D6, 1)
            .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
            .SetNotificationTag("GambitDie")
            .SetFrequencyLimit(limit)
            .AddToDB();
    }

    private static FeatureDefinition BuildTacticalSurge()
    {
        const string CONDITION_NAME = "ConditionTacticianTacticalSurge";

        var tick = FeatureDefinitionBuilder
            .Create("FeatureTacticianTacticalSurgeTick")
            .SetGuiPresentation(CONDITION_NAME, Category.Condition)
            .AddToDB();

        tick.SetCustomSubFeatures(new TacticalSurgeTick(GambitPool, tick));

        var feature = FeatureDefinitionBuilder
            .Create("FeatureTacticianTacticalSurge")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var condition = ConditionDefinitionBuilder
            .Create(CONDITION_NAME)
            .SetGuiPresentation(Category.Condition, Sprites.ConditionTacticalSurge)
            .SetFeatures(tick)
            .AddToDB();

        feature.SetCustomSubFeatures(new TacticalSurge(GambitPool, feature, condition));

        return feature;
    }

    private static void BuildGambits()
    {
        string name;
        AssetReferenceSprite sprite;
        FeatureDefinition feature;
        FeatureDefinitionPower power;
        ICustomConditionFeature reaction;

        #region Helpers

        //sub-feature that spends gambit die when melee attack hits
        var spendDieOnMeleeHit = new AddUsablePowerFromCondition(FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerReactionSpendGambitDieOnMeleeHit")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden, ForcePowerUseInSpendPowerAction.Marker)
            .SetSharedPool(ActivationTime.OnAttackHitMeleeAuto, GambitPool)
            .AddToDB());

        var spendDieOnAttackHit = new AddUsablePowerFromCondition(FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerReactionSpendGambitDieOnAttackHit")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden, ForcePowerUseInSpendPowerAction.Marker)
            .SetSharedPool(ActivationTime.OnAttackHitAuto, GambitPool)
            .AddToDB());

        //power that is used spends gambit die
        var spendDiePower = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerReactionSpendGambitDieOnConditionRemoval")
            .SetGuiPresentationNoContent(true)
            .SetSharedPool(ActivationTime.NoCost, GambitPool)
            .AddToDB();

        //sub-feature that uses `spendDiePower` to spend die when character attacks
        var spendDieOnAttack = new SpendPowerAfterAttack(spendDiePower);

        //feature that has `spendDieOnAttack` sub-feature
        var featureSpendDieOnAttack = FeatureDefinitionBuilder
            .Create("FeatureSpendGambitDieOnConditionRemoval")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(spendDieOnAttack)
            .AddToDB();

        var conditionGambitDieDamage = ConditionDefinitionBuilder
            .Create("ConditionGambitDieDamage")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(BuildGambitDieDamage("Reaction"))
            .AddToDB();

        #endregion

        #region Blind

        name = "GambitBlind";
        //TODO: add proper icon
        sprite = Sprites.ActionGambit;

        reaction = new AddUsablePowerFromCondition(FeatureDefinitionPowerBuilder
            .Create($"Power{name}React")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden, ForcePowerUseInSpendPowerAction.Marker)
            .SetUsesFixed(ActivationTime.OnAttackHitAuto)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.Individuals)
                .SetDurationData(DurationType.Round, 1)
                .SetHasSavingThrow(AttributeDefinitions.Constitution,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    AttributeDefinitions.Intelligence)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(ConditionDefinitions.ConditionBlinded, ConditionForm.ConditionOperation.Add)
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .Build())
                .Build())
            .AddToDB());

        power = FeatureDefinitionPowerBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .SetCustomSubFeatures(PowerFromInvocation.Marker, GambitLimiter)
            .SetUniqueInstance()
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(ConditionDefinitionBuilder
                        .Create($"Condition{name}")
                        .SetGuiPresentation(name, Category.Feature, Sprites.ConditionGambit)
                        .SetCustomSubFeatures(reaction, spendDieOnAttackHit)
                        .SetSilent(Silent.None)
                        .SetPossessive()
                        .SetSpecialInterruptions(ConditionInterruption.Attacks)
                        .SetFeatures(GambitDieDamage)
                        .AddToDB(), ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .AddToDB();

        BuildFeatureInvocation(name, sprite, power);

        #endregion

        #region Knockdown

        name = "GambitKnockdown";
        //TODO: add proper icon
        sprite = Sprites.ActionGambit;

        reaction = new AddUsablePowerFromCondition(FeatureDefinitionPowerBuilder
            .Create($"Power{name}React")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden, ForcePowerUseInSpendPowerAction.Marker)
            .SetUsesFixed(ActivationTime.OnAttackHitAuto)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.Individuals)
                .SetDurationData(DurationType.Round, 1)
                .SetHasSavingThrow(AttributeDefinitions.Strength,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    AttributeDefinitions.Intelligence)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetMotionForm(MotionForm.MotionType.FallProne)
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .Build())
                .Build())
            .AddToDB());

        power = FeatureDefinitionPowerBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .SetCustomSubFeatures(PowerFromInvocation.Marker, GambitLimiter)
            .SetUniqueInstance()
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(ConditionDefinitionBuilder
                        .Create($"Condition{name}")
                        .SetGuiPresentation(name, Category.Feature, Sprites.ConditionGambit)
                        .SetCustomSubFeatures(reaction, spendDieOnAttackHit)
                        .SetSilent(Silent.None)
                        .SetPossessive()
                        .SetSpecialInterruptions(ConditionInterruption.Attacks)
                        .SetFeatures(GambitDieDamage)
                        .AddToDB(), ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .AddToDB();

        BuildFeatureInvocation(name, sprite, power);

        #endregion

        #region Repel

        name = "GambitRepel";
        //TODO: add proper icon
        sprite = Sprites.ActionGambit;

        reaction = new AddUsablePowerFromCondition(FeatureDefinitionPowerBuilder
            .Create($"Power{name}React")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden, ForcePowerUseInSpendPowerAction.Marker)
            .SetUsesFixed(ActivationTime.OnAttackHitAuto)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.Individuals)
                .SetDurationData(DurationType.Round, 1)
                .SetHasSavingThrow(AttributeDefinitions.Strength,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    AttributeDefinitions.Intelligence)
                .SetEffectForms(
                    EffectFormBuilder.Create()
                        .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 1)
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .Build(),
                    EffectFormBuilder.Create()
                        .SetConditionForm(CustomConditionsContext.StopMovement, ConditionForm.ConditionOperation.Add)
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .Build())
                .Build())
            .AddToDB());

        power = FeatureDefinitionPowerBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .SetCustomSubFeatures(PowerFromInvocation.Marker, GambitLimiter)
            .SetUniqueInstance()
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(ConditionDefinitionBuilder
                        .Create($"Condition{name}")
                        .SetGuiPresentation(name, Category.Feature, Sprites.ConditionGambit)
                        .SetCustomSubFeatures(reaction, spendDieOnAttackHit)
                        .SetSilent(Silent.None)
                        .SetPossessive()
                        .SetSpecialInterruptions(ConditionInterruption.Attacks)
                        .SetFeatures(GambitDieDamage)
                        .AddToDB(), ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .AddToDB();

        BuildFeatureInvocation(name, sprite, power);

        #endregion

        #region Threaten

        name = "GambitThreaten";
        //TODO: add proper icon
        sprite = Sprites.ActionGambit;

        reaction = new AddUsablePowerFromCondition(FeatureDefinitionPowerBuilder
            .Create($"Power{name}React")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .SetUsesFixed(ActivationTime.OnAttackHitMeleeAuto)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.Individuals)
                .SetDurationData(DurationType.Round, 1)
                .SetHasSavingThrow(AttributeDefinitions.Wisdom,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    AttributeDefinitions.Intelligence)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(ConditionDefinitions.ConditionFrightenedFear,
                        ConditionForm.ConditionOperation.Add)
                    .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurnNoPerceptionOfSource)
                    .Build())
                .Build())
            .AddToDB());

        power = FeatureDefinitionPowerBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .SetCustomSubFeatures(PowerFromInvocation.Marker, GambitLimiter)
            .SetUniqueInstance()
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(ConditionDefinitionBuilder
                        .Create($"Condition{name}")
                        .SetGuiPresentation(name, Category.Feature, Sprites.ConditionGambit)
                        .SetCustomSubFeatures(reaction, spendDieOnMeleeHit)
                        .SetSilent(Silent.None)
                        .SetPossessive()
                        .SetSpecialInterruptions(ConditionInterruption.Attacks)
                        .SetFeatures(GambitDieDamage)
                        .AddToDB(), ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .AddToDB();

        BuildFeatureInvocation(name, sprite, power);

        #endregion

        #region Goading

        name = "GambitGoading";
        //TODO: add proper icon
        sprite = Sprites.ActionGambit;

        reaction = new AddUsablePowerFromCondition(FeatureDefinitionPowerBuilder
            .Create($"Power{name}React")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden, ForcePowerUseInSpendPowerAction.Marker)
            .SetUsesFixed(ActivationTime.OnAttackHitMeleeAuto)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.Individuals)
                .SetDurationData(DurationType.Round, 1)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(ConditionDefinitionBuilder
                        .Create($"Condition{name}Effect")
                        .SetGuiPresentation(Category.Condition, Gui.NoLocalization,
                            ConditionDefinitions.ConditionDistracted)
                        .SetConditionType(ConditionType.Detrimental)
                        .SetFeatures(FeatureDefinitionCombatAffinityBuilder
                            .Create($"CombatAffinity{name}")
                            .SetGuiPresentationNoContent()
                            .SetMyAttackAdvantage(AdvantageType.Disadvantage)
                            .SetSituationalContext(ExtraSituationalContext.TargetIsNotEffectSource)
                            .AddToDB())
                        .SetSpecialDuration()
                        //Lasts until the end of the target's turn
                        .SetDuration(DurationType.Round, 0, false)
                        .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
                        .AddToDB(), ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .AddToDB());

        power = FeatureDefinitionPowerBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .SetCustomSubFeatures(PowerFromInvocation.Marker, GambitLimiter)
            .SetUniqueInstance()
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(ConditionDefinitionBuilder
                        .Create($"Condition{name}")
                        .SetGuiPresentation(name, Category.Feature, Sprites.ConditionGambit)
                        .SetCustomSubFeatures(reaction, spendDieOnMeleeHit)
                        .SetSilent(Silent.None)
                        .SetPossessive()
                        .SetSpecialInterruptions(ConditionInterruption.Attacks)
                        .SetFeatures(GambitDieDamage)
                        .AddToDB(), ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .AddToDB();

        BuildFeatureInvocation(name, sprite, power);

        #endregion

        #region Feint

        name = "GambitFeint";
        //TODO: add proper icon
        sprite = Sprites.ActionGambit;

        power = FeatureDefinitionPowerBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .SetCustomSubFeatures(PowerFromInvocation.Marker, GambitLimiter)
            .SetUniqueInstance()
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(ConditionDefinitionBuilder
                        .Create($"Condition{name}")
                        .SetGuiPresentation(name, Category.Feature, Sprites.ConditionGambit)
                        .SetSilent(Silent.None)
                        .SetPossessive()
                        .SetSpecialInterruptions(ConditionInterruption.Attacks)
                        .SetFeatures(GambitDieDamage, featureSpendDieOnAttack, FeatureDefinitionCombatAffinityBuilder
                            .Create($"CombatAffinity{name}")
                            .SetGuiPresentation(name, Category.Feature)
                            .SetMyAttackAdvantage(AdvantageType.Advantage)
                            .AddToDB())
                        .AddToDB(), ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .AddToDB();

        BuildFeatureInvocation(name, sprite, power);

        #endregion

        #region Lunging

        name = "GambitLunging";
        //TODO: add proper icon
        sprite = Sprites.ActionGambit;

        power = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .SetCustomSubFeatures(PowerFromInvocation.Marker)
            .SetUniqueInstance()
            .SetSharedPool(ActivationTime.NoCost, GambitPool)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(ConditionDefinitionBuilder
                        .Create($"Condition{name}")
                        .SetGuiPresentation(name, Category.Feature, Sprites.ConditionGambit)
                        .SetSilent(Silent.None)
                        .SetPossessive()
                        .SetFeatures(GambitDieDamageOnce, FeatureDefinitionBuilder
                            .Create($"Feature{name}")
                            .SetGuiPresentationNoContent(true)
                            .SetCustomSubFeatures(new IncreaseMeleeAttackReach(1, ValidatorsWeapon.AlwaysValid),
                                new BumpWeaponAttackRangeToMax(ValidatorsWeapon.AlwaysValid))
                            .AddToDB())
                        .AddToDB(), ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .AddToDB();

        BuildFeatureInvocation(name, sprite, power);

        #endregion

        #region Urgent Orders

        name = "GambitUrgent";
        //TODO: add proper icon
        sprite = Sprites.ActionGambit;

        power = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetCustomSubFeatures(PowerFromInvocation.Marker)
            .SetUniqueInstance()
            .SetSharedPool(ActivationTime.BonusAction, GambitPool)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.Individuals)
                .ExcludeCaster()
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(ConditionDefinitionBuilder
                        .Create($"Condition{name}")
                        .SetGuiPresentation(ConditionDefinitions.ConditionHasted.GuiPresentation)
                        .SetSilent(Silent.None)
                        .SetFeatures(ConditionDefinitions.ConditionHasted.Features)
                        .AddToDB(), ConditionForm.ConditionOperation.Add)
                    .Build())
                .SetParticleEffectParameters(SpellDefinitions.Haste)
                .Build())
            .AddToDB();

        BuildFeatureInvocation(name, sprite, power);

        #endregion

        #region Bait and Switch

        name = "GambitSwitch";
        //TODO: add proper icon
        sprite = Sprites.ActionGambit;

        var good = ConditionDefinitionBuilder
            .Create($"Condition{name}Good")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBlessed)
            .SetSilent(Silent.None)
            .SetAmountOrigin(ExtraOriginOfAmount.SourceProficiencyBonus)
            .SetFeatures(FeatureDefinitionAttributeModifierBuilder
                .Create($"AttributeModifier{name}Good")
                .SetGuiPresentation($"Condition{name}Good", Category.Condition)
                .SetAddConditionAmount(AttributeDefinitions.ArmorClass)
                .AddToDB())
            .SetSpecialDuration()
            .SetDuration(DurationType.Round, 1)
            .AddToDB();

        var bad = ConditionDefinitionBuilder
            .Create($"Condition{name}Bad")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBranded)
            .IsDetrimental()
            .SetSilent(Silent.None)
            .SetAmountOrigin(ExtraOriginOfAmount.SourceProficiencyBonusNegative)
            .SetFeatures(FeatureDefinitionAttributeModifierBuilder
                .Create($"AttributeModifier{name}Bad")
                .SetGuiPresentation($"Condition{name}Bad", Category.Condition)
                .SetAddConditionAmount(AttributeDefinitions.ArmorClass)
                .AddToDB())
            .SetSpecialDuration()
            .SetDuration(DurationType.Round, 1)
            .AddToDB();

        power = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetCustomSubFeatures(PowerFromInvocation.Marker)
            .SetUniqueInstance()
            .SetShowCasting(false)
            .SetSharedPool(ActivationTime.BonusAction, GambitPool)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                .SetTargetingData(Side.All, RangeType.Touch, 1, TargetType.Individuals)
                .ExcludeCaster()
                .SetHasSavingThrow(AttributeDefinitions.Wisdom,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    AttributeDefinitions.Intelligence,
                    disableSavingThrowOnAllies: true)
                .SetEffectForms(
                    EffectFormBuilder.Create()
                        .SetMotionForm(ExtraMotionType.CustomSwap, 1)
                        .Build(),
                    EffectFormBuilder.Create()
                        .SetConditionForm(ConditionDefinitionBuilder
                            .Create($"Condition{name}")
                            .SetGuiPresentationNoContent(true)
                            .SetCustomSubFeatures(new ApplyConditionDependingOnSide(good, bad))
                            .SetSilent(Silent.WhenAddedOrRemoved)
                            .AddToDB(), ConditionForm.ConditionOperation.Add)
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .Build())
                .SetParticleEffectParameters(SpellDefinitions.Haste)
                .Build())
            .AddToDB();

        BuildFeatureInvocation(name, sprite, power);

        #endregion

        #region Riposte

        name = "GambitRiposte";
        //TODO: add proper icon
        sprite = Sprites.ActionGambit;

        feature = FeatureDefinitionBuilder
            .Create($"Feature{name}")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetCustomSubFeatures(new Retaliate(spendDiePower, conditionGambitDieDamage))
            .AddToDB();

        BuildFeatureInvocation(name, sprite, feature);

        #endregion

        #region Brace

        name = "GambitBrace";
        //TODO: add proper icon
        sprite = Sprites.ActionGambit;

        feature = FeatureDefinitionBuilder
            .Create($"Feature{name}")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetCustomSubFeatures(new Brace(spendDiePower, conditionGambitDieDamage))
            .AddToDB();

        BuildFeatureInvocation(name, sprite, feature);

        #endregion
    }

    private static void BuildFeatureInvocation(
        string name,
        AssetReferenceSprite sprite,
        FeatureDefinition feature)
    {
        CustomInvocationDefinitionBuilder
            .Create($"CustomInvocation{name}")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetPoolType(InvocationPoolTypeCustom.Pools.Gambit)
            .SetGrantedFeature(feature)
            .AddToDB();
    }

    private class SpendPowerAfterAttack : IBeforeAttackEffect
    {
        private readonly FeatureDefinitionPower power;

        public SpendPowerAfterAttack(FeatureDefinitionPower power)
        {
            this.power = power;
        }

        public void BeforeOnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            if (attackMode == null)
            {
                return;
            }

            var character = attacker.RulesetCharacter;

            character?.UsePower(UsablePowersProvider.Get(power, character));
        }
    }

    private class RefundPowerUseAfterCrit : IAfterAttackEffect
    {
        private readonly FeatureDefinition feature;
        private readonly FeatureDefinitionPower power;

        public RefundPowerUseAfterCrit(FeatureDefinitionPower power, FeatureDefinition feature)
        {
            this.power = power;
            this.feature = feature;
        }

        public void AfterOnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            if (outcome is not (RollOutcome.CriticalFailure or RollOutcome.CriticalSuccess))
            {
                return;
            }

            if (attackMode == null)
            {
                return;
            }

            var character = attacker.RulesetCharacter;

            if (character == null)
            {
                return;
            }

            if (character.GetRemainingPowerUses(power) >= character.GetMaxUsesForPool(power))
            {
                return;
            }

            GameConsoleHelper.LogCharacterUsedFeature(character, feature, indent: true);
            character.UpdateUsageForPower(power, -1);
        }
    }

    private class Retaliate : IReactToAttackOnMeFinished
    {
        private readonly ConditionDefinition condition;
        private readonly FeatureDefinitionPower pool;

        public Retaliate(FeatureDefinitionPower pool, ConditionDefinition condition)
        {
            this.condition = condition;
            this.pool = pool;
        }

        public IEnumerator HandleReactToAttackOnMeFinished(
            GameLocationCharacter attacker,
            GameLocationCharacter me,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode mode,
            ActionModifier modifier)
        {
            //trigger only on a miss
            if (outcome is not (RollOutcome.Failure or RollOutcome.CriticalFailure))
            {
                yield break;
            }

            if (me.GetActionTypeStatus(ActionDefinitions.ActionType.Reaction) !=
                ActionDefinitions.ActionStatus.Available)
            {
                yield break;
            }

            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var battle = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (manager == null || battle == null)
            {
                yield break;
            }

            var (retaliationMode, retaliationModifier) = me.GetFirstMeleeAttackThatCanAttack(attacker);

            if (retaliationMode == null)
            {
                yield break;
            }

            var reactionParams = new CharacterActionParams(me, ActionDefinitions.Id.AttackOpportunity);

            reactionParams.TargetCharacters.Add(attacker);
            reactionParams.ActionModifiers.Add(retaliationModifier);
            reactionParams.AttackMode = retaliationMode;

            var character = me.RulesetCharacter;
            var rulesetCondition = RulesetCondition.CreateActiveCondition(
                character.Guid,
                condition,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                character.Guid,
                string.Empty);

            character.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);

            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestReactionAttack("GambitRiposte", reactionParams)
            {
                Resource = new ReactionResourcePowerPool(pool, Sprites.GambitResourceIcon)
            };

            manager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(attacker, manager, previousReactionCount);

            //Can we detect this before attack starts? Currently we get to this part after attack finishes, if reaction was validated
            if (reactionParams.ReactionValidated)
            {
                character.UsePower(UsablePowersProvider.Get(pool, character));
            }

            character.RemoveCondition(rulesetCondition);
        }
    }

    private class ApplyConditionDependingOnSide : ICustomConditionFeature
    {
        private readonly ConditionDefinition good, bad;

        public ApplyConditionDependingOnSide(ConditionDefinition good, ConditionDefinition bad)
        {
            this.good = good;
            this.bad = bad;
        }

        public void ApplyFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            var caster = EffectHelpers.GetCharacterByGuid(rulesetCondition.sourceGuid);

            if (caster == null)
            {
                return;
            }

            var condition = caster.IsOppositeSide(target.Side) ? bad : good;

            target.InflictCondition(condition.Name, DurationType.Round, 1, TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagCombat, caster.Guid, caster.CurrentFaction.Name, 1, null, 0, 0, 0);
        }

        public void RemoveFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
        }
    }

    internal class Brace
    {
        private readonly ConditionDefinition condition;
        private readonly FeatureDefinitionPower pool;

        public Brace(FeatureDefinitionPower pool, ConditionDefinition condition)
        {
            this.pool = pool;
            this.condition = condition;
        }

        internal IEnumerator Process(
            GameLocationCharacter me,
            GameLocationCharacter mover,
            (int3, int3) movement,
            GameLocationBattleManager battle)
        {
            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (manager == null)
            {
                yield break;
            }

            if (!me.CanPerformOpportunityAttackOnCharacter(mover, movement.Item2, movement.Item1,
                    out var retaliationMode, out var retaliationModifier, battle))
            {
                yield break;
            }

            var reactionParams = new CharacterActionParams(
                me,
                ActionDefinitions.Id.AttackOpportunity,
                retaliationMode,
                mover,
                retaliationModifier);

            var character = me.RulesetCharacter;

            var rulesetCondition = RulesetCondition.CreateActiveCondition(character.Guid,
                condition,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                character.Guid,
                string.Empty
            );

            character.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);

            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestReactionAttack("GambitBrace", reactionParams)
            {
                Resource = new ReactionResourcePowerPool(pool, Sprites.GambitResourceIcon)
            };

            manager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(mover, manager, previousReactionCount);

            //Can we detect this before attack starts? Currently we get to this part after attack finishes, if reaction was validated
            if (reactionParams.ReactionValidated)
            {
                character.UsePower(UsablePowersProvider.Get(pool, character));
            }

            character.RemoveCondition(rulesetCondition);
        }
    }

    private class TacticalSurge : IOnAfterActionFeature
    {
        private readonly ConditionDefinition condition;
        private readonly FeatureDefinition feature;
        private readonly FeatureDefinitionPower power;

        public TacticalSurge(FeatureDefinitionPower power, FeatureDefinition feature,
            ConditionDefinition condition)
        {
            this.power = power;
            this.feature = feature;
            this.condition = condition;
        }

        public void OnAfterAction(
            GameLocationCharacter actingCharacter,
            CharacterActionParams actionParams,
            ActionDefinition actionDefinition)
        {
            if (actionParams.TargetAction is not CharacterActionActionSurge)
            {
                return;
            }

            var character = actingCharacter.RulesetCharacter;
            var charges = character.GetRemainingPowerUses(power) - character.GetMaxUsesForPool(power);
            charges = Math.Max(charges, -2);

            GameConsoleHelper.LogCharacterUsedFeature(character, feature, indent: true);
            if (charges < 0)
            {
                character.UpdateUsageForPower(power, charges);
            }

            character.InflictCondition(condition.Name, DurationType.Minute, 1, TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagCombat, character.Guid, character.CurrentFaction.Name, 1, feature.Name, 1, 0,
                0);
        }
    }

    private class TacticalSurgeTick : ICharacterTurnStartListener
    {
        private readonly FeatureDefinition feature;
        private readonly FeatureDefinitionPower power;

        public TacticalSurgeTick(FeatureDefinitionPower power, FeatureDefinition feature)
        {
            this.power = power;
            this.feature = feature;
        }

        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            var character = locationCharacter.RulesetCharacter;
            var charges = character.GetRemainingPowerUses(power) - character.GetMaxUsesForPool(power);

            charges = Math.Max(charges, -1);

            if (charges >= 0)
            {
                return;
            }

            GameConsoleHelper.LogCharacterUsedFeature(character, feature, indent: true);
            character.UpdateUsageForPower(power, charges);
        }
    }

    internal class GambitActionDiceBox : IActionItemDiceBox
    {
        private GambitActionDiceBox()
        {
        }

        public static IActionItemDiceBox Instance { get; } = new GambitActionDiceBox();

        public (DieType type, int number, string format) GetDiceInfo(RulesetCharacter character)
        {
            return (GetGambitDieSize(character), character.GetRemainingPowerUses(GambitPool),
                "Screen/&GambitDieDescription");
        }
    }
}
