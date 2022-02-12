using System.Diagnostics.CodeAnalysis;
using System.Text;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterInspection
{
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

            if (additionalBackstory == null || additionalBackstory == string.Empty)
            {
                return;
            }

            var builder = new StringBuilder();

            builder.Append(__result);
            builder.Append("\n\n<B>ADDITIONAL BACKSTORY</B>\n\n");
            builder.Append(additionalBackstory);

            __result = builder.ToString();
        }
    }
}
