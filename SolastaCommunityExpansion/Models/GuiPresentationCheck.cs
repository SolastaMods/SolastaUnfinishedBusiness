using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HarmonyLib;

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

            Main.Log("PostDatabaseLoad GuiPresentation Check start ------------------------------------------");

            TABaseDefinitions = GetAllDefinitions();

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

            var allDefinitions = GetAllDefinitions();

            foreach (var definition in allDefinitions.Where(db => !TABaseDefinitions.Contains(db)))
            {
                var gp = definition.GuiPresentation;

                if (string.IsNullOrWhiteSpace(gp?.Title) || string.IsNullOrWhiteSpace(gp?.Description))
                {
                    Main.Log($"Definition {definition.Name}: Title='{gp?.Title ?? string.Empty}', Desc='{gp?.Description ?? string.Empty}'");
                }
            }

            Main.Log("PostCELoad GuiPresentation Check end --------------------------------------------------");
        }
    }
}
