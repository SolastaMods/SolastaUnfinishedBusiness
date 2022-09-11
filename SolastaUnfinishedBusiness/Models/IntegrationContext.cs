using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;

namespace SolastaUnfinishedBusiness.Models;

internal static class IntegrationContext
{
    private const string ClassTinkerer = "Tinkerer";
    private const string SubclassConartist = "RoguishConArtist";
    private const string SubclassSpellshield = "MartialSpellShield";
    private const string SubclassPathOfTheRageMage = "PathOfTheRageMage";

    // Sentinel blueprints to avoid a bunch of null check in code

    private static CharacterClassDefinition ClassDummy { get; } = CharacterClassDefinitionBuilder
        .Create("ClassDummy", DefinitionBuilder.CENamespaceGuid)
        .SetGuiPresentationNoContent(true)
        .AddToDB();

    private static CharacterSubclassDefinition DummySubclass { get; } = CharacterSubclassDefinitionBuilder
        .Create("SubClassDummy", DefinitionBuilder.CENamespaceGuid)
        .SetGuiPresentationNoContent(true)
        .AddToDB();

    internal static CharacterClassDefinition TinkererClass { get; private set; } = ClassDummy;
    internal static CharacterSubclassDefinition ConArtistSubclass { get; private set; } = DummySubclass;
    internal static CharacterSubclassDefinition SpellShieldSubclass { get; private set; } = DummySubclass;
    internal static CharacterSubclassDefinition PathOfTheRageMageSubclass { get; private set; } = DummySubclass;

    internal static void LateLoad()
    {
        var dbCharacterClassDefinition = DatabaseRepository.GetDatabase<CharacterClassDefinition>();
        var dbCharacterSubclassDefinition = DatabaseRepository.GetDatabase<CharacterSubclassDefinition>();

        dbCharacterClassDefinition.TryGetElement(ClassTinkerer, out var unofficialTinkerer);
        dbCharacterSubclassDefinition.TryGetElement(SubclassConartist, out var unofficialConArtist);
        dbCharacterSubclassDefinition.TryGetElement(SubclassSpellshield, out var unofficialSpellShield);
        dbCharacterSubclassDefinition.TryGetElement(SubclassPathOfTheRageMage, out var unofficialPathOfTheRageMage);

        // NOTE: don't use ?? here which bypasses Unity object lifetime check

        TinkererClass = unofficialTinkerer ? unofficialTinkerer : ClassDummy;
        ConArtistSubclass = unofficialConArtist ? unofficialConArtist : DummySubclass;
        SpellShieldSubclass = unofficialSpellShield ? unofficialSpellShield : DummySubclass;
        PathOfTheRageMageSubclass = unofficialPathOfTheRageMage ? unofficialPathOfTheRageMage : DummySubclass;
    }
}
