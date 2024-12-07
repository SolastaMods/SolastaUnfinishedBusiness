using SolastaUnfinishedBusiness.ModelLoader.Data.DataStore;
using SolastaUnfinishedBusiness.ModelLoader.TypeParsers.Interfaces;

namespace SolastaUnfinishedBusiness.ModelLoader.TypeParsers;

public class GroupParser(IGroupDataStore groupDataStore) : TypeParserBase, IGroupParser
{
    protected override string Keyword => "g";

    public override void Parse(string line)
    {
        groupDataStore.PushGroup(line);
    }
}
