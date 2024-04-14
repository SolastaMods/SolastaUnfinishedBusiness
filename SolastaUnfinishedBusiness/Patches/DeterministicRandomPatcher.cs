using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using TA;
using Random = UnityEngine.Random;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class DeterministicRandomPatcher
{
    private static ulong MySeed => (ulong)DateTime.Now.Ticks;

    private static PcgRandom MyRandom { get; } = new(MySeed);

    private static Random.State MyStateToRandomState(ulong myState)
    {
        var ms = new MyState { State = myState };

        unsafe
        {
            return *(Random.State*)(&ms);
        }
    }

    private static ulong RandomStateToMyState(Random.State state)
    {
        unsafe
        {
            return *(ulong*)&state;
        }
    }

    private static float Next(double minValue, double maxValue)
    {
        var result = (MyRandom.NextDouble() * (maxValue - minValue)) + minValue;

        return (float)result;
    }

    private struct MyState
    {
        // ReSharper disable once NotAccessedField.Local
        public ulong State;
#pragma warning disable CS0169
#pragma warning disable IDE0051
        private readonly ulong _unused;
#pragma warning restore IDE0051
#pragma warning restore CS0169
    }

    [HarmonyPatch(typeof(DeterministicRandom), nameof(DeterministicRandom.value), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class State_Getter_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(ref float __result)
        {
            if (!Main.Settings.EnablePcgRandom || Global.IsMultiplayer)
            {
                return true;
            }

            var service = ServiceRepository.GetService<IGameService>();

            if (service == null || !service.Game || DeterministicRandom.lockFlags.Count > 0)
            {
                __result = (float)MyRandom.NextDouble();

                return false;
            }

            service.Game.ResetDeterministicRandomState();

            var num = (float)MyRandom.NextDouble();

            service.Game.RecordDeterministicRandomState();

            __result = num;

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
            if (!Main.Settings.EnablePcgRandom || Global.IsMultiplayer)
            {
                return true;
            }

            var service = ServiceRepository.GetService<IGameService>();

            if (service == null || !service.Game || DeterministicRandom.lockFlags.Count > 0)
            {
                __result = MyRandom.Next(min, max);

                return false;
            }

            service.Game.ResetDeterministicRandomState();

            var num = MyRandom.Next(min, max);

            service.Game.RecordDeterministicRandomState();

            __result = num;

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
            if (!Main.Settings.EnablePcgRandom || Global.IsMultiplayer)
            {
                return true;
            }

            var service = ServiceRepository.GetService<IGameService>();

            if (service == null || !service.Game || DeterministicRandom.lockFlags.Count > 0)
            {
                __result = Next(min, max);

                return false;
            }

            service.Game.ResetDeterministicRandomState();

            var num = Next(min, max);

            service.Game.RecordDeterministicRandomState();

            __result = num;

            return false;
        }
    }

    [HarmonyPatch(typeof(Game), nameof(Game.RecordDeterministicRandomState))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RecordDeterministicRandomState_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(Game __instance)
        {
            if (!Main.Settings.EnablePcgRandom || Global.IsMultiplayer)
            {
                return true;
            }

            __instance.randomState = MyStateToRandomState(MyRandom.State);

            return false;
        }
    }

    [HarmonyPatch(typeof(Game), nameof(Game.ResetDeterministicRandomState))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ResetDeterministicRandomState_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(Game __instance)
        {
            if (!Main.Settings.EnablePcgRandom || Global.IsMultiplayer)
            {
                return true;
            }

            MyRandom.State = RandomStateToMyState(__instance.randomState);

            return false;
        }
    }

    [HarmonyPatch(typeof(Game), nameof(Game.GenerateRandomSeed))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GenerateRandomSeed_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(Game __instance)
        {
            if (!Main.Settings.EnablePcgRandom || Global.IsMultiplayer)
            {
                return true;
            }

            var service = ServiceRepository.GetService<INetworkingService>();

            if (__instance.randomSeed == 0)
            {
                __instance.randomSeed = service?.RoomRandomSeed ?? 0;

                if (__instance.randomSeed == 0)
                {
                    __instance.randomSeed = (int)MySeed;
                }
            }
            else
            {
                var num = service?.RoomRandomSeed ?? 0;

                if (num == 0)
                {
                    num = __instance.randomSeed;
                }

                __instance.randomSeed = (num ^ 3) * 3 / 2;
            }

            var seed = (ulong)__instance.randomSeed;

            MyRandom.State = (seed << 32) + seed;

            __instance.RecordDeterministicRandomState();

            return false;
        }
    }
}
