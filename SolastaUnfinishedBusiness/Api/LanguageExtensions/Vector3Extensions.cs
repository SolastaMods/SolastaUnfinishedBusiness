using System;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Api.LanguageExtensions;

public static class Vector3Extensions
{
    public static float Manhattan(this Vector3 self)
    {
        return Math.Max(Math.Abs(self.x), Math.Max(Math.Abs(self.y), Math.Abs(self.z)));
    }
}
