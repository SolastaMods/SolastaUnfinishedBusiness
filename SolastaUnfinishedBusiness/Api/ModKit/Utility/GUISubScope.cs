using System;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Api.ModKit.Utility;

public class GUISubScope : IDisposable
{
    public GUISubScope() : this(null) { }

    public GUISubScope(string subtitle)
    {
        if (!string.IsNullOrEmpty(subtitle))
        {
            GUILayout.Label(subtitle.Bold());
        }

        GUILayout.BeginHorizontal();
        GUILayout.Space(10f);
        GUILayout.BeginVertical();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }
}
