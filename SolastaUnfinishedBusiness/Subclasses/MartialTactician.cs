using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomBuilders;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
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

        // backward compatibility
        _ = BuildEverVigilant();
        _ = BuildSharedVigilance();
        _ = BuildGambitDieSize(DieType.D8);

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite(Name, Resources.MartialTactician, 256))
            .AddFeaturesAtLevel(3, BuildSharpMind(), GambitsBuilders.GambitPool,
                GambitsBuilders.Learn3Gambit)
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

    private static FeatureDefinitionAttributeModifier BuildEverVigilant()
    {
        return FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierTacticianEverVigilant")
            .SetGuiPresentation(Category.Feature)
            .SetModifierAbilityScore(AttributeDefinitions.Initiative, AttributeDefinitions.Intelligence)
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildSharedVigilance()
    {
        return FeatureDefinitionPowerBuilder
            .Create("PowerTacticianSharedVigilance")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .SetUsesFixed(ActivationTime.PermanentUnlessIncapacitated)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 6)
                    .ExcludeCaster()
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                    .SetDurationData(DurationType.Permanent)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitionBuilder
                                    .Create("ConditionTacticianSharedVigilance")
                                    .SetGuiPresentationNoContent(true)
                                    .SetSilent(Silent.WhenAddedOrRemoved)
                                    .SetAmountOrigin(ExtraOriginOfAmount.SourceAbilityBonus,
                                        AttributeDefinitions.Intelligence)
                                    .SetFeatures(
                                        FeatureDefinitionAttributeModifierBuilder
                                            .Create("AttributeModifierTacticianSharedVigilance")
                                            .SetGuiPresentation("AttributeModifierTacticianEverVigilant",
                                                Category.Feature)
                                            .SetAddConditionAmount(AttributeDefinitions.Initiative)
                                            .AddToDB())
                                    .AddToDB(), ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
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

        feature.AddCustomSubFeatures(new RefundPowerUsePhysicalAttackAfterCrit(GambitsBuilders.GambitPool, feature));

        return feature;
    }

    private static FeatureDefinitionFeatureSet BuildImproviseStrategy()
    {
        var feature = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureImproviseStrategy")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(BuildGambitPoolIncrease(2, "ImproviseStrategy"))
            .AddToDB();

        feature.AddCustomSubFeatures(new RefundPowerUsePhysicalAttackAfterCrit(GambitsBuilders.GambitPool, feature));

        return feature;
    }

    private static FeatureDefinition BuildOvercomingStrategy()
    {
        var feature = FeatureDefinitionBuilder
            .Create("FeatureOvercomingStrategy")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        feature.AddCustomSubFeatures(new OnReducedToZeroHpByMeRefundPowerUse(GambitsBuilders.GambitPool, feature));

        ConditionDefinitionBuilder
            .Create(MarkDamagedByGambit)
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddCustomSubFeatures(
                new RefundPowerUseWhenTargetWithConditionDies(GambitsBuilders.GambitPool, feature),
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

    private static FeatureDefinitionFeatureSet BuildTacticalAwareness()
    {
        var additionalDamageTacticalAwareness = FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageTacticianTacticalAwareness")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("TacticalAwareness")
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.ProficiencyBonus)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .AddToDB();

        var combatAffinityTacticalAwareness = FeatureDefinitionCombatAffinityBuilder
            .Create("CombatAffinityTacticianTacticalAwareness")
            .SetGuiPresentation("FeatureSetTacticianTacticalAwareness", Category.Feature)
            .SetAttackOfOpportunityOnMeAdvantage(AdvantageType.Disadvantage)
            .AddToDB();

        combatAffinityTacticalAwareness.AddCustomSubFeatures(
            new PhysicalAttackInitiatedByMeTacticalAwareness(combatAffinityTacticalAwareness));

        return FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetTacticianTacticalAwareness")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(additionalDamageTacticalAwareness, combatAffinityTacticalAwareness)
            .AddToDB();
    }

#if false
    private static void BuildTacticalSurge()
    {
        const string CONDITION_NAME = "ConditionTacticianTacticalSurge";

        var tick = FeatureDefinitionBuilder
            .Create("FeatureTacticianTacticalSurgeTick")
            .SetGuiPresentation(CONDITION_NAME, Category.Condition)
            .AddToDB();

        tick.AddCustomSubFeatures(new TacticalSurgeTick(GambitsBuilders.GambitPool, tick));

        var feature = FeatureDefinitionBuilder
            .Create("FeatureTacticianTacticalSurge")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var condition = ConditionDefinitionBuilder
            .Create(CONDITION_NAME)
            .SetGuiPresentation(Category.Condition)
            .SetPossessive()
            .SetFeatures(tick)
            .AddToDB();

        feature.AddCustomSubFeatures(new TacticalSurge(GambitsBuilders.GambitPool, feature, condition));
    }
#endif

    private class RefundPowerUsePhysicalAttackAfterCrit : IPhysicalAttackAfterDamage
    {
        private readonly FeatureDefinition _feature;
        private readonly FeatureDefinitionPower _power;

        public RefundPowerUsePhysicalAttackAfterCrit(FeatureDefinitionPower power, FeatureDefinition feature)
        {
            _power = power;
            _feature = feature;
        }

        public void OnPhysicalAttackAfterDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            if (outcome is not (RollOutcome.CriticalFailure or RollOutcome.CriticalSuccess))
            {
                // ReSharper disable once InvocationIsSkipped
                Main.Log("AdaptiveStrategy: not critical. exiting.");
                return;
            }

            if (attackMode == null)
            {
                return;
            }

            // once per turn
            if (!attacker.OncePerTurnIsValid("AdaptiveStrategy"))
            {
                // ReSharper disable once InvocationIsSkipped
                Main.Log("AdaptiveStrategy: once per turn. exiting.");
                return;
            }

            var character = attacker.RulesetCharacter;

            if (character is not { IsDeadOrDyingOrUnconscious: false })
            {
                return;
            }

            if (character.GetRemainingPowerUses(_power) >= character.GetMaxUsesForPool(_power))
            {
                // ReSharper disable once InvocationIsSkipped
                Main.Log("AdaptiveStrategy: nothing to refuel. exiting.");
                return;
            }

            character.LogCharacterUsedFeature(_feature, indent: true);
            attacker.UsedSpecialFeatures.TryAdd("AdaptiveStrategy", 1);
            character.UpdateUsageForPower(_power, -1);
            // ReSharper disable once InvocationIsSkipped
            Main.Log("AdaptiveStrategy: refueled.");
        }
    }

    private class OnReducedToZeroHpByMeRefundPowerUse : IOnReducedToZeroHpByMe
    {
        private readonly FeatureDefinition _feature;
        private readonly FeatureDefinitionPower _power;

        public OnReducedToZeroHpByMeRefundPowerUse(FeatureDefinitionPower power, FeatureDefinition feature)
        {
            _power = power;
            _feature = feature;
        }

        public IEnumerator HandleReducedToZeroHpByMe(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            if (attacker.RulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (downedCreature.RulesetCharacter.HasConditionOfType(MarkDamagedByGambit))
            {
                // ReSharper disable once InvocationIsSkipped
                Main.Log("OvercomingStrategy: enemy is marked. exiting.");
                yield break;
            }

            if (attackMode == null)
            {
                yield break;
            }

            // once per turn
            if (!attacker.OncePerTurnIsValid("OvercomingStrategy"))
            {
                // ReSharper disable once InvocationIsSkipped
                Main.Log("OvercomingStrategy: once per turn. exiting.");
                yield break;
            }

            var character = attacker.RulesetCharacter;

            if (character is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (character.GetRemainingPowerUses(_power) >= character.GetMaxUsesForPool(_power))
            {
                // ReSharper disable once InvocationIsSkipped
                Main.Log("OvercomingStrategy: nothing to refuel. exiting.");
                yield break;
            }

            character.LogCharacterUsedFeature(_feature, indent: true);
            attacker.UsedSpecialFeatures.TryAdd("OvercomingStrategy", 1);
            character.UpdateUsageForPower(_power, -1);
            // ReSharper disable once InvocationIsSkipped
            Main.Log("OvercomingStrategy: refueled.");
        }
    }

    private class RefundPowerUseWhenTargetWithConditionDies : IOnConditionAddedOrRemoved
    {
        private readonly FeatureDefinition _feature;
        private readonly FeatureDefinitionPower _power;

        public RefundPowerUseWhenTargetWithConditionDies(FeatureDefinitionPower power, FeatureDefinition feature)
        {
            _power = power;
            _feature = feature;
        }

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

            if (character == null)
            {
                return;
            }

            if (!character.HasAnyFeature(_feature))
            {
                return;
            }

            var locCharacter = GameLocationCharacter.GetFromActor(character);

            if (locCharacter == null)
            {
                return;
            }

            // once per turn
            if (!locCharacter.OncePerTurnIsValid("OvercomingStrategy"))
            {
                return;
            }

            if (character.GetRemainingPowerUses(_power) >= character.GetMaxUsesForPool(_power))
            {
                return;
            }

            character.LogCharacterUsedFeature(_feature, indent: true);
            locCharacter.UsedSpecialFeatures.TryAdd("OvercomingStrategy", 1);
            character.UpdateUsageForPower(_power, -1);
        }
    }

#if false
    private class TacticalSurge : IActionFinishedByMe
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

        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            if (action is not CharacterActionActionSurge)
            {
                yield break;
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
#endif

    private sealed class PhysicalAttackInitiatedByMeTacticalAwareness : IPhysicalAttackInitiatedByMe
    {
        private readonly FeatureDefinition _featureDefinition;

        public PhysicalAttackInitiatedByMeTacticalAwareness(FeatureDefinition featureDefinition)
        {
            _featureDefinition = featureDefinition;
        }

        public IEnumerator OnPhysicalAttackInitiatedByMe(
            GameLocationBattleManager __instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackerAttackMode)
        {
            if (attackerAttackMode.actionType != ActionDefinitions.ActionType.Reaction &&
                !attackerAttackMode.attackTags.Contains(TacticalAwareness))
            {
                yield break;
            }

            attackModifier.attackAdvantageTrends.Add(
                new TrendInfo(1, FeatureSourceType.CharacterFeature, _featureDefinition.Name, _featureDefinition));
        }
    }
}
