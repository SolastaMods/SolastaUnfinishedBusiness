using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRules.ScaleMerchantPricesCorrectly;
//Original method - if cost is say '1' and the multiplier is less than one, it ends up as zero.
//For the Spear it starts out costing 1gp and ends up costing 0gp.

/*
    public static void ScaleAndRoundCosts(float priceMultiplier, int[] baseCosts, int[] scaledCosts)
    {
        for (int index = 0; index< 5; ++index)
            scaledCosts[index] = Mathf.RoundToInt(priceMultiplier* (float) baseCosts[index]);
    }
    */

[HarmonyPatch(typeof(EquipmentDefinitions), "ScaleAndRoundCosts")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class EquipmentDefinitions_ScaleAndRoundCosts
{
    internal static bool Prefix(float priceMultiplier, int[] baseCosts, int[] scaledCosts)
    {
        if (!Main.Settings.ScaleMerchantPricesCorrectly)
        {
            return true;
        }

        // convert to copper
        var cp =
            (1000 * baseCosts[0]) + // platinum
            (100 * baseCosts[1]) + // gold
            (50 * baseCosts[2]) + // electrum?
            (10 * baseCosts[3]) + // silver
            baseCosts[4]; // copper

        // scale
        var scaledCopper = priceMultiplier * cp;

        if (scaledCopper < 1 && cp >= 1)
        {
            // it's always worth at least 1cp
            scaledCopper = 1;
        }

        // scaled as gold
        var scaledGold = scaledCopper / 100.0;

        if (scaledGold > 10)
        {
            // if more than 10 gp round to nearest gp
            scaledGold = Math.Round(scaledGold, MidpointRounding.AwayFromZero);
        }
        else
        {
            // else keep 2 sig fig
            scaledGold = scaledGold.Round(2);
        }

        // convert back to cp
        var scaledAndRoundedCopper = (int)(scaledGold * 100);

        // convert back to 5 digit array
        scaledCosts[4] = scaledAndRoundedCopper % 10;
        scaledCosts[3] = (scaledAndRoundedCopper - scaledCosts[4]) / 10 % 10;
        // scaledCosts[2] always zero?
        scaledCosts[1] = (scaledAndRoundedCopper - (scaledCosts[3] * 10) - scaledCosts[4]) / 100 % 10;
        scaledCosts[0] =
            (scaledAndRoundedCopper - (scaledCosts[1] * 100) - (scaledCosts[3] * 10) - scaledCosts[4]) / 1000;

        // only show platinum if baseCosts had platinum
        if (baseCosts[0] != 0)
        {
            return false;
        }

        scaledCosts[1] += 10 * scaledCosts[0];
        scaledCosts[0] = 0;

        return false;
    }
}

/// <summary>
///     Borrowed from https://stackoverflow.com/questions/158172/formatting-numbers-with-significant-figures-in-c-sharp
/// </summary>
internal static class Precision
{
    // 2^-24
    private const float FLOAT_EPSILON = 0.0000000596046448f;

    // 2^-53
    private const double DOUBLE_EPSILON = 0.00000000000000011102230246251565d;

    public static bool AlmostEquals(this double a, double b, double epsilon = DOUBLE_EPSILON)
    {
        // ReSharper disable CompareOfFloatsByEqualityOperator
        if (a == b)
        {
            return true;
        }
        // ReSharper restore CompareOfFloatsByEqualityOperator

        return Math.Abs(a - b) < epsilon;
    }

    public static bool AlmostEquals(this float a, float b, float epsilon = FLOAT_EPSILON)
    {
        // ReSharper disable CompareOfFloatsByEqualityOperator
        if (a == b)
        {
            return true;
        }
        // ReSharper restore CompareOfFloatsByEqualityOperator

        return Math.Abs(a - b) < epsilon;
    }
}

internal static class SignificantDigits
{
    public static double Round(this double value, int significantDigits)
    {
        return RoundSignificantDigits(value, significantDigits, out _);
    }

    private static double RoundSignificantDigits(double value, int significantDigits, out int roundingPosition)
    {
        // this method will return a rounded double value at a number of signifigant figures.
        // the sigFigures parameter must be between 0 and 15, exclusive.

        roundingPosition = 0;

        if (value.AlmostEquals(0d))
        {
            roundingPosition = significantDigits - 1;
            return 0d;
        }

        if (double.IsNaN(value))
        {
            return double.NaN;
        }

        if (double.IsPositiveInfinity(value))
        {
            return double.PositiveInfinity;
        }

        if (double.IsNegativeInfinity(value))
        {
            return double.NegativeInfinity;
        }

        if (significantDigits < 1 || significantDigits > 15)
        {
            throw new ArgumentOutOfRangeException(nameof(significantDigits), value,
                "The significantDigits argument must be between 1 and 15.");
        }

        // The resulting rounding position will be negative for rounding at whole numbers, and positive for decimal places.
        roundingPosition = significantDigits - 1 - (int)Math.Floor(Math.Log10(Math.Abs(value)));

        // try to use a rounding position directly, if no scale is needed.
        // this is because the scale mutliplication after the rounding can introduce error, although 
        // this only happens when you're dealing with really tiny numbers, i.e 9.9e-14.
        if (roundingPosition > 0 && roundingPosition < 16)
        {
            return Math.Round(value, roundingPosition, MidpointRounding.AwayFromZero);
        }

        // Shouldn't get here unless we need to scale it.
        // Set the scaling value, for rounding whole numbers or decimals past 15 places
        var scale = Math.Pow(10, Math.Ceiling(Math.Log10(Math.Abs(value))));

        return Math.Round(value / scale, significantDigits, MidpointRounding.AwayFromZero) * scale;
    }
}
