using Open.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

namespace Open.Numeric.Primes;

/// <summary>
/// A useful set of prime discovery and prime factorization functions.<br/>
/// Unique overloads for certain number types including BigInteger in order to ensure efficiency and compiler optimizations.
/// </summary>
/// <remarks>Negative numbers are allowed where possible and the signs are preserved.</remarks>
public static class Prime
{
	internal static readonly Polynomial.U32 Numbers32 = new();

	/// <summary>
	/// The default <see cref="Optimized"/> instance.
	/// </summary>
	public static readonly Optimized Numbers = new();

	/// <inheritdoc cref="PrimalityBase{T}.Factors(T)"/>
	public static IEnumerable<uint> Factors(uint value)
		=> Numbers32.Factors(value);

	/// <inheritdoc cref="PrimalityBase{T}.Factors(T)"/>
	public static IEnumerable<ulong> Factors(ulong value)
		=> Numbers.Factors(value);

	/// <inheritdoc cref="PrimalityBase{T}.Factors(T)"/>
	public static IEnumerable<long> Factors(long value)
		=> Numbers.Factors(value);

	/// <inheritdoc cref="PrimalityBigIntBase.Factors(BigInteger)"/>
	public static IEnumerable<BigInteger> Factors(BigInteger value)
		=> Numbers.Big.Factors(value);

	const float FloatLargestContiguousInt = 16777216;

	/// <summary>
	/// Iterates the prime factors of the provided value.
	/// First multiple is always 0, 1 or -1.
	/// </summary>
	/// <param name="value">The value to factorize.</param>
	/// <returns>
	/// An enumerable that contains the prime factors of the provided value starting with 0, 1, or -1 for sign retention.
	/// Value types may differ depending on the magnitude of the provided value.
	/// </returns>
	[SuppressMessage("Style", "IDE0046:Convert to conditional expression")]
	public static IEnumerable<float> Factors(float value)
	{
		// ReSharper disable once CompareOfFloatsByEqualityOperator
		if (float.IsNaN(value) || value == 0 || value % 1f != 0 || float.IsInfinity(value))
		{
			yield return value;
			yield break;
		}

		var sign = 1f;
		if (value < 0)
		{
			sign = -1f;
			value = Math.Abs(value);
		}

		if (value > FloatLargestContiguousInt)
		{
			throw new ArgumentOutOfRangeException(nameof(value), value,
				"Cannot accurately factor a single precision number larger than 16777216.");
		}

		yield return sign;

		// ReSharper disable once CompareOfFloatsByEqualityOperator
		if (value == 1d)
			yield break;

		foreach (var n in Factors((uint)value).Skip(1))
			yield return n;
	}

	const double DoubleLargestContiguousInt = 9007199254740992;

	/// <summary>
	/// Iterates the prime factors of the provided value.
	/// First multiple is always 0, 1 or -1.
	/// </summary>
	/// <param name="value">The value to factorize.</param>
	/// <returns>
	/// An enumerable that contains the prime factors of the provided value starting with 0, 1, or -1 for sign retention.
	/// Value types may differ depending on the magnitude of the provided value.
	/// </returns>
	[SuppressMessage("Style", "IDE0046:Convert to conditional expression")]
	public static IEnumerable<double> Factors(double value)
	{
		// ReSharper disable once CompareOfFloatsByEqualityOperator
		if (double.IsNaN(value) || value == 0 || value % 1d != 0 || double.IsInfinity(value))
		{
			yield return value;
			yield break;
		}

		var sign = 1d;
		if (value < 0)
		{
			sign = -1d;
			value = Math.Abs(value);
		}

		if (value > DoubleLargestContiguousInt)
		{
			throw new ArgumentOutOfRangeException(nameof(value), value,
				"Cannot accurately factor a double precision number larger than 9007199254740992.");
		}

		yield return sign;

		// ReSharper disable once CompareOfFloatsByEqualityOperator
		if (value == 1d)
			yield break;

		if (value <= uint.MaxValue)
		{
			// Use more efficient uint instead.
			foreach (var n in Factors((uint)value).Skip(1))
				yield return n;
		}
		else
		{
			foreach (var n in Factors((ulong)value).Skip(1))
				yield return n;
		}
	}

