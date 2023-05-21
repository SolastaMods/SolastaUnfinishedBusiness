namespace SolastaUnfinishedBusiness.CustomBehaviors;

interface ICustomUnicityTag
{
    string UnicityTag { get; }
}

internal class CustomUnicityTag : ICustomUnicityTag
{
    public CustomUnicityTag(string tag)
    {
        this.UnicityTag = tag;
    }

    public string UnicityTag { get; }
}
