using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
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
    internal const string DualFlurryAttackMark = "DualFlurryAttackMark";
    internal const string DualFlurryTriggerMark = "DualFlurryTriggerMark";

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

        var feature = FeatureDefinitionBuilder
            .Create($"Feature{NAME}")
            .SetGuiPresentation(NAME, Category.Feat)
            .AddToDB();

        feature.AddCustomSubFeatures(new PhysicalAttackFinishedByMeDualFlurry(feature));

        return FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(feature)
            .AddToDB();
    }

    private sealed class PhysicalAttackFinishedByMeDualFlurry(FeatureDefinition feature) : IPhysicalAttackFinishedByMe
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
            var attackModeWeapon = attackMode.SourceDefinition as ItemDefinition;
            var offhandWeapon = rulesetAttacker.GetOffhandWeapon()?.ItemDefinition;

            if (attackModeWeapon != offhandWeapon)
            {
                yield break;
            }

            if ((action.ActionType != ActionType.Bonus &&
                 !attackMode.AttackTags.Contains(DualFlurryTriggerMark)) ||
                !attackMode.IsOneHandedWeapon() ||
                !ValidatorsCharacter.HasMeleeWeaponInMainAndOffhand(rulesetAttacker) ||
                defender.RulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var actionModifier = new ActionModifier
            {
                proximity = attacker.IsWithinRange(defender, attackMode.reachRange)
                    ? AttackProximity.Melee
                    : AttackProximity.Range
            };

            var attackModeCopy = RulesetAttackMode.AttackModesPool.Get();

            attackModeCopy.Copy(attackMode);
            attackModeCopy.AddAttackTagAsNeeded(DualFlurryAttackMark);
            rulesetAttacker.LogCharacterUsedFeature(feature);
            attacker.MyExecuteActionAttack(
                Id.AttackFree,
                defender,
                attackModeCopy,
                actionModifier);
        }
    }
}
