using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomUI;
using UnityEngine.AddressableAssets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class MartialTactician : AbstractSubclass
{
    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;

    //TODO: seems to be used somewhere
    internal const string CounterStrikeTag = "CounterStrike";
    internal FeatureDefinitionPower GambitPool { get; set; }
    public FeatureDefinitionAdditionalDamage GambitDieDamage { get; set; }

    private int _gambitPoolIncreases;


    internal MartialTactician()
    {
        GambitPool = FeatureDefinitionPowerBuilder
            .Create("PowerPoolTacticianGambit")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest, 1, 4)
            .AddToDB();

        GambitPool.AddCustomSubFeatures(new CustomPortraitPoolPower(GambitPool, icon: Sprites.GambitResourceIcon));

        GambitDieDamage = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamageGambitDie")
            .SetGuiPresentationNoContent(hidden: true)
            .SetDamageDice(DieType.D6, 1)
            .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
            .SetNotificationTag("GambitDie")
            .AddToDB();

        var learn1Gambit = BuildLearn(1);
        var learn3Gambits = BuildLearn(3);

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("MartialTactician")
            .SetGuiPresentation(Category.Subclass, RoguishShadowCaster)
            .AddFeaturesAtLevel(3, GambitPool, learn3Gambits)
            .AddFeaturesAtLevel(7, BuildGambitPoolIncrease(), learn1Gambit)
            .AddFeaturesAtLevel(10)
            .AddFeaturesAtLevel(15, BuildGambitPoolIncrease(), learn1Gambit)
            .AddToDB();

        BuildGambits();
    }


    private FeatureDefinition BuildGambitPoolIncrease()
    {
        return FeatureDefinitionPowerUseModifierBuilder
            .Create($"PowerUseModifierTacticianGambitPool{_gambitPoolIncreases++:D2}")
            .SetGuiPresentation("PowerUseModifierTacticianGambitPool", Category.Feature)
            .SetFixedValue(GambitPool, 1)
            .AddToDB();
    }

    private FeatureDefinitionCustomInvocationPool BuildLearn(int points)
    {
        return CustomInvocationPoolDefinitionBuilder
            .Create($"InvocationPoolGambitLearn{points}")
            .SetGuiPresentation(Category.Feature)
            .Setup(InvocationPoolTypeCustom.Pools.Gambit, points)
            .AddToDB();
    }

    private void BuildGambits()
    {
        var name = "GambitKnockdown";
        //TODO: add proper icon
        var sprite = SpellDefinitions.Banishment.GuiPresentation.SpriteReference;

        var reaction = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{name}React")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden, ForcePowerUseInSpendPowerAction.Marker)
            .SetSharedPool(ActivationTime.OnAttackHitMeleeAuto, GambitPool, 1)
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
            .AddToDB();

        var power = FeatureDefinitionPowerBuilder
            .Create($"Power{name}Acivate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            //TODO: add limitaer so only 1 on-attack power is active
            .SetCustomSubFeatures(PowerFromInvocation.Marker)
            .SetUniqueInstance()
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                .SetDurationData(DurationType.Round, 1)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(ConditionDefinitionBuilder
                        .Create($"Condition{name}")
                        .SetGuiPresentation(name, Category.Feature, Sprites.ConditionGambit)
                        .SetCustomSubFeatures(new AddUsablePowerFromCondition(reaction))
                        .SetSilent(Silent.None)
                        .SetPossessive()
                        //TODO: should it not end if we missed?
                        .SetSpecialInterruptions(ConditionInterruption.Attacks, ConditionInterruption.AnyBattleTurnEnd)
                        .SetFeatures(GambitDieDamage)
                        .AddToDB(), ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .AddToDB();

        BuildFeatureInvocation(name, sprite, power);
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
}
