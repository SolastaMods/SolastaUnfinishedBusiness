using SolastaUnfinishedBusiness.Api.Helpers;

namespace SolastaUnfinishedBusiness.Api.Extensions;

public static class RulesetSpellRepertoireExtensions
{
    public static RulesetCharacterHero GetCasterHero(this RulesetSpellRepertoire repertoire)
    {
        return EffectHelpers.GetCharacterByGuid(repertoire.CharacterInventory?.BearerGuid ?? 0) as RulesetCharacterHero;
    }
}
