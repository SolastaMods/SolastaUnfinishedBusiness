using SolastaUnfinishedBusiness.ModelLoader.Common;
using SolastaUnfinishedBusiness.ModelLoader.TypeParsers.Interfaces;

namespace SolastaUnfinishedBusiness.ModelLoader.TypeParsers;

public abstract class TypeParserBase : ITypeParser
{
    protected abstract string Keyword { get; }

    public bool CanParse(string keyword)
    {
        return keyword.EqualsOrdinalIgnoreCase(Keyword);
    }

    public abstract void Parse(string line);
}
