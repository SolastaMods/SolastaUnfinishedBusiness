using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using TA;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class DeterministicRandomPatcher
{
    private static ulong _state;

    private static float Next(double minValue, double maxValue)
    {
        var result = (PcgRandom.Random.NextDouble() * (maxValue - minValue)) + minValue;

        return (float)result;
    }

    private static void ResetRandomState()
    {
        PcgRandom.Random.State = _state;
    }

    private static void RecordRandomState()
    {
        _state = PcgRandom.Random.State;
    }

    [HarmonyPatch(typeof(DeterministicRandom), nameof(DeterministicRandom.value), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class State_Getter_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(ref float __result)
        {
            if (!Main.Settings.EnablePcgRandom)
            {
                return true;
            }

            if (DeterministicRandom.lockFlags.Count > 0)
            {
                __result = (float)PcgRandom.Random.NextDouble();

                return false;
            }

            ResetRandomState();
            __result = (float)PcgRandom.Random.NextDouble();
            RecordRandomState();

            return false;
        }
    }

    [HarmonyPatch(typeof(DeterministicRandom), nameof(DeterministicRandom.Range), typeof(int), typeof(int))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Range_Int_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(ref int __result, int min, int max)
        {
            if (!Main.Settings.EnablePcgRandom)
            {
                return true;
            }

            if (DeterministicRandom.lockFlags.Count > 0)
            {
                __result = PcgRandom.Random.Next(min, max);

                return false;
            }

            ResetRandomState();
            __result = PcgRandom.Random.Next(min, max);
            RecordRandomState();

            return false;
        }
    }

    [HarmonyPatch(typeof(DeterministicRandom), nameof(DeterministicRandom.Range), typeof(float), typeof(float))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Range_Float_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(ref float __result, float min, float max)
        {
            if (!Main.Settings.EnablePcgRandom)
            {
                return true;
            }

            if (DeterministicRandom.lockFlags.Count > 0)
            {
                __result = Next(min, max);

                return false;
            }

            ResetRandomState();
            __result = Next(min, max);
            RecordRandomState();

            return false;
        }
    }

    [HarmonyPatch(typeof(DeterministicRandom), nameof(DeterministicRandom.ResetRandomState))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ResetRandomState_Patch
    {
        [UsedImplicitly]
        public static bool Prefix()
        {
            if (!Main.Settings.EnablePcgRandom)
            {
                return true;
            }

            if (DeterministicRandom.lockFlags.Count > 0)
            {
                return false;
            }

            ResetRandomState();

            return false;
        }
    }

    [HarmonyPatch(typeof(DeterministicRandom), nameof(DeterministicRandom.RecordRandomState))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RecordRandomState_Patch
    {
        [UsedImplicitly]
        public static bool Prefix()
        {
            if (!Main.Settings.EnablePcgRandom)
            {
                return true;
            }

            if (DeterministicRandom.lockFlags.Count > 0)
            {
                return false;
            }

            RecordRandomState();

            return false;
        }
    }
}
