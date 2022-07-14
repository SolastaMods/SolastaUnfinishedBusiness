using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.Models;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionAdditionalActions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaCommunityExpansion.FightingStyles;

internal sealed class Merciless : AbstractFightingStyle
{
    private readonly Guid guidNamespace = new("3f7f25de-0ff9-4b63-b38d-8cd7f3a401fc");
    private FightingStyleDefinitionCustomizable instance;

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

        var additionalActionMerciless = FeatureDefinitionAdditionalActionBuilder
            .Create(AdditionalActionSurgedMain, "AdditionalActionMerciless", guidNamespace)
            .SetGuiPresentationNoContent(AdditionalActionSurgedMain.GuiPresentation.SpriteReference)
            .SetActionType(ActionDefinitions.ActionType.Main)
            .SetRestrictedActions(ActionDefinitions.Id.AttackMain)
            .AddToDB();

        var conditionMerciless = ConditionDefinitionBuilder
            .Create("ConditionMerciless", guidNamespace)
            .Configure(RuleDefinitions.DurationType.Round, 0, false, additionalActionMerciless)
            .SetConditionType(RuleDefinitions.ConditionType.Beneficial)
            .SetGuiPresentation(Category.Condition)
            .SetPossessive(true)
            .SetTurnOccurence(RuleDefinitions.TurnOccurenceType.StartOfTurn)
            .SetAllowMultipleInstances(false)
            .SetFeatures(additionalActionMerciless)
            .AddToDB();

        void OnCharacterKill(
            GameLocationCharacter character,
            bool dropLoot,
            bool removeBody,
            bool forceRemove,
            bool considerDead,
            bool becomesDying)
        {
            var activePlayerCharacter = Global.ActivePlayerCharacter;
            var activeMercilessCondition = RulesetCondition.CreateActiveCondition(
                activePlayerCharacter.RulesetCharacter.Guid,
                conditionMerciless, RuleDefinitions.DurationType.Round, 0,
                RuleDefinitions.TurnOccurenceType.StartOfTurn,
                activePlayerCharacter.RulesetCharacter.Guid,
                activePlayerCharacter.RulesetCharacter.CurrentFaction.Name);

            activePlayerCharacter.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat,
                activeMercilessCondition);

            var battle = ServiceRepository.GetService<IGameLocationBattleService>()?.Battle;

            if (battle == null)
            {
                return;
            }

            foreach (var enemy in battle.EnemyContenders)
            {
                if (!enemy.PerceivedFoes.Contains(activePlayerCharacter))
                {
                    continue;
                }

                var activeFrightenedCondition = RulesetCondition.CreateActiveCondition(
                    enemy.Guid,
                    DatabaseHelper.ConditionDefinitions.ConditionFrightened,
                    RuleDefinitions.DurationType.Round,
                    0,
                    RuleDefinitions.TurnOccurenceType.EndOfTurnNoPerceptionOfSource,
                    activePlayerCharacter.Guid,
                    activePlayerCharacter.RulesetCharacter.CurrentFaction.Name);

                enemy.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat,
                    activeFrightenedCondition);
            }
        }

        var featureMerciless = FeatureDefinitionOnCharacterKillBuilder
            .Create("FeatureMerciless", guidNamespace)
            .SetGuiPresentationNoContent()
            .SetOnCharacterKill(OnCharacterKill)
            .AddToDB();

        instance = CustomizableFightingStyleBuilder
            .Create("Merciless", "f570d166-c65c-4a68-ab78-aeb16d491fce")
            .SetGuiPresentation(Category.FightingStyle,
                DatabaseHelper.CharacterSubclassDefinitions.MartialChampion.GuiPresentation.SpriteReference)
            .SetFeatures(featureMerciless)
            .AddToDB();

        return instance;
    }
}
