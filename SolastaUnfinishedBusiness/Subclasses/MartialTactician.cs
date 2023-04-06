using System;
using System.Collections;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using UnityEngine.AddressableAssets;
using static ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class MartialTactician : AbstractSubclass
{
    internal const string Name = "MartialTactician";
    private const string MarkCondition = "ConditionTacticianDamagedByGambit";
    private static readonly LimitEffectInstances GambitLimiter = new("Gambit", _ => 1);
    private static readonly DamageDieProvider UpgradeDice = (character, _) => GetGambitDieSize(character);
    private static int _gambitPoolIncreases;

    internal MartialTactician()
    {
        GambitDieDamage = BuildGambitDieDamage("");
        GambitDieDamageOnce = BuildGambitDieDamage("Once", FeatureLimitedUsage.OncePerTurn);

        var unlearn = BuildUnlearn();

        var strategicPlan = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSefTacticianStrategicPlan")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
            .AddFeatureSetNoSort(
                BuildTacticalSurge(),
                BuildAdaptiveStrategy(),
                BuildOvercomingStrategy()
            )
            .AddToDB();

        EverVigilant = BuildEverVigilant();
        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite(Name, Resources.MartialTactician, 256))
            .AddFeaturesAtLevel(3,
                BuildSharpMind(), GambitPool, Learn4Gambit, EverVigilant)
            .AddFeaturesAtLevel(5, BuildGambitDieSize(DieType.D8), Learn1Gambit)
            .AddFeaturesAtLevel(7, BuildGambitPoolIncrease(), Learn1Gambit, unlearn, BuildSharedVigilance())
            .AddFeaturesAtLevel(10, strategicPlan, BuildGambitDieSize(DieType.D10))
            .AddFeaturesAtLevel(15, strategicPlan, BuildGambitPoolIncrease(), Learn2Gambit, unlearn,
                BuildGambitDieSize(DieType.D12))
            .AddToDB();

        BuildGambits();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;

    internal override DeityDefinition DeityDefinition => null;

    internal static FeatureDefinitionPower GambitPool { get; } = FeatureDefinitionPowerBuilder
        .Create("PowerPoolTacticianGambit")
        .SetGuiPresentation(Category.Feature)
        .SetCustomSubFeatures(IsPowerPool.Marker, HasModifiedUses.Marker)
        // force to zero here and add 4 on same level for better integration with tactician adept feat
        .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest, 1, 0)
        .AddToDB();

    private static FeatureDefinitionCustomInvocationPool Learn1Gambit { get; } =
        CustomInvocationPoolDefinitionBuilder
            .Create("InvocationPoolGambitLearn1")
            .SetGuiPresentation(Category.Feature)
            .Setup(InvocationPoolTypeCustom.Pools.Gambit)
            .AddToDB();

    internal static FeatureDefinitionCustomInvocationPool Learn2Gambit { get; } =
        CustomInvocationPoolDefinitionBuilder
            .Create("InvocationPoolGambitLearn2")
            .SetGuiPresentation(Category.Feature)
            .Setup(InvocationPoolTypeCustom.Pools.Gambit, 2)
            .AddToDB();

    private static FeatureDefinitionCustomInvocationPool Learn4Gambit { get; } =
        CustomInvocationPoolDefinitionBuilder
            .Create("InvocationPoolGambitLearn4")
            .SetGuiPresentation(Category.Feature)
            //adding base pool here instead of the pool power to make it properly work on pre-existing characters and not interfere with new feat
            .SetCustomSubFeatures(InitialPool.Instance)
            .Setup(InvocationPoolTypeCustom.Pools.Gambit, 4)
            .AddToDB();

    private static FeatureDefinitionAdditionalDamage GambitDieDamage { get; set; }
    private static FeatureDefinitionAdditionalDamage GambitDieDamageOnce { get; set; }
    private static FeatureDefinition EverVigilant { get; set; }

    private static DieType GetGambitDieSize(RulesetCharacter character)
    {
        var level = character.GetClassLevel(CharacterClassDefinitions.Fighter);

        var hero = character as RulesetCharacterHero ?? character.OriginalFormCharacter as RulesetCharacterHero;

        // required to ensure Tactician Adept feat doesn't increase dice for other fighter subclasses
        if (hero != null &&
            hero.ClassesAndSubclasses.TryGetValue(CharacterClassDefinitions.Fighter,
                out var characterSubclassDefinition) && characterSubclassDefinition.Name != Name)
        {
            level = 1;
        }

        return level switch
        {
            >= 15 => DieType.D12,
            >= 10 => DieType.D10,
            >= 5 => DieType.D8,
            >= 3 => DieType.D6,
            _ => DieType.D4
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
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 6)
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

    internal static FeatureDefinition BuildGambitPoolIncrease(int number, string name)
    {
        return FeatureDefinitionPowerUseModifierBuilder
            .Create($"PowerUseModifierTacticianGambitPool{name}")
            .SetGuiPresentation("PowerUseModifierTacticianGambitPool", Category.Feature)
            .SetFixedValue(GambitPool, number)
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

    private static FeatureDefinition BuildOvercomingStrategy()
    {
        var feature = FeatureDefinitionBuilder
            .Create("FeatureOvercomingStrategy")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        feature.SetCustomSubFeatures(new RefundPowerUseAfterKill(GambitPool, feature));

        ConditionDefinitionBuilder
            .Create(MarkCondition)
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetCustomSubFeatures(
                new RefundPowerUseWhenTargetWithConditionDies(GambitPool, feature),
                RemoveConditionOnSourceTurnStart.Mark,
                //by default this condition is applied under Effects tag, which is removed right at death - too early for us to detect
                //this feature will add this effect under Combat tag, which is not removed
                new ForceConditionCategory(AttributeDefinitions.TagCombat))
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .AddToDB();

        return feature;
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
            .SetConditionOperations(new ConditionOperationDescription
            {
                operation = ConditionOperationDescription.ConditionOperation.Add, conditionName = MarkCondition
            })
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
            .SetPossessive()
            .SetFeatures(tick)
            .AddToDB();

        feature.SetCustomSubFeatures(new TacticalSurge(GambitPool, feature, condition));

        return feature;
    }

    private static void BuildGambits()
    {
        const int HIGH_LEVEL = 7;

        #region Helpers

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

        var hasGambitDice = new ValidatorsPowerUse(character => character.GetRemainingPowerCharges(GambitPool) > 0);

        #endregion

        #region Blind

        var name = "GambitBlind";
        //TODO: add proper icon
        var sprite = Sprites.GetSprite(name, Resources.GambitBlind, 128);

        ICustomConditionFeature reaction = new AddUsablePowerFromCondition(FeatureDefinitionPowerBuilder
            .Create($"Power{name}React")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden, ForcePowerUseInSpendPowerAction.Marker)
            .SetUsesFixed(ActivationTime.OnAttackHitAuto)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.Individuals)
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetSavingThrowData(false,
                    AttributeDefinitions.Constitution, false,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Intelligence)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(ConditionDefinitions.ConditionBlinded, ConditionForm.ConditionOperation.Add)
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .Build())
                .Build())
            .AddToDB());

        var power = FeatureDefinitionPowerBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .SetCustomSubFeatures(PowerFromInvocation.Marker, GambitLimiter, hasGambitDice)
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

        BuildFeatureInvocation(name, sprite, power, HIGH_LEVEL);

        #endregion

        #region Knockdown

        name = "GambitKnockdown";
        //TODO: add proper icon
        sprite = Sprites.GetSprite(name, Resources.GambitKnockdown, 128);

        reaction = new AddUsablePowerFromCondition(FeatureDefinitionPowerBuilder
            .Create($"Power{name}React")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden, ForcePowerUseInSpendPowerAction.Marker)
            .SetUsesFixed(ActivationTime.OnAttackHitAuto)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.Individuals)
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetSavingThrowData(false,
                    AttributeDefinitions.Strength, false, EffectDifficultyClassComputation.AbilityScoreAndProficiency,
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
            .SetCustomSubFeatures(PowerFromInvocation.Marker, GambitLimiter, hasGambitDice)
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
        sprite = Sprites.GetSprite(name, Resources.GambitRepel, 128);

        reaction = new AddUsablePowerFromCondition(FeatureDefinitionPowerBuilder
            .Create($"Power{name}React")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden, ForcePowerUseInSpendPowerAction.Marker)
            .SetUsesFixed(ActivationTime.OnAttackHitAuto)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.Individuals)
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetSavingThrowData(false,
                    AttributeDefinitions.Strength, false, EffectDifficultyClassComputation.AbilityScoreAndProficiency,
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
            .SetCustomSubFeatures(PowerFromInvocation.Marker, GambitLimiter, hasGambitDice)
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
        sprite = Sprites.GetSprite(name, Resources.GambitThreaten, 128);

        reaction = new AddUsablePowerFromCondition(FeatureDefinitionPowerBuilder
            .Create($"Power{name}React")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .SetUsesFixed(ActivationTime.OnAttackHitAuto)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.Individuals)
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetSavingThrowData(false,
                    AttributeDefinitions.Wisdom, false, EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    AttributeDefinitions.Intelligence)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(ConditionDefinitions.ConditionFrightenedFear,
                        ConditionForm.ConditionOperation.Add)
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .Build())
                .Build())
            .AddToDB());

        power = FeatureDefinitionPowerBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .SetCustomSubFeatures(PowerFromInvocation.Marker, GambitLimiter, hasGambitDice)
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

        BuildFeatureInvocation(name, sprite, power, HIGH_LEVEL);

        #endregion

        #region Debilitate

        name = "GambitDebilitate";
        //TODO: add proper icon
        sprite = Sprites.GetSprite(name, Resources.GambitDebilitate, 128);

        reaction = new AddUsablePowerFromCondition(FeatureDefinitionPowerBuilder
            .Create($"Power{name}React")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .SetUsesFixed(ActivationTime.OnAttackHitAuto)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.Individuals)
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetSavingThrowData(false,
                    AttributeDefinitions.Constitution, false,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Intelligence)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(ConditionDefinitionBuilder
                        .Create($"Condition{name}")
                        .SetGuiPresentation(Category.Condition,
                            ConditionDefinitions.ConditionPatronHiveWeakeningPheromones)
                        .SetFeatures(FeatureDefinitionSavingThrowAffinitys
                            .SavingThrowAffinityPatronHiveWeakeningPheromones)
                        .AddToDB(), ConditionForm.ConditionOperation.Add)
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .Build())
                .Build())
            .AddToDB());

        power = FeatureDefinitionPowerBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .SetCustomSubFeatures(PowerFromInvocation.Marker, GambitLimiter, hasGambitDice)
            .SetUniqueInstance()
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(ConditionDefinitionBuilder
                        .Create($"Condition{name}Trigger")
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

        #region Goading

        name = "GambitGoading";
        //TODO: add proper icon
        sprite = Sprites.GetSprite(name, Resources.GambitProvoke, 128);

        reaction = new AddUsablePowerFromCondition(FeatureDefinitionPowerBuilder
            .Create($"Power{name}React")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden, ForcePowerUseInSpendPowerAction.Marker)
            .SetUsesFixed(ActivationTime.OnAttackHitAuto)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.Individuals)
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
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
                        //Lasts until the end of the target's turn
                        .SetSpecialDuration(DurationType.Round, 1)
                        .AddToDB(), ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .AddToDB());

        power = FeatureDefinitionPowerBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .SetCustomSubFeatures(PowerFromInvocation.Marker, GambitLimiter, hasGambitDice)
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
        sprite = Sprites.GetSprite(name, Resources.GambitFeint, 128);

        power = FeatureDefinitionPowerBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .SetCustomSubFeatures(PowerFromInvocation.Marker, GambitLimiter, hasGambitDice)
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

        BuildFeatureInvocation(name, sprite, power, HIGH_LEVEL);

        #endregion

        #region Lunging

        name = "GambitLunging";
        //TODO: add proper icon
        sprite = Sprites.GetSprite(name, Resources.GambitReach, 128);

        power = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .SetCustomSubFeatures(PowerFromInvocation.Marker, hasGambitDice)
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
        sprite = Sprites.GetSprite(name, Resources.GambitUrgentOrders, 128);

        power = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetCustomSubFeatures(PowerFromInvocation.Marker, hasGambitDice)
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

        BuildFeatureInvocation(name, sprite, power, HIGH_LEVEL);

        #endregion

        #region Bait and Switch

        name = "GambitSwitch";
        //TODO: add proper icon
        sprite = Sprites.GetSprite(name, Resources.GambitSwitch, 128);

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
            .SetSpecialDuration(DurationType.Round, 1)
            .AddToDB();

        var bad = ConditionDefinitionBuilder
            .Create($"Condition{name}Bad")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBranded)
            .SetConditionType(ConditionType.Detrimental)
            .SetSilent(Silent.None)
            .SetAmountOrigin(ExtraOriginOfAmount.SourceProficiencyBonusNegative)
            .SetFeatures(FeatureDefinitionAttributeModifierBuilder
                .Create($"AttributeModifier{name}Bad")
                .SetGuiPresentation($"Condition{name}Bad", Category.Condition)
                .SetAddConditionAmount(AttributeDefinitions.ArmorClass)
                .AddToDB())
            .SetSpecialDuration(DurationType.Round, 1)
            .AddToDB();

        power = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetCustomSubFeatures(PowerFromInvocation.Marker, hasGambitDice)
            .SetUniqueInstance()
            .SetShowCasting(false)
            .SetSharedPool(ActivationTime.BonusAction, GambitPool)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                .SetTargetingData(Side.All, RangeType.Touch, 1, TargetType.Individuals)
                .ExcludeCaster()
                .SetSavingThrowData(true,
                    AttributeDefinitions.Dexterity, false, EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    AttributeDefinitions.Intelligence)
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
        sprite = Sprites.GetSprite(name, Resources.GambitCounterAttack, 128);

        var feature = FeatureDefinitionBuilder
            .Create($"Feature{name}")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetCustomSubFeatures(new Retaliate(spendDiePower, conditionGambitDieDamage, true))
            .AddToDB();

        BuildFeatureInvocation(name, sprite, feature);

        #endregion

        #region Return Fire

        name = "GambitReturnFire";
        //TODO: add proper icon
        sprite = Sprites.GetSprite(name, Resources.GambitReturnFire, 128);

        feature = FeatureDefinitionBuilder
            .Create($"Feature{name}")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetCustomSubFeatures(new Retaliate(spendDiePower, conditionGambitDieDamage, false))
            .AddToDB();

        BuildFeatureInvocation(name, sprite, feature);

        #endregion

        #region Brace

        name = "GambitBrace";
        //TODO: add proper icon
        sprite = Sprites.GetSprite(name, Resources.GambitBrace, 128);

        feature = FeatureDefinitionBuilder
            .Create($"Feature{name}")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetCustomSubFeatures(new Brace(spendDiePower, conditionGambitDieDamage))
            .AddToDB();

        BuildFeatureInvocation(name, sprite, feature);

        #endregion

        #region Precise

        name = "GambitPrecise";
        //TODO: add proper icon
        sprite = Sprites.GetSprite(name, Resources.GambitPrecision, 128);

        feature = FeatureDefinitionBuilder
            .Create($"Feature{name}")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .AddToDB();

        feature.SetCustomSubFeatures(new Precise(GambitPool, feature));


        BuildFeatureInvocation(name, sprite, feature);

        #endregion

        #region Parry

        name = "GambitParry";
        //TODO: add proper icon
        sprite = Sprites.GetSprite(name, Resources.GambitParry, 128);

        feature = FeatureDefinitionBuilder
            .Create($"Feature{name}")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .AddToDB();

        feature.SetCustomSubFeatures(new Parry(GambitPool, feature));


        BuildFeatureInvocation(name, sprite, feature);

        #endregion
    }

    private static void BuildFeatureInvocation(
        string name,
        AssetReferenceSprite sprite,
        FeatureDefinition feature,
        int level = 1)
    {
        CustomInvocationDefinitionBuilder
            .Create($"CustomInvocation{name}")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetPoolType(InvocationPoolTypeCustom.Pools.Gambit)
            .SetGrantedFeature(feature)
            .SetRequirements(level)
            .AddToDB();
    }

    private class InitialPool : IPowerUseModifier
    {
        private InitialPool()
        {
        }

        public static IPowerUseModifier Instance { get; } = new InitialPool();
        public FeatureDefinitionPower PowerPool => GambitPool;

        public int PoolChangeAmount(RulesetCharacter character)
        {
            return 4;
        }
    }

    private class SpendPowerAfterAttack : IAfterAttackEffect
    {
        private readonly FeatureDefinitionPower power;

        public SpendPowerAfterAttack(FeatureDefinitionPower power)
        {
            this.power = power;
        }

        public void AfterOnAttackHit(
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

    private class RefundPowerUseAfterKill : ITargetReducedToZeroHp
    {
        private readonly FeatureDefinition feature;
        private readonly FeatureDefinitionPower power;

        public RefundPowerUseAfterKill(FeatureDefinitionPower power, FeatureDefinition feature)
        {
            this.power = power;
            this.feature = feature;
        }

        public IEnumerator HandleCharacterReducedToZeroHp(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            if (downedCreature.RulesetCharacter.HasConditionOfType(MarkCondition))
            {
                yield break;
            }

            if (attackMode == null)
            {
                yield break;
            }

            var character = attacker.RulesetCharacter;

            if (character == null)
            {
                yield break;
            }

            if (character.GetRemainingPowerUses(power) >= character.GetMaxUsesForPool(power))
            {
                yield break;
            }

            GameConsoleHelper.LogCharacterUsedFeature(character, feature, indent: true);
            character.UpdateUsageForPower(power, -1);
        }
    }

    private class RefundPowerUseWhenTargetWithConditionDies : INotifyConditionRemoval
    {
        private readonly FeatureDefinition feature;
        private readonly FeatureDefinitionPower power;

        public RefundPowerUseWhenTargetWithConditionDies(FeatureDefinitionPower power, FeatureDefinition feature)
        {
            this.power = power;
            this.feature = feature;
        }

        public void BeforeDyingWithCondition(RulesetActor rulesetActor, RulesetCondition rulesetCondition)
        {
            var character = EffectHelpers.GetCharacterByGuid(rulesetCondition.sourceGuid);


            if (character == null)
            {
                return;
            }

            if (!character.HasAnyFeature(feature))
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

        public void AfterConditionRemoved(RulesetActor removedFrom, RulesetCondition rulesetCondition)
        {
        }
    }

    private class Retaliate : IReactToAttackOnMeFinished
    {
        private readonly ConditionDefinition condition;
        private readonly bool melee;
        private readonly FeatureDefinitionPower pool;

        public Retaliate(FeatureDefinitionPower pool, ConditionDefinition condition, bool melee)
        {
            this.condition = condition;
            this.melee = melee;
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

            //do not trigger on my own turn, so won't retaliate on AoO
            if (Gui.Battle?.ActiveContenderIgnoringLegendary == me)
            {
                yield break;
            }

            if (!me.CanReact())
            {
                yield break;
            }

            if (me.RulesetCharacter.GetRemainingPowerCharges(pool) <= 0)
            {
                yield break;
            }

            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var battle = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (manager == null || battle == null)
            {
                yield break;
            }

            if (!melee && battle.IsWithin1Cell(me, attacker))
            {
                yield break;
            }

            var (retaliationMode, retaliationModifier) = melee
                ? me.GetFirstMeleeModeThatCanAttack(attacker)
                : me.GetFirstRangedModeThatCanAttack(attacker);

            if (retaliationMode == null)
            {
                yield break;
            }

            retaliationMode.AddAttackTagAsNeeded(AttacksOfOpportunity.NotAoOTag);

            var reactionParams = new CharacterActionParams(me, Id.AttackOpportunity);

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
            var tag = melee ? "GambitRiposte" : "GambitReturnFire";
            var reactionRequest = new ReactionRequestReactionAttack(tag, reactionParams)
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

    private class Brace : CanMakeAoOOnReachEntered
    {
        private readonly ConditionDefinition condition;
        private readonly FeatureDefinitionPower pool;

        public Brace(FeatureDefinitionPower pool, ConditionDefinition condition)
        {
            this.pool = pool;
            this.condition = condition;
            ValidateAttacker = character => character.GetRemainingPowerCharges(pool) > 0;
            BeforeReaction = AddCondition;
            AfterReaction = RemoveCondition;
        }

        private IEnumerator AddCondition(GameLocationCharacter attacker, GameLocationCharacter defender,
            GameLocationBattleManager battleManager, GameLocationActionManager actionManager, ReactionRequest request)
        {
            var character = attacker.RulesetCharacter;
            var rulesetCondition = RulesetCondition.CreateActiveCondition(character.Guid,
                condition,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                character.Guid,
                string.Empty
            );

            character.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);

            yield break;
        }

        private IEnumerator RemoveCondition(GameLocationCharacter attacker, GameLocationCharacter defender,
            GameLocationBattleManager battleManager, GameLocationActionManager actionManager, ReactionRequest request)
        {
            var character = attacker.RulesetCharacter;
            var reactionParams = request.reactionParams;
            //Can we detect this before attack starts? Currently we get to this part after attack finishes, if reaction was validated
            if (reactionParams.ReactionValidated)
            {
                character.UsePower(UsablePowersProvider.Get(pool, character));
            }

            character.RemoveAllConditionsOfCategoryAndType(AttributeDefinitions.TagCombat, condition.Name);

            yield break;
        }

        protected override ReactionRequest MakeReactionRequest(GameLocationCharacter attacker,
            GameLocationCharacter defender, RulesetAttackMode attackMode, ActionModifier attackModifier)
        {
            return new ReactionRequestReactionAttack("GambitBrace", new CharacterActionParams(
                attacker,
                Id.AttackOpportunity,
                attackMode,
                defender,
                attackModifier)) { Resource = new ReactionResourcePowerPool(pool, Sprites.GambitResourceIcon) };
        }
    }

    private class Precise : IAlterAttackOutcome
    {
        private const string Format = "Reaction/&CustomReactionGambitPreciseDescription";
        private const string Line = "Feedback/&GambitPreciseToHitRoll";
        private readonly FeatureDefinition feature;
        private readonly FeatureDefinitionPower pool;

        public Precise(FeatureDefinitionPower pool, FeatureDefinition feature)
        {
            this.pool = pool;
            this.feature = feature;
        }

        public IEnumerator TryAlterAttackOutcome(GameLocationBattleManager battle, CharacterAction action,
            GameLocationCharacter me, GameLocationCharacter target, ActionModifier attackModifier)
        {
            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (manager == null)
            {
                yield break;
            }

            var character = me.RulesetCharacter;

            if (character.GetRemainingPowerCharges(pool) <= 0)
            {
                yield break;
            }

            var dieType = GetGambitDieSize(character);
            var max = DiceMaxValue[(int)dieType];
            var delta = Math.Abs(action.AttackSuccessDelta);
            if (max < delta)
            {
                yield break;
            }

            var guiMe = new GuiCharacter(me);
            var guiTarget = new GuiCharacter(target);

            var description = Gui.Format(Format, guiMe.Name, guiTarget.Name, delta.ToString(),
                Gui.FormatDieTitle(dieType));
            var reactionParams =
                new CharacterActionParams(me, (Id)ExtraActionId.DoNothingFree) { StringParameter = description };

            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("GambitPrecise", reactionParams)
            {
                Resource = new ReactionResourcePowerPool(pool, Sprites.GambitResourceIcon)
            };

            manager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(me, manager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            character.UpdateUsageForPower(pool, 1);

            var dieRoll = RollDie(dieType, AdvantageType.None, out _, out _);

            var hitTrends = attackModifier.AttacktoHitTrends;

            hitTrends?.Add(new TrendInfo(dieRoll, FeatureSourceType.Power, pool.Name, null)
            {
                dieType = dieType, dieFlag = TrendInfoDieFlag.None
            });

            action.AttackSuccessDelta += dieRoll;
            attackModifier.attackRollModifier += dieRoll;

            var success = action.AttackSuccessDelta >= 0;

            if (success)
            {
                action.AttackRollOutcome = RollOutcome.Success;
            }

            character.ShowDieRoll(dieType, dieRoll,
                title: feature.GuiPresentation.Title,
                outcome: success ? RollOutcome.Success : RollOutcome.Failure,
                displayOutcome: true
            );


            GameConsoleHelper.LogCharacterUsedFeature(character, feature, Line,
                extra: new[]
                {
                    (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(dieType)),
                    (ConsoleStyleDuplet.ParameterType.Positive, dieRoll.ToString())
                });
        }
    }

    private class Parry : IDefenderBeforeAttackHitConfirmed
    {
        private const string Format = "Reaction/&CustomReactionGambitParryDescription";
        private const string Line = "Feedback/&GambitParryDamageReduction";
        private readonly FeatureDefinition feature;
        private readonly FeatureDefinitionPower pool;

        public Parry(FeatureDefinitionPower pool, FeatureDefinition feature)
        {
            this.pool = pool;
            this.feature = feature;
        }

        public IEnumerator DefenderBeforeAttackHitConfirmed(
            GameLocationBattleManager battle,
            GameLocationCharacter attacker,
            GameLocationCharacter me,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool criticalHit,
            bool firstTarget)
        {
            if (rangedAttack)
            {
                yield break;
            }

            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (manager == null)
            {
                yield break;
            }

            if (!me.CanReact())
            {
                yield break;
            }

            var character = me.RulesetCharacter;


            if (character.GetRemainingPowerCharges(pool) <= 0)
            {
                yield break;
            }

            var dieType = GetGambitDieSize(character);

            var guiMe = new GuiCharacter(me);
            var guiTarget = new GuiCharacter(attacker);

            var description = Gui.Format(Format, guiMe.Name, guiTarget.Name, Gui.FormatDieTitle(dieType));
            var reactionParams =
                new CharacterActionParams(me, (Id)ExtraActionId.DoNothingReaction) { StringParameter = description };

            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("GambitParry", reactionParams)
            {
                Resource = new ReactionResourcePowerPool(pool, Sprites.GambitResourceIcon)
            };

            manager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(me, manager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            character.UpdateUsageForPower(pool, 1);

            var dieRoll = RollDie(dieType, AdvantageType.None, out _, out _);

            var pb = 2 * character.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            var reduction = dieRoll + pb;

            attackModifier.damageRollReduction += reduction;

            character.ShowDieRoll(dieType, dieRoll,
                title: feature.GuiPresentation.Title,
                displayModifier: true, modifier: pb);

            GameConsoleHelper.LogCharacterUsedFeature(character, feature, Line,
                extra: new[]
                {
                    (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(dieType)),
                    (ConsoleStyleDuplet.ParameterType.Positive, reduction.ToString())
                });
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

        public void OnAfterAction(CharacterAction action)
        {
            if (action is not CharacterActionActionSurge)
            {
                return;
            }

            var character = action.ActingCharacter.RulesetCharacter;
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
