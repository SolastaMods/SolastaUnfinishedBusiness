using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Validators;
using static ActionDefinitions;
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

        return FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .AddCustomSubFeatures(new PhysicalAttackFinishedByMeDualFlurry())
            .AddToDB();
    }

    private sealed class PhysicalAttackFinishedByMeDualFlurry : IPhysicalAttackFinishedByMe
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

            if (action.ActionType != ActionType.Bonus ||
                !attackMode.IsOneHandedWeapon() ||
                !ValidatorsCharacter.HasMeleeWeaponInMainAndOffhand(rulesetAttacker) ||
                defender.RulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var attackModeCopy = RulesetAttackMode.AttackModesPool.Get();

            attackModeCopy.Copy(attackMode);
            attackModeCopy.ActionType = ActionType.NoCost;

            attacker.MyExecuteActionAttack(
                Id.AttackFree,
                defender,
                attackModeCopy,
                new ActionModifier());
        }
    }
}
