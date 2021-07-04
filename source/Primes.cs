using Open.Collections;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace Open.Numeric.Primes
{
	/// <summary>
	/// A useful set of prime detection functions.
	/// Unique overloads for certain number types including BigInteger in order to ensure efficiency and compiler optimizations.
	/// </summary>
	public static class Number
	{
		/// <summary>
		/// Validates if a number is prime.
		/// </summary>
		/// <param name="value">Value to verify.</param>
		/// <returns>True if the provided value is a prime number</returns>
		public static bool IsPrime(uint value)
			=> Polynomial.IsPrimeInternal(value);

		/// <summary>
		/// Validates if a number is prime.
		/// </summary>
		/// <param name="value">Value to verify.</param>
		/// <returns>True if the provided value is a prime number</returns>
		public static bool IsPrime(in ulong value)
			=> Prime.Numbers.IsPrime(in value);

		// Overload for use with simplified delegate use.
		/// <summary>
		/// Validates if a number is prime.
		/// </summary>
		/// <param name="value">Value to verify.</param>
		/// <returns>True if the provided value is a prime number</returns>
		public static bool IsPrime(ulong value)
			=> Prime.Numbers.IsPrime(in value);

		/// <summary>
		/// Validates if a number is prime.
		/// </summary>
		/// <param name="value">Value to verify.</param>
		/// <returns>True if the provided value is a prime number</returns>
		public static bool IsPrime(in BigInteger value)
			=> Prime.Numbers.Big.IsPrime(in value);

		/// <summary>
		/// Validates if a number is prime.
		/// </summary>
		/// <param name="value">Value to verify.</param>
		/// <returns>True if the provided value is a prime number</returns>
		public static bool IsPrime(in long value)
			=> Prime.Numbers.IsPrime(in value);

		/// <summary>
		/// Validates if a number is prime.
		/// </summary>
		/// <param name="value">Value to verify.</param>
		/// <returns>True if the provided value is a prime number</returns>
		public static bool IsPrime(in double value)
		{
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if (Math.Floor(value) != value)
				return false;

			var abs = Math.Abs(value);
			return value <= ulong.MaxValue
				? IsPrime((ulong)abs)
				: IsPrime((BigInteger)abs);
		}

		/// <summary>
		/// Validates if a number is prime.
		/// </summary>
		/// <param name="value">Value to verify.</param>
		/// <returns>True if the provided value is a prime number</returns>
		public static bool IsPrime(in decimal value)
		{
			switch (value)
			{
				case decimal.Zero:
				case decimal.One:
				case decimal.MinusOne:
					return false;
			}

			if (Math.Floor(value) != value)
				return false;

			if (value > 0)
				return value <= ulong.MaxValue
					? IsPrime((ulong)value)
					: IsPrime((BigInteger)value);

			var v = Math.Abs(value);
			return v <= ulong.MaxValue
				? IsPrime((ulong)v)
				: IsPrime((BigInteger)v);
		}
	}

	/// <summary>
	/// A useful set of prime discovery and prime factorization functions.
	/// Unique overloads for certain number types including BigInteger in order to ensure efficiency and compiler optimizations.
	/// Negative numbers are allowed and the signs are preserved.
	/// </summary>
	public static class Prime
	{
		static double ToDouble(in float value)
		{
			// Need to propertly convert a float to double to avoid potential precision error.
			if (float.IsNaN(value))
				return double.NaN;

			if (float.IsPositiveInfinity(value))
				return double.PositiveInfinity;

			return float.IsNegativeInfinity(value)
				? double.NegativeInfinity
				: double.Parse(value.ToString(CultureInfo.InvariantCulture));
		}

		public static readonly Optimized Numbers = new();

		/// <summary>
		/// Iterates the prime factors of the provided value.
		/// First multiple is always 0 or 1 (and for other overloads can be -1).
		/// </summary>
		/// <param name="value">The value to factorize.</param>
		/// <returns>An enumerable that contains the prime factors of the provided value starting with 0 or 1 for sign retention.</returns>
		public static IEnumerable<ulong> Factors(ulong value)
			=> Numbers.Factors(value);

		/// <summary>
		/// Iterates the prime factors of the provided value.
		/// First multiple is always 0, 1 or -1.
		/// </summary>
		/// <param name="value">The value to factorize.</param>
		/// <returns>An enumerable that contains the prime factors of the provided value starting with 0, 1, or -1 for sign retention.</returns>
		public static IEnumerable<long> Factors(long value)
			=> Numbers.Factors(value);


		/// <summary>
		/// Iterates the prime factors of the provided value.
		/// First multiple is always 0, 1 or -1.
		/// </summary>
		/// <param name="value">Value to factorize.</param>
		/// <returns>An enumerable that contains the prime factors of the provided value starting with 0, 1, or -1 for sign retention.</returns>
		public static IEnumerable<BigInteger> Factors(BigInteger value)
			=> Numbers.Big.Factors(value);

		/// <summary>
		/// Iterates the prime factors of the provided value.
		/// First multiple is always 0, 1 or -1.
		/// </summary>
		/// <param name="value">The value to factorize.</param>
		/// <returns>
		/// An enumerable that contains the prime factors of the provided value starting with 0, 1, or -1 for sign retention.
		/// Value types may differ depending on the magnitude of the provided value.
		/// </returns>
		public static IEnumerable<dynamic> Factors(double value)
		{
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if (double.IsNaN(value) || value == 0)
			{
				yield return value;
			}
			else
			{
				yield return value < 1 ? -1 : 1;
				if (value < 0) value = Math.Abs(value);
				// ReSharper disable once CompareOfFloatsByEqualityOperator
				if (value == 1d)
					yield break;

				// ReSharper disable once CompareOfFloatsByEqualityOperator
				if (value != Math.Floor(value) || double.IsInfinity(value))
				{
					yield return value;
				}
				else
				{
					if (value <= ulong.MaxValue)
					{
						// Use more efficient ulong instead.
						foreach (var n in Factors((ulong)value).Skip(1))
							yield return n;
					}
					else
					{
						foreach (var b in Factors((BigInteger)value).Skip(1))
							yield return b;
					}
				}
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
		public static IEnumerable<dynamic> Factors(float value)
			=> Factors(ToDouble(value));

		/// <summary>
		/// Iterates the prime factors of the provided value.
		/// If omitOneAndValue==false, first multiple is always 0, 1 or -1.
		/// Else if the value is prime or not a whole number, then there will be no results.
		/// </summary>
		/// <param name="value">The value to factorize.</param>
		/// <param name="omitOneAndValue">If true, only positive integers greater than 1 and less than the number itself are returned.</param>
		public static IEnumerable<dynamic> Factors(double value, bool omitOneAndValue)
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
		public static IEnumerable<dynamic> Factors(float value, bool omitOneAndValue)
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
					var i = 0; // ** As we progress through the factors, we are attempt to keep track so as to minimize unnecessary iteratons.
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
			var maxFactor = -1;
			try
			{
				while (true)
				{

					// 0 = just starting a loop
					var current = BigInteger.Zero;
					var retryIndex = -1;

				retry:
					var i = 0; // ** As we progress through the factors, we are attempt to keep track so as to minimize unnecessary iteratons.
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


	namespace Extensions
	{
		public static class PrimeExtensions
		{
			/// <summary>
			/// Validates if a number is prime.
			/// </summary>
			/// <returns>True if the provided value is a prime number</returns>
			public static bool IsPrime(in this ulong value)
				=> Number.IsPrime(in value);

			/// <summary>
			/// Validates if a number is prime.
			/// </summary>
			/// <returns>True if the provided value is a prime number</returns>
			public static bool IsPrime(in this long value)
				=> Number.IsPrime(in value);

			/// <summary>
			/// Validates if a number is prime.
			/// </summary>
			/// <returns>True if the provided value is a prime number</returns>
			public static bool IsPrime(in this int value)
				=> Number.IsPrime(value);

			/// <summary>
			/// Validates if a number is prime.
			/// </summary>
			/// <returns>True if the provided value is a prime number</returns>
			public static bool IsPrime(this uint value)
				=> Number.IsPrime(value);

			/// <summary>
			/// Validates if a number is prime.
			/// </summary>
			/// <returns>True if the provided value is a prime number</returns>
			public static bool IsPrime(this short value)
				=> Number.IsPrime(value);

			/// <summary>
			/// Validates if a number is prime.
			/// </summary>
			/// <returns>True if the provided value is a prime number</returns>
			public static bool IsPrime(this ushort value)
				=> Number.IsPrime(value);

			/// <summary>
			/// Validates if a number is prime.
			/// </summary>
			/// <returns>True if the provided value is a prime number</returns>
			public static bool IsPrime(this sbyte value)
				=> Number.IsPrime(value);

			/// <summary>
			/// Validates if a number is prime.
			/// </summary>
			/// <returns>True if the provided value is a prime number</returns>
			public static bool IsPrime(this byte value)
				=> Number.IsPrime(value);


			/// <summary>
			/// Validates if a number is prime.
			/// </summary>
			/// <returns>True if the provided value is a prime number</returns>
			public static bool IsPrime(this double value)
				=> Number.IsPrime(value);


			/// <summary>
			/// Validates if a number is prime.
			/// </summary>
			/// <returns>True if the provided value is a prime number</returns>
			public static bool IsPrime(this decimal value)
				=> Number.IsPrime(in value);

			/// <summary>
			/// Validates if a number is prime.
			/// </summary>
			/// <returns>True if the provided value is a prime number</returns>
			public static bool IsPrime(this BigInteger value)
				=> Number.IsPrime(in value);

			/// <summary>
			/// Finds the next prime number after the number given.
			/// </summary>
			/// <returns>The next prime after the number provided.</returns>
			/// <exception cref="ArgumentException">Cannot coerce to a valid long value.</exception>
			public static ulong NextPrime(this ulong value)
				=> Prime.Numbers.Next(in value);

			/// <summary>
			/// Finds the next prime number after the number given.  If this number is negative, then the result will be the next greater magnitude value prime as negative number.
			/// </summary>
			/// <returns>The next prime after the number provided.</returns>
			public static long NextPrime(this long value)
				=> Prime.Numbers.Next(in value);

			/// <summary>
			/// Finds the next prime number after the number given.  If this number is negative, then the result will be the next greater magnitude value prime as negative number.
			/// </summary>
			/// <returns>The next prime after the number provided.</returns>
			public static BigInteger NextPrime(this float value)
				=> Prime.Numbers.Big.Next(value);

			/// <summary>
			/// Finds the next prime number after the number given.  If this number is negative, then the result will be the next greater magnitude value prime as negative number.
			/// </summary>
			/// <returns>The next prime after the number provided.</returns>
			/// <exception cref="ArgumentException">Cannot coerce to a valid long value.</exception>
			public static BigInteger NextPrime(this double value)
				=> Prime.Numbers.Big.Next(value);

			/// <summary>
			/// Finds the next prime number after the number given.  If this number is negative, then the result will be the next greater magnitude value prime as negative number.
			/// </summary>
			/// <returns>The next prime after the number provided.</returns>
			/// <exception cref="ArgumentException">Cannot coerce to a valid long value.</exception>
			public static BigInteger NextPrime(this BigInteger value)
				=> Prime.Numbers.Big.Next(in value);

			/// <summary>
			/// Iterates the prime factors of the provided value.
			/// If omitOneAndValue==false, first multiple is always 0 or 1.
			/// Else if the value is prime, then there will be no results.
			/// </summary>
			/// <param name="value">The value to factor.</param>
			/// <param name="omitOneAndValue">If true, only positive integers greater than 1 and less than the number itself are returned.</param>
			public static IEnumerable<ulong> PrimeFactors(this ulong value, bool omitOneAndValue = false)
				=> Prime.Factors(value, omitOneAndValue);

			/// <summary>
			/// Iterates the prime factors of the provided value.
			/// If omitOneAndValue==false, first multiple is always 0, 1 or -1.
			/// Else if the value is prime, then there will be no results.
			/// </summary>
			/// <param name="value">The value to factor.</param>
			/// <param name="omitOneAndValue">If true, only positive integers greater than 1 and less than the number itself are returned.</param>
			public static IEnumerable<long> PrimeFactors(this long value, bool omitOneAndValue = false)
				=> Prime.Factors(value, omitOneAndValue);

			/// <summary>
			/// Iterates the prime factors of the provided value.
			/// If omitOneAndValue==false, first multiple is always 0, 1 or -1.
			/// Else if the value is prime, then there will be no results.
			/// </summary>
			/// <param name="value">The value to factor.</param>
			/// <param name="omitOneAndValue">If true, only positive integers greater than 1 and less than the number itself are returned.</param>
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
	}
}
