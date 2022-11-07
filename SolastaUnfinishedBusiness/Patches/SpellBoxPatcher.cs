using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class SpellBoxPatcher
{
    [HarmonyPatch(typeof(SpellBox), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Bind_Patch
    {
        private static string extraTag;

        public static void Prefix(
            SpellBox __instance,
            ref string autoPreparedTag,
            ref string extraSpellTag,
            SpellBox.BindMode bindMode
        )
        {
            if (string.IsNullOrEmpty(extraSpellTag)) { return; }

            //PATCH: show actual class/subclass name in the multiclass tag during spell selection on levelup
            if (extraSpellTag.StartsWith(LevelUpContext.ExtraClassTag)
                || extraSpellTag.StartsWith(LevelUpContext.ExtraSubclassTag))
            {
                //store original extra tag and reset both - actual texts would be handled on Postfix for this case
                extraTag = extraSpellTag;
                autoPreparedTag = null;
                extraSpellTag = null;
                return;
            }

            //PATCH: if extra spell tag has no translation, but auto prepared translation for same tag exists - use that one.
            if (TranslatorContext.HasTranslation($"Screen/&{extraSpellTag}ExtraSpellTitle")
                || !TranslatorContext.HasTranslation($"Screen/&{extraSpellTag}SpellTitle"))
            {
                return;
            }

            autoPreparedTag = extraSpellTag;
            extraSpellTag = null;
        }

        public static void Postfix(SpellBox __instance)
        {
            //PATCH: show actual class/subclass name in the multiclass tag during spell selection on levelup
            if (string.IsNullOrEmpty(extraTag)) { return; }

            var parts = extraTag.Split('|');
            extraTag = null;

            if (parts.Length != 2) { return; }

            var type = parts[0];
            var name = parts[1];

            const string classFormat = "Screen/&ClassExtraSpellDescriptionFormat";
            const string subclassFormat = "Screen/&SubclassClassExtraSpellDescriptionFormat";

            if (type == LevelUpContext.ExtraClassTag &&
                DatabaseHelper.TryGetDefinition<CharacterClassDefinition>(name, out var classDef))
            {
                name = classDef.FormatTitle();
                __instance.autoPreparedTitle.Text = name;
                __instance.autoPreparedTooltip.Content = Gui.Format(classFormat, name);
            }
            else if (type == LevelUpContext.ExtraSubclassTag &&
                     DatabaseHelper.TryGetDefinition<CharacterSubclassDefinition>(name, out var subDef))
            {
                name = subDef.FormatTitle();
                __instance.autoPreparedTitle.Text = name;
                __instance.autoPreparedTooltip.Content = Gui.Format(subclassFormat, name);
            }
        }
    }
}
