// author:  https://github.com/bgrainger/PcgRandom
// license: https://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Api.Infrastructure;

/// <summary>
/// Provides an implementation of <see cref="System.Random"/> that uses <see cref="Pcg32Single"/> to generate its random numbers.
/// </summary>
public sealed class PcgRandom : Random
{
    private readonly Pcg32Single _rng;

    /// <summary>
    /// Initializes a new instance of the <see cref="PcgRandom"/> class, using a time-dependent default seed value.
    /// </summary>
    private PcgRandom()
    {
        _rng = new Pcg32Single(unchecked((ulong)Stopwatch.GetTimestamp()));
    }

    public static PcgRandom Random { get; } = new();

    public ulong State
    {
        get => _rng.State;
        set => _rng.State = value;
    }

    /// <summary>
    /// Returns a random integer that is within a specified range.
    /// </summary>
    /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
    /// <param name="maxValue">The exclusive upper bound of the random number returned. <paramref name="maxValue"/> must be greater than or equal to <paramref name="minValue"/>.</param>
    /// <returns>A 32-bit signed integer greater than or equal to <paramref name="minValue"/> and less than <paramref name="maxValue"/>;
    /// that is, the range of return values includes <paramref name="minValue"/> but not <paramref name="maxValue"/>. If <paramref name="minValue"/>
    /// equals <paramref name="maxValue"/>, <paramref name="minValue"/> is returned.</returns>
    public override int Next(int minValue, int maxValue)
    {
        if (minValue > maxValue)
            // ReSharper disable once LocalizableElement
        {
            throw new ArgumentOutOfRangeException(nameof(maxValue), maxValue,
                $"maxValue must be greater than minValue ({minValue})");
        }

        var range = (uint)((long)maxValue - minValue);
        return (int)(minValue + (range <= 1 ? 0 : _rng.GenerateNext(range)));
    }

    /// <summary>
    /// Returns a random floating-point number between 0.0 and 1.0.
    /// </summary>
    /// <returns>A double-precision floating point number that is greater than or equal to 0.0, and less than 1.0.</returns>
    protected override double Sample()
    {
        // as per http://www.pcg-random.org/using-pcg-c.html#generating-doubles, return (rnd * 1/2**32)
        return _rng.GenerateNext() * Math.Pow(2, -32);
    }

    /// <summary>
    /// Returns a random floating-point number between 0.0 and 1.0.
    /// </summary>
    /// <returns>A double-precision floating point number that is greater than or equal to 0.0, and less than 1.0.</returns>
    public override double NextDouble()
    {
        return Sample();
    }
}

/// <summary>
/// Implements the <code>pcg32s</code> random number generator described
/// at <a href="http://www.pcg-random.org/using-pcg-c.html">http://www.pcg-random.org/using-pcg-c.html</a>.
/// </summary>
public sealed class Pcg32Single
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Pcg32Single"/> pseudorandom number generator.
    /// </summary>
    /// <param name="state">The starting state for the RNG; you can pass any 64-bit value.</param>
    public Pcg32Single(ulong state)
    {
        // implements pcg_oneseq_64_srandom_r
        Step();
        State += state;
        Step();
    }

    public ulong State { get; set; }

    /// <summary>
    /// Generates the next random number.
    /// </summary>
    /// <returns></returns>
    public uint GenerateNext()
    {
        // implements pcg_oneseq_64_xsh_rr_32_random_r
        var oldState = State;
        Step();
        return Helpers.OutputXshRr(oldState);
    }

    /// <summary>
    /// Generates a uniformly distributed 32-bit unsigned integer less than <paramref name="bound"/> (i.e., <c>x</c> where
    /// <c>0 &lt;= x &lt; bound</c>.
    /// </summary>
    /// <param name="bound">The exclusive upper bound of the random number to be generated.</param>
    /// <returns>A random number between <c>0</c> and <paramref name="bound"/> (exclusive).</returns>
    public uint GenerateNext(uint bound)
    {
        // implements pcg_oneseq_64_xsh_rr_32_boundedrand_r
        var threshold = (uint)-bound % bound;
        while (true)
        {
            var r = GenerateNext();
            if (r >= threshold)
            {
                return r % bound;
            }
        }
    }

    /// <summary>
    /// Advances the RNG by <paramref name="delta"/> steps, doing so in <c>log(delta)</c> time.
    /// </summary>
    /// <param name="delta">The number of steps to advance; pass <c>2<sup>64</sup> - delta</c> (i.e., <c>-delta</c>) to go backwards.</param>
    [UsedImplicitly]
    public void Advance(ulong delta)
    {
        // implements pcg_oneseq_64_advance_r
        State = Helpers.Advance(State, delta, Helpers.Multiplier64, Helpers.Increment64);
    }

    private void Step()
    {
        // implements pcg_oneseq_64_step_r
        State = unchecked((State * Helpers.Multiplier64) + Helpers.Increment64);
    }
}

