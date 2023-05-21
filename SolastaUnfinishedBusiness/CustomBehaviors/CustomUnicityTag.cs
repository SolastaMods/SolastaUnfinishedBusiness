namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal interface ICustomUnicityTag
{
    string UnicityTag { get; }
}

internal class CustomUnicityTag : ICustomUnicityTag
{
    public CustomUnicityTag(string tag)
    {
        UnicityTag = tag;
    }

    public string UnicityTag { get; }
}
