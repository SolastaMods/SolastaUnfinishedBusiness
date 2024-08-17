#if DEBUG
using System;
using System.IO;
using SolastaUnfinishedBusiness.Api.Diagnostics;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.DataMiner;

internal static class ItemDefinitionVerification
{
    private const string LogName = "ItemDefinition.txt";

    private static Verification Mode { get; set; } = Verification.None;

    internal static void Load()
    {
        // Delete the log file to stop it growing out of control
        var path = Path.Combine(DiagnosticsContext.DiagnosticsFolder, LogName);

        try
        {
            File.Delete(path);
        }
        catch (Exception ex)
        {
            Main.Error(ex);
        }

        // Apply mode before any definitions are created
        Mode = Main.Settings.DebugLogVariantMisuse ? Verification.Log : Verification.None;
    }

    internal static void VerifyUsage<T>(ItemDefinition definition, bool hasFlag, ref T __result) where T : class
    {
        if (Mode == Verification.None)
        {
            return;
        }

        // If the return val is null we can be sure it's either throwing an exception
        // or being checked for.
        if (hasFlag || __result == null)
        {
            return;
        }

        var msg =
            $"ItemDefinition {definition.Name}[{definition.GUID}] property {typeof(T)} does not have the matching flag set.";

        if (!Mode.HasFlag(Verification.Log))
        {
            return;
        }

        Main.Log(msg);

        var path = Path.Combine(DiagnosticsContext.DiagnosticsFolder, LogName);
        File.AppendAllLines(path,
        [
            $"{Environment.NewLine}",
                "------------------------------------------------------------------------------------", msg
        ]);
        File.AppendAllText(path, Environment.StackTrace);

        if (Mode.HasFlag(Verification.ReturnNull))
        {
            __result = null;
        }

        if (Mode.HasFlag(Verification.Throw))
        {
            throw new SolastaUnfinishedBusinessException(msg);
        }
    }

    [Flags]
    private enum Verification
    {
        None,
        ReturnNull = 1,
        Log = 2,
        Throw = 4
    }
}
#endif
