using SolastaCommunityExpansion.Builders;
using SolastaModApi;

namespace SolastaCommunityExpansion.Classes.Monk.Subclasses
{
    public static class ZenArcher
    {
        public static CharacterSubclassDefinition Build()
        {
            return CharacterSubclassDefinitionBuilder
                .Create("ClassMonkTraditionZenArcher", DefinitionBuilder.CENamespaceGuid)
                .SetOrUpdateGuiPresentation(Category.Subclass,
                    DatabaseHelper.CharacterSubclassDefinitions.RangerMarksman.GuiPresentation.SpriteReference)
                .AddFeaturesAtLevel(3, BuildLevel03Features())
                .AddFeaturesAtLevel(6, BuildLevel06Features())
                .AddFeaturesAtLevel(11, BuildLevel11Features())
                .AddFeaturesAtLevel(17, BuildLevel17Features())
                .AddToDB();
        }

        private static FeatureDefinition[] BuildLevel03Features()
        {
            return System.Array.Empty<FeatureDefinition>();
        }
        
        private static FeatureDefinition[] BuildLevel06Features()
        {
            return System.Array.Empty<FeatureDefinition>();
        }
        
        private static FeatureDefinition[] BuildLevel11Features()
        {
            return System.Array.Empty<FeatureDefinition>();
        }
        
        private static FeatureDefinition[] BuildLevel17Features()
        {
            return System.Array.Empty<FeatureDefinition>();
        }
    }
}
