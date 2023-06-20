using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMoveModes;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using System;
using SolastaUnfinishedBusiness.CustomInterfaces;
using System.Collections;
using SolastaUnfinishedBusiness.CustomBehaviors;
using static FeatureDefinitionAttributeModifier;

namespace SolastaUnfinishedBusiness.Races;

internal static class WyrmkinRaceBuilder
{
    internal static CharacterRaceDefinition RaceWyrmkin { get; } = BuildWyrmkin();

    [NotNull]
    private static CharacterRaceDefinition BuildWyrmkin()
    {
        var proficiencyWyrmkinLanguages = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyWyrmkinLanguages")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Language, "Language_Common", "Language_Draconic")
            .AddToDB();

        var raceWyrmkin = CharacterRaceDefinitionBuilder
            .Create(Dragonborn, "RaceWyrmkin")
            .SetOrUpdateGuiPresentation(Category.Race)
            .SetSizeDefinition(CharacterSizeDefinitions.Medium)
            .SetBaseWeight(35)
            .SetBaseHeight(3)
            .SetMinimalAge(6)
            .SetMaximalAge(200)
            .SetFeaturesAtLevel(1,
                MoveModeMove6,
                SenseNormalVision,
                SenseDarkvision,
                proficiencyWyrmkinLanguages)
            .AddToDB();

        raceWyrmkin.subRaces =
            new List<CharacterRaceDefinition> { 
                BuildHighWyrmkin(raceWyrmkin)
                // ,
                // BuildDarkWyrmkin(raceWyrmkin)
                };
        RacesContext.RaceScaleMap[raceWyrmkin] = 7.0f / 6.4f;
        return raceWyrmkin;
    }

    private static CharacterRaceDefinition BuildDarkWyrmkin(CharacterRaceDefinition raceWyrmkin)
    {
        throw new NotImplementedException();
    }

    private static CharacterRaceDefinition BuildHighWyrmkin(CharacterRaceDefinition characterRaceDefinition)
    {
        var koboldSpriteReference = Sprites.GetSprite("Kobold", Resources.Kobold, 1024, 512);

        var attributeModifierHighWyrmkinIntelligenceAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifierHighWyrmkinIntelligenceAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Intelligence, 2)
            .AddToDB();

        var attributeModifierHighWyrmkinStrengthAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifierHighWyrmkinStrengthAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Strength, 1)
            .AddToDB();

        var powerHighWyrmkinReactiveRetribution = FeatureDefinitionPowerBuilder
            .Create("PowerHighWyrmkinReactiveRetribution")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.ShortRest)
            .SetReactionContext(ExtraReactionContext.Custom)
            .AddToDB();

        powerHighWyrmkinReactiveRetribution.SetCustomSubFeatures(
            new ReactToAttackOnMeReactiveRetribution(powerHighWyrmkinReactiveRetribution));

        var effectPsionicWave = EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cone, 3)
                .ExcludeCaster()
                .SetSavingThrowData(
                    false,
                    AttributeDefinitions.Intelligence,
                    true,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    AttributeDefinitions.Intelligence)
                .SetParticleEffectParameters(ColorSpray.EffectDescription.effectParticleParameters)
                .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypePsychic, 2, DieType.D6)
                    .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                    .SetDiceAdvancement(LevelSourceType.CharacterLevel, 1, 1, 5, 6)
                    .Build())
                .Build();
        var powerHighWyrmkinPsionicWave = FeatureDefinitionPowerBuilder
            .Create("PowerHighWyrmkinPsionicWave")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerDraconicCry", Resources.PowerDraconicCry, 256, 128))
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ShortRest)
            .SetEffectDescription(effectPsionicWave)
            .AddToDB();

        var raceHighWyrmkin = CharacterRaceDefinitionBuilder
            .Create(characterRaceDefinition, "RaceHighWyrmkin")
            .SetOrUpdateGuiPresentation(Category.Race)
            .SetFeaturesAtLevel(1,
            attributeModifierHighWyrmkinStrengthAbilityScoreIncrease,
            attributeModifierHighWyrmkinIntelligenceAbilityScoreIncrease,
            powerHighWyrmkinPsionicWave,
            powerHighWyrmkinReactiveRetribution
            ).AddToDB();

        return raceHighWyrmkin;
    }

    private class ReactToAttackOnMeReactiveRetribution : IReactToAttackOnMeFinished
    {
        private FeatureDefinitionPower pool;

        public ReactToAttackOnMeReactiveRetribution(FeatureDefinitionPower powerHighWyrmkinSwiftRetribution)
        {
            pool = powerHighWyrmkinSwiftRetribution;
        }

        public IEnumerator OnReactToAttackOnMeFinished(GameLocationCharacter attacker, 
            GameLocationCharacter me,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode mode, 
            ActionModifier modifier)
        {
            //do not trigger on my own turn, so won't retaliate on AoO
            if (Gui.Battle?.ActiveContenderIgnoringLegendary == me)
            {
                yield break;
            }
            // only trigger on a hit
            if (outcome != RollOutcome.Success && outcome != RollOutcome.CriticalSuccess)
            {
                yield break;
            }
            var rulesetEnemy = attacker.RulesetCharacter;

            if (!me.CanReact() ||
            rulesetEnemy == null ||
                rulesetEnemy.IsDeadOrDying)
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

            var (retaliationMode, retaliationModifier) = me.GetFirstMeleeModeThatCanAttack(attacker);

            if (retaliationMode == null)
            {
                (retaliationMode, retaliationModifier) = me.GetFirstRangedModeThatCanAttack(attacker);
            }

            if (retaliationMode == null)
            {
                yield break;
            }

            retaliationMode.AddAttackTagAsNeeded(AttacksOfOpportunity.NotAoOTag);

            var reactionParams = new CharacterActionParams(me, ActionDefinitions.Id.AttackOpportunity);

            reactionParams.TargetCharacters.Add(attacker);
            reactionParams.ActionModifiers.Add(retaliationModifier);
            reactionParams.AttackMode = retaliationMode;

            var rulesetCharacter = me.RulesetCharacter;

            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var tag = "ReactiveRetribution";
            var reactionRequest = new ReactionRequestReactionAttack(tag, reactionParams);

            manager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(attacker, manager, previousReactionCount);

            //Can we detect this before attack starts? Currently we get to this part after attack finishes, if reaction was validated
            if (reactionParams.ReactionValidated)
            {
                rulesetCharacter.UsePower(UsablePowersProvider.Get(pool, rulesetCharacter));
            }

        }
    }
}
