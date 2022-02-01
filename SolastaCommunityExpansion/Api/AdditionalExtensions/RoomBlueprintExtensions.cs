using System;
using SolastaModApi.Diagnostics;
using UnityEngine;

namespace SolastaModApi.Extensions
{
    public static partial class RoomBlueprintExtensions
    {
        [Obsolete("Use SetGroundMaskSpriteReference")]
        public static T SetGrounMaskSprite<T>(this T entity, Sprite value)
            where T : RoomBlueprint
        {
            throw new SolastaModApiException("You must use SetGroundMaskSpriteReference");
        }
    }
}
