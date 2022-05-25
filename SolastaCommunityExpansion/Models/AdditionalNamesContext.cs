using System;
using System.Collections.Generic;
using System.IO;
using SolastaCommunityExpansion.Properties;
using static SolastaModApi.DatabaseHelper.CharacterRaceDefinitions;

namespace SolastaCommunityExpansion.Models
{
    internal static class AdditionalNamesContext
    {
        internal static void Load()
        {
            if (Main.Settings.OfferAdditionalLoreFriendlyNames)
            {
                var payload = Resources.Names_en;
                var lines = new List<string>(payload.Split(new[] { Environment.NewLine }, StringSplitOptions.None));

                foreach (var line in lines)
                {
                    var splitted = line.Split(new[] { '\t', ' ' }, 2);
                    var term = splitted[0];
                    var name = splitted[1];

                    if (term.Contains("DwarfFemale"))
                    {
                        Dwarf.RacePresentation.FemaleNameOptions.Add(name);
                    }
                    else if (term.Contains("DwarfMale"))
                    {
                        Dwarf.RacePresentation.MaleNameOptions.Add(name);
                    }
                    else if (term.Contains("DwarfSur"))
                    {
                        Dwarf.RacePresentation.SurNameOptions.Add(name);
                    }
                    else if (term.Contains("ElfFemale"))
                    {
                        ElfHigh.RacePresentation.FemaleNameOptions.Add(name);
                    }
                    else if (term.Contains("ElfMale"))
                    {
                        ElfHigh.RacePresentation.MaleNameOptions.Add(name);
                    }
                    else if (term.Contains("ElfSur"))
                    {
                        ElfHigh.RacePresentation.SurNameOptions.Add(name);
                    }
                    else if (term.Contains("SylvanElfFemale"))
                    {
                        ElfSylvan.RacePresentation.FemaleNameOptions.Add(name);
                    }
                    else if (term.Contains("SylvanElfMale"))
                    {
                        ElfSylvan.RacePresentation.MaleNameOptions.Add(name);
                    }
                    else if (term.Contains("SylvanElfSur"))
                    {
                        ElfSylvan.RacePresentation.SurNameOptions.Add(name);
                    }
                    else if (term.Contains("HalfElfFemale"))
                    {
                        HalfElf.RacePresentation.FemaleNameOptions.Add(name);
                    }
                    else if (term.Contains("HalfElfMale"))
                    {
                        HalfElf.RacePresentation.MaleNameOptions.Add(name);
                    }
                    else if (term.Contains("HalfElfSur"))
                    {
                        HalfElf.RacePresentation.SurNameOptions.Add(name);
                    }
                    else if (term.Contains("HalfOrcFemale"))
                    {
                        HalfOrc.RacePresentation.FemaleNameOptions.Add(name);
                    }
                    else if (term.Contains("HalfOrcMale"))
                    {
                        HalfOrc.RacePresentation.MaleNameOptions.Add(name);
                    }
                    else if (term.Contains("HalflingFemale"))
                    {
                        Halfling.RacePresentation.FemaleNameOptions.Add(name);
                    }
                    else if (term.Contains("HalflingMale"))
                    {
                        Halfling.RacePresentation.MaleNameOptions.Add(name);
                    }
                    else if (term.Contains("HalflingSur"))
                    {
                        Halfling.RacePresentation.SurNameOptions.Add(name);
                    }
                    else if (term.Contains("HumanFemale"))
                    {
                        Human.RacePresentation.FemaleNameOptions.Add(name);
                    }
                    else if (term.Contains("HumanMale"))
                    {
                        Human.RacePresentation.MaleNameOptions.Add(name);
                    }
                    else if (term.Contains("HumanSur"))
                    {
                        Human.RacePresentation.SurNameOptions.Add(name);
                    }
                }
            }
        }
    }
}
