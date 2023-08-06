namespace SolastaUnfinishedBusiness.Subclasses;

internal abstract class AbstractSubclass
{
    internal abstract CharacterSubclassDefinition Subclass { get; }
    internal abstract CharacterClassDefinition Klass { get; }
    internal abstract FeatureDefinitionSubclassChoice SubclassChoice { get; }
    internal abstract DeityDefinition DeityDefinition { get; }
}
