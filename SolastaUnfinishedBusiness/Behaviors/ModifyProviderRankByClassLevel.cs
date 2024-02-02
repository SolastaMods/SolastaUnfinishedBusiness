using SolastaUnfinishedBusiness.Api.GameExtensions;

namespace SolastaUnfinishedBusiness.Behaviors;

internal interface IModifyProviderRank
{
    int GetRank(RulesetCharacter character);
}

internal class ModifyProviderRankByClassLevel(CharacterClassDefinition klass) : IModifyProviderRank
{
    public int GetRank(RulesetCharacter character)
    {
        return character.GetClassLevel(klass);
    }
}
