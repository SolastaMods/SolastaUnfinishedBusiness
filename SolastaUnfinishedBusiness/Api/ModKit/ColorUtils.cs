using UnityEngine;

namespace SolastaUnfinishedBusiness.Api.ModKit;

// https://docs.unity3d.com/Manual/StyledText.html
public enum Rgba : uint
{
    Aqua = 0x00ffffff,
    Blue = 0x8080ffff,
    Brown = 0xC09050ff,
    Crimson = 0x7b0340ff,
    Cyan = 0x00ffffff,
    Darkblue = 0x0000a0ff,
    Darkgrey = 0x808080ff,
    Darkred = 0xa0333bff,
    Fuchsia = 0xff40ffff,
    Green = 0x40C040ff,
    Gold = 0xED9B1Aff,
    Lightblue = 0xd8e6ff,
    Lightgrey = 0xE8E8E8ff,
    Lime = 0x40ff40ff,
    Magenta = 0xff40ffff,
    Maroon = 0xFF6060ff,
    MedRed = 0xd03333ff,
    Navy = 0x3b5681ff,
    Olive = 0xb0b000ff,
    Orange = 0xffa500ff,
    Darkorange = 0xb1521fff,
    Pink = 0xf03399ff,
    Purple = 0xC060F0ff,
    Red = 0xFF4040ff,
    Black = 0x000000ff,
    MedGrey = 0xA8A8A8ff,
    Grey = 0xC0C0C0ff,
    Silver = 0xD0D0D0ff,
    Teal = 0x80f0c0ff,
    Yellow = 0xffff00ff,
    White = 0xffffffff
}

public static class ColorUtils
{
    public static Color Color(this Rgba rga, float adjust = 0)
    {
        var red = ((long)rga >> 24) / 256f;
        var green = (0xFF & ((long)rga >> 16)) / 256f;
        var blue = (0xFF & ((long)rga >> 8)) / 256f;
        var alpha = (0xFF & (long)rga) / 256f;
        var color = new Color(red, green, blue, alpha);

        switch (adjust)
        {
            case < 0:
                color = UnityEngine.Color.Lerp(color, UnityEngine.Color.black, -adjust);
                break;
            case > 0:
                color = UnityEngine.Color.Lerp(color, UnityEngine.Color.white, adjust);
                break;
        }

        return color;
    }
}
