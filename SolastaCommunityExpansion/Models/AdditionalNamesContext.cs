using System.IO;
using static SolastaModApi.DatabaseHelper.CharacterRaceDefinitions;

namespace SolastaCommunityExpansion.Models
{
    internal static class AdditionalNamesContext
    {
        internal static void Load()
        {
            if (Main.Settings.OfferAdditionalNames)
            {
                CharacterRaceDefinition dwarfNames = Dwarf;
                CharacterRaceDefinition elfHighNames = ElfHigh;
                CharacterRaceDefinition elfSylvanNames = ElfSylvan;
                CharacterRaceDefinition halfElfNames = HalfElf;
                CharacterRaceDefinition halflingNames = Halfling;
                CharacterRaceDefinition humanNames = Human;

                foreach (var line in File.ReadLines($"{Main.MOD_FOLDER}/Names-en.txt"))
                {
                    var splitted = line.Split(new[] { '\t', ' ' }, 2);
                    var term = splitted[0];
                    var name = splitted[1];

                    if (term.Contains("DwarfFemale"))
                    {
                        dwarfNames.RacePresentation.FemaleNameOptions.Add(name);
                    }
                    else if (term.Contains("DwarfMale"))
                    {
                        dwarfNames.RacePresentation.MaleNameOptions.Add(name);
                    }
                    else if (term.Contains("DwarfSur"))
                    {
                        dwarfNames.RacePresentation.SurNameOptions.Add(name);
                    }

                    else if (term.Contains("ElfFemale"))
                    {
                        elfHighNames.RacePresentation.FemaleNameOptions.Add(name);
                    }
                    else if (term.Contains("ElfMale"))
                    {
                        elfHighNames.RacePresentation.MaleNameOptions.Add(name);
                    }
                    else if (term.Contains("ElfSur"))
                    {
                        elfHighNames.RacePresentation.SurNameOptions.Add(name);
                    }

                    else if (term.Contains("SylvanElfFemale"))
                    {
                        elfSylvanNames.RacePresentation.FemaleNameOptions.Add(name);
                    }
                    else if (term.Contains("SylvanElfMale"))
                    {
                        elfSylvanNames.RacePresentation.MaleNameOptions.Add(name);
                    }
                    else if (term.Contains("SylvanElfSur"))
                    {
                        elfSylvanNames.RacePresentation.SurNameOptions.Add(name);
                    }

                    else if (term.Contains("HalfElfFemale"))
                    {
                        halfElfNames.RacePresentation.FemaleNameOptions.Add(name);
                    }
                    else if (term.Contains("HalfElfMale"))
                    {
                        halfElfNames.RacePresentation.MaleNameOptions.Add(name);
                    }
                    else if (term.Contains("HalfElfSur"))
                    {
                        halfElfNames.RacePresentation.SurNameOptions.Add(name);
                    }

                    else if (term.Contains("HalflingFemale"))
                    {
                        halflingNames.RacePresentation.FemaleNameOptions.Add(name);
                    }
                    else if (term.Contains("HalflingMale"))
                    {
                        halflingNames.RacePresentation.MaleNameOptions.Add(name);
                    }
                    else if (term.Contains("HalflingSur"))
                    {
                        halflingNames.RacePresentation.SurNameOptions.Add(name);
                    }

                    else if (term.Contains("HumanFemale"))
                    {
                        humanNames.RacePresentation.FemaleNameOptions.Add(name);
                    }
                    else if (term.Contains("HumanMale"))
                    {
                        humanNames.RacePresentation.MaleNameOptions.Add(name);
                    }
                    else if (term.Contains("HumanSur"))
                    {
                        humanNames.RacePresentation.SurNameOptions.Add(name);
                    }
                }
            }
        }
    }
}
