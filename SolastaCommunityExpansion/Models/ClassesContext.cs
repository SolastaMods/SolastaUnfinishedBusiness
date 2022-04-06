using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolastaCommunityExpansion.Classes.Tinkerer;
using SolastaCommunityExpansion.Classes.Warlock;
using SolastaCommunityExpansion.Classes.Witch;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Models
{
    internal static class ClassesContext
    {
        internal static HashSet<CharacterClassDefinition> Classes { get; private set; } = new();

        internal static void SortClassesFeatures()
        {
            var dbCharacterClassDefinition = DatabaseRepository.GetDatabase<CharacterClassDefinition>();

            foreach (var characterClassDefinition in dbCharacterClassDefinition)
            {
                characterClassDefinition.FeatureUnlocks.Sort((a, b) =>
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
            LoadClass(TinkererClass.BuildTinkererClass());
            LoadClass(Warlock.BuildWarlockClass());
            LoadClass(Witch.Instance);

            Classes = Classes.OrderBy(x => x.FormatTitle()).ToHashSet();

            if (Main.Settings.EnableSortingFutureFeatures)
            {
                SortClassesFeatures();
            }
        }

        private static void LoadClass(CharacterClassDefinition characterClassDefinition)
        {
            if (!Classes.Contains(characterClassDefinition))
            {
                Classes.Add(characterClassDefinition);
            }

            UpdateClassVisibility(characterClassDefinition);
        }

        private static void UpdateClassVisibility(CharacterClassDefinition characterClassDefinition)
        {
            characterClassDefinition.GuiPresentation.SetHidden(!Main.Settings.ClassEnabled.Contains(characterClassDefinition.Name));
        }

        internal static void Switch(CharacterClassDefinition characterClassDefinition, bool active)
        {
            if (!Classes.Contains(characterClassDefinition))
            {
                return;
            }

            var name = characterClassDefinition.Name;

            if (active)
            {
                Main.Settings.ClassEnabled.TryAdd(name);
            }
            else
            {
                Main.Settings.ClassEnabled.Remove(name);
            }

            UpdateClassVisibility(characterClassDefinition);
        }

#if DEBUG
        public static string GenerateClassDescription()
        {
            var outString = new StringBuilder();

            foreach (var characterClass in Classes)
            {
                outString.Append("\n[*][b]");
                outString.Append(characterClass.FormatTitle());
                outString.Append("[/b]: ");
                outString.Append(characterClass.FormatDescription());
            }

            return outString.ToString();
        }
#endif
    }
}
