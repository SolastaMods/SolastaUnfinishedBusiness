﻿using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class PosterScreenPatcher
{
    //PATCH: supports speech feature
    [HarmonyPatch(typeof(PosterScreen), nameof(PosterScreen.ProceedAndHide))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ProceedAndHide_Patch
    {
        [UsedImplicitly]
        public static void Prefix()
        {
            SpeechContext.ShutUp();
        }
    }
}
