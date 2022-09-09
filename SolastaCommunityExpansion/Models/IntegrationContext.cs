using SolastaCommunityExpansion.Builders;

namespace SolastaCommunityExpansion.Models;

internal static class IntegrationContext
{
    private const string ClassTinkerer = "ClassTinkerer";
    private const string SubclassConartist = "RoguishConArtist";
    private const string SubclassSpellshield = "FighterSpellShield";
    private const string SubclassPathOfTheRageMage = "BarbarianPathOfTheRageMage";

    // Sentinel blueprints to avoid a bunch of null check in code

    private static CharacterClassDefinition ClassDummy { get; } = CharacterClassDefinitionBuilder
        .Create("ClassDummy", "d223ce4c8ee34c59a04e38cb5d668b0d")
        .SetGuiPresentationNoContent(true)
        .AddToDB();

    private static CharacterSubclassDefinition DummySubclass { get; } = CharacterSubclassDefinitionBuilder
        .Create("DummySubClass", "97425bff55404677a24fe6a4fe137aa2")
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
