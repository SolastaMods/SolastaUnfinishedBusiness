using System.Diagnostics.CodeAnalysis;
using System.Text;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterInspection;

[HarmonyPatch(typeof(GuiCharacter), "MainClassDefinition", MethodType.Getter)]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GuiCharacter_MainClassDefinition_Getter
{
    internal static void Postfix(ref CharacterClassDefinition __result)
    {
        if (!Main.Settings.EnableEnhancedCharacterInspection)
        {
            return;
        }

        // NOTE: don't use SelectedClass??. which bypasses Unity object lifetime check
        var selectedClass = InspectionPanelContext.SelectedClass;

        if (selectedClass)
        {
            __result = selectedClass;
        }
    }
}

[HarmonyPatch(typeof(GuiCharacter), "LevelAndClassAndSubclass", MethodType.Getter)]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GuiCharacter_LevelAndClassAndSubclass_Getter
{
    internal static void Postfix(GuiCharacter __instance, ref string __result)
    {
        __result = MulticlassGameUiContext.GetAllClassesLabel(__instance, ' ') ?? __result;
    }
}

[HarmonyPatch(typeof(GuiCharacter), "ClassAndLevel", MethodType.Getter)]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GuiCharacter_ClassAndLevel_Getter
{
    internal static void Postfix(GuiCharacter __instance, ref string __result)
    {
        __result = MulticlassGameUiContext.GetAllClassesLabel(__instance, ' ') ?? __result;
    }
}

// shouldn't be protected as we touch translation terms and this will abort otherwise
[HarmonyPatch(typeof(GuiCharacter), "LevelAndExperienceTooltip", MethodType.Getter)]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GuiCharacter_LevelAndExperienceTooltip_Getter
{
    internal static void Postfix(GuiCharacter __instance, ref string __result)
    {
        __result = MulticlassGameUiContext.GetLevelAndExperienceTooltip(__instance) ?? __result;
    }
}

// Enable additional background display on inspection panel
[HarmonyPatch(typeof(GuiCharacter), "BackgroundDescription", MethodType.Getter)]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GuiCharacter_BackgroundDescription_Getter
{
    internal static void Postfix(GuiCharacter __instance, ref string __result)
    {
        if (!Main.Settings.EnableAdditionalBackstoryDisplay)
        {
            return;
        }

        var hero = __instance.RulesetCharacterHero;

        if (hero == null)
        {
            return;
        }

        var additionalBackstory = hero.AdditionalBackstory;

        if (string.IsNullOrEmpty(additionalBackstory))
        {
            return;
        }

        var builder = new StringBuilder();

        builder.Append(__result);
        builder.Append("\n\n<B>");
        builder.Append(Gui.Format("Stage/&IdentityAdditionalBackstoryHeader"));
        builder.Append("</B>\n\n");
        builder.Append(additionalBackstory);

        __result = builder.ToString();
    }
}
