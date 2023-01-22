using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class NarrativeStatePlayerRoleAssignmentPatcher
{
    //PATCH: ensures dialogs won't break on official campaigns with parties less than 4 (PARTYSIZE)
    [HarmonyPatch(typeof(NarrativeStatePlayerRoleAssignment), nameof(NarrativeStatePlayerRoleAssignment.BuildHook))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class BuildHook_Patch
    {
        // official game code except for the 4 patches that replace Add with TryAdd
        // ideally we should use a transpiler here but hard one to tackle
        [UsedImplicitly]
        public static bool Prefix(
            NarrativeStatePlayerRoleAssignment __instance,
            Dictionary<string, WorldLocationCharacter> playerRolesMap,
            Dictionary<string, WorldLocationCharacter> npcRolesMap)
        {
            __instance.hasValidRoles = true;
            
            if (__instance.NarrativeStateDescription.ClearPreviousRoles)
            {
                __instance.PlayerRolesMap.Clear();
            }

            var actors = new List<WorldLocationCharacter>();
            var flag1 = !__instance.NarrativeSequence.IsGameplaySequence;

            for (var index = 0; index < __instance.NarrativeSequence.PlayerActors.Count; ++index)
            {
                var flag2 = false;
                var playerActor = __instance.NarrativeSequence.PlayerActors[index];
                
                if (!__instance.NarrativeStateDescription.ClearPreviousRoles &&
                    __instance.NarrativeStateDescription.SelectActorsWithoutARole)
                {
                    if (!__instance.PlayerRolesMap.ContainsValue(playerActor))
                    {
                        flag2 = true;
                    }
                }
                else
                {
                    flag2 = true;
                }

                if (flag2 && (flag1 || !playerActor.GameLocationCharacter.RulesetCharacter.IsDeadOrUnconscious))
                {
                    actors.Add(playerActor);
                }
            }

            if (__instance.NarrativeStateDescription.CampaignDefinition == null)
            {
                if (__instance.NarrativeStateDescription.DialogLines.Count > 0)
                {
                    for (var index = 0;
                         index < __instance.NarrativeStateDescription.DialogLines.Count && actors.Count > 0;
                         ++index)
                    {
                        var dialogLine = __instance.NarrativeStateDescription.DialogLines[index];

                        if (!__instance.PlayerRolesMap.ContainsKey(dialogLine.TextLine))
                        {
                            if (dialogLine.FlavorFlags != null && dialogLine.FlavorFlags.Count > 0)
                            {
                                __instance.SortByPersonalityFlagRelevance(actors, dialogLine.FlavorFlags);

                                if (ServiceRepository.GetService<IRulesetImplementationService>()
                                    .HasPositivePersonalityFlagFromLastSort(actors[0].GameLocationCharacter
                                        .RulesetCharacter))
                                {
                                    __instance.PlayerRolesMap.TryAdd(dialogLine.TextLine, actors[0]); // PATCH HERE
                                    actors.RemoveAt(0);
                                }
                            }
                            else
                            {
                                __instance.PlayerRolesMap.TryAdd(dialogLine.TextLine, actors[0]); // PATCH HERE
                                actors.RemoveAt(0);
                            }
                        }
                    }
                }

                if (!__instance.NarrativeSequence.IsGameplaySequence || __instance.PlayerRolesMap.Count >=
                    __instance.NarrativeStateDescription.DialogLines.Count)
                {
                    return false;
                }

                ServiceRepository.GetService<INarrativeDirectionService>()
                    .AbortCurrentSequenceForNextStartingSequenceIFN(null);
                
                __instance.hasValidRoles = false;
            }
            else
            {
                var service = ServiceRepository.GetService<ISessionService>();

                if (service.Session.CampaignDefinitionName !=
                    __instance.NarrativeStateDescription.CampaignDefinition.Name)
                {
                    Trace.LogError("GameplayRoles are being accessed from the campaign '" +
                                   __instance.NarrativeStateDescription.CampaignDefinition.Name +
                                   "', but do not belong to the current campaign '" +
                                   service.Session.CampaignDefinitionName + "'");
                }

                for (var index1 = 0;
                     index1 < __instance.NarrativeStateDescription.CampaignDefinition.AutoGameplayRoles.Count &&
                     actors.Count > 0;
                     ++index1)
                {
                    var autoGameplayRole =
                        __instance.NarrativeStateDescription.CampaignDefinition.AutoGameplayRoles[index1];

                    if (!__instance.PlayerRolesMap.ContainsKey(autoGameplayRole.GameplayRoleName))
                    {
                        var flag3 = false;
                        
                        for (var index2 = 0; index2 < actors.Count; ++index2)
                        {
                            var locationCharacter = actors[index2];

                            if (locationCharacter.GameLocationCharacter.RulesetCharacter.Tags.Contains(autoGameplayRole
                                    .GameplayRoleName))
                            {
                                __instance.PlayerRolesMap.TryAdd(autoGameplayRole.GameplayRoleName,
                                    locationCharacter); // PATCH HERE
                                actors.RemoveAt(index2);
                                flag3 = true;
                            }
                        }

                        if (!flag3)
                        {
                            __instance.autoFlags.Clear();
                            __instance.autoFlags.Add(autoGameplayRole.PersonalityFlag);
                            __instance.SortByPersonalityFlagRelevance(actors, __instance.autoFlags);
                            __instance.PlayerRolesMap.TryAdd(autoGameplayRole.GameplayRoleName,
                                actors[0]); // PATCH HERE
                            actors.RemoveAt(0);
                        }
                    }
                }

                if (!__instance.NarrativeSequence.IsGameplaySequence || __instance.PlayerRolesMap.Count >=
                    __instance.NarrativeStateDescription.CampaignDefinition.AutoGameplayRoles.Count)
                {
                    return false;
                }

                ServiceRepository.GetService<INarrativeDirectionService>()
                    .AbortCurrentSequenceForNextStartingSequenceIFN(null);
                __instance.hasValidRoles = false;
            }

            return false;
        }
    }
}
