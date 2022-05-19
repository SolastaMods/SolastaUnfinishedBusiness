using System.Collections;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Features;
using SolastaModApi.Extensions;
using static GameLocationCharacterEventSystem;
using static RuleDefinitions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomReactions;

internal static class CharacterActionDeflectMissilePatcher
{
    [HarmonyPatch(typeof(CharacterActionDeflectMissile), "ExecuteImpl")]
    internal static class ReadyActionSelectionPanel_Bind
    {
        internal static bool Prefix(CharacterActionDeflectMissile __instance)
        {
            return false;
        }

        internal static IEnumerator Postfix(IEnumerator values, CharacterActionDeflectMissile __instance)
        {
            var actionParams = __instance.ActionParams;
            var attacker = actionParams.TargetCharacters[0];
            var actingCharacter = __instance.ActingCharacter;
            var rulesCharacter = actingCharacter.RulesetCharacter;
            var actionDefinition = __instance.ActionDefinition;

            actingCharacter.TurnTowards(attacker, false);

            yield return actingCharacter.EventSystem.UpdateMotionsAndWaitForEvent(Event.RotationEnd);
            yield return actingCharacter.WaitForHitAnimation();

            actingCharacter.DeflectAttack(actionParams.TargetCharacters[0]);

            var reductionAmount = 0;

            var customDeflector = rulesCharacter.GetSubFeaturesByType<ICustomMissileDeflection>().FirstOrDefault();

            if (customDeflector == null)
            {
                //Default behavior
                reductionAmount += RollDie(actionDefinition.DieType, AdvantageType.None, out _, out _);

                var attribute = rulesCharacter.GetAttribute(actionDefinition.AbilityScore);
                reductionAmount += AttributeDefinitions.ComputeAbilityScoreModifier(attribute.CurrentValue);
            }
            else
            {
                reductionAmount += customDeflector.GetDamageReduction(rulesCharacter, attacker.RulesetCharacter);
            }

            actionParams.ActionModifiers[0].DamageRollReduction += reductionAmount;

            rulesCharacter.EnumerateFeaturesToBrowse<FeatureDefinitionActionAffinity>(rulesCharacter.FeaturesToBrowse);


            var feature = (FeatureDefinition) null;
            foreach (var featureDefinition in rulesCharacter.FeaturesToBrowse)
            {
                var definitionActionAffinity = (FeatureDefinitionActionAffinity) featureDefinition;
                if (definitionActionAffinity.AuthorizedActions.Contains(ActionDefinitions.Id.DeflectMissile))
                {
                    feature = definitionActionAffinity;
                    break;
                }
            }

            if (feature != null)
            {
                rulesCharacter.DamageReduced(rulesCharacter, feature, reductionAmount);
            }

            yield return actingCharacter.WaitForHitAnimation();
            actingCharacter.TurnTowards(attacker);
            yield return actingCharacter.EventSystem.UpdateMotionsAndWaitForEvent(Event.RotationEnd);
        }
    }
}