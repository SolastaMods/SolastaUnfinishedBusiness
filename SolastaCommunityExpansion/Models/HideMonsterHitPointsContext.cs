using System.Reflection;

namespace SolastaCommunityExpansion.Models;

internal static class HideMonsterHitPointsContext
{
    /// <summary>
    ///     Call 'HasHealthUpdated' which returns true/false but as a side effect updates the health state and dirty flags.
    /// </summary>
    internal static bool UpdateHealthStatus(this GuiCharacter __instance)
    {
        // call badly named method
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
        var rb = typeof(GuiCharacter).GetMethod("HasHealthUpdated",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields

        var retval = false;

        if (rb != null)
        {
            retval = (bool)rb.Invoke(__instance, null);
        }

        return retval;
    }

    /// <summary>
    ///     Converts continuous ratio into series of stepped values.
    /// </summary>
    internal static float GetSteppedHealthRatio(float ratio)
    {
        return ratio switch
        {
            // Green
            >= 1f => 1f,
            // Green
            >= 0.5f => 0.75f,
            // Orange
            >= 0.25f => 0.5f,
            // Red
            > 0f => 0.25f,
            _ => ratio
        };
    }
}
