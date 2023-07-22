using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using static SolastaUnfinishedBusiness.Api.GameExtensions.ExtraCombatAffinityValueDetermination;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class FeatureDefinitionCombatAffinityPatcher
{
    [HarmonyPatch(typeof(FeatureDefinitionCombatAffinity),
        nameof(FeatureDefinitionCombatAffinity.ComputeAttackModifier))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ComputeAttackModifier_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            FeatureDefinitionCombatAffinity __instance,
            RulesetCharacter myself,
            RulesetCharacter defender,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier,
            RuleDefinitions.FeatureOrigin featureOrigin,
            int bardicDieRoll,
            float distance)
        {
            //PATCH: supports custom attack roll determination
            var determination = (ExtraCombatAffinityValueDetermination)__instance.myAttackModifierValueDetermination;

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (determination)
            {
                case ConditionAmount:
                case ConditionAmountIfFavoriteEnemy when myself.IsMyFavoriteEnemy(defender):
                case ConditionAmountIfNotFavoriteEnemy when !myself.IsMyFavoriteEnemy(defender):
                    var amount = GetConditionAmount(__instance, myself);
                    if (__instance.myAttackModifierSign == RuleDefinitions.AttackModifierSign.Substract)
                    {
                        amount = -amount;
                    }

                    attackModifier.AttackRollModifier += amount;
                    attackModifier.AttacktoHitTrends.Add(new RuleDefinitions.TrendInfo(amount, featureOrigin.sourceType,
                        featureOrigin.sourceName, null));
                    break;
            }
        }

        private static int GetConditionAmount(FeatureDefinition feature, RulesetActor myself)
        {
            var condition = myself.FindFirstConditionHoldingFeature(feature);
            return condition?.Amount ?? 0;
        }
    }
}
