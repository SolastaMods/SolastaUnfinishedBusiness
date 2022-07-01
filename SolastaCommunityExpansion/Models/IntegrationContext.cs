using SolastaCommunityExpansion.Builders;

namespace SolastaCommunityExpansion.Models;

internal static class IntegrationContext
{
    internal const string CLASS_MONK = "ClassMonk";

    internal const string CLASS_TINKERER = "ClassTinkerer";

    //internal const string CLASS_WARDEN = "ClassWarden";
    internal const string CLASS_WARLOCK = "ClassWarlock";

    internal const string CLASS_WITCH = "ClassWitch";

    //internal const string CLASS_MAGUS = "ClassMagus";
    private const string SUBCLASS_CONARTIST = "RoguishConArtist";
    private const string SUBCLASS_SPELLSHIELD = "FighterSpellShield";

    // Sentinel blueprints to avoid a bunch of null check in code

    private static CharacterClassDefinition DummyClass { get; } = CharacterClassDefinitionBuilder
        .Create("DummyClass", "d223ce4c8ee34c59a04e38cb5d668b0d")
        .SetGuiPresentationNoContent(true)
        .AddToDB();

    private static CharacterSubclassDefinition DummySubclass { get; } = CharacterSubclassDefinitionBuilder
        .Create("DummySubClass", "97425bff55404677a24fe6a4fe137aa2")
        .SetGuiPresentationNoContent(true)
        .AddToDB();

    internal static CharacterClassDefinition MonkClass { get; private set; } = DummyClass;

    internal static CharacterClassDefinition TinkererClass { get; private set; } = DummyClass;

    //internal static CharacterClassDefinition WardenClass { get; private set; } = DummyClass;
    internal static CharacterClassDefinition WarlockClass { get; private set; } = DummyClass;

    internal static CharacterClassDefinition WitchClass { get; private set; } = DummyClass;

    //internal static CharacterClassDefinition MagusClass { get; private set; } = DummyClass;
    internal static CharacterSubclassDefinition ConArtistSubclass { get; private set; } = DummySubclass;
    internal static CharacterSubclassDefinition SpellShieldSubclass { get; private set; } = DummySubclass;

    internal static void LateLoad()
    {
        var dbCharacterClassDefinition = DatabaseRepository.GetDatabase<CharacterClassDefinition>();
        var dbCharacterSubclassDefinition = DatabaseRepository.GetDatabase<CharacterSubclassDefinition>();

        dbCharacterClassDefinition.TryGetElement(CLASS_MONK, out var unofficialMonk);
        dbCharacterClassDefinition.TryGetElement(CLASS_TINKERER, out var unofficialTinkerer);
        //dbCharacterClassDefinition.TryGetElement(CLASS_WARDEN, out var unofficialWarden);
        dbCharacterClassDefinition.TryGetElement(CLASS_WARLOCK, out var unofficialWarlock);
        dbCharacterClassDefinition.TryGetElement(CLASS_WITCH, out var unofficialWitch);
        //dbCharacterClassDefinition.TryGetElement(CLASS_MAGUS, out var unofficialMagus);
        dbCharacterSubclassDefinition.TryGetElement(SUBCLASS_CONARTIST, out var unofficialConArtist);
        dbCharacterSubclassDefinition.TryGetElement(SUBCLASS_SPELLSHIELD, out var unofficialSpellShield);

        // NOTE: don't use ?? here which bypasses Unity object lifetime check

        MonkClass = unofficialMonk ? unofficialMonk : DummyClass;
        TinkererClass = unofficialTinkerer ? unofficialTinkerer : DummyClass;
        //WardenClass = unofficialWarden ? unofficialWarden : DummyClass;
        WitchClass = unofficialWitch ? unofficialWitch : DummyClass;
        WarlockClass = unofficialWarlock ? unofficialWarlock : DummyClass;
        //MagusClass = unofficialMagus ? unofficialMagus : DummyClass;
        ConArtistSubclass = unofficialConArtist ? unofficialConArtist : DummySubclass;
        SpellShieldSubclass = unofficialSpellShield ? unofficialSpellShield : DummySubclass;
    }
}
