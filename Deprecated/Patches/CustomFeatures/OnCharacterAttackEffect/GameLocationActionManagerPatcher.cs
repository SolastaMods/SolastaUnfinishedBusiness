using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.OnCharacterAttackEffect;

//
// this patch shouldn't be protected
//
[HarmonyPatch(typeof(GameLocationActionManager), "ExecuteActionAsync")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GameLocationActionManager_ExecuteActionAsync
{
    internal static IEnumerator Postfix(
        [NotNull] IEnumerator values,
        [NotNull] CharacterAction action)
    {
        Main.Logger.Log(action.ActionDefinition.Name);

        var features = action.ActingCharacter.RulesetCharacter.GetSubFeaturesByType<ICustomOnActionFeature>();

        foreach (var feature in features)
        {
            feature.OnBeforeAction(action);
        }

        while (values.MoveNext())
        {
            yield return values.Current;
        }

        foreach (var feature in features)
        {
            feature.OnAfterAction(action);
        }
    }
}