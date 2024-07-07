using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Subclasses;
using TA.AI;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class AiLocationManagerPatcher
{
    //PATCH: support for Circle of the Wildfire cauterizing flames
    [HarmonyPatch(typeof(AiLocationManager), nameof(AiLocationManager.ProcessBattleTurn))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ProcessBattleTurn_Patch
    {
        [UsedImplicitly]
        public static IEnumerator Postfix(IEnumerator values, AiLocationManager __instance)
        {
            yield return CircleOfTheWildfire.HandleCauterizingFlamesBehavior(__instance.battle.ActiveContender);

            while (values.MoveNext())
            {
                yield return values.Current;
            }
        }
    }

    [HarmonyPatch(typeof(AiLocationManager), nameof(AiLocationManager.BuildActivitiesMaps))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class BuildActivitiesMaps_Patch
    {
        [UsedImplicitly]
        public static void Postfix(AiLocationManager __instance)
        {
            foreach (var type in
                     Assembly.GetExecutingAssembly().GetTypes()
                         .Where(t => t.IsSubclassOf(typeof(ActivityBase))))
            {
                __instance.activitiesMap.TryAdd(type.ToString().Split('.').Last(), type);

                foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public))
                {
                    var parameters = method.GetParameters();
                    if (method.ReturnType == typeof(ContextType))
                    {
                        __instance.activityContextsMap.TryAdd(
                            type.ToString().Split('.').Last(),
                            (AiLocationDefinitions.GetContextTypeHandler)Delegate.CreateDelegate(
                                typeof(AiLocationDefinitions.GetContextTypeHandler), method));
                    }
                    else if (method.ReturnType == typeof(void) &&
                             parameters.Length == 2 &&
                             parameters[0].ParameterType.GetElementType() == typeof(ActionDefinitions.Id) &&
                             parameters[1].ParameterType.GetElementType() == typeof(ActionDefinitions.Id))
                    {
                        __instance.activityActionIdsMap.TryAdd(
                            type.ToString().Split('.').Last(),
                            (AiLocationDefinitions.GetActionIdHandler)Delegate.CreateDelegate(
                                typeof(AiLocationDefinitions.GetActionIdHandler), method));
                    }
                    else if (method.ReturnType == typeof(bool) &&
                             parameters.Length == 2 && parameters[0].ParameterType.GetElementType() ==
                             typeof(GameLocationCharacter))
                    {
                        __instance.activityShouldBeSkippedMap.TryAdd(
                            type.ToString().Split('.').Last(),
                            (AiLocationDefinitions.ShouldBeSkippedHandler)Delegate.CreateDelegate(
                                typeof(AiLocationDefinitions.ShouldBeSkippedHandler), method));
                    }
                    else if (method.ReturnType == typeof(bool) && parameters.Length == 0)
                    {
                        __instance.activityUsesMovementContextsMap.TryAdd(
                            type.ToString().Split('.').Last(),
                            (AiLocationDefinitions.UsesMovementContextsHandler)Delegate.CreateDelegate(
                                typeof(AiLocationDefinitions.UsesMovementContextsHandler), method));
                    }
                }
            }
        }
    }
}
