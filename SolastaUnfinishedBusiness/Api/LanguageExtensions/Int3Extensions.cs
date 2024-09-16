using System;
using JetBrains.Annotations;
using TA;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Api.LanguageExtensions;

public static class Int3Extensions
{
    /// <summary>
    /// Returns Chessboard/Chebyshev length of this vector
    /// </summary>
    [UsedImplicitly]
    public static int ChessboardLength(this int3 self)
    {
        return Math.Max(Math.Abs(self.x), Math.Max(Math.Abs(self.y), Math.Abs(self.z)));
    }

    /// <summary>
    /// Returns Chessboard/Chebyshev distance between 2 points
    /// </summary>
    [UsedImplicitly]
    public static int ChessboardDistance(this int3 self, int3 other)
    {
        return (self - other).ChessboardLength();
    }

    public static Vector3 ToVector3(this int3 self)
    {
        return new Vector3(self.x, self.y, self.z);
    }
}
