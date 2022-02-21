using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using I2.Loc;
using SolastaCommunityExpansion.Json;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Models
{
    internal static class Diagnostics
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

        public static void PostDatabaseLoad()
        {
            #region Preconditions
            if (TABaseDefinitions != null)
            {
                // Only load TABaseDefinitions once
                return;
            }

            TABaseDefinitions = GetAllDefinitions();
            
            var taDefinitions = TABaseDefinitions.OrderBy(x => x.Name).ThenBy(x => x.GetType().Name).ToList();

            if (!Main.Settings.DebugEnableTADefinitionDiagnostics)
            {
                return;
            }

            var folder = Environment.GetEnvironmentVariable("SolastaCEDiagnosticsDir", EnvironmentVariableTarget.Machine);

            if (string.IsNullOrWhiteSpace(folder))
            {
                Main.Log("[SolastaCEDiagnosticsDir] is not set.  Aborting PostDatabaseLoad diagnostics.");
                return;
            }

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            #endregion

            /////////////////////////////////////////////////////////////////////////////////////////////////
            // Write all TA definitions with no GUI presentation to file
            File.WriteAllLines(Path.Combine(folder, "TA-Definitions-GuiPresentation-MissingValue.txt"),
                taDefinitions
                    .Where(d => string.IsNullOrWhiteSpace(d.GuiPresentation?.Title) || string.IsNullOrWhiteSpace(d.GuiPresentation?.Description))
                    .Select(d => $"{d.Name}:\tTitle='{d.GuiPresentation?.Title ?? string.Empty}', Desc='{d.GuiPresentation?.Description ?? string.Empty}'"));

            /////////////////////////////////////////////////////////////////////////////////////////////////
            // Write all TA definitions with GUI presentation but missing translation to file
            var languageSourceData = LocalizationManager.Sources[0];
            var currentLanguage = LocalizationManager.CurrentLanguageCode;
            var languageIndex = languageSourceData.GetLanguageIndexFromCode(currentLanguage);

            var allLines = taDefinitions
                .Select(d => new[] {
                    new { d.Name, Key = d.GuiPresentation?.Title, Type = "Title" },
                    new { d.Name, Key = d.GuiPresentation?.Description, Type = "Description" }
                })
                .SelectMany(d => d)
                .Where(d =>
                {
                    if (!string.IsNullOrWhiteSpace(d.Key))
                    {
                        var termData = languageSourceData.GetTermData(d.Key);
                        return string.IsNullOrWhiteSpace(termData?.Languages[languageIndex]);
                    }
                    else
                    {
                        return false;
                    }
                })
                .Select(d => $"{d.Name}\t{d.Type}='{d.Key}'.");

            File.WriteAllLines(Path.Combine(folder, $"TA-Definitions-GuiPresentation-MissingTranslation-{currentLanguage}.txt"), allLines);
        }

        public static void PostCELoad()
        {
            #region Preconditions
            if (TABaseDefinitions == null)
            {
                return;
            }

            if (!Main.Settings.DebugEnableCEDefinitionDiagnostics)
            {
                return;
            }

            var folder = Environment.GetEnvironmentVariable("SolastaCEDiagnosticsDir", EnvironmentVariableTarget.Machine);

            if (string.IsNullOrWhiteSpace(folder))
            {
                Main.Log("[SolastaCEDiagnosticsDir] is not set.  Aborting PostCELoad diagnostics.");
                return;
            }

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            #endregion

            var ceDefinitions = GetAllDefinitions().Except(TABaseDefinitions).OrderBy(x => x.Name).ThenBy(x => x.GetType().Name).ToList();

            /////////////////////////////////////////////////////////////////////////////////////////////////
            // Write all CE definitions name/guid to file (txt)
            File.WriteAllLines(Path.Combine(folder, "CE-Definitions.txt"),
                ceDefinitions.Select(d => $"{d.Name}, {d.GUID}"));

            // Write all CE definitions to file (json)
            ExportDefinitions(ceDefinitions, Path.Combine(folder, "CE-Definitions.json"));

            // Write all CE definitions with no GUI presentation to file
            File.WriteAllLines(Path.Combine(folder, "CE-Definitions-GuiPresentation-MissingValue.txt"),
                ceDefinitions
                    .Where(d => string.IsNullOrWhiteSpace(d.GuiPresentation?.Title) || string.IsNullOrWhiteSpace(d.GuiPresentation?.Description))
                    .Select(d => $"{d.Name}:\tTitle='{d.GuiPresentation?.Title ?? string.Empty}', Desc='{d.GuiPresentation?.Description ?? string.Empty}'"));

            /////////////////////////////////////////////////////////////////////////////////////////////////
            // Write all CE definitions with GUI presentation but missing translation to file
            var languageSourceData = LocalizationManager.Sources[0];
            var currentLanguage = LocalizationManager.CurrentLanguageCode;
            var languageIndex = languageSourceData.GetLanguageIndexFromCode(currentLanguage);

            var allLines = ceDefinitions
                .Select(d => new[] {
                    new { d.Name, Key = d.GuiPresentation?.Title, Type = "Title" },
                    new { d.Name, Key = d.GuiPresentation?.Description, Type = "Description" }
                })
                .SelectMany(d => d)
                .Where(d =>
                {
                    if (!string.IsNullOrWhiteSpace(d.Key))
                    {
                        var termData = languageSourceData.GetTermData(d.Key);
                        return string.IsNullOrWhiteSpace(termData?.Languages[languageIndex]);
                    }
                    else
                    {
                        return false;
                    }
                })
                .Select(d => $"{d.Name}\t{d.Type}='{d.Key}'.");

            File.WriteAllLines(Path.Combine(folder, $"CE-Definitions-GuiPresentation-MissingTranslation-{currentLanguage}.txt"), allLines);
        }

        private static Dictionary<BaseDefinition, IEnumerable<EffectForm>> _effectFormCopiesDict = new Dictionary<BaseDefinition, IEnumerable<EffectForm>>();

        public static void ExportDefinitions(IEnumerable<BaseDefinition> definitions, string path)
        {
            foreach (var definition in definitions)
            {
                if (definition is SpellDefinition spellDefinition)
                {
                    SanitizeEffectDescription(spellDefinition.EffectDescription);
                }
                else if (definition is MonsterAttackDefinition monsterAttackDefinition)
                {
                    SanitizeEffectDescription(monsterAttackDefinition.EffectDescription);
                }
                else if (definition is EnvironmentEffectDefinition environmentEffectDefinition)
                {
                    SanitizeEffectDescription(environmentEffectDefinition.EffectDescription);
                }
                else if (definition is FeatureDefinitionPower featureDefinitionPower)
                {
                    SanitizeEffectDescription(featureDefinitionPower.EffectDescription);
                }
                // TODO: sanitize more uses of EffectDescription
             
                void SanitizeEffectDescription(EffectDescription effectDescription)
                {
                    var effectFormsCopy = effectDescription.EffectForms.ConvertAll(f => f.Copy());
                    _effectFormCopiesDict.Add(definition, effectFormsCopy);
                    effectDescription.EffectForms.SetRange(SanitizeEffectForms(effectFormsCopy));
                }
            }

            JsonUtil.Dump(definitions, path);

            foreach(var definition in definitions)
            {
                if (definition is SpellDefinition spellDefinition)
                {
                    RestoreOriginalEffectForms(spellDefinition.EffectDescription);
                }
                else if (definition is MonsterAttackDefinition monsterAttackDefinition)
                {
                    RestoreOriginalEffectForms(monsterAttackDefinition.EffectDescription);
                }
                else if (definition is EnvironmentEffectDefinition environmentEffectDefinition)
                {
                    RestoreOriginalEffectForms(environmentEffectDefinition.EffectDescription);
                }
                else if (definition is FeatureDefinitionPower featureDefinitionPower)
                {
                    RestoreOriginalEffectForms(featureDefinitionPower.EffectDescription);
                }
                // TODO: more uses of EffectDescription

                void RestoreOriginalEffectForms(EffectDescription effectDescription)
                {
                    effectDescription.EffectForms.SetRange(_effectFormCopiesDict[definition]);
                }
            }

            IEnumerable<EffectForm> SanitizeEffectForms(IEnumerable<EffectForm> effectForms)
            {
                try
                {
                    foreach (var effectForm in effectForms)
                    {
                        var type = effectForm.FormType;
                        var t = Traverse.Create(effectForm);

                        foreach (var value in Enum.GetValues(typeof(EffectForm.EffectFormType)))
                        {
                            if (type != (EffectForm.EffectFormType)value)
                            {
                                t.Property($"{value}Form").SetValue(null);
                            }
                        }
                    }

                    return effectForms;
                }
                catch (Exception ex)
                {
                    Main.Error(ex);
                }

                return effectForms;
            }
        }
    }
}
