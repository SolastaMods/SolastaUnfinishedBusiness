using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Feats;

internal static class TwoWeaponCombatFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var featDualFlurry = BuildDualFlurry();
        var featDualWeaponDefense = BuildDualWeaponDefense();

        feats.AddRange(featDualFlurry, featDualWeaponDefense);

        GroupFeats.FeatGroupDefenseCombat.AddFeats(
            featDualWeaponDefense);

        GroupFeats.FeatGroupTwoWeaponCombat.AddFeats(
            featDualFlurry,
            featDualWeaponDefense);
    }

    private static FeatDefinition BuildDualWeaponDefense()
    {
        const string NAME = "FeatDualWeaponDefense";

        return FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionAttackModifiers.AttackModifierFeatAmbidextrous,
                FeatureDefinitionAttributeModifierBuilder
                    .Create("AttributeModifierFeatDualWeaponDefense")
                    .SetGuiPresentation(NAME, Category.Feat)
                    .SetModifier(AttributeModifierOperation.Additive,
                        AttributeDefinitions.ArmorClass, 1)
                    .SetSituationalContext(SituationalContext.DualWieldingMeleeWeapons)
                    .AddToDB())
            .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
            .AddToDB();
    }

    private static FeatDefinition BuildDualFlurry()
    {
        const string NAME = "FeatDualFlurry";

        // kept for backward compatibility
        _ = FeatureDefinitionBuilder
            .Create($"OnAttackDamageEffect{NAME}")
            .SetGuiPresentationNoContent(true)
            .AddToDB();


        var conditionMark = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Mark")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionGuided)
            .SetPossessive()
            .AddToDB();

        return FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .AddCustomSubFeatures(new PhysicalAttackFinishedByMeDualFlurry(conditionMark))
            .AddToDB();
    }

    private sealed class PhysicalAttackFinishedByMeDualFlurry(ConditionDefinition conditionMark)
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
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!ValidatorsCharacter.HasMeleeWeaponInMainAndOffhand(rulesetAttacker))
            {
                yield break;
            }

            var hasMark = rulesetAttacker.TryGetConditionOfCategoryAndType(
                AttributeDefinitions.TagEffect, conditionMark.Name, out var activeCondition);

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (action.ActionType)
            {
                case ActionDefinitions.ActionType.Main when
                    rollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess &&
                    !hasMark:

                    rulesetAttacker.InflictCondition(
                        conditionMark.Name,
                        DurationType.Round,
                        0,
                        TurnOccurenceType.EndOfTurn,
                        AttributeDefinitions.TagEffect,
                        rulesetAttacker.guid,
                        rulesetAttacker.CurrentFaction.Name,
                        1,
                        conditionMark.Name,
                        0,
                        0,
                        0);
                    break;

                case ActionDefinitions.ActionType.Bonus when
                    hasMark &&
                    defender.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                    attackMode.IsOneHandedWeapon():

                    var attackModeCopy = RulesetAttackMode.AttackModesPool.Get();

                    attackModeCopy.Copy(attackMode);
                    attackModeCopy.ActionType = ActionDefinitions.ActionType.NoCost;

                    var actionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.AttackFree)
                    {
                        AttackMode = attackModeCopy,
                        TargetCharacters = { defender },
                        ActionModifiers = { new ActionModifier() }
                    };

                    rulesetAttacker.RemoveCondition(activeCondition);

                    ServiceRepository.GetService<IGameLocationActionService>()?
                        .ExecuteAction(actionParams, null, true);
                    break;
            }
        }
    }
}
