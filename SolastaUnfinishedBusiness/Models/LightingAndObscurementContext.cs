using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using TA;
using UnityEngine;
using static LocationDefinitions;
using static SolastaUnfinishedBusiness.Spells.SpellBuilders;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.EffectProxyDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCombatAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPerceptionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.InvocationDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static class LightingAndObscurementContext
{
    private const string BlindExtendedDescription = "Condition/&ConditionBlindedExtendedDescription";

    // ProxyDarkness is a special use case that is handled apart
    private static readonly HashSet<EffectProxyDefinition> SightImpairedProxies =
    [
        ProxyCloudKill,
        ProxyFogCloud,
        ProxyIncendiaryCloud,
        ProxyPetalStorm,
        ProxySleetStorm,
        ProxyStinkingCloud
    ];

    internal static readonly ConditionDefinition ConditionBlindedByDarkness = ConditionDefinitionBuilder
        .Create(ConditionBlinded, "ConditionBlindedByDarkness")
        .SetOrUpdateGuiPresentation(Category.Condition)
        .SetParentCondition(ConditionBlinded)
        .SetFeatures()
        .AddToDB();

    private static readonly ConditionDefinition ConditionBlindedByCloudKill = ConditionDefinitionBuilder
        .Create(ConditionBlinded, "ConditionBlindedByCloudKill")
        .SetOrUpdateGuiPresentation(Category.Condition)
        .SetParentCondition(ConditionBlinded)
        .SetFeatures()
        .AddToDB();

    private static readonly ConditionDefinition ConditionBlindedByFogCloud = ConditionDefinitionBuilder
        .Create(ConditionBlinded, "ConditionBlindedByFogCloud")
        .SetOrUpdateGuiPresentation(Category.Condition)
        .SetParentCondition(ConditionBlinded)
        .SetFeatures()
        .AddToDB();

    private static readonly ConditionDefinition ConditionBlindedByIncendiaryCloud = ConditionDefinitionBuilder
        .Create(ConditionBlinded, "ConditionBlindedByIncendiaryCloud")
        .SetOrUpdateGuiPresentation(Category.Condition)
        .SetParentCondition(ConditionBlinded)
        .SetFeatures()
        .AddToDB();

    private static readonly ConditionDefinition ConditionBlindedByPetalStorm = ConditionDefinitionBuilder
        .Create(ConditionBlinded, "ConditionBlindedByPetalStorm")
        .SetOrUpdateGuiPresentation(Category.Condition)
        .SetParentCondition(ConditionBlinded)
        .SetFeatures()
        .AddToDB();

    private static readonly ConditionDefinition ConditionBlindedBySleetStorm = ConditionDefinitionBuilder
        .Create(ConditionBlinded, "ConditionBlindedBySleetStorm")
        .SetOrUpdateGuiPresentation(Category.Condition)
        .SetParentCondition(ConditionBlinded)
        .SetFeatures()
        .AddToDB();

    private static readonly ConditionDefinition ConditionBlindedByStinkingCloud = ConditionDefinitionBuilder
        .Create(ConditionBlinded, "ConditionBlindedByStinkingCloud")
        .SetOrUpdateGuiPresentation(Category.Condition)
        .SetParentCondition(ConditionBlinded)
        .SetFeatures()
        .AddToDB();

    private static readonly ConditionDefinition ConditionLightlyObscured = ConditionDefinitionBuilder
        .Create(ConditionHeavilyObscured, "ConditionLightlyObscured")
        .SetOrUpdateGuiPresentation(Category.Condition)
        .SetFeatures(
            FeatureDefinitionAbilityCheckAffinityBuilder
                .Create("AbilityCheckAffinityLightlyObscured")
                .SetOrUpdateGuiPresentation("ConditionLightlyObscured", Category.Condition)
                .BuildAndSetAffinityGroups(RuleDefinitions.CharacterAbilityCheckAffinity.Disadvantage,
                    abilityProficiencyPairs: (AttributeDefinitions.Wisdom, SkillDefinitions.Perception))
                .AddToDB())
        .AddToDB();

    private static readonly EffectForm FormBlindedByCloudKill =
        EffectFormBuilder.ConditionForm(ConditionBlindedByCloudKill);

    private static readonly EffectForm FormBlindedByIncendiaryCloud =
        EffectFormBuilder.ConditionForm(ConditionBlindedByIncendiaryCloud);

    private static readonly EffectForm FormLightlyObscured = EffectFormBuilder.ConditionForm(ConditionLightlyObscured);

    private static readonly EffectForm FormProjectileBlocker = EffectFormBuilder
        .Create()
        .SetTopologyForm(TopologyForm.Type.ProjectileBlocker, true)
        .Build();

    private static readonly Dictionary<int3, LightingState> PositionLightingStateCache = [];

    internal static void LateLoad()
    {
        ConditionBlindedByDarkness.GuiPresentation.description = BlindExtendedDescription;
        ConditionBlindedByCloudKill.GuiPresentation.description = BlindExtendedDescription;
        ConditionBlindedByFogCloud.GuiPresentation.description = BlindExtendedDescription;
        ConditionBlindedByIncendiaryCloud.GuiPresentation.description = BlindExtendedDescription;
        ConditionBlindedByPetalStorm.GuiPresentation.description = BlindExtendedDescription;
        ConditionBlindedBySleetStorm.GuiPresentation.description = BlindExtendedDescription;
        ConditionBlindedByStinkingCloud.GuiPresentation.description = BlindExtendedDescription;
        SwitchOfficialObscurementRules();
    }

    internal static void SwitchOfficialObscurementRules()
    {
        foreach (var condition in DatabaseRepository.GetDatabase<ConditionDefinition>()
                     .Where(x => x.IsSubtypeOf(ConditionBlinded.Name)))
        {
            condition.GuiPresentation.description = Main.Settings.UseOfficialLightingObscurementAndVisionRules
                ? BlindExtendedDescription
                : "Rules/&ConditionBlindedDescription";
        }

        SwitchHeavilyObscuredOnObscurementRules();
        SwitchMagicalDarknessOnObscurementRules();
        SwitchMonstersOnObscurementRules();
        SrdAndHouseRulesContext.SwitchConditionBlindedShouldNotAllowOpportunityAttack();

        if (Main.Settings.UseOfficialLightingObscurementAndVisionRules)
        {
            ConditionBlinded.Features.SetRange(
                CombatAffinityHeavilyObscured,
                CombatAffinityHeavilyObscuredSelf,
                PerceptionAffinityConditionBlinded);

            // >> ConditionVeil
            // ConditionAffinityVeilImmunity
            // PowerDefilerDarkness

            ConditionAffinityVeilImmunity.conditionType = ConditionBlindedByDarkness.Name;

            PowerDefilerDarkness.EffectDescription.EffectForms[1].ConditionForm.ConditionDefinition =
                ConditionBlindedByDarkness;

            // >> ConditionDarkness
            // ConditionAffinityInvocationDevilsSight
            // Darkness

            FeatureSetInvocationDevilsSight.FeatureSet.SetRange(SenseTruesight16, SenseDarkvision24);
            DevilsSight.GuiPresentation.description = "Invocation/&DevilsSightExtendedDescription";

            Darkness.EffectDescription.EffectForms[1].ConditionForm.ConditionDefinition = ConditionBlindedByDarkness;

            // >> ConditionHeavilyObscured
            // FogCloud
            // PetalStorm

            FogCloud.EffectDescription.EffectForms[0].ConditionForm.ConditionDefinition = ConditionBlindedByFogCloud;

            SpellsContext.PetalStorm.EffectDescription.EffectForms[1].ConditionForm.ConditionDefinition =
                ConditionBlindedByPetalStorm;

            // >> ConditionInStinkingCloud
            // StinkingCloud

            StinkingCloud.EffectDescription.EffectForms[0].ConditionForm.ConditionDefinition =
                ConditionBlindedByStinkingCloud;

            // >> ConditionSleetStorm
            // SleetStorm

            SleetStorm.EffectDescription.EffectForms[0].ConditionForm.ConditionDefinition =
                ConditionBlindedBySleetStorm;

            // Cloud Kill / Incendiary Cloud need same debuff as other heavily obscured
            CloudKill.EffectDescription.EffectForms.TryAdd(FormBlindedByCloudKill);
            IncendiaryCloud.EffectDescription.EffectForms.TryAdd(FormBlindedByIncendiaryCloud);

            // Make Insect Plague lightly obscured
            InsectPlague.EffectDescription.EffectForms.Add(FormLightlyObscured);
            InsectPlague.EffectDescription.EffectForms[1].TopologyForm.changeType = TopologyForm.Type.None;

            // vanilla has this set as disadvantage so we flip it with nullified requirements
            CombatAffinityHeavilyObscured.attackOnMeAdvantage = RuleDefinitions.AdvantageType.Advantage;
            CombatAffinityHeavilyObscured.nullifiedBySenses = [];
            CombatAffinityHeavilyObscured.nullifiedBySelfSenses =
                [SenseMode.Type.Blindsight, SenseMode.Type.Tremorsense];

            CombatAffinityHeavilyObscuredSelf.nullifiedBySenses = [];
            CombatAffinityHeavilyObscuredSelf.nullifiedBySelfSenses =
                [SenseMode.Type.Blindsight, SenseMode.Type.Tremorsense];
        }
        else
        {
            ConditionBlinded.Features.SetRange(
                CombatAffinityBlinded,
                PerceptionAffinityConditionBlinded);

            // >> ConditionVeil
            // ConditionAffinityVeilImmunity
            // PowerDefilerDarkness

            ConditionAffinityVeilImmunity.conditionType = ConditionVeil.Name;

            PowerDefilerDarkness.EffectDescription.EffectForms[1].ConditionForm.ConditionDefinition = ConditionVeil;

            // >> ConditionDarkness
            // ConditionAffinityInvocationDevilsSight
            // Darkness

            FeatureSetInvocationDevilsSight.FeatureSet.SetRange(
                SenseBlindSight16,
                SenseSeeInvisible16,
                ConditionAffinityInvocationDevilsSight);
            DevilsSight.GuiPresentation.description = "Invocation/&DevilsSightDescription";

            Darkness.EffectDescription.EffectForms[1].ConditionForm.ConditionDefinition = ConditionDarkness;

            // >> ConditionHeavilyObscured
            // FogCloud
            // PetalStorm

            FogCloud.EffectDescription.EffectForms[0].ConditionForm.ConditionDefinition = ConditionHeavilyObscured;

            SpellsContext.PetalStorm.EffectDescription.EffectForms[1].ConditionForm.ConditionDefinition =
                ConditionHeavilyObscured;

            // >> ConditionInStinkingCloud
            // StinkingCloud

            StinkingCloud.EffectDescription.EffectForms[0].ConditionForm.ConditionDefinition = ConditionInStinkingCloud;

            // >> ConditionSleetStorm
            // SleetStorm

            SleetStorm.EffectDescription.EffectForms[0].ConditionForm.ConditionDefinition = ConditionSleetStorm;

            // Cloud Kill / Incendiary Cloud need same debuff as other heavily obscured
            CloudKill.EffectDescription.EffectForms.Remove(FormBlindedByCloudKill);
            IncendiaryCloud.EffectDescription.EffectForms.Remove(FormBlindedByIncendiaryCloud);

            // Remove lightly obscured from Insect Plague
            InsectPlague.EffectDescription.EffectForms.Remove(FormLightlyObscured);
            InsectPlague.effectDescription.EffectForms[1].TopologyForm.changeType = TopologyForm.Type.SightImpaired;

            // vanilla has this set as disadvantage so we flip it with nullified requirements
            CombatAffinityHeavilyObscured.attackOnMeAdvantage = RuleDefinitions.AdvantageType.Disadvantage;
            CombatAffinityHeavilyObscured.nullifiedBySenses =
                [SenseMode.Type.Truesight, SenseMode.Type.Blindsight];
            CombatAffinityHeavilyObscured.nullifiedBySelfSenses = [];

            CombatAffinityHeavilyObscuredSelf.nullifiedBySenses = [];
            CombatAffinityHeavilyObscuredSelf.nullifiedBySelfSenses =
                [SenseMode.Type.Truesight, SenseMode.Type.Blindsight];
        }
    }

    internal static void SwitchHeavilyObscuredOnObscurementRules()
    {
        if (Main.Settings.OfficialObscurementRulesHeavilyObscuredAsProjectileBlocker)
        {
            FogCloud.EffectDescription.EffectForms.TryAdd(FormProjectileBlocker);
            SpellsContext.PetalStorm.EffectDescription.EffectForms.TryAdd(FormProjectileBlocker);
            StinkingCloud.EffectDescription.EffectForms.TryAdd(FormProjectileBlocker);
            SleetStorm.EffectDescription.EffectForms.TryAdd(FormProjectileBlocker);
            CloudKill.EffectDescription.EffectForms.TryAdd(FormProjectileBlocker);
            IncendiaryCloud.EffectDescription.EffectForms.TryAdd(FormProjectileBlocker);
        }
        else
        {
            FogCloud.EffectDescription.EffectForms.Remove(FormProjectileBlocker);
            SpellsContext.PetalStorm.EffectDescription.EffectForms.Remove(FormProjectileBlocker);
            StinkingCloud.EffectDescription.EffectForms.Remove(FormProjectileBlocker);
            SleetStorm.EffectDescription.EffectForms.Remove(FormProjectileBlocker);
            CloudKill.EffectDescription.EffectForms.Remove(FormProjectileBlocker);
            IncendiaryCloud.EffectDescription.EffectForms.Remove(FormProjectileBlocker);
        }
    }

    internal static void SwitchMagicalDarknessOnObscurementRules()
    {
        if (Main.Settings.OfficialObscurementRulesMagicalDarknessAsProjectileBlocker)
        {
            PowerDefilerDarkness.EffectDescription.EffectForms.TryAdd(FormProjectileBlocker);
            Darkness.EffectDescription.EffectForms.TryAdd(FormProjectileBlocker);
        }
        else
        {
            PowerDefilerDarkness.EffectDescription.EffectForms.Remove(FormProjectileBlocker);
            Darkness.EffectDescription.EffectForms.Remove(FormProjectileBlocker);
        }
    }

    internal static void SwitchMonstersOnObscurementRules()
    {
        foreach (var monster in DatabaseRepository.GetDatabase<MonsterDefinition>())
        {
            var name = monster.Name;

            if (Main.Settings.OfficialObscurementRulesTweakMonsters)
            {
                if (Main.Settings.MonstersThatShouldHaveDarkvision.Contains(name))
                {
                    monster.Features.TryAdd(SenseDarkvision);
                }

                if (Main.Settings.MonstersThatShouldHaveTrueSight.Contains(name))
                {
                    monster.Features.TryAdd(SenseTruesight16);
                }

                if (Main.Settings.MonstersThatShouldHaveBlindSight.Contains(name))
                {
                    monster.Features.TryAdd(SenseBlindSight16);
                }

                if (Main.Settings.MonstersThatShouldNotHaveTremorSense.Contains(name))
                {
                    monster.Features.Remove(SenseTremorsense16);
                }
            }
            else
            {
                if (Main.Settings.MonstersThatShouldHaveDarkvision.Contains(name))
                {
                    monster.Features.Remove(SenseDarkvision);
                }

                if (Main.Settings.MonstersThatShouldHaveTrueSight.Contains(name))
                {
                    monster.Features.Remove(SenseTruesight16);
                }

                if (Main.Settings.MonstersThatShouldHaveBlindSight.Contains(name))
                {
                    monster.Features.Remove(SenseBlindSight16);
                }

                if (Main.Settings.MonstersThatShouldNotHaveTremorSense.Contains(name))
                {
                    monster.Features.TryAdd(SenseTremorsense16);
                }
            }
        }
    }

    internal static void ResetState()
    {
        PositionLightingStateCache.Clear();
    }

    // called from GLBM.CanAttack to correctly determine ADV/DIS scenarios
    internal static void ApplyObscurementRules(BattleDefinitions.AttackEvaluationParams attackParams)
    {
        if (!Main.Settings.UseOfficialLightingObscurementAndVisionRules ||
            attackParams.effectDescription is
                { RangeType: not (RuleDefinitions.RangeType.MeleeHit or RuleDefinitions.RangeType.RangeHit) })
        {
            return;
        }

        const string TAG = "Perceive";

        var attackAdvantageTrends = attackParams.attackModifier.AttackAdvantageTrends;

        var attacker = attackParams.attacker;
        var attackerActor = attacker.RulesetActor;
        var attackerIsBlind = attackerActor.HasConditionOfTypeOrSubType(RuleDefinitions.ConditionBlinded);

        var defender = attackParams.defender;
        var defenderActor = defender.RulesetActor;
        var defenderIsBlind = defenderActor.HasConditionOfTypeOrSubType(RuleDefinitions.ConditionBlinded);

        HandleTrueSightSpecialCase();

        if (Main.Settings.OfficialObscurementRulesCancelAdvDisPairs)
        {
            if (attackAdvantageTrends.Any(BlindedAdvantage) &&
                attackAdvantageTrends.Any(BlindedDisadvantage))
            {
                attackAdvantageTrends.RemoveAll(BlindedAdvantage);
                attackAdvantageTrends.RemoveAll(BlindedDisadvantage);
            }
        }

        // nothing to do here if both contenders are already blinded
        if (attackerIsBlind && defenderIsBlind)
        {
            return;
        }

        var attackerIsStealthy = attackerActor.HasConditionOfType(ConditionStealthy);

        var attackerCanPerceiveDefender = attacker.CanPerceiveTarget(defender);
        var attackerHasNoLight = attacker.LightingState is LightingState.Unlit or LightingState.Darkness;

        var defenderCanPerceiveAttacker = defender.CanPerceiveTarget(attacker);
        var defenderHasNoLight = defender.LightingState is LightingState.Unlit or LightingState.Darkness;

        var adv =
            !attackerIsStealthy &&
            !defenderIsBlind &&
            !defenderCanPerceiveAttacker &&
            (attackerCanPerceiveDefender || attackerHasNoLight);

        var dis =
            !attackerIsBlind &&
            !attackerCanPerceiveDefender &&
            (defenderCanPerceiveAttacker || defenderHasNoLight);

        if (Main.Settings.OfficialObscurementRulesCancelAdvDisPairs && !(adv ^ dis))
        {
            return;
        }

        if (adv)
        {
            attackAdvantageTrends.Add(PerceiveAdvantage());
        }

        if (dis)
        {
            attackAdvantageTrends.Add(PerceiveDisadvantage());
        }

        return;

        RuleDefinitions.TrendInfo PerceiveAdvantage()
        {
            return new RuleDefinitions.TrendInfo(1, RuleDefinitions.FeatureSourceType.Lighting, TAG, attackerActor);
        }

        RuleDefinitions.TrendInfo PerceiveDisadvantage()
        {
            return new RuleDefinitions.TrendInfo(-1, RuleDefinitions.FeatureSourceType.Lighting, TAG, defenderActor);
        }

        static bool BlindedAdvantage(RuleDefinitions.TrendInfo trendInfo)
        {
            return trendInfo.sourceName == ConditionBlinded.Name && trendInfo.value == 1;
        }

        static bool BlindedDisadvantage(RuleDefinitions.TrendInfo trendInfo)
        {
            return trendInfo.sourceName == ConditionBlinded.Name && trendInfo.value == -1;
        }

        // conditions with parent inherit their features which makes true sight quite hard to manage
        // the combat affinity won't have true sight as nullified sense so we check it here and revert
        void HandleTrueSightSpecialCase()
        {
            if (attackerActor is not RulesetCharacter attackerCharacter ||
                !attackAdvantageTrends.Any(BlindedDisadvantage))
            {
                return;
            }

            var lightingState = ComputeLightingStateOnTargetPosition(attacker, defender.LocationPosition);

            if (lightingState == (LightingState)MyLightingState.HeavilyObscured)
            {
                return;
            }

            var senseModeTrueSight =
                attackerCharacter.SenseModes.FirstOrDefault(x => x.SenseType == SenseMode.Type.Truesight);

            if (senseModeTrueSight == null || !attacker.IsWithinRange(defender, senseModeTrueSight.SenseRange))
            {
                return;
            }

            attackAdvantageTrends.RemoveAll(BlindedDisadvantage);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsMagicEffectValidIfHeavilyObscuredOrInNaturalDarkness(
        GameLocationCharacter source,
        IMagicEffect magicEffect,
        GameLocationCharacter target)
    {
        return target == null ||
               !Main.Settings.UseOfficialLightingObscurementAndVisionRules ||
               magicEffect.EffectDescription is not
               {
                   RangeType: RuleDefinitions.RangeType.Distance,
                   TargetType: RuleDefinitions.TargetType.Individuals or RuleDefinitions.TargetType.IndividualsUnique
               } ||
               Main.Settings.EffectsThatTargetDistantIndividualsAndDontRequireSight.Contains(magicEffect.Name) ||
               source.CanPerceiveTarget(target);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsBlindFromDarkness(RulesetActor actor)
    {
        return actor != null && actor.HasConditionOfType(ConditionBlindedByDarkness);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsBlindNotFromDarkness(RulesetActor actor)
    {
        return
            actor != null &&
            actor.AllConditions
                .Select(y => y.ConditionDefinition)
                .Any(x => (x == ConditionBlinded || x.parentCondition == ConditionBlinded) &&
                          x != ConditionBlindedByDarkness);
    }

    // improved cell perception routine that takes sight into consideration
    // most of the usages is to determine if a character can perceive a cell in teleport scenarios
    // when target not null it helps determine visibility on attacks and effects targeting scenarios
    internal static bool MyIsCellPerceivedByCharacter(
        this GameLocationVisibilityManager instance,
        int3 cellPosition,
        GameLocationCharacter sensor,
        GameLocationCharacter target = null,
        LightingState additionalBlockedLightingState = LightingState.Darkness)
    {
        // let vanilla do the heavy lift on perception
        var result = instance.IsCellPerceivedByCharacter(cellPosition, sensor);
        var targetLightingState = ComputeLightingStateOnTargetPosition(sensor, cellPosition);

        // if setting is off or vanilla cannot perceive
        if (!result ||
            !Main.Settings.UseOfficialLightingObscurementAndVisionRules)
        {
            if (!result)
            {
                return false;
            }

            // Silhouette Step is the only one using additionalBlockedLightingState as it requires to block BRIGHT
            return additionalBlockedLightingState == LightingState.Darkness ||
                   targetLightingState != additionalBlockedLightingState;
        }

        var distance = DistanceCalculation.GetDistanceFromTwoPositions(sensor.LocationPosition, cellPosition);
        var sensorCharacter = sensor.RulesetCharacter;
        var sourceIsBlindFromDarkness = IsBlindFromDarkness(sensorCharacter);
        var sourceIsBlindNotFromDarkness = IsBlindNotFromDarkness(sensorCharacter);
        var targetIsNotTouchingGround = target != null && !target.RulesetActor.IsTouchingGround();

        // try to find any sense mode that is valid for the current lighting state and is within range
        foreach (var senseMode in sensor.RulesetCharacter.SenseModes)
        {
            var senseType = senseMode.SenseType;

            if (sourceIsBlindNotFromDarkness && senseType is
                    SenseMode.Type.NormalVision or
                    SenseMode.Type.Darkvision or
                    SenseMode.Type.SuperiorDarkvision or
                    SenseMode.Type.Truesight)
            {
                continue;
            }

            if (sourceIsBlindFromDarkness && senseType is
                    SenseMode.Type.NormalVision or
                    SenseMode.Type.Darkvision or
                    SenseMode.Type.SuperiorDarkvision)
            {
                continue;
            }

            if (targetIsNotTouchingGround && senseType is SenseMode.Type.Tremorsense)
            {
                continue;
            }

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (targetLightingState is LightingState.Unlit)
            {
                if (senseType == SenseMode.Type.NormalVision)
                {
                    continue;
                }
            }

            else if (targetLightingState is (LightingState)MyLightingState.HeavilyObscured)
            {
                if (senseType is
                    SenseMode.Type.NormalVision or
                    SenseMode.Type.Darkvision or
                    SenseMode.Type.SuperiorDarkvision or
                    SenseMode.Type.Truesight)
                {
                    continue;
                }
            }

            else if (targetLightingState is LightingState.Darkness)
            {
                if (senseType is
                    SenseMode.Type.NormalVision or
                    SenseMode.Type.Darkvision or
                    SenseMode.Type.SuperiorDarkvision)
                {
                    continue;
                }
            }

            if (distance <= senseMode.SenseRange)
            {
                // Silhouette Step is the only one using additionalBlockedLightingState as it requires to block BRIGHT
                return additionalBlockedLightingState == LightingState.Darkness ||
                       targetLightingState != additionalBlockedLightingState;
            }
        }

        return false;
    }

    private static LightingState ComputeLightingStateOnTargetPosition(
        GameLocationCharacter instance,
        int3 targetPosition)
    {
        if (PositionLightingStateCache.TryGetValue(targetPosition, out var lightingState))
        {
            return lightingState;
        }

        var savePosition = new int3(
            instance.LocationPosition.x,
            instance.LocationPosition.y,
            instance.LocationPosition.z);

        instance.LocationPosition = targetPosition;

        var illumination = ComputeIllumination(instance, targetPosition);

        instance.LocationPosition = savePosition;

        PositionLightingStateCache.Add(targetPosition, illumination);

        return illumination;

        static LightingState ComputeIllumination(IIlluminable illuminable, int3 targetPosition)
        {
            const LightingState UNLIT = LightingState.Unlit;

            var visibilityManager =
                ServiceRepository.GetService<IGameLocationVisibilityService>() as GameLocationVisibilityManager;

            visibilityManager!.positionCache.Clear();
            illuminable.GetAllPositionsToCheck(visibilityManager.positionCache);

            if (visibilityManager.positionCache == null || visibilityManager.positionCache.Empty())
            {
                return UNLIT;
            }

            //
            // try to determine if heavily obscured or darkness
            //

            // alternate heavily obscured determination that differentiate between heavily obscured and darkness
            var gridAccessor = new GridAccessor(visibilityManager.positionCache[0]);
            var locationCharacters = new List<GameLocationCharacter>();

            if (gridAccessor.Occupants_TryGet(targetPosition, out var value))
            {
                locationCharacters.SetRange(value);
            }

            var isDarkness = false;

            foreach (var locationCharacter in locationCharacters)
            {
                if (locationCharacter.RulesetActor is not RulesetCharacterEffectProxy rulesetProxy)
                {
                    continue;
                }

                if (rulesetProxy.EffectProxyDefinition == ProxyDarkness)
                {
                    isDarkness = true;
                }
                else
                {
                    if (SightImpairedProxies.All(effectProxy => rulesetProxy.EffectProxyDefinition != effectProxy))
                    {
                        continue;
                    }

                    return (LightingState)MyLightingState.HeavilyObscured;
                }
            }

            if (isDarkness)
            {
                return LightingState.Darkness;
            }

            //
            // try to determine if outside and if in daylight exit earlier
            //

            var globalLightingState = LightingState.Unlit;

            foreach (var position in visibilityManager.positionCache)
            {
                gridAccessor.FetchSector(position);
                if (gridAccessor.sector == null ||
                    gridAccessor.sector.GlobalLightingState == LightingState.Unlit)
                {
                    continue;
                }

                globalLightingState = gridAccessor.sector.GlobalLightingState;

                if (globalLightingState != LightingState.Bright)
                {
                    continue;
                }

                return LightingState.Bright;
            }

            //
            // try to fetch all light sources to correctly determine if bright, dim or unlit
            //

            visibilityManager.lightsByDistance.Clear();

            foreach (var lightSources in visibilityManager.lightSourcesMap)
            {
                var key = lightSources.Value;
                var locationPosition = key.LocationPosition;

                if (key.RulesetLightSource.DayCycleType != RuleDefinitions.LightSourceDayCycleType.Always &&
                    !key.RulesetLightSource.IsDayCycleActive)
                {
                    continue;
                }

                var dimRange = key.RulesetLightSource.DimRange;
                var magnitude = (locationPosition - targetPosition).magnitude;

                if (magnitude <= dimRange + illuminable.DetectionRange &&
                    (!visibilityManager.charactersByLight.TryGetValue(key.RulesetLightSource, out var glc) ||
                     glc is { IsValidForVisibility: true }))
                {
                    visibilityManager.lightsByDistance.Add(
                        new KeyValuePair<GameLocationLightSource, float>(key, magnitude));
                }
            }

            visibilityManager.lightsByDistance.Sort(visibilityManager.lightSortMethod);

            var lightsLightingState = LightingState.Unlit;

            foreach (var int3 in visibilityManager.positionCache)
            {
                foreach (var keyValuePair in visibilityManager.lightsByDistance)
                {
                    var key = keyValuePair.Key;
                    var dimRange = key.RulesetLightSource.DimRange;
                    var locationPosition = key.LocationPosition;
                    var magnitudeSqr = (locationPosition - int3).magnitudeSqr;

                    if (!magnitudeSqr.IsInferiorOrNearlyEqual(dimRange * dimRange))
                    {
                        continue;
                    }

                    var hasLineOfSight = true;
                    var sourcePosition =
                        visibilityManager.gameLocationPositioningService.GetWorldPositionFromGridPosition(
                            key.LocationPosition);
                    var destinationPosition =
                        visibilityManager.gameLocationPositioningService.GetWorldPositionFromGridPosition(int3);

                    visibilityManager.AdaptRayForVerticalityAndDiagonals(
                        key.LocationPosition, int3, ref sourcePosition, true);

                    if (key.RulesetLightSource.IsSpot)
                    {
                        var to = destinationPosition - sourcePosition;

                        to.Normalize();
                        hasLineOfSight = Vector3.Angle(key.RulesetLightSource.SpotDirection, to)
                            .IsInferiorOrNearlyEqual(key.RulesetLightSource.SpotAngle * 0.5f);
                    }

                    if (!hasLineOfSight ||
                        visibilityManager.gameLocationPositioningService.RaycastGridSightBlocker(
                            sourcePosition, destinationPosition, visibilityManager.GameLocationService) ||
                        visibilityManager.gameLocationPositioningService.IsSightImpaired(key.LocationPosition, int3))
                    {
                        continue;
                    }

                    lightsLightingState =
                        key.RulesetLightSource.BrightRange <= 0.0 || magnitudeSqr >
                        key.RulesetLightSource.BrightRange * key.RulesetLightSource.BrightRange
                            ? LightingState.Dim
                            : LightingState.Bright;

                    if (lightsLightingState == LightingState.Bright)
                    {
                        break;
                    }
                }

                if (lightsLightingState != LightingState.Unlit)
                {
                    break;
                }
            }

            return globalLightingState != LightingState.Dim || lightsLightingState != LightingState.Unlit
                ? lightsLightingState
                : LightingState.Dim;
        }
    }

    private enum MyLightingState
    {
        HeavilyObscured = 9000
    }
}