	/// <summary>
	/// Iterates the prime factors of the provided value.
	/// First multiple is always 0, 1 or -1.
	/// </summary>
	/// <param name="value">The value to factorize.</param>
	/// <returns>
	/// An enumerable that contains the prime factors of the provided value starting with 0, 1, or -1 for sign retention.
	/// Value types may differ depending on the magnitude of the provided value.
	/// </returns>
	public static IEnumerable<decimal> Factors(decimal value)
	{
		// ReSharper disable once CompareOfFloatsByEqualityOperator
		if (value == decimal.Zero
			|| value % decimal.One != decimal.Zero)
		{
			yield return value;
			yield break;
		}

		yield return value < decimal.Zero ? -decimal.One : decimal.One;
		value = Math.Abs(value);

		if (value == decimal.One)
			yield break;

		if (value <= uint.MaxValue)
		{
			// Use more efficient uint instead.
			foreach (var n in Factors((uint)value).Skip(1))
				yield return n;
		}
		else if (value <= ulong.MaxValue)
		{
			// Use more efficient ulong instead.
			foreach (var n in Factors((ulong)value).Skip(1))
				yield return n;
		}
		else
		{
			foreach (var b in Factors((BigInteger)value).Skip(1))
				yield return (decimal)b;
		}
	}

	/// <summary>
	/// Iterates the prime factors of the provided value.
	/// If omitOneAndValue==false, first multiple is always 0, 1 or -1.
	/// Else if the value is prime or not a whole number, then there will be no results.
	/// </summary>
	/// <param name="value">The value to factorize.</param>
	/// <param name="omitOneAndValue">If true, only positive integers greater than 1 and less than the number itself are returned.</param>
	public static IEnumerable<double> Factors(double value, bool omitOneAndValue)
		=> omitOneAndValue
			? Factors(value).Skip(1).TakeWhile(v => v != value)
			: Factors(value);

	/// <summary>
	/// Iterates the prime factors of the provided value.
	/// If omitOneAndValue==false, first multiple is always 0, 1 or -1.
	/// Else if the value is prime or not a whole number, then there will be no results.
	/// </summary>
	/// <param name="value">The value to factorize.</param>
	/// <param name="omitOneAndValue">If true, only positive integers greater than 1 and less than the number itself are returned..</param>
	public static IEnumerable<float> Factors(float value, bool omitOneAndValue)
		=> omitOneAndValue
			? Factors(value).Skip(1).TakeWhile(v => v != value)
			: Factors(value);

	/// <summary>
	/// Iterates the prime factors of the provided value.
	/// If omitOneAndValue==false, first multiple is always 0 or 1.
	/// Else if the value is prime, then there will be no results.
	/// </summary>
	/// <param name="value">The value to factorize.</param>
	/// <param name="omitOneAndValue">If true, only positive integers greater than 1 and less than the number itself are returned.</param>
	public static IEnumerable<ulong> Factors(ulong value, bool omitOneAndValue)
		=> omitOneAndValue
			? Factors(value).Skip(1).TakeWhile(v => v != value)
			: Factors(value);

	/// <summary>
	/// Iterates the prime factors of the provided value.
	/// If omitOneAndValue==false, first multiple is always 0, 1 or -1.
	/// Else if the value is prime, then there will be no results.
	/// </summary>
	/// <param name="value">The value to factorize.</param>
	/// <param name="omitOneAndValue">If true, only positive integers greater than 1 and less than the number itself are returned.</param>
	public static IEnumerable<long> Factors(long value, bool omitOneAndValue)
		=> omitOneAndValue
			? Factors(value).Skip(1).TakeWhile(v => v != value)
			: Factors(value);

	/// <summary>
	/// Iterates the prime factors of the provided value.
	/// If omitOneAndValue==false, first multiple is always 0, 1 or -1.
	/// Else if the value is prime, then there will be no results.
	/// </summary>
	/// <param name="value">The value to factorize.</param>
	/// <param name="omitOneAndValue">If true, only positive integers greater than 1 and less than the number itself are returned.</param>
	public static IEnumerable<BigInteger> Factors(BigInteger value, bool omitOneAndValue)
		=> omitOneAndValue
			? Factors(value).Skip(1).TakeWhile(v => v != value)
			: Factors(value);

	/// <summary>
	/// Iterates all the possible common prime factors of the provided numbers.
	/// </summary>
	/// <param name="values">The values to find common prime factors from.</param>
	/// <returns>An enumerable of the common prime factors.</returns>
	public static IEnumerable<long> CommonFactors(IEnumerable<long> values)
		=> CommonFactors(values.Distinct().Select(v => (ulong)Math.Abs(v)))
			.Select(Convert.ToInt64);

	/// <summary>
	/// Iterates all the possible common prime factors of the provided numbers.
	/// </summary>
	/// <param name="values">The values to find common prime factors from.</param>
	/// <returns>An enumerable of the common prime factors.</returns>
	public static IEnumerable<ulong> CommonFactors(IEnumerable<ulong> values)
	{
		//#if DEBUG
		//			foreach (var v in values.Distinct())
		//			{
		//				Debug.WriteLine($"{v}:");
		//				foreach (var f in Factors(v))
		//				{
		//					Debug.Write($"{f},");
		//				}
		//				Debug.WriteLine("\n");
		//			}
		//#endif

		// Use a persistent enumerator to get through (or fail) results.
		using var factors = values
			.Distinct()
			.Select(v => Factors(v).GetEnumerator())
			.Memoize();
		var maxFactor = -1;
		try
		{
			while (true)
			{
				// 0 = just starting a loop
				var current = 0UL;
				var retryIndex = -1;

			retry:
				var i = 0; // ** As we progress through the factors, we are attempt to keep track so as to minimize unnecessary iterations.
				foreach (var e in factors)
				{
					if (maxFactor < i)
					{
						if (!e.MoveNext() && e.Current == 0UL)
							yield break;

						maxFactor = i;
					}

					// The retryIndex means we came to a point in the list where we may come back to it and need to reuse the existing value.
					if (retryIndex == i)
						retryIndex = -1;
					else if (!e.MoveNext())
						yield break;

					if (current != 0)
					{
						// Get the next candidate...
						while (current > e.Current)
						{
							if (!e.MoveNext())
								yield break;
						}

						if (current < e.Current)
						{
							// Whoops... New first level factor...
							current = e.Current;
							retryIndex = i;
							goto retry;
						}
					}

					// Have a match? Keep checking.
					++i;
					current = e.Current;
				}

				// If we arrive here with a valid value, then it is common.
				if (current != 0)
					yield return current;
			}
		}
		finally
		{
			foreach (var d in factors)
				d.Dispose(); // dispose of the underlying enumerators before disposing the LazyList (.Memoize());
		}
	}

	/// <summary>
	/// Iterates all the possible common prime factors of the provided numbers.
	/// </summary>
	/// <param name="values">The values to find common prime factors from.</param>
	/// <returns>An enumerable of the common prime factors.</returns>
	public static IEnumerable<ulong> CommonFactors(params ulong[] values)
		=> CommonFactors((IEnumerable<ulong>)values);

	/// <summary>
	/// Iterates all the possible common prime factors of the provided numbers.
	/// </summary>
	/// <param name="values">The values to find common prime factors from.</param>
	/// <returns>An enumerable of the common prime factors.</returns>
	public static IEnumerable<BigInteger> CommonFactors(IEnumerable<BigInteger> values)
	{
		// Use a persistent enumerator to get through (or fail) results.
		using var factors = values
			.Distinct()
			.Select(v => Factors(v).GetEnumerator())
			.Memoize();

		var maxFactor = -BigInteger.One;
		try
		{
			while (true)
			{
				// 0 = just starting a loop
				var current = BigInteger.Zero;
				var retryIndex = -1;

			retry:
				var i = 0; // ** As we progress through the factors, we are attempt to keep track so as to minimize unnecessary iterations.
				foreach (var e in factors)
				{
					if (maxFactor < i)
					{
						if (!e.MoveNext() && e.Current.IsZero)
							yield break;

						maxFactor = i;
					}

					/*	The retryIndex means we came to a point in the list
						where we may come back to it and need to reuse the existing value
						before continuing. */

					if (retryIndex == i)
						retryIndex = -1;
					else if (!e.MoveNext())
						yield break;

					if (!current.IsZero)
					{
						// Get the next candidate...
						while (current > e.Current)
						{
							if (!e.MoveNext())
								yield break;
						}

						if (current < e.Current)
						{
							// Whoops... New first level factor...
							current = e.Current;
							retryIndex = i;
							goto retry;
						}
					}

					// Have a match? Keep checking.
					++i;
					current = e.Current;
				}

				// If we arrive here with a valid value, then it is common.
				if (!current.IsZero)
					yield return current;
			}
		}
		finally
		{
			foreach (var d in factors)
				d.Dispose(); // dispose of the underlying enumerators before disposing the LazyList (.Memoize());
		}
	}

	/// <summary>
	/// Iterates all the possible common prime factors of the provided numbers.
	/// </summary>
	/// <param name="values">The values to find common prime factors from.</param>
	/// <returns>An enumerable of the common prime factors.</returns>
	public static IEnumerable<BigInteger> CommonFactors(params BigInteger[] values)
		=> CommonFactors((IEnumerable<BigInteger>)values);

	/// <summary>
	/// Returns the greatest common (positive) factor (GCF) of all the provided values.
	/// Returns 1 if none found.
	/// </summary>
	/// <param name="values">The values to find common prime factors from.</param>
	/// <returns>The greatest common factor or 1 if none found.</returns>
	public static long GreatestFactor(IEnumerable<long> values)
		=> CommonFactors(values).Aggregate(1L, (p, c) => p * c);

	/// <summary>
	/// Returns the greatest common (positive) factor (GCF) of all the provided values.
	/// Returns 1 if none found.
	/// </summary>
	/// <param name="values">The values to find common prime factors from.</param>
	/// <returns>The greatest common factor or 1 if none found.</returns>
	public static ulong GreatestFactor(IEnumerable<ulong> values)
		=> CommonFactors(values).Aggregate(1UL, (p, c) => p * c);

	/// <summary>
	/// Returns the greatest common (positive) factor (GCF) of all the provided values.
	/// Returns 1 if none found.
	/// </summary>
	/// <param name="values">The values to find common prime factors from.</param>
	/// <returns>The greatest common factor or 1 if none found.</returns>
	public static ulong GreatestFactor(params ulong[] values)
		=> CommonFactors(values).Aggregate(1UL, (p, c) => p * c);

	/// <summary>
	/// Returns the greatest common (positive) factor (GCF) of all the provided values.
	/// Returns 1 if none found.
	/// </summary>
	/// <param name="values">The values to find common prime factors from.</param>
	/// <returns>The greatest common factor or 1 if none found.</returns>
	public static BigInteger GreatestFactor(IEnumerable<BigInteger> values)
		=> CommonFactors(values).Aggregate(BigInteger.One, (p, c) => p * c);

	/// <summary>
	/// Returns the greatest common (positive) factor (GCF) of all the provided values.
	/// Returns 1 if none found.
	/// </summary>
	/// <param name="values">The values to find common prime factors from.</param>
	/// <returns>The greatest common factor or 1 if none found.</returns>
	public static BigInteger GreatestFactor(params BigInteger[] values)
		=> CommonFactors(values).Aggregate(BigInteger.One, (p, c) => p * c);
}
