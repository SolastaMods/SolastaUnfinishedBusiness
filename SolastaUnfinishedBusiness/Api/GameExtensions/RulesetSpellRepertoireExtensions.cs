using SolastaUnfinishedBusiness.Api.Helpers;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

public static class RulesetSpellRepertoireExtensions
{
    public static RulesetCharacterHero GetCasterHero(this RulesetSpellRepertoire repertoire)
    {
        // don't use GetOriginalHero() here as it breaks vanilla boot up
        return EffectHelpers.GetCharacterByGuid(repertoire?.CharacterInventory?.BearerGuid ?? 0) as RulesetCharacterHero
               ?? Global.InspectedHero;
    }

    public static bool AtLeastOneSpellSlotAvailable(this RulesetSpellRepertoire repertoire)
    {
        for (var spellLevel = 1;
             spellLevel <= repertoire.MaxSpellLevelOfSpellCastingLevel;
             spellLevel++)
        {
            repertoire.GetSlotsNumber(spellLevel, out var remaining, out _);

            if (remaining <= 0)
            {
                continue;
            }

            return true;
        }

        return false;
    }
}
