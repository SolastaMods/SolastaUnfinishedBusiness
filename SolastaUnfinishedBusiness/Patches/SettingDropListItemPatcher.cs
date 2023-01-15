using System;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class SettingDropListItemPatcher
{
    [HarmonyPatch(typeof(SettingDropListItem), "Bind")]
    internal static class Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(SettingDropListItem __instance)
        {
            //PATCH: add additional items to language selection drop down for each unofficial detected language
            var settingTypeDropListAttribute = __instance.settingTypeDropListAttribute;

            if (settingTypeDropListAttribute.Name != "TextLanguage")
            {
                return;
            }

            var top = settingTypeDropListAttribute.Items.Length;
            var items = new string[settingTypeDropListAttribute.Items.Length + TranslatorContext.Languages.Count];

            Array.Copy(settingTypeDropListAttribute.Items, items, top);
            settingTypeDropListAttribute.Items = items;

            foreach (var language in TranslatorContext.Languages)
            {
                items[top++] = language.Code;

                __instance.dropList.options.Add(new GuiDropdown.OptionDataAdvanced
                {
                    text = language.Text, TooltipContent = language.Text
                });
            }
        }
    }
}
