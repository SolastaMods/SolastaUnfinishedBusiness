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

            foreach (var CharacterRaceDefinition in dbCharacterRaceDefinition)
            {
                CharacterRaceDefinition.FeatureUnlocks.Sort((a, b) =>
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

            if (Main.Settings.EnableSortingFutureFeatures)
            {
                SortRacesFeatures();
            }
        }

        private static void LoadRace(CharacterRaceDefinition characterClass)
        {
            if (!Races.ContainsKey(characterClass.Name))
            {
                Races.Add(characterClass.Name, characterClass);
            }

            Races = Races.OrderBy(x => x.Value.FormatTitle()).ToDictionary(x => x.Key, x => x.Value);

            UpdateRaceVisibility(characterClass.Name);
        }

        private static void UpdateRaceVisibility(string className)
        {
            Races[className].GuiPresentation.SetHidden(!Main.Settings.ClassEnabled.Contains(className));
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
