using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.Models;
using TA;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalActions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Merciless : AbstractFightingStyle
{
    private static FeatureDefinitionPower _powerMerciless;
    private CustomFightingStyleDefinition instance;

    [NotNull]
    internal override List<FeatureDefinitionFightingStyleChoice> GetChoiceLists()
    {
        return new List<FeatureDefinitionFightingStyleChoice>
        {
            FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin, FightingStyleRanger
        };
    }

    internal override FightingStyleDefinition GetStyle()
    {
        if (instance != null)
        {
            return instance;
        }

        _powerMerciless = FeatureDefinitionPowerBuilder
            .Create("PowerFightingStyleMerciless", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("Fear", Category.Spell)
            .Configure(
                1,
                RuleDefinitions.UsesDetermination.ProficiencyBonus,
                AttributeDefinitions.Strength,
                RuleDefinitions.ActivationTime.NoCost,
                0,
                RuleDefinitions.RechargeRate.AtWill,
                false,
                false,
                AttributeDefinitions.Strength,
                DatabaseHelper.SpellDefinitions.Fear.EffectDescription.Copy())
            .AddToDB();

        _powerMerciless.effectDescription.targetParameter = 1;
        _powerMerciless.effectDescription.TargetType = RuleDefinitions.TargetType.IndividualsUnique;
        _powerMerciless.effectDescription.durationType = RuleDefinitions.DurationType.Round;
        _powerMerciless.effectDescription.effectForms[0].canSaveToCancel = false;

        var additionalActionMerciless = FeatureDefinitionAdditionalActionBuilder
            .Create(AdditionalActionHunterHordeBreaker, "AdditionalActionFightingStyleMerciless")
            .SetGuiPresentationNoContent()
            .AddToDB();

        var onCharacterKillMerciless = FeatureDefinitionOnCharacterKillBuilder
            .Create("OnCharacterKillFightingStyleMerciless", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentationNoContent()
            .SetOnCharacterKill(OnMercilessKill)
            .AddToDB();

        instance = CustomizableFightingStyleBuilder
            .Create("Merciless", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.FightingStyle,
                DatabaseHelper.CharacterSubclassDefinitions.MartialChampion.GuiPresentation.SpriteReference)
            .SetFeatures(additionalActionMerciless, onCharacterKillMerciless)
            .AddToDB();

        return instance;
    }

    private static void OnMercilessKill(GameLocationCharacter character)
    {
        if (Global.CurrentAction is not CharacterActionAttack actionAttack)
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
        var strength = attacker.GetAttribute(AttributeDefinitions.Strength).CurrentValue;
        var distance = Global.CriticalHit ? proficiencyBonus : (proficiencyBonus + 1) / 2;
        var usablePower = new RulesetUsablePower(_powerMerciless, attacker.RaceDefinition, attacker.ClassesHistory[0]);
        var effectPower = new RulesetEffectPower(attacker, usablePower);

        usablePower.SaveDC = 8 + proficiencyBonus + AttributeDefinitions.ComputeAbilityScoreModifier(strength);

        foreach (var enemy in battle.EnemyContenders
                     .Where(enemy =>
                         enemy != character
                         && int3.Distance(character.LocationPosition, enemy.LocationPosition) <= distance))
        {
            effectPower.ApplyEffectOnCharacter(enemy.RulesetCharacter, true, enemy.LocationPosition);
        }
    }
}
