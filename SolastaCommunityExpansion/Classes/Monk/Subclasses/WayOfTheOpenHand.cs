using SolastaCommunityExpansion.Builders;
using static SolastaModApi.DatabaseHelper;

namespace SolastaCommunityExpansion.Classes.Monk.Subclasses;

public static class WayOfTheOpenHand
{
    public static CharacterSubclassDefinition Build()
    {
        return CharacterSubclassDefinitionBuilder
            .Create("ClassMonkTraditionWayOfTheOpenHand", DefinitionBuilder.CENamespaceGuid)
            .SetOrUpdateGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.DomainLife.GuiPresentation.SpriteReference)
            .AddToDB();
    }
}