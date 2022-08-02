using System.Linq;
using JetBrains.Annotations;

namespace SolastaCommunityExpansion.Api.Extensions;

internal static class GameGadgetExtensions
{
    public const string Enabled = "Enabled";
    public const string Triggered = "Triggered";
    public const string RemoteEnabled = "RemoteEnabled";
    public const string ParamEnabled = "Param_Enabled";
    public const string Invisible = "Invisible";

    /// <summary>
    ///     Returns state of Invisible parameter, or false if not present
    /// </summary>
    public static bool IsInvisible([NotNull] this GameGadget gadget)
    {
        return gadget.CheckConditionName(Invisible, true, false);
    }

    public static bool IsEnabled([NotNull] this GameGadget gadget, bool valueIfParamsNotPresent = false)
    {
        // We need to know if both Enabled and ParamEnabled are missing
        var names = gadget.conditionNames;

        if (!names.Any(n => n is Enabled or ParamEnabled))
        {
            // if not present return supplied default value
            return valueIfParamsNotPresent;
        }

        // if at least one is present then return if either is true
        var enabled = gadget.CheckConditionName(Enabled, true, false);
        var paramEnabled = gadget.CheckConditionName(ParamEnabled, true, false);

        return enabled || paramEnabled;
    }
}
