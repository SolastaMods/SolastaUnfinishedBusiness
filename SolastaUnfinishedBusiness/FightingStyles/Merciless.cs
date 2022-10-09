using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.Models;
using TA;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Merciless : AbstractFightingStyle
{
    internal Merciless()
    {
        var powerFightingStyleMerciless = FeatureDefinitionPowerBuilder
            .Create("PowerFightingStyleMerciless")
            .SetGuiPresentation("Fear", Category.Spell)
            .Configure(
                UsesDetermination.ProficiencyBonus,
                ActivationTime.NoCost,
                RechargeRate.AtWill,
                DatabaseHelper.SpellDefinitions.Fear.EffectDescription)
            .AddToDB();

        powerFightingStyleMerciless.effectDescription.targetParameter = 1;
        powerFightingStyleMerciless.effectDescription.TargetType = TargetType.IndividualsUnique;
        powerFightingStyleMerciless.effectDescription.durationType = DurationType.Round;
        powerFightingStyleMerciless.effectDescription.effectForms[0].canSaveToCancel = false;

        void OnMercilessKill(GameLocationCharacter character)
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
            var usablePower = new RulesetUsablePower(
                powerFightingStyleMerciless, attacker.RaceDefinition, attacker.ClassesHistory[0]);
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

        FightingStyle = CustomizableFightingStyleBuilder
            .Create("Merciless")
            .SetGuiPresentation(Category.FightingStyle,
                DatabaseHelper.CharacterSubclassDefinitions.MartialChampion.GuiPresentation.SpriteReference)
            .SetFeatures(
                // FeatureDefinitionAdditionalActionBuilder
                //     .Create(AdditionalActionHunterHordeBreaker, "AdditionalActionFightingStyleMerciless")
                //     .SetGuiPresentationNoContent()
                //     .AddToDB(),
                FeatureDefinitionOnCharacterKillBuilder
                    .Create("OnCharacterKillFightingStyleMerciless")
                    .SetGuiPresentationNoContent()
                    .SetOnCharacterKill(OnMercilessKill)
                    .AddToDB())
            .AddToDB();
    }

    internal override FightingStyleDefinition FightingStyle { get; }

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin, FightingStyleRanger
    };
}
