namespace SolastaUnfinishedBusiness.Behaviors;

/**
 * Used to mark spells or powers to state that they are eligible for retargeting, even if they have conditions applied to self
 * Note that you probably should make enemy targeting condition be first in the list
 */
public class ForceRetargetAvailability
{
    private ForceRetargetAvailability()
    {
    }

    public static ForceRetargetAvailability Mark { get; } = new();
}
