namespace SolastaUnfinishedBusiness.CustomInterfaces;

public sealed class IgnoreInterruptionCheck : IIgnoreInterruptionCheck
{
    public static readonly IgnoreInterruptionCheck Marker = new();
}

public interface IIgnoreInterruptionCheck
{
}
