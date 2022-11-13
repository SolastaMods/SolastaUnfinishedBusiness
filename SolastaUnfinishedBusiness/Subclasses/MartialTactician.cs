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
using UnityEngine.AddressableAssets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class MartialTactician : AbstractSubclass
{
    private static readonly LimitedEffectInstances GambitLimiter = new("Gambit", _ => 1);

    private int _gambitPoolIncreases;

    private static DamageDieProvider UpgradeDice = (character, _) => GetGambitDieSize(character);

    internal MartialTactician()
    {
        BuildGambitPool();

        GambitDieDamage = FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageGambitDie")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(UpgradeDice)
            .SetDamageDice(DieType.D6, 1)
            .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
            .SetNotificationTag("GambitDie")
            .AddToDB();

        //make sure that if we add any custom sub-features to base one we add them to this one too
        GambitDieDamageOnce = FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageGambitDieOnce")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(UpgradeDice)
            .SetDamageDice(DieType.D6, 1)
            .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
            .SetNotificationTag("GambitDie")
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .AddToDB();

        var learn1Gambit = BuildLearn(1);
        var learn3Gambits = BuildLearn(3);

        EverVigilant = BuildEverVigilant();
        Subclass = CharacterSubclassDefinitionBuilder
            .Create("MartialTactician")
            .SetGuiPresentation(Category.Subclass, RoguishShadowCaster)
            .AddFeaturesAtLevel(3, BuildSharpMind(), GambitPool, learn3Gambits, EverVigilant)
            .AddFeaturesAtLevel(7, BuildGambitPoolIncrease(), learn1Gambit, BuildSharedVigilance())
            .AddFeaturesAtLevel(10, BuildAdaptiveStrategy())
            .AddFeaturesAtLevel(15, BuildGambitPoolIncrease(), learn1Gambit)
            .AddToDB();

        BuildGambits();
    }

    private static void BuildGambitPool()
    {
        GambitPool = FeatureDefinitionPowerBuilder
            .Create("PowerPoolTacticianGambit")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(IsPowerPool.Marker)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest, 1, 4)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;

    private static FeatureDefinitionPower GambitPool { get; set; }
    private FeatureDefinitionAdditionalDamage GambitDieDamage { get; }
    private FeatureDefinitionAdditionalDamage GambitDieDamageOnce { get; }
    private FeatureDefinition EverVigilant { get; }

    internal static DieType GetGambitDieSize(RulesetCharacter character)
    {
        var level = character.GetClassLevel(CharacterClassDefinitions.Fighter);
        if (level >= 15)
        {
            return DieType.D12;
        }
        else if (level >= 10)
        {
            return DieType.D10;
        }
        else if (level >= 5)
        {
            return DieType.D8;
        }

        return DieType.D6;
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

    private FeatureDefinition BuildSharedVigilance()
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
                        .SetFeatures(EverVigilant)
                        .AddToDB(), ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .AddToDB();
    }

    private FeatureDefinition BuildGambitPoolIncrease()
    {
        return FeatureDefinitionPowerUseModifierBuilder
            .Create($"PowerUseModifierTacticianGambitPool{_gambitPoolIncreases++:D2}")
            .SetGuiPresentation("PowerUseModifierTacticianGambitPool", Category.Feature)
            .SetFixedValue(GambitPool, 1)
            .AddToDB();
    }

    private FeatureDefinition BuildAdaptiveStrategy()
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

    private void BuildGambits()
    {
        string name;
        AssetReferenceSprite sprite;
        FeatureDefinition feature;
        FeatureDefinitionPower power;
        ICustomConditionFeature reaction;

        #region Helpers

        // sub-feature that spends gambit die when melee attack hits
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
                    AttributeDefinitions.Strength)
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
                    AttributeDefinitions.Strength)
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
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden, ForcePowerUseInSpendPowerAction.Marker)
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
            .SetUsesFixed(ActivationTime.OnAttackHitAuto)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.Individuals)
                .SetDurationData(DurationType.Round, 1)
                .SetHasSavingThrow(AttributeDefinitions.Wisdom,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    AttributeDefinitions.Intelligence)
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
                        .SetSpecialDuration(true)
                        //Lasts until the end of the target's turn
                        .SetDuration(DurationType.Round, 0, false)
                        .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
                        .AddToDB(), ConditionForm.ConditionOperation.Add)
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

        #region Riposte

        name = "GambitRiposte";
        //TODO: add proper icon
        sprite = Sprites.ActionGambit;

        feature = FeatureDefinitionBuilder
            .Create($"Feature{name}")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetCustomSubFeatures(new Retaliate(spendDiePower, ConditionDefinitionBuilder
                .Create($"Condition{name}")
                .SetGuiPresentationNoContent(true)
                .SetSilent(Silent.WhenAddedOrRemoved)
                .SetFeatures(GambitDieDamage)
                .AddToDB()))
            .AddToDB();

        BuildFeatureInvocation(name, sprite, feature);

        #endregion
    }

    private static void BuildFeatureInvocation(string name, AssetReferenceSprite sprite,
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

        public void BeforeOnAttackHit(GameLocationCharacter attacker, GameLocationCharacter defender,
            RollOutcome outcome, CharacterActionParams actionParams, RulesetAttackMode attackMode,
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

        public void AfterOnAttackHit(GameLocationCharacter attacker, GameLocationCharacter defender,
            RollOutcome outcome, CharacterActionParams actionParams, RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            if (outcome is not (RollOutcome.CriticalFailure or RollOutcome.CriticalSuccess)) { return; }

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
            character.RepayPowerUse(UsablePowersProvider.Get(power, character));
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

        public IEnumerator HandleReactToAttackOnMeFinished(GameLocationCharacter attacker, GameLocationCharacter me,
            RollOutcome outcome, CharacterActionParams actionParams, RulesetAttackMode mode, ActionModifier modifier)
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
            var rulesetCondition = RulesetCondition.CreateActiveCondition(character.Guid,
                condition,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                character.Guid,
                string.Empty
            );

            character.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);

            //TODO: try making this reaction request show how many dice remaining
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

    internal class GambitActionDiceBox : IActionItemDiceBox
    {
        public static IActionItemDiceBox Instance { get; } = new GambitActionDiceBox();

        private GambitActionDiceBox()
        {
        }

        public (DieType type, int number, string format) GetDiceInfo(RulesetCharacter character)
        {
            return (GetGambitDieSize(character), character.GetRemainingPowerUses(GambitPool), "Screen/&GambitDieDescription");
        }
    }
}
