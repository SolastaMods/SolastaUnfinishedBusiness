using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolastaCommunityExpansion.Races;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Models
{
    internal static class RacesContext
    {
        internal static HashSet<CharacterRaceDefinition> Races { get; private set; } = new();

        internal static void SortRacesFeatures()
        {
            var dbCharacterRaceDefinition = DatabaseRepository.GetDatabase<CharacterRaceDefinition>();

            foreach (var characterRaceDefinition in dbCharacterRaceDefinition)
            {
                characterRaceDefinition.FeatureUnlocks.Sort((a, b) =>
                {
                    var result = a.Level - b.Level;

                    if (result == 0)
                    {
                        result = a.FeatureDefinition.FormatTitle().CompareTo(b.FeatureDefinition.FormatTitle());
                    }

                    return result;
                });
            }
        }

        internal static void Load()
        {
            LoadRace(BolgrifRaceBuilder.BolgrifRace);
            LoadRace(GnomeRaceBuilder.GnomeRace);

            Races = Races.OrderBy(x => x.FormatTitle()).ToHashSet();

            if (Main.Settings.EnableSortingFutureFeatures)
            {
                SortRacesFeatures();
            }
        }

        private static void LoadRace(CharacterRaceDefinition characterRaceDefinition)
        {
            if (!Races.Contains(characterRaceDefinition))
            {
                Races.Add(characterRaceDefinition);
            }

            UpdateRaceVisibility(characterRaceDefinition);
        }

        private static void UpdateRaceVisibility(CharacterRaceDefinition characterRaceDefinition)
        {
            characterRaceDefinition.GuiPresentation.SetHidden(!Main.Settings.RaceEnabled.Contains(characterRaceDefinition.Name));
        }

        internal static void Switch(CharacterRaceDefinition characterRaceDefinition, bool active)
        {
            if (!Races.Contains(characterRaceDefinition))
            {
                return;
            }

            var name = characterRaceDefinition.Name;

            if (active)
            {
                if (!Main.Settings.RaceEnabled.Contains(name))
                {
                    Main.Settings.RaceEnabled.Add(name);
                }
            }
            else
            {
                Main.Settings.RaceEnabled.Remove(name);
            }

            UpdateRaceVisibility(characterRaceDefinition);
        }

#if DEBUG
        public static string GenerateRaceDescription()
        {
            var outString = new StringBuilder("[size=3][b]Races[/b][/size]\n");

            outString.Append("\n[list]");

            foreach (var characterRace in Races.Values)
            {
                outString.Append("\n[*][b]");
                outString.Append(characterRace.FormatTitle());
                outString.Append("[/b]: ");
                outString.Append(characterRace.FormatDescription());
            }

            outString.Append("\n[/list]");

            return outString.ToString();
        }
#endif
    }
}
