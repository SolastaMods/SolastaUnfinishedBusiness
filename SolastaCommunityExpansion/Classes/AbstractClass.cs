using SolastaModApi;

namespace SolastaCommunityExpansion.Classes
{
    internal abstract class AbstractClass
    {
        internal abstract CharacterClassDefinition GetClass();
        internal abstract void BuildEquipment(CharacterClassDefinitionBuilder classBuilder);
        internal abstract void BuildProficiencies(CharacterClassDefinitionBuilder classBuilder);
        internal abstract void BuildSubclasses(CharacterClassDefinitionBuilder classBuilder);
        internal abstract void BuildProgression(CharacterClassDefinitionBuilder classBuilder);
        internal abstract void BuildClassStats(CharacterClassDefinitionBuilder classBuilder);

    }
}
