using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Models;
using TA;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionAdditionalActions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaCommunityExpansion.FightingStyles;

internal sealed class Merciless : AbstractFightingStyle
{
    private readonly Guid guidNamespace = new("3f7f25de-0ff9-4b63-b38d-8cd7f3a401fc");
    private FightingStyleDefinitionCustomizable instance;
    private static FeatureDefinitionPower _powerMerciless;

    [NotNull]
    internal override List<FeatureDefinitionFightingStyleChoice> GetChoiceLists()
    {
        return new List<FeatureDefinitionFightingStyleChoice>
        {
            FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin
        };
    }

    internal override FightingStyleDefinition GetStyle()
    {
        if (instance != null)
        {
            return instance;
        }

        _powerMerciless = FeatureDefinitionPowerBuilder
            .Create("PowerMerciless", guidNamespace)
            .Configure(
                1,
                RuleDefinitions.UsesDetermination.Fixed,
                AttributeDefinitions.Strength,
                RuleDefinitions.ActivationTime.NoCost,
                0,
                RuleDefinitions.RechargeRate.AtWill,
                false,
                false,
                AttributeDefinitions.Strength,
                DatabaseHelper.SpellDefinitions.Fear.EffectDescription.Copy())
            .SetUsesProficiency()
            .AddToDB();

       // _powerMerciless.abilityScoreDetermination = RuleDefinitions.AbilityScoreDetermination.Explicit;
        _powerMerciless.effectDescription.TargetType = RuleDefinitions.TargetType.IndividualsUnique;
        _powerMerciless.effectDescription.durationType = RuleDefinitions.DurationType.Round;
        _powerMerciless.effectDescription.difficultyClassComputation =
            RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency;
        _powerMerciless.effectDescription.fixedSavingThrowDifficultyClass = 15;
        _powerMerciless.effectDescription.savingThrowDifficultyAbility = AttributeDefinitions.Strength;

        var additionalActionMerciless = FeatureDefinitionAdditionalActionBuilder
            .Create(AdditionalActionHunterHordeBreaker, "AdditionalActionMerciless", guidNamespace)
            .SetGuiPresentationNoContent()
            .AddToDB();

        var onCharacterKillMerciless = FeatureDefinitionOnCharacterKillBuilder
            .Create("FeatureMerciless", guidNamespace)
            .SetGuiPresentationNoContent()
            .SetOnCharacterKill(OnMercilessKill)
            .AddToDB();

        instance = CustomizableFightingStyleBuilder
            .Create("Merciless", "f570d166-c65c-4a68-ab78-aeb16d491fce")
            .SetGuiPresentation(Category.FightingStyle,
                DatabaseHelper.CharacterSubclassDefinitions.MartialChampion.GuiPresentation.SpriteReference)
            .SetFeatures(additionalActionMerciless, onCharacterKillMerciless)
            .AddToDB();

        return instance;
    }

    private static void OnMercilessKill(
        GameLocationCharacter character,
        bool dropLoot,
        bool removeBody,
        bool forceRemove,
        bool considerDead,
        bool becomesDying)
    {
        if (!considerDead || Global.CurrentAction is not CharacterActionAttack actionAttack)
        {
            return;
        }

        var battle = ServiceRepository.GetService<IGameLocationBattleService>()?.Battle;

        if (battle == null)
        {
            return;
        }

        var attacker = actionAttack.ActingCharacter.RulesetCharacter as RulesetCharacterHero
                       ?? actionAttack.ActingCharacter.RulesetCharacter.OriginalFormCharacter as
                           RulesetCharacterHero;

        if (attacker == null || attacker.IsWieldingRangedWeapon())
        {
            return;
        }

        var proficiencyBonus = attacker.GetAttribute(AttributeDefinitions.ProficiencyBonus).CurrentValue;
        var distance = Global.CriticalHit ? proficiencyBonus : (proficiencyBonus + 1) / 2;
        var usablePower = new RulesetUsablePower(_powerMerciless, attacker.RaceDefinition, attacker.ClassesHistory[0]);
        var effectPower = new RulesetEffectPower(attacker, usablePower);

        foreach (var enemy in battle.EnemyContenders
                     .Where(enemy =>
                         enemy != character
                         && enemy.PerceivedAllies.Contains(character)
                         && int3.Distance(character.LocationPosition, enemy.LocationPosition) <= distance))
        {
            effectPower.ApplyEffectOnCharacter(enemy.RulesetCharacter, true, enemy.LocationPosition);
        }
    }
}
