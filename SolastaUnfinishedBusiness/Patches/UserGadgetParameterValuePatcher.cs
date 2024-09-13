using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

//PATCH: support variable placeholders on custom content (EnableVariablePlaceholdersOnTexts)
[UsedImplicitly]
public static class UserGadgetParameterValuePatcher
{
    private static readonly Regex MatchPlaceholders = new(@"\{\w+?\}", RegexOptions.Compiled);

    private static string ReplacePlaceholders(string userContent)
    {
        return string.IsNullOrEmpty(userContent) ? userContent : MatchPlaceholders.Replace(userContent, Replace);

        static string Replace(Match match)
        {
            var variableName = match.Value.Trim('{', '}');
            var variableService = ServiceRepository.GetService<IGameVariableService>();

            if (variableService?.TryFindVariable(variableName, out var variable) == true)
            {
                return variable.Type switch
                {
                    GameVariableDefinitions.Type.Bool => variable.BoolValue.ToString(),
                    GameVariableDefinitions.Type.Int => variable.IntValue.ToString(),
                    _ => variable.StringValue
                };
            }

            return match.Value;
        }
    }
    
    [HarmonyPatch(typeof(UserGadgetParameterValue), nameof(UserGadgetParameterValue.StringValue), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class StringValue_Getter_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(UserGadgetParameterValue __instance, ref string __result)
        {
            if (!Main.Settings.EnableVariablePlaceholdersOnTexts) { return true; }

            if (!Gui.Game) { return true; } // this if statement should be standalone to avoid unity life check issues

            var userContent = __instance.stringValue;

            __result = ReplacePlaceholders(userContent);

            return false;
        }
    }

    [HarmonyPatch(typeof(UserContentDefinitions), nameof(UserContentDefinitions.CensorUserContent))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CensorUserContent_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(string userContent, ref string __result)
        {
            if (!Main.Settings.EnableVariablePlaceholdersOnTexts) { return true; }

            if (!Gui.Game) { return true; } // this if statement should be standalone to avoid unity life check issues

            __result = string.IsNullOrEmpty(userContent) ? userContent : ReplacePlaceholders(userContent);

            return false;
        }
    }
}
