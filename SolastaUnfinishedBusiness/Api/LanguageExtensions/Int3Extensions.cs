using TA;
using UnityEngine;
#if DEBUG
using System;
#endif

namespace SolastaUnfinishedBusiness.Api.LanguageExtensions;

public static class Int3Extensions
{
#if DEBUG
    public static int Manhattan(this int3 self)
    {
        return Math.Max(Math.Abs(self.x), Math.Max(Math.Abs(self.y), Math.Abs(self.z)));
    }
#endif

    public static int Manhattan(this int3 self, int3 other)
    {
        return (self - other).Manhattan();
    }

    public static Vector3 ToVector3(this int3 self)
    {
        return new Vector3(self.x, self.y, self.z);
    }
}
