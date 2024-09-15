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
    //TODO: move to separate class?
    private static T CreateDelegate<T>(this MethodInfo method) where T : Delegate
    {
        return (T)Delegate.CreateDelegate(typeof(T), method);
    }

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
                var name = type.ToString().Split('.').Last();
                __instance.activitiesMap.AddOrReplace(name, type);

                foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public))
                {
                    var parameters = method.GetParameters();
                    if (method.ReturnType == typeof(ContextType))
                    {
                        __instance.activityContextsMap.AddOrReplace(name,
                            method.CreateDelegate<AiLocationDefinitions.GetContextTypeHandler>());
                    }
                    else if (method.ReturnType == typeof(void)
                             && parameters.Length == 2
                             && parameters[0].ParameterType.GetElementType() == typeof(ActionDefinitions.Id)
                             && parameters[1].ParameterType.GetElementType() == typeof(ActionDefinitions.Id))
                    {
                        __instance.activityActionIdsMap.AddOrReplace(name,
                            method.CreateDelegate<AiLocationDefinitions.GetActionIdHandler>());
                    }
                    else if (method.ReturnType == typeof(bool)
                             && parameters.Length == 2
                             && parameters[0].ParameterType.GetElementType() == typeof(GameLocationCharacter))
                    {
                        __instance.activityShouldBeSkippedMap.AddOrReplace(name,
                            method.CreateDelegate<AiLocationDefinitions.ShouldBeSkippedHandler>());
                    }
                    else if (method.ReturnType == typeof(bool) && parameters.Length == 0)
                    {
                        __instance.activityUsesMovementContextsMap.AddOrReplace(name,
                            method.CreateDelegate<AiLocationDefinitions.UsesMovementContextsHandler>());
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(AiLocationManager), nameof(AiLocationManager.BuildConsiderationsMap))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class BuildConsiderationsMap_Patch
    {
        [UsedImplicitly]
        public static void Postfix(AiLocationManager __instance)
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes()
                         .Where(t => t.IsSubclassOf(typeof(ConsiderationBase))))
            {
                var name = type.ToString().Split('.').Last();
                foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public))
                {
                    if (method.ReturnType == typeof(IEnumerator))
                    {
                        __instance.considerationsWithAllocMap.AddOrReplace(name,
                            method.CreateDelegate<AiLocationDefinitions.ScoreConsiderationWithAllocHandler>());
                    }
                    else if (method.ReturnType == typeof(void))
                    {
                        __instance.considerationsMap.AddOrReplace(name,
                            method.CreateDelegate<AiLocationDefinitions.ScoreConsiderationHandler>());
                    }
                }
            }
        }
    }
}
