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
        private static string _extraTag;

        public static void Prefix(
            SpellBox __instance,
            ref string autoPreparedTag,
            ref string extraSpellTag,
            SpellBox.BindMode bindMode
        )
        {
            if (string.IsNullOrEmpty(extraSpellTag))
            {
                return;
            }

            //PATCH: show actual class/subclass name in the multiclass tag during spell selection on levelup
            if (extraSpellTag.StartsWith(LevelUpContext.ExtraClassTag)
                || extraSpellTag.StartsWith(LevelUpContext.ExtraSubclassTag))
            {
                //store original extra tag and reset both - actual texts would be handled on Postfix for this case
                _extraTag = extraSpellTag;
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
            if (string.IsNullOrEmpty(_extraTag))
            {
                return;
            }

            var parts = _extraTag.Split('|');
            _extraTag = null;

            if (parts.Length != 2)
            {
                return;
            }

            var type = parts[0];
            var name = parts[1];

            const string MULTICLASS = "Screen/&MulticlassExtraSpellTitle";
            const string CLASS_FORMAT = "Screen/&ClassExtraSpellDescriptionFormat";
            const string SUBCLASS_FORMAT = "Screen/&SubclassClassExtraSpellDescriptionFormat";

            if (type == LevelUpContext.ExtraClassTag &&
                DatabaseHelper.TryGetDefinition<CharacterClassDefinition>(name, out var classDef))
            {
                name = classDef.FormatTitle();
                __instance.autoPreparedTitle.Text = MULTICLASS;
                __instance.autoPreparedTooltip.Content = Gui.Format(CLASS_FORMAT, name);
            }
            else if (type == LevelUpContext.ExtraSubclassTag &&
                     DatabaseHelper.TryGetDefinition<CharacterSubclassDefinition>(name, out var subDef))
            {
                name = subDef.FormatTitle();
                __instance.autoPreparedTitle.Text = MULTICLASS;
                __instance.autoPreparedTooltip.Content = Gui.Format(SUBCLASS_FORMAT, name);
            }
        }
    }
}
