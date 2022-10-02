namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal sealed class ExclusiveAcBonus
{
    //AC formula is `Value + DEX`
    internal const string TagLikeArmor = "AC_LIKE_ARMOR";

    //AC formula is `Value`
    internal const string TagNaturalArmor = "AC_NATURAL_ARMOR";

    //AC Formula is `10 + DEX + Value`
    internal const string TagUnarmoredDefense = "AC_UNARMORED_DEFENSE";
    internal static readonly ExclusiveAcBonus MarkLikeArmor = new(TagLikeArmor);
    internal static readonly ExclusiveAcBonus MarkNaturalArmor = new(TagNaturalArmor);
    internal static readonly ExclusiveAcBonus MarkUnarmoredDefense = new(TagUnarmoredDefense);

    internal readonly string Tag;

    private ExclusiveAcBonus(string tag)
    {
        Tag = tag;
    }
}
