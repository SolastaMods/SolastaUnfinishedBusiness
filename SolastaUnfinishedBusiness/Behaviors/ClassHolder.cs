using SolastaUnfinishedBusiness.Classes;
using clazz = SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaUnfinishedBusiness.Behaviors;

/// <summary>
///     Used for IAdditionalDamageProvider features to allow its damage to scale with class level (using
///     DiceByRankTable), even if it isn't directly added to the class (via AddFeaturesAtLevel).
///     Used by Magic Initiate feats to specify class so that Potent Spellcaster feats can recognize proper class.
/// </summary>
public record ClassHolder(CharacterClassDefinition Class)
{
    public static readonly ClassHolder Barbarian = new(clazz.Barbarian);
    // public static readonly ClassHolder Bard = new(clazz.Bard);
    // public static readonly ClassHolder Cleric = new(clazz.Cleric);
    // public static readonly ClassHolder Druid = new(clazz.Druid);
    public static readonly ClassHolder Inventor = new(InventorClass.Class);
    public static readonly ClassHolder Rogue = new(clazz.Rogue);
    // public static readonly ClassHolder Sorcerer = new(clazz.Sorcerer);
    public static readonly ClassHolder Warlock = new(clazz.Warlock);
    // public static readonly ClassHolder Wizard = new(clazz.Wizard);
    public readonly CharacterClassDefinition Class = Class;
}
