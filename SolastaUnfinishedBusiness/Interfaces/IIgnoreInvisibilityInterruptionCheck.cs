namespace SolastaUnfinishedBusiness.Interfaces;

public sealed class IgnoreInvisibilityInterruptionCheck : IIgnoreInvisibilityInterruptionCheck
{
    public static readonly IgnoreInvisibilityInterruptionCheck Marker = new();
}

public interface IIgnoreInvisibilityInterruptionCheck;
