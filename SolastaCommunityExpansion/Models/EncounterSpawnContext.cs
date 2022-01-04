using System.Collections.Generic;
using System.Linq;
using TA;
using static SolastaModApi.DatabaseHelper.DecisionPackageDefinitions;
using static SolastaModApi.DatabaseHelper.FactionDefinitions;
using static SolastaModApi.DatabaseHelper.FormationDefinitions;

namespace SolastaCommunityExpansion.Models
{
    internal static class EncountersSpawnContext
    {
        private const InputCommands.Id CTRL_SHIFT_E = (InputCommands.Id)44440005;

        internal const int MAX_ENCOUNTER_CHARACTERS = 16;

        private static ulong EncounterId { get; set; } = 10000;

        private static readonly List<RulesetCharacterHero> Heroes = new List<RulesetCharacterHero>();

        private static readonly List<MonsterDefinition> Monsters = new List<MonsterDefinition>();

        internal static readonly List<RulesetCharacter> EncounterCharacters = new List<RulesetCharacter>();

        internal static void Load()
        {
            ServiceRepository.GetService<IInputService>().RegisterCommand(CTRL_SHIFT_E, 101, 304, 306, -1, -1, -1);
        }

        internal static void AddToEncounter(RulesetCharacterHero hero)
        {
            if (EncounterCharacters.Count < MAX_ENCOUNTER_CHARACTERS)
            {
                EncounterCharacters.Add(hero);
            }
        }

        internal static void AddToEncounter(MonsterDefinition monsterDefinition)
        {
            if (EncounterCharacters.Count < MAX_ENCOUNTER_CHARACTERS)
            {
                EncounterCharacters.Add(new RulesetCharacterMonster(monsterDefinition, 0, new RuleDefinitions.SpawnOverrides(), GadgetDefinitions.CreatureSex.Male));
            }
        }

        internal static void RemoveFromEncounter(int index)
        {
            if (index < EncounterCharacters.Count)
            {
                EncounterCharacters.RemoveAt(index);
            }
        }

        internal static List<MonsterDefinition> GetMonsters()
        {
            if (Monsters.Count == 0)
            {
                var monsterDefinitionDatabase = DatabaseRepository.GetDatabase<MonsterDefinition>();

                if (monsterDefinitionDatabase != null)
                {
                    Monsters.AddRange(monsterDefinitionDatabase.Where(x => x.DungeonMakerPresence == MonsterDefinition.DungeonMaker.Monster));
                    Monsters.Sort((a, b) => a.FormatTitle().CompareTo(b.FormatTitle()));
                }
            }

            return Monsters;
        }

        internal static List<RulesetCharacterHero> GetHeroes()
        {
            if (Heroes.Count == 0)
            {
                var characterPoolService = ServiceRepository.GetService<ICharacterPoolService>();

                if (characterPoolService != null)
                {
                    foreach (var name in characterPoolService.Pool.Keys)
                    {
                        var filename = characterPoolService.BuildCharacterFilename(name.Substring(0, name.Length - 4));

                        characterPoolService.LoadCharacter(filename, out RulesetCharacterHero hero, out RulesetCharacterHero.Snapshot _);
                        Heroes.Add(hero);
                    }

                    Heroes.Sort((a, b) =>
                    {
                        var compareName = a.Name.CompareTo(b.Name);

                        if (compareName == 0)
                        {
                            compareName = a.SurName.CompareTo(b.SurName);
                        }

                        return compareName;
                    });
                }
            }

            return Heroes;
        }

        internal static void ConfirmStageEncounter(InputCommands.Id command)
        {
            if (command == CTRL_SHIFT_E && EncounterCharacters.Count > 0)
            {
                var position = GetEncounterPosition();

                Gui.GuiService.ShowMessage(
                    MessageModal.Severity.Attention2,
                    "Message/&SpawnCustomEncounterTitle",
                    Gui.Format("Message/&SpawnCustomEncounterDescription", position.x.ToString(), position.x.ToString()),
                    "Message/&MessageYesTitle", "Message/&MessageNoTitle",
                    new MessageModal.MessageValidatedHandler(() => { StageEncounter(position); }),
                    null);
            }
        }

        private static int3 GetEncounterPosition()
        {
            var gameLocationService = ServiceRepository.GetService<IGameLocationService>();

            int x = (int)gameLocationService.GameLocation.LastCameraPosition.x;
            int z = (int)gameLocationService.GameLocation.LastCameraPosition.z;

            return new int3(x, 0, z);
        }

        private static void StageEncounter(int3 position)
        {
            var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var gameLocationPositioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
            var positions = new List<int3>();
            var formationPositions = new List<int3>();
            var sizeList = new List<RulesetActor.SizeParameters>();
            var characters = new List<GameLocationCharacter>();

            for (var iy = 0; iy < 4; iy++)
            {
                for (var ix = 0; ix < 4; ix++)
                {
                    formationPositions.Add(new int3(ix, 0, iy));
                }
            }

            foreach (var character in EncounterCharacters)
            {
                var gameLocationCharacter = gameLocationCharacterService.CreateCharacter(PlayerControllerManager.DmControllerId, character, RuleDefinitions.Side.Enemy, new GameLocationBehaviourPackage
                {
                    BattleStartBehavior = GameLocationBehaviourPackage.BattleStartBehaviorType.DoNotRaiseAlarm,
                    DecisionPackageDefinition = IdleGuard_Default,
                    EncounterId = EncounterId++,
                    FormationDefinition = EncounterCharacters.Count > 1 ? Squad4 : SingleCreature
                });

                gameLocationCharacter.CollectExistingLightSources(true);
                gameLocationCharacter.RefreshActionPerformances();
                gameLocationCharacter.RulesetCharacter.SetBaseFaction(HostileMonsters);
                characters.Add(gameLocationCharacter);
            }

            gameLocationPositioningService.ComputeFormationPlacementPositions(
                characters, position, LocationDefinitions.Orientation.North, formationPositions, CellHelpers.PlacementMode.Station, positions, sizeList, 25);

            for (var index = 0; index < positions.Count; index++)
            {
                gameLocationPositioningService.PlaceCharacter(characters[index], positions[index], LocationDefinitions.Orientation.North);
                gameLocationCharacterService.RevealCharacter(characters[index]);
            }

            Heroes.Clear();
            Monsters.Clear();
            EncounterCharacters.Clear();
        }
    }
}
