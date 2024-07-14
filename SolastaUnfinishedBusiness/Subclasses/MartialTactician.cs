using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Subclasses.Builders;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class MartialTactician : AbstractSubclass
{
    internal const string Name = "MartialTactician";
    internal const string MarkDamagedByGambit = "ConditionTacticianDamagedByGambit";
    internal const string TacticalAwareness = "TacticalAwareness";

    private static int _gambitPoolIncreases;

    public MartialTactician()
    {
        var unlearn = BuildUnlearn();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite(Name, Resources.MartialTactician, 256))
            .AddFeaturesAtLevel(3, BuildSharpMind(), BuildGambitPoolIncrease(4, Name),
                GambitsBuilders.Learn3Gambit,
                GambitsBuilders.GambitPool)
            .AddFeaturesAtLevel(7, BuildHonedCraft(), BuildGambitPoolIncrease(),
                GambitsBuilders.Learn2Gambit,
                unlearn)
            .AddFeaturesAtLevel(10, BuildStrategicPlan(), BuildGambitDieSize(DieType.D10),
                unlearn)
            .AddFeaturesAtLevel(15, BuildBattleClarity(), BuildGambitPoolIncrease(),
                GambitsBuilders.Learn2Gambit,
                unlearn)
            .AddFeaturesAtLevel(18, BuildTacticalAwareness(), BuildGambitDieSize(DieType.D12),
                unlearn)
            .AddToDB();

        GambitsBuilders.BuildGambits();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Fighter;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;

    internal override DeityDefinition DeityDefinition => null;

    private static FeatureDefinitionFeatureSet BuildSharpMind()
    {
        return FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetTacticianSharpMind")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionPointPoolBuilder
                    .Create("PointPoolTacticianSharpMindSkill")
                    .SetGuiPresentationNoContent()
                    .SetPool(HeroDefinitions.PointsPoolType.Skill, 1)
                    .AddToDB())
            .AddToDB();
    }

    private static FeatureDefinitionFeatureSet BuildHonedCraft()
    {
        return FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetTacticianHonedCraft")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionPointPoolBuilder
                    .Create("PointPoolTacticianSharpMindExpertise")
                    .SetGuiPresentationNoContent()
                    .SetPool(HeroDefinitions.PointsPoolType.Expertise, 1)
                    .AddToDB())
            .AddToDB();
    }

    private static FeatureDefinitionFeatureSet BuildBattleClarity()
    {
        var features = new FeatureDefinition[]
        {
            FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityCreedOfMisaye,
            FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityCreedOfPakri,
            FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityCreedOfMaraike,
            FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityCreedOfSolasta
        };

        foreach (var feature in features.OfType<FeatureDefinitionSavingThrowAffinity>())
        {
            var term = $"Attribute/&{feature.affinityGroups[0].abilityScoreName}TitleLong";

            feature.GuiPresentation.title = term;
            feature.GuiPresentation.description = term;
        }

        return FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetTacticianBattleClarity")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
            .AddFeatureSet(features)
            .AddToDB();
    }

    private static FeatureDefinitionPowerUseModifier BuildGambitPoolIncrease()
    {
        return FeatureDefinitionPowerUseModifierBuilder
            .Create($"PowerUseModifierTacticianGambitPool{_gambitPoolIncreases++:D2}")
            .SetGuiPresentation("PowerUseModifierTacticianGambitPool", Category.Feature)
            .SetFixedValue(GambitsBuilders.GambitPool, 1)
            .AddToDB();
    }

    internal static FeatureDefinition BuildGambitPoolIncrease(int number, string name)
    {
        return FeatureDefinitionPowerUseModifierBuilder
            .Create($"PowerUseModifierTacticianGambitPool{name}")
            .SetGuiPresentation("PowerUseModifierTacticianGambitPool", Category.Feature)
            .SetFixedValue(GambitsBuilders.GambitPool, number)
            .AddToDB();
    }

    private static FeatureDefinitionFeatureSet BuildStrategicPlan()
    {
        return FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSefTacticianStrategicPlan")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
            .AddFeatureSet(
                BuildAdaptiveStrategy(),
                BuildImproviseStrategy(),
                BuildOvercomingStrategy())
            .AddToDB();
    }

    private static FeatureDefinition BuildAdaptiveStrategy()
    {
        var feature = FeatureDefinitionBuilder
            .Create("FeatureAdaptiveStrategy")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        feature.AddCustomSubFeatures(
            new PhysicalAttackFinishedByMeAdaptiveStrategy(GambitsBuilders.GambitPool, feature));

        return feature;
    }

    private static FeatureDefinitionFeatureSet BuildImproviseStrategy()
    {
        var feature = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureImproviseStrategy")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(BuildGambitPoolIncrease(2, "ImproviseStrategy"))
            .AddToDB();

        return feature;
    }

    private static FeatureDefinition BuildOvercomingStrategy()
    {
        var feature = FeatureDefinitionBuilder
            .Create("FeatureOvercomingStrategy")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        ConditionDefinitionBuilder
            .Create(MarkDamagedByGambit)
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddCustomSubFeatures(new RefundPowerUseWhenTargetWithConditionDies(GambitsBuilders.GambitPool, feature))
            .SetSpecialDuration(DurationType.Round, 1, (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn)
            .AddToDB();

        return feature;
    }

    private static FeatureDefinitionCustomInvocationPool BuildUnlearn()
    {
        return CustomInvocationPoolDefinitionBuilder
            .Create("InvocationPoolGambitUnlearn")
            .SetGuiPresentation(Category.Feature)
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

    private static FeatureDefinition BuildTacticalAwareness()
    {
        var feature = FeatureDefinitionBuilder
            .Create("FeatureSetTacticianTacticalAwareness")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        feature.AddCustomSubFeatures(new CharacterTurnStartListenerTacticalAwareness(feature));

        return feature;
    }

    private class PhysicalAttackFinishedByMeAdaptiveStrategy(
        FeatureDefinitionPower power,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition feature)
        : IPhysicalAttackFinishedByMe
    {
        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            if (rollOutcome is not (RollOutcome.CriticalFailure or RollOutcome.CriticalSuccess))
            {
                yield break;
            }

            if (attackMode == null)
            {
                yield break;
            }

            // once per turn
            if (!attacker.OncePerTurnIsValid(feature.Name))
            {
                yield break;
            }

            var character = attacker.RulesetCharacter;

            if (character is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (character.GetRemainingPowerUses(power) >= character.GetMaxUsesForPool(power))
            {
                yield break;
            }

            attacker.UsedSpecialFeatures.TryAdd(feature.Name, 1);
            character.LogCharacterUsedFeature(feature, indent: true);
            character.UpdateUsageForPower(power, -1);
        }
    }

    private class RefundPowerUseWhenTargetWithConditionDies(
        FeatureDefinitionPower power,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition feature)
        : IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // SHOULD ONLY TRIGGER ON DEATH
            if (target is not { IsDeadOrDyingOrUnconscious: true })
            {
                return;
            }

            var character = EffectHelpers.GetCharacterByGuid(rulesetCondition.sourceGuid);

            if (character == null || character.GetClassLevel(CharacterClassDefinitions.Fighter) < 10)
            {
                return;
            }

            var glc = GameLocationCharacter.GetFromActor(character);

            if (glc == null)
            {
                return;
            }

            // once per turn
            if (!glc.OncePerTurnIsValid(feature.Name))
            {
                return;
            }

            if (character.GetRemainingPowerUses(power) >= character.GetMaxUsesForPool(power))
            {
                return;
            }

            glc.UsedSpecialFeatures.TryAdd(feature.Name, 1);
            character.LogCharacterUsedFeature(feature, indent: true);
            character.UpdateUsageForPower(power, -1);
        }
    }

    private sealed class CharacterTurnStartListenerTacticalAwareness(FeatureDefinition featureDefinition)
        : ICharacterTurnStartListener
    {
        public void OnCharacterTurnStarted(GameLocationCharacter character)
        {
            var rulesetCharacter = character.RulesetCharacter;
            var usablePower = PowerProvider.Get(GambitsBuilders.GambitPool, rulesetCharacter);

            usablePower.RepayUse();
            rulesetCharacter.LogCharacterUsedFeature(featureDefinition);
        }
    }
}
