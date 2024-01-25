using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.BehaviorsSpecific;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Definitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class InvocationItemPatcher
{
    [HarmonyPatch(typeof(InvocationItem), nameof(InvocationItem.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(InvocationItem __instance)
        {
            //PATCH: makes custom invocations have custom tooltips
            if (__instance.GuiInvocationDefinition
                    .InvocationDefinition is not InvocationDefinitionCustom custom)
            {
                return;
            }

            var tooltip = __instance.Tooltip;
            var hero = tooltip.Context as RulesetCharacter ?? Global.CurrentCharacter;

            if (hero == null)
            {
                return;
            }

            PowerBundle.ValidatePrerequisites(hero, custom, custom.Validators, out var requirements);

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
