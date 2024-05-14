using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class SpellBoxPatcher
{
    [HarmonyPatch(typeof(SpellBox), nameof(SpellBox.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        private static string _extraTag;

        [UsedImplicitly]
        public static void Prefix(
            ref bool autoPrepared,
            ref bool extraSpell,
            ref string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                return;
            }

            //PATCH: show actual class/subclass name in the multiclass tag during spell selection on level up
            if (tag.StartsWith(LevelUpContext.ExtraClassTag)
                || tag.StartsWith(LevelUpContext.ExtraSubclassTag))
            {
                //store original extra tag and reset both - actual texts would be handled on Postfix for this case
                _extraTag = tag;
                autoPrepared = false;
                extraSpell = false;
                return;
            }

            //PATCH: if extra spell tag has no translation, but auto prepared translation for same tag exists - use that one.
            if (TranslatorContext.HasTranslation($"Screen/&{tag}ExtraSpellTitle")
                || !TranslatorContext.HasTranslation($"Screen/&{tag}SpellTitle"))
            {
                return;
            }

            autoPrepared = true;
            extraSpell = false;
        }

        [UsedImplicitly]
        public static void Postfix(SpellBox __instance)
        {
            //PATCH: show actual class/subclass name in the multiclass tag during spell selection on level up
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

            const string CLASS_FORMAT = "Screen/&ClassExtraSpellDescriptionFormat";
            const string SUBCLASS_FORMAT = "Screen/&SubclassClassExtraSpellDescriptionFormat";

            //__instance.autoPreparedTitle.Text = "Screen/&MulticlassExtraSpellTitle";

            switch (type)
            {
                case LevelUpContext.ExtraClassTag when
                    DatabaseHelper.TryGetDefinition<CharacterClassDefinition>(name, out var classDef):
                    name = classDef.FormatTitle();
                    __instance.autoPreparedTooltip.Content = Gui.Format(CLASS_FORMAT, name);
                    break;

                case LevelUpContext.ExtraSubclassTag when
                    DatabaseHelper.TryGetDefinition<CharacterSubclassDefinition>(name, out var subDef):
                    name = subDef.FormatTitle();
                    __instance.autoPreparedTooltip.Content = Gui.Format(SUBCLASS_FORMAT, name);
                    break;
            }
        }
    }
}
