#if DEBUG
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.DataMiner;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Displays;

internal static class DiagnosticsDisplay
{
    internal static void DisplayDiagnostics()
    {
        UI.Label(". You can set the environment variable " +
                 DiagnosticsContext.ProjectEnvironmentVariable.Italic().Khaki() +
                 " to customize the output folder");

        if (DiagnosticsContext.ProjectFolder == null)
        {
            UI.Label(". The output folder is set to " + "your game folder".Khaki().Bold());
        }
        else
        {
            UI.Label(". The output folder is set to " + DiagnosticsContext.DiagnosticsFolder.Khaki().Bold());
        }

        UI.Label();

        string exportTaLabel;
        string exportTaLabel2;
        string exportCeLabel;
        var percentageCompleteTa = BlueprintExporter.CurrentExports[DiagnosticsContext.Ta].PercentageComplete;
        var percentageCompleteTa2 = BlueprintExporter.CurrentExports[DiagnosticsContext.Ta2].PercentageComplete;
        var percentageCompleteCe = BlueprintExporter.CurrentExports[DiagnosticsContext.Ce].PercentageComplete;

        if (percentageCompleteTa == 0)
        {
            exportTaLabel = "Export TA blueprints";
        }
        else
        {
            exportTaLabel = "Cancel TA export at " + $"{percentageCompleteTa:00.0%}".Bold().Khaki();
        }

        if (percentageCompleteTa2 == 0)
        {
            exportTaLabel2 = "Export Modded blueprints";
        }
        else
        {
            exportTaLabel2 = "Cancel TA export at " + $"{percentageCompleteTa2:00.0%}".Bold().Khaki();
        }

        if (percentageCompleteCe == 0)
        {
            exportCeLabel = "Export UB blueprints";
        }
        else
        {
            exportCeLabel = "Cancel UB export at " + $"{percentageCompleteCe:00.0%}".Bold().Khaki();
        }

        using (UI.HorizontalScope())
        {
            UI.ActionButton(exportTaLabel, () =>
            {
                if (percentageCompleteTa == 0)
                {
                    DiagnosticsContext.ExportTaDefinitions();
                }
                else
                {
                    BlueprintExporter.Cancel(DiagnosticsContext.Ta);
                }
            }, 200.Width());

            UI.ActionButton(exportCeLabel, () =>
            {
                if (percentageCompleteCe == 0)
                {
                    DiagnosticsContext.ExportCeDefinitions();
                }
                else
                {
                    BlueprintExporter.Cancel(DiagnosticsContext.Ce);
                }
            }, 200.Width());
        }

        using (UI.HorizontalScope())
        {
            UI.ActionButton("Create TA diagnostics", DiagnosticsContext.CreateTaDefinitionDiagnostics,
                200.Width());
            UI.ActionButton("Create UB diagnostics", DiagnosticsContext.CreateCeDefinitionDiagnostics,
                200.Width());
        }

        UI.ActionButton(exportTaLabel2, () =>
        {
            if (percentageCompleteTa2 == 0)
            {
                DiagnosticsContext.ExportTaDefinitionsAfterCeLoaded();
            }
            else
            {
                BlueprintExporter.Cancel(DiagnosticsContext.Ta2);
            }
        }, 200.Width());

        UI.Label();

        var logVariantMisuse = Main.Settings.DebugLogVariantMisuse;

        if (UI.Toggle("Log misuse of EffectForm and ItemDefinition [requires restart]", ref logVariantMisuse))
        {
            Main.Settings.DebugLogVariantMisuse = logVariantMisuse;
        }

        UI.Label();
    }
}
#endif
