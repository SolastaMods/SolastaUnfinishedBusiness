using SolastaUnfinishedBusiness.Api.Helpers;

namespace SolastaUnfinishedBusiness.Api.Extensions;

public static class RulesetSpellRepertoireExtensions
{
    public static RulesetCharacterHero GetCasterHero(this RulesetSpellRepertoire repertoire)
    {
        return EffectHelpers.GetCharacterByGuid(repertoire.CharacterInventory?.BearerGuid ?? 0) as RulesetCharacterHero;
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
