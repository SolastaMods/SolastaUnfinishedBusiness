using SolastaUnfinishedBusiness.Classes;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaUnfinishedBusiness.Interfaces;

/// <summary>
///     Implement on an IAdditionalDamageProvider feature to allow its damage to scale with class level (using
///     DiceByRankTable), even if it isn't directly added to the class (via AddFeaturesAtLevel).
/// </summary>
public interface IModifyAdditionalDamageClassLevel
{
    public CharacterClassDefinition Class { get; }
}

public sealed class ModifyAdditionalDamageClassLevelBarbarian : IModifyAdditionalDamageClassLevel
{
    private ModifyAdditionalDamageClassLevelBarbarian()
    {
    }

    public static IModifyAdditionalDamageClassLevel Instance { get; } = new ModifyAdditionalDamageClassLevelBarbarian();

    public CharacterClassDefinition Class => Barbarian;
}

public sealed class ModifyAdditionalDamageClassLevelInventor : IModifyAdditionalDamageClassLevel
{
    private ModifyAdditionalDamageClassLevelInventor()
    {
    }

    public static IModifyAdditionalDamageClassLevel Instance { get; } = new ModifyAdditionalDamageClassLevelInventor();

    public CharacterClassDefinition Class => InventorClass.Class;
}

public sealed class ModifyAdditionalDamageClassLevelRogue : IModifyAdditionalDamageClassLevel
{
    private ModifyAdditionalDamageClassLevelRogue()
    {
    }

    public static IModifyAdditionalDamageClassLevel Instance { get; } = new ModifyAdditionalDamageClassLevelRogue();

    public CharacterClassDefinition Class => Rogue;
}

public sealed class ModifyAdditionalDamageClassLevelWarlock : IModifyAdditionalDamageClassLevel
{
    private ModifyAdditionalDamageClassLevelWarlock()
    {
    }

    public static IModifyAdditionalDamageClassLevel Instance { get; } = new ModifyAdditionalDamageClassLevelWarlock();

    public CharacterClassDefinition Class => Warlock;
}
