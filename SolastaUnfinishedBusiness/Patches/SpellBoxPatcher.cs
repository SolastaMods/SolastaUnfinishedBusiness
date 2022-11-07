using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class SpellBoxPatcher
{
    [HarmonyPatch(typeof(SpellBox), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Bind_Patch
    {
        public static void Prefix(
            SpellBox __instance,
            bool autoPrepared,
            ref string autoPreparedTag,
            bool extraSpell,
            ref string extraSpellTag,
            SpellBox.BindMode bindMode
        )
        {
            //PATCH: if extra spell tag has no translation, but auto prepared translation for same tag exists - use that one.
            if (string.IsNullOrEmpty(extraSpellTag)
                || TranslatorContext.HasTranslation($"Screen/&{extraSpellTag}ExtraSpellTitle")
                || !TranslatorContext.HasTranslation($"Screen/&{extraSpellTag}SpellTitle"))
            {
                return;
            }

            autoPreparedTag = extraSpellTag;
            extraSpellTag = null;
        }
    }
}
