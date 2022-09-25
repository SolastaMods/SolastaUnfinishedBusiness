using SolastaUnfinishedBusiness.Builders;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Classes.Inventor.Subclasses;

public static class InnovationAlchemy
{
    public static CharacterSubclassDefinition Build()
    {
        return CharacterSubclassDefinitionBuilder
            .Create("InventorInnovationAlchemy")
            .SetGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.DomainElementalFire)
            .AddToDB();
    }
}