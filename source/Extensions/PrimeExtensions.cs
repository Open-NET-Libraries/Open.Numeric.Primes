using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Open.Numeric.Primes.Extensions;

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
	public static bool IsPrime(in this int value)
		=> Number.IsPrime(value);

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
		=> Number.IsPrime(value);

	/// <inheritdoc cref="IsPrime(in ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPrime(this sbyte value)
		=> Number.IsPrime(value);

	/// <inheritdoc cref="IsPrime(in ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPrime(this byte value)
		=> Number.IsPrime(value);

	/// <inheritdoc cref="IsPrime(in ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPrime(this double value)
		=> Number.IsPrime(value);

	/// <inheritdoc cref="IsPrime(in ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPrime(this decimal value)
		=> Number.IsPrime(in value);

	/// <inheritdoc cref="IsPrime(in ulong)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPrime(this BigInteger value)
		=> Number.IsPrime(in value);

	/// <summary>
	/// Finds the next prime number after the number given.
	/// </summary>
	/// <returns>The next prime after the number provided.</returns>
	/// <exception cref="ArgumentException">Cannot coerce to a valid long value.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ulong NextPrime(this ulong value)
		=> Prime.Numbers.Next(in value);

	/// <summary>
	/// Finds the next prime number after the number given.  If this number is negative, then the result will be the next greater magnitude value prime as negative number.
	/// </summary>
	/// <returns>The next prime after the number provided.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long NextPrime(this long value)
		=> Prime.Numbers.Next(in value);

	/// <summary>
	/// Finds the next prime number after the number given.  If this number is negative, then the result will be the next greater magnitude value prime as negative number.
	/// </summary>
	/// <returns>The next prime after the number provided.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static BigInteger NextPrime(this float value)
		=> Prime.Numbers.Big.Next(value);

	/// <summary>
	/// Finds the next prime number after the number given.  If this number is negative, then the result will be the next greater magnitude value prime as negative number.
	/// </summary>
	/// <returns>The next prime after the number provided.</returns>
	/// <exception cref="ArgumentException">Cannot coerce to a valid long value.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static BigInteger NextPrime(this double value)
		=> Prime.Numbers.Big.Next(value);

	/// <summary>
	/// Finds the next prime number after the number given.  If this number is negative, then the result will be the next greater magnitude value prime as negative number.
	/// </summary>
	/// <returns>The next prime after the number provided.</returns>
	/// <exception cref="ArgumentException">Cannot coerce to a valid long value.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static BigInteger NextPrime(this BigInteger value)
		=> Prime.Numbers.Big.Next(in value);

	/// <summary>
	/// Iterates the prime factors of the provided value.
	/// If omitOneAndValue==false, first multiple is always 0 or 1.
	/// Else if the value is prime, then there will be no results.
	/// </summary>
	/// <param name="value">The value to factor.</param>
	/// <param name="omitOneAndValue">If true, only positive integers greater than 1 and less than the number itself are returned.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IEnumerable<ulong> PrimeFactors(this ulong value, bool omitOneAndValue = false)
		=> Prime.Factors(value, omitOneAndValue);

	/// <summary>
	/// Iterates the prime factors of the provided value.
	/// If omitOneAndValue==false, first multiple is always 0, 1 or -1.
	/// Else if the value is prime, then there will be no results.
	/// </summary>
	/// <param name="value">The value to factor.</param>
	/// <param name="omitOneAndValue">If true, only positive integers greater than 1 and less than the number itself are returned.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IEnumerable<long> PrimeFactors(this long value, bool omitOneAndValue = false)
		=> Prime.Factors(value, omitOneAndValue);

	/// <summary>
	/// Iterates the prime factors of the provided value.
	/// If omitOneAndValue==false, first multiple is always 0, 1 or -1.
	/// Else if the value is prime, then there will be no results.
	/// </summary>
	/// <param name="value">The value to factor.</param>
	/// <param name="omitOneAndValue">If true, only positive integers greater than 1 and less than the number itself are returned.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IEnumerable<BigInteger> PrimeFactors(this BigInteger value, bool omitOneAndValue = false)
		=> Prime.Factors(value, omitOneAndValue);

	/// <summary>
	/// Iterates the prime factors of the provided value.
	/// If omitOneAndValue==false, first multiple is always 0, 1 or -1.
	/// Else if the value is prime or not a whole number, then there will be no results.
	/// </summary>
	/// <param name="value">The value to factor.</param>
	/// <param name="omitOneAndValue">If true, only positive integers greater than 1 and less than the number itself are returned.</param>
	public static IEnumerable<dynamic> PrimeFactors(this double value, bool omitOneAndValue = false)
		=> Prime.Factors(value, omitOneAndValue);

	/// <summary>
	/// Iterates the prime factors of the provided value.
	/// If omitOneAndValue==false, first multiple is always 0, 1 or -1.
	/// Else if the value is prime or not a whole number, then there will be no results.
	/// </summary>
	/// <param name="value">The value to factor.</param>
	/// <param name="omitOneAndValue">If true, only positive integers greater than 1 and less than the number itself are returned.</param>
	public static IEnumerable<dynamic> PrimeFactors(this float value, bool omitOneAndValue = false)
		=> Prime.Factors(value, omitOneAndValue);

	/// <summary>
	/// Iterates all the possible common prime factors of the provided numbers.
	/// </summary>
	/// <param name="values">The values to find common prime factors from.</param>
	/// <returns>An enumerable of the common prime factors.</returns>
	public static IEnumerable<long> CommonPrimeFactors(this IEnumerable<long> values)
		=> Prime.CommonFactors(values);

	/// <summary>
	/// Iterates all the possible common prime factors of the provided numbers.
	/// </summary>
	/// <param name="values">The values to find common prime factors from.</param>
	/// <returns>An enumerable of the common prime factors.</returns>
	public static IEnumerable<ulong> CommonPrimeFactors(this IEnumerable<ulong> values)
		=> Prime.CommonFactors(values);

	/// <summary>
	/// Iterates all the possible common prime factors of the provided numbers.
	/// </summary>
	/// <param name="values">The values to find common prime factors from.</param>
	/// <returns>An enumerable of the common prime factors.</returns>
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

	/// <summary>
	/// Returns the greatest common (positive) factor (GCF) of all the provided values.
	/// Returns 1 if none found.
	/// </summary>
	/// <param name="values">The values to find common prime factors from.</param>
	/// <returns>The greatest common factor or 1 if none found.</returns>
	public static ulong GreatestPrimeFactor(this IEnumerable<ulong> values)
		=> Prime.GreatestFactor(values);

	/// <summary>
	/// Returns the greatest common (positive) factor (GCF) of all the provided values.
	/// Returns 1 if none found.
	/// </summary>
	/// <param name="values">The values to find common prime factors from.</param>
	/// <returns>The greatest common factor or 1 if none found.</returns>
	public static BigInteger GreatestPrimeFactor(this IEnumerable<BigInteger> values)
		=> Prime.GreatestFactor(values);
}
