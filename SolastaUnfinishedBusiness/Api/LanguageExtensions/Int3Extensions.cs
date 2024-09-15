using System;
using TA;

namespace SolastaUnfinishedBusiness.Api.LanguageExtensions;

public static class Int3Extensions
{
    public static int Manhattan(this int3 self)
    {
        return Math.Max(Math.Abs(self.x), Math.Max(Math.Abs(self.y), Math.Abs(self.z)));
    }
}
