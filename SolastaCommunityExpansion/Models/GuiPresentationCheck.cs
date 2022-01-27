using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HarmonyLib;
using I2.Loc;

namespace SolastaCommunityExpansion.Models
{
    internal static class GuiPresentationCheck
    {
        private static HashSet<BaseDefinition> TABaseDefinitions;

        private static HashSet<BaseDefinition> GetAllDefinitions()
        {
            var definitions = new HashSet<BaseDefinition>();

            foreach (var db in (Dictionary<Type, object>)AccessTools.Field(typeof(DatabaseRepository), "databases").GetValue(null))
            {
                foreach (var bd in (IEnumerable)db.Value)
                {
                    definitions.Add((BaseDefinition)bd);
                }
            }

            return definitions;
        }

        [Conditional("DEBUG")]
        public static void PostDatabaseLoadCheck()
        {
            if (TABaseDefinitions != null)
            {
                return;
            }

            TABaseDefinitions = GetAllDefinitions();

            if (!Main.Settings.ShowTADefinitionsWithMissingGuiPresentation)
            {
                return;
            }

            Main.Log("PostDatabaseLoad GuiPresentation Check start ------------------------------------------");

            foreach (var definition in TABaseDefinitions)
            {
                var gp = definition.GuiPresentation;

                if (string.IsNullOrWhiteSpace(gp?.Title) || string.IsNullOrWhiteSpace(gp?.Description))
                {
                    Main.Log($"Definition {definition.Name}: Title='{gp?.Title ?? string.Empty}', Desc='{gp?.Description ?? string.Empty}'");
                }
            }

            Main.Log("PostDatabaseLoad GuiPresentation Check end --------------------------------------------");
        }

        [Conditional("DEBUG")]
        public static void PostCELoadCheck()
        {
            Main.Log("PostCELoad GuiPresentation Check start ------------------------------------------------");

            if (!Main.Settings.ShowCEDefinitionsWithMissingGuiPresentation)
            {
                return;
            }

            var allDefinitions = GetAllDefinitions();

            var languageSourceData = LocalizationManager.Sources[0];
            var currentLanguage = LocalizationManager.CurrentLanguageCode;
            var languageIndex = languageSourceData.GetLanguageIndexFromCode(currentLanguage);

            foreach (var definition in allDefinitions.Except(TABaseDefinitions))
            {
                var gp = definition.GuiPresentation;

                if (string.IsNullOrWhiteSpace(gp?.Title) || string.IsNullOrWhiteSpace(gp?.Description))
                {
                    Main.Log($"*Definition {definition.Name}: Title='{gp?.Title ?? string.Empty}', Desc='{gp?.Description ?? string.Empty}'");
                }

                if (!string.IsNullOrWhiteSpace(gp?.Title))
                {
                    var termData = languageSourceData.GetTermData(gp.Title);

                    if (string.IsNullOrWhiteSpace(termData?.Languages[languageIndex]))
                    {
                        Main.Log($"Definition {definition.Name}: Title='{gp.Title}' has no English translation.");
                    }
                }

                if (!string.IsNullOrWhiteSpace(gp?.Description))
                {
                    var termData = languageSourceData.GetTermData(gp.Title);

                    if (string.IsNullOrWhiteSpace(termData?.Languages[languageIndex]))
                    {
                        Main.Log($"Definition {definition.Name}: Description='{gp.Description}' has no English translation.");
                    }
                }
            }

            Main.Log("PostCELoad GuiPresentation Check end --------------------------------------------------");
        }
    }
}
