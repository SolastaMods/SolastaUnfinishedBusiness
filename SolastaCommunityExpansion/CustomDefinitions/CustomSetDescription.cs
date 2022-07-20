namespace SolastaCommunityExpansion.CustomDefinitions;

public sealed class CustomSetDescription
{
    public static readonly CustomSetDescription Marker = new();

    private CustomSetDescription()
    {
    }
}
