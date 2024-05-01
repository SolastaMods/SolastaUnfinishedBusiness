using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using TA.AI;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class UserEncounterPatcher
{
    //BUGFIX: vanilla isn't properly copying the encounter CR
    [HarmonyPatch(typeof(UserEncounter), nameof(UserEncounter.CreateEncounterDefinition))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CreateEncounterTableDefinition_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(UserEncounter __instance, out EncounterDefinition __result)
        {
            __result = CreateEncounterDefinition(__instance);

            return false;
        }

        private static EncounterDefinition CreateEncounterDefinition(UserEncounter __instance)
        {
            var instance = ScriptableObject.CreateInstance<EncounterDefinition>();

            instance.ForceName(__instance.InternalName);
            instance.IsUserContent = true;
            instance.DungeonMakerPresence = true;
            instance.Type = EncounterDefinitions.Type.Battle;
            instance.GuiPresentation.Title = __instance.DisplayTitle;
            instance.GuiPresentation.Description = __instance.Description;
            
            //BEGIN PATCH
            instance.ChallengeRating = __instance.ChallengeRating;
            //END PATCH

            foreach (var monsterOccurence in __instance.monsterOccurences
                         .Where(monsterOccurence =>
                             !string.IsNullOrEmpty(monsterOccurence.Monster) &&
                             DatabaseRepository.GetDatabase<MonsterDefinition>()
                                 .HasElement(monsterOccurence.Monster) &&
                             !string.IsNullOrEmpty(monsterOccurence.PlacementDecision) &&
                             DatabaseRepository.GetDatabase<DecisionPackageDefinition>()
                                 .HasElement(monsterOccurence.PlacementDecision)))
            {
                instance.MonsterOccurences.Add(new MonsterOccurenceDescription
                {
                    Number = monsterOccurence.Number,
                    MonsterDefinition =
                        DatabaseRepository.GetDatabase<MonsterDefinition>()
                            .GetElement(monsterOccurence.Monster),
                    EncounterPlacementDecision = 
                        DatabaseRepository.GetDatabase<DecisionPackageDefinition>()
                            .GetElement(monsterOccurence.PlacementDecision)
                });
            }

            instance.LocationBlacklist ??= [];

            foreach (var key in __instance.encounterLocationsBlacklist
                         .Where(key =>
                             !string.IsNullOrEmpty(key) &&
                             DatabaseRepository.GetDatabase<LocationDefinition>().HasElement(key)))
            {
                instance.LocationBlacklist.Add(DatabaseRepository.GetDatabase<LocationDefinition>().GetElement(key));
            }

            return instance;
        }
    }
}
