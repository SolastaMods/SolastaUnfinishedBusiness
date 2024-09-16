using System;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Api.LanguageExtensions;

public static class Vector3Extensions
{
    /// <summary>
    /// Returns Chessboard/Chebyshev length of this vector
    /// </summary>
    public static float ChessboardLength(this Vector3 self)
    {
        return Math.Max(Math.Abs(self.x), Math.Max(Math.Abs(self.y), Math.Abs(self.z)));
    }
}
