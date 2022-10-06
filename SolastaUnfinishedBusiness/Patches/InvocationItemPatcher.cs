using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public class InvocationItemPatcher
{
    [HarmonyPatch(typeof(InvocationItem), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Bind_Patch
    {
        public static void Postfix(InvocationItem __instance)
        {
            //PATCH: makes custom invocations have custom tooltips
            if (__instance.GuiInvocationDefinition.InvocationDefinition is CustomInvocationDefinition custom)
            {
                var tooltip = __instance.Tooltip;
                var hero = tooltip.Context as RulesetCharacter ?? Global.CurrentCharacter;
                CustomFeaturesContext.ValidatePrerequisites(hero, custom, custom.Validators, out var requirements);

                var gui = new GuiPresentationBuilder(custom.GuiPresentation).Build();
                var item = custom.Item;
                var dataProvider = item == null
                    ? new CustomTooltipProvider(custom, gui)
                    : new CustomItemTooltipProvider(custom, gui, item);

                dataProvider.SetPrerequisites(requirements);
                tooltip.TooltipClass = dataProvider.TooltipClass;
                tooltip.Content = custom.GuiPresentation.Description;
                tooltip.Context = hero;
                tooltip.DataProvider = dataProvider;
            }
        }
    }
}
