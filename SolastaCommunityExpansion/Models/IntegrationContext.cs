using UnityEngine;

namespace SolastaCommunityExpansion.Models;

public static class IntegrationContext
{
    public const string CLASS_MONK = "ClassMonk";
    public const string CLASS_TINKERER = "ClassTinkerer";
    public const string CLASS_WARDEN = "ClassWarden";
    public const string CLASS_WARLOCK = "ClassWarlock";
    public const string CLASS_WITCH = "ClassWitch";
    public const string SUBCLASS_CONARTIST = "RoguishConArtist";
    public const string SUBCLASS_SPELLSHIELD = "FighterSpellShield";

    // Sentinel blueprints to avoid a bunch of null check in code

    public static CharacterClassDefinition DummyClass { get; } =
        (CharacterClassDefinition)ScriptableObject.CreateInstance("CharacterClassDefinition");

    public static CharacterSubclassDefinition DummySubclass { get; } =
        (CharacterSubclassDefinition)ScriptableObject.CreateInstance("CharacterSubclassDefinition");

    public static CharacterClassDefinition MonkClass { get; private set; } = DummyClass;
    public static CharacterClassDefinition TinkererClass { get; private set; } = DummyClass;
    public static CharacterClassDefinition WardenClass { get; private set; } = DummyClass;
    public static CharacterClassDefinition WarlockClass { get; private set; } = DummyClass;
    public static CharacterClassDefinition WitchClass { get; private set; } = DummyClass;
    public static CharacterSubclassDefinition ConArtistSubclass { get; private set; } = DummySubclass;
    public static CharacterSubclassDefinition SpellShieldSubclass { get; private set; } = DummySubclass;

    public static void LateLoad()
    {
        var dbCharacterClassDefinition = DatabaseRepository.GetDatabase<CharacterClassDefinition>();
        var dbCharacterSubclassDefinition = DatabaseRepository.GetDatabase<CharacterSubclassDefinition>();

        dbCharacterClassDefinition.TryGetElement(CLASS_MONK, out var unofficialMonk);
        dbCharacterClassDefinition.TryGetElement(CLASS_TINKERER, out var unofficialTinkerer);
        dbCharacterClassDefinition.TryGetElement(CLASS_WARDEN, out var unofficialWarden);
        dbCharacterClassDefinition.TryGetElement(CLASS_WARLOCK, out var unofficialWarlock);
        dbCharacterClassDefinition.TryGetElement(CLASS_WITCH, out var unofficialWitch);
        dbCharacterSubclassDefinition.TryGetElement(SUBCLASS_CONARTIST, out var unofficialConArtist);
        dbCharacterSubclassDefinition.TryGetElement(SUBCLASS_SPELLSHIELD, out var unofficialSpellShield);

        // NOTE: don't use ?? here which bypasses Unity object lifetime check

        MonkClass = unofficialMonk ? unofficialMonk : DummyClass;
        TinkererClass = unofficialTinkerer ? unofficialTinkerer : DummyClass;
        WardenClass = unofficialWarden ? unofficialWarden : DummyClass;
        WitchClass = unofficialWitch ? unofficialWitch : DummyClass;
        WarlockClass = unofficialWarlock ? unofficialWarlock : DummyClass;
        ConArtistSubclass = unofficialConArtist ? unofficialConArtist : DummySubclass;
        SpellShieldSubclass = unofficialSpellShield ? unofficialSpellShield : DummySubclass;
    }
}
