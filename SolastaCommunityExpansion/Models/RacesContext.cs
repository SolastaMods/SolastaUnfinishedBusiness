using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolastaCommunityExpansion.Races;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Models
{
    internal static class RacesContext
    {
        internal static Dictionary<string, CharacterRaceDefinition> Races { get; private set; } = new Dictionary<string, CharacterRaceDefinition>();

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

            Races = Races.OrderBy(x => x.Value.FormatTitle()).ToDictionary(x => x.Key, x => x.Value);

            if (Main.Settings.EnableSortingFutureFeatures)
            {
                SortRacesFeatures();
            }
        }

        private static void LoadRace(CharacterRaceDefinition definition)
        {
            var name = definition.Name;

            if (!Races.ContainsKey(name))
            {
                Races.Add(name, definition);
            }

            UpdateRaceVisibility(name);
        }

        private static void UpdateRaceVisibility(string raceName)
        {
            Races[raceName].GuiPresentation.SetHidden(!Main.Settings.RaceEnabled.Contains(raceName));
        }

        internal static void Switch(string raceName, bool active)
        {
            if (!Races.ContainsKey(raceName))
            {
                return;
            }

            if (active)
            {
                if (!Main.Settings.RaceEnabled.Contains(raceName))
                {
                    Main.Settings.RaceEnabled.Add(raceName);
                }
            }
            else
            {
                Main.Settings.RaceEnabled.Remove(raceName);
            }

            UpdateRaceVisibility(raceName);
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
