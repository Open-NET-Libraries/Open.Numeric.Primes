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
		public static bool IsPrime(in ulong value)
			=> Prime.Numbers.IsPrime(in value);

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
		public static bool IsPrime(BigInteger value)
			=> Prime.Numbers.Big.IsPrime(in value);

		/// <summary>
		/// Validates if a number is prime.
		/// </summary>
		/// <param name="value">Value to verify.</param>
		/// <returns>True if the provided value is a prime number</returns>
		public static bool IsPrime(long value)
			=> Prime.Numbers.IsPrime(value);

		/// <summary>
		/// Validates if a number is prime.
		/// </summary>
		/// <param name="value">Value to verify.</param>
		/// <returns>True if the provided value is a prime number</returns>
		public static bool IsPrime(double value)
		{
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if (Math.Floor(value) != value)
				return false;

			value = Math.Abs(value);
			return value <= ulong.MaxValue ? IsPrime((ulong)value) : IsPrime((BigInteger)value);
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
		static double ToDouble(float value)
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

		public static readonly Optimized Numbers = new Optimized();

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

	}


	namespace Extensions
	{
		public static class PrimeExtensions
		{
			/// <summary>
			/// Validates if a number is prime.
			/// </summary>
			/// <returns>True if the provided value is a prime number</returns>
			public static bool IsPrime(this ulong value)
				=> Number.IsPrime(in value);

			/// <summary>
			/// Validates if a number is prime.
			/// </summary>
			/// <returns>True if the provided value is a prime number</returns>
			public static bool IsPrime(this long value)
				=> Number.IsPrime(value);

			/// <summary>
			/// Validates if a number is prime.
			/// </summary>
			/// <returns>True if the provided value is a prime number</returns>
			public static bool IsPrime(this int value)
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

		}
	}
}
