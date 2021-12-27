using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.CustomFeatureDefinitions
{
    public class NegativeFeatureDefinition : FeatureDefinition
    {
        public int ClassLevel;
        public FeatureDefinition FeatureToRemove;
        public CharacterClassDefinition CharacterClass;
        public CharacterSubclassDefinition CharacterSubclass;
        public string Tag => CharacterSubclass == null ? AttributeDefinitions.GetClassTag(CharacterClass, ClassLevel) : AttributeDefinitions.GetSubclassTag(CharacterClass, ClassLevel, CharacterSubclass);
    }

    public sealed class NegativeFeatureBuilder : BaseDefinitionBuilder<NegativeFeatureDefinition>
    {
        public NegativeFeatureBuilder(string name, string guid, FeatureDefinition featureToRemove, int classLevel, CharacterClassDefinition characterClass, CharacterSubclassDefinition characterSubclass = null)
            : base(name, guid)
        {
            Definition.ClassLevel = classLevel;
            Definition.FeatureToRemove = featureToRemove;
            Definition.CharacterClass = characterClass;
            Definition.CharacterSubclass = characterSubclass;
            Definition.GuiPresentation.SetHidden(true);
        }
    }
}
