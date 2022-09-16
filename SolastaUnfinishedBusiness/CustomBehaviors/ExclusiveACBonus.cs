namespace SolastaUnfinishedBusiness.CustomBehaviors;

public sealed class ExclusiveACBonus
{
    //AC formula is `Value + DEX`
    public const string TagLikeArmor = "AC_LIKE_ARMOR";

    //AC formula is `Value`
    public const string TagNaturalArmor = "AC_NATURAL_ARMOR";

    //AC Formula is `10 + DEX + Value`
    public const string TagUnarmoredDefense = "AC_UNARMORED_DEFENSE";
    public static readonly ExclusiveACBonus MarkLikeArmor = new(TagLikeArmor);
    public static readonly ExclusiveACBonus MarkNaturalArmor = new(TagNaturalArmor);
    public static readonly ExclusiveACBonus MarkUnarmoredDefense = new(TagUnarmoredDefense);

    public readonly string tag;

    private ExclusiveACBonus(string tag)
    {
        this.tag = tag;
    }
}
