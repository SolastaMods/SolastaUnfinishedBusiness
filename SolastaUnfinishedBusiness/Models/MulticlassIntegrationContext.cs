using SolastaUnfinishedBusiness.Builders;

namespace SolastaUnfinishedBusiness.Models;

internal static class MulticlassIntegrationContext
{
    private const string ClassArtisan = "Artisan";
    private const string SubclassConartist = "RoguishConArtist";
    private const string SubclassSpellshield = "MartialSpellShield";
    private const string SubclassPathOfTheRageMage = "PathOfTheRageMage";

    // Sentinel blueprints to avoid a bunch of null check in code

    private static CharacterClassDefinition ClassDummy { get; } = CharacterClassDefinitionBuilder
        .Create("ClassDummy")
        .SetGuiPresentationNoContent(true)
        .AddToDB();

    private static CharacterSubclassDefinition DummySubclass { get; } = CharacterSubclassDefinitionBuilder
        .Create("SubClassDummy")
        .SetGuiPresentationNoContent(true)
        .AddToDB();

    internal static CharacterClassDefinition ArtisanClass { get; private set; } = ClassDummy;
    internal static CharacterSubclassDefinition ConArtistSubclass { get; private set; } = DummySubclass;
    internal static CharacterSubclassDefinition SpellShieldSubclass { get; private set; } = DummySubclass;
    internal static CharacterSubclassDefinition PathOfTheRageMageSubclass { get; private set; } = DummySubclass;

    internal static void LateLoad()
    {
        var dbCharacterClassDefinition = DatabaseRepository.GetDatabase<CharacterClassDefinition>();
        var dbCharacterSubclassDefinition = DatabaseRepository.GetDatabase<CharacterSubclassDefinition>();

        dbCharacterClassDefinition.TryGetElement(ClassArtisan, out var unofficialArtisan);
        dbCharacterSubclassDefinition.TryGetElement(SubclassConartist, out var unofficialConArtist);
        dbCharacterSubclassDefinition.TryGetElement(SubclassSpellshield, out var unofficialSpellShield);
        dbCharacterSubclassDefinition.TryGetElement(SubclassPathOfTheRageMage, out var unofficialPathOfTheRageMage);

        // NOTE: don't use ?? here which bypasses Unity object lifetime check

        ArtisanClass = unofficialArtisan ? unofficialArtisan : ClassDummy;
        ConArtistSubclass = unofficialConArtist ? unofficialConArtist : DummySubclass;
        SpellShieldSubclass = unofficialSpellShield ? unofficialSpellShield : DummySubclass;
        PathOfTheRageMageSubclass = unofficialPathOfTheRageMage ? unofficialPathOfTheRageMage : DummySubclass;
    }
}
