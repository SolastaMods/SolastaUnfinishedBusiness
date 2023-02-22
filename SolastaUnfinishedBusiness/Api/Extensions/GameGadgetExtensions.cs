using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Api.Extensions;

internal static class GameGadgetExtensions
{
    private const string Invisible = "Invisible";
    // internal const string Triggered = "Triggered";

    /// <summary>
    ///     Returns state of Invisible parameter, or false if not present
    /// </summary>
    internal static bool IsInvisible([NotNull] this GameGadget gadget)
    {
        return gadget.CheckConditionName(Invisible, true, false);
    }

    internal static bool IsExit([NotNull] this GameGadget gadget)
    {
        return gadget.HasFunctor(FunctorDefinitions.FunctorQuitLocation);
    }

    internal static bool IsCamp([NotNull] this GameGadget gadget)
    {
        return gadget.HasFunctor(FunctorDefinitions.FunctorStartLongRest);
    }

    internal static bool IsTeleport([NotNull] this GameGadget gadget)
    {
        return gadget.HasFunctor(FunctorDefinitions.FunctorTeleport);
    }
}
