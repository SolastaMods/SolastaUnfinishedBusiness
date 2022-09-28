namespace SolastaUnfinishedBusiness.CustomBehaviors;

public sealed class ExclusiveAcBonus
{
    //AC formula is `Value + DEX`
    public const string TagLikeArmor = "AC_LIKE_ARMOR";

    //AC formula is `Value`
    public const string TagNaturalArmor = "AC_NATURAL_ARMOR";

    //AC Formula is `10 + DEX + Value`
    public const string TagUnarmoredDefense = "AC_UNARMORED_DEFENSE";
    public static readonly ExclusiveAcBonus MarkLikeArmor = new(TagLikeArmor);
    public static readonly ExclusiveAcBonus MarkNaturalArmor = new(TagNaturalArmor);
    public static readonly ExclusiveAcBonus MarkUnarmoredDefense = new(TagUnarmoredDefense);

    public readonly string Tag;

    private ExclusiveAcBonus(string tag)
    {
        Tag = tag;
    }
}