internal static class Helpers
{
    /// <summary>
    /// Represents <c>PCG_DEFAULT_MULTIPLIER_64</c>.
    /// </summary>
    /// <remarks>See <a href="https://github.com/imneme/pcg-c/blob/e2383c4bfcc862b40c3d85a43c9d495ff61186cb/include/pcg_variants.h#L253">original source</a>.</remarks>
    public const ulong Multiplier64 = 6364136223846793005ul;

    /// <summary>
    /// Represents <c>PCG_DEFAULT_INCREMENT_64</c>.
    /// </summary>
    /// <remarks>See <a href="https://github.com/imneme/pcg-c/blob/e2383c4bfcc862b40c3d85a43c9d495ff61186cb/include/pcg_variants.h#L258">original source</a>.</remarks>
    public const ulong Increment64 = 1442695040888963407ul;

    /// <summary>
    /// Implements <c>pcg_advance_lcg_64</c>.
    /// </summary>
    /// <remarks>See <a href="https://github.com/imneme/pcg-c/blob/e2383c4bfcc862b40c3d85a43c9d495ff61186cb/src/pcg-advance-64.c#L46">original source</a>.</remarks>
    public static ulong Advance(ulong state, ulong delta, ulong multiplier, ulong increment)
    {
        // corresponds to pcg_advance_lcg_64
        ulong accumulatedMultiplier = 1u;
        ulong accumulatedIncrement = 0u;
        while (delta > 0)
        {
            if (delta % 2 == 1)
            {
                accumulatedMultiplier *= multiplier;
                accumulatedIncrement = (accumulatedIncrement * multiplier) + increment;
            }

            increment = (multiplier + 1) * increment;
            multiplier *= multiplier;
            delta /= 2;
        }

        return (accumulatedMultiplier * state) + accumulatedIncrement;
    }

    /// <summary>
    /// Implements <c>pcg_output_xsh_rs_64_32</c>.
    /// </summary>
    /// <remarks>See <a href="https://github.com/imneme/pcg-c/blob/e2383c4bfcc862b40c3d85a43c9d495ff61186cb/include/pcg_variants.h#L133">original source</a>.</remarks>
    [UsedImplicitly]
    public static uint OutputXshRs(ulong state)
    {
        return unchecked((uint)(((state >> 22) ^ state) >> (int)((state >> 61) + 22u)));
    }

    /// <summary>
    /// Implements <c>pcg_output_xsh_rr_64_32</c>.
    /// </summary>
    /// <remarks>See <a href="https://github.com/imneme/pcg-c/blob/e2383c4bfcc862b40c3d85a43c9d495ff61186cb/include/pcg_variants.h#L158">original source</a>.</remarks>
    public static uint OutputXshRr(ulong state)
    {
        return RotateRight((uint)(((state >> 18) ^ state) >> 27), (int)(state >> 59));
    }

    /// <summary>
    /// Implements <c>pcg_rotr_32</c>.
    /// </summary>
    /// <param name="value">The value to rotate right.</param>
    /// <param name="rotate">The number of bits to rotate.</param>
    /// <returns>The input <paramref name="value"/>, rotated right by <paramref name="rotate"/> bits.</returns>
    /// <remarks>See <a href="https://github.com/imneme/pcg-c/blob/e2383c4bfcc862b40c3d85a43c9d495ff61186cb/include/pcg_variants.h#L88">original source</a>.</remarks>
    private static uint RotateRight(uint value, int rotate)
    {
        return (value >> rotate) | (value << (-rotate & 31));
    }
}
