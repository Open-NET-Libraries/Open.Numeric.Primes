using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Open.Numeric.Primes.Extensions;

/// <summary>
/// An easily accessible set of extensions for detecting prime numbers as well as factors.
/// </summary>
[ExcludeFromCodeCoverage] // Because it's just passing through to the actual methods.
public static class PrimeExtensions
{
	/// <inheritdoc cref="Number.IsPrime(in ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPrime(in this ulong value)
		=> Number.IsPrime(in value);

	/// <inheritdoc cref="IsPrime(in ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPrime(in this long value)
		=> Number.IsPrime(in value);

	/// <inheritdoc cref="IsPrime(in ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPrime(this int value)
		=> Number.IsPrime(value < 0 ? (uint)-value : (uint)value);

	/// <inheritdoc cref="IsPrime(in ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPrime(this uint value)
		=> Number.IsPrime(value);

	/// <inheritdoc cref="IsPrime(in ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPrime(this short value)
		=> Number.IsPrime(value);

	/// <inheritdoc cref="IsPrime(in ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPrime(this ushort value)
		=> Number.IsPrime((uint)value);

	/// <inheritdoc cref="IsPrime(in ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPrime(this sbyte value)
		=> Number.IsPrime(value);

	/// <inheritdoc cref="IsPrime(in ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPrime(this byte value)
		=> Number.IsPrime((uint)value);

	/// <inheritdoc cref="IsPrime(in ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPrime(in this double value)
		=> Number.IsPrime(value);

	/// <inheritdoc cref="IsPrime(in ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPrime(in this decimal value)
		=> Number.IsPrime(in value);

	/// <inheritdoc cref="IsPrime(in ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPrime(in this BigInteger value)
		=> Number.IsPrime(in value);

	/// <inheritdoc cref="PrimalityBase{T}.Next(in T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ulong NextPrime(in this ulong value)
		=> Prime.Numbers.Next(in value);

	/// <inheritdoc cref="NextPrime(in ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long NextPrime(in this long value)
		=> Prime.Numbers.Next(in value);

	/// <inheritdoc cref="NextPrime(in ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static BigInteger NextPrime(this float value)
		=> Prime.Numbers.Big.Next(value);

	/// <inheritdoc cref="NextPrime(in ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static BigInteger NextPrime(in this double value)
		=> Prime.Numbers.Big.Next(value);

	/// <inheritdoc cref="NextPrime(in ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static BigInteger NextPrime(this BigInteger value)
		=> Prime.Numbers.Big.Next(in value);

	/// <inheritdoc cref="Prime.Factors(ulong, bool)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IEnumerable<ulong> PrimeFactors(in this ulong value, bool omitOneAndValue = false)
		=> Prime.Factors(value, omitOneAndValue);

	/// <inheritdoc cref="PrimeFactors(in ulong, bool)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IEnumerable<long> PrimeFactors(in this long value, bool omitOneAndValue = false)
		=> Prime.Factors(value, omitOneAndValue);

	/// <summary>
	/// Iterates the prime factors of the provided value.
	/// If omitOneAndValue==false, first multiple is always 0, 1 or -1.
	/// Else if the value is prime, then there will be no results.
	/// </summary>
	/// <param name="value">The value to factor.</param>
	/// <param name="omitOneAndValue">If true, only positive integers greater than 1 and less than the number itself are returned.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IEnumerable<BigInteger> PrimeFactors(in this BigInteger value, bool omitOneAndValue = false)
		=> Prime.Factors(value, omitOneAndValue);

	/// <inheritdoc cref="PrimeFactors(in BigInteger, bool)"/>
	public static IEnumerable<double> PrimeFactors(in this double value, bool omitOneAndValue = false)
		=> Prime.Factors(value, omitOneAndValue);

	/// <inheritdoc cref="PrimeFactors(in BigInteger, bool)"/>
	public static IEnumerable<float> PrimeFactors(this float value, bool omitOneAndValue = false)
		=> Prime.Factors(value, omitOneAndValue);

	/// <summary>
	/// Iterates all the possible common prime factors of the provided numbers.
	/// </summary>
	/// <param name="values">The values to find common prime factors from.</param>
	/// <returns>An enumerable of the common prime factors.</returns>
	public static IEnumerable<long> CommonPrimeFactors(this IEnumerable<long> values)
		=> Prime.CommonFactors(values);

	/// <inheritdoc cref="CommonPrimeFactors(IEnumerable{long})"/>
	public static IEnumerable<ulong> CommonPrimeFactors(this IEnumerable<ulong> values)
		=> Prime.CommonFactors(values);

	/// <inheritdoc cref="CommonPrimeFactors(IEnumerable{long})"/>
	public static IEnumerable<BigInteger> CommonPrimeFactors(this IEnumerable<BigInteger> values)
		=> Prime.CommonFactors(values);

	/// <summary>
	/// Returns the greatest common (positive) factor (GCF) of all the provided values.
	/// Returns 1 if none found.
	/// </summary>
	/// <param name="values">The values to find common prime factors from.</param>
	/// <returns>The greatest common factor or 1 if none found.</returns>
	public static long GreatestPrimeFactor(this IEnumerable<long> values)
		=> Prime.GreatestFactor(values);

	/// <inheritdoc cref="GreatestPrimeFactor(IEnumerable{long})"/>
	public static ulong GreatestPrimeFactor(this IEnumerable<ulong> values)
		=> Prime.GreatestFactor(values);

	/// <inheritdoc cref="GreatestPrimeFactor(IEnumerable{long})"/>
	public static BigInteger GreatestPrimeFactor(this IEnumerable<BigInteger> values)
		=> Prime.GreatestFactor(values);
}
