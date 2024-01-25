using System.Diagnostics.CodeAnalysis;
using System.Text;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class FeatureDefinitionPointPoolPatcher
{
    [HarmonyPatch(typeof(FeatureDefinitionPointPool), nameof(FeatureDefinitionPointPool.FormatDescription))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class FormatDescription_Patch
    {
        [UsedImplicitly]
        public static bool Prefix([NotNull] FeatureDefinitionPointPool __instance, ref string __result)
        {
            var choices = __instance.RestrictedChoices;

            if (__instance.poolType != HeroDefinitions.PointsPoolType.Tool || choices == null || choices.Count == 0)
            {
                return true;
            }

            var builder = new StringBuilder();
            var separator = Gui.ListSeparator();

            foreach (var restrictedChoice in choices)
            {
                if (builder.Length > 0)
                {
                    builder.Append(separator);
                }

                var tool = DatabaseHelper.GetDefinition<ToolTypeDefinition>(restrictedChoice);

                builder.Append(Gui.Localize(tool.GuiPresentation.Title));
            }

            __result = Gui.Format(__instance.GuiPresentation.Description, __instance.PoolAmount.ToString(),
                builder.ToString());

            return false;
        }
    }
}
