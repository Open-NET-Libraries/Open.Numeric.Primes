using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;

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
        public static bool IsPrime(ulong value)
        {
            return Prime.Numbers.IsPrime(value);
        }

        /// <summary>
        /// Validates if a number is prime.
        /// </summary>
        /// <param name="value">Value to verify.</param>
        /// <returns>True if the provided value is a prime number</returns>
        public static bool IsPrime(BigInteger value)
        {
            return Prime.Numbers.Big.IsPrime(value);
        }

        /// <summary>
        /// Validates if a number is prime.
        /// </summary>
        /// <param name="value">Value to verify.</param>
        /// <returns>True if the provided value is a prime number</returns>
        public static bool IsPrime(long value)
        {
            return IsPrime((ulong)Math.Abs(value));
        }

        /// <summary>
        /// Validates if a number is prime.
        /// </summary>
        /// <param name="value">Value to verify.</param>
        /// <returns>True if the provided value is a prime number</returns>
        public static bool IsPrime(double value)
        {
            if (Math.Floor(value) != value)
                return false;

            value = Math.Abs(value);
            if (value <= ulong.MaxValue)
                return IsPrime((ulong)value);

            return IsPrime((BigInteger)value);
        }

        /// <summary>
        /// Validates if a number is prime.
        /// </summary>
        /// <param name="value">Value to verify.</param>
        /// <returns>True if the provided value is a prime number</returns>
        public static bool IsPrime(decimal value)
        {
            if (Math.Floor(value) != value)
                return false;

            value = Math.Abs(value);
            if (value <= ulong.MaxValue)
                return IsPrime((ulong)value);

            return IsPrime((BigInteger)value);
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
            else if (float.IsPositiveInfinity(value))
                return double.PositiveInfinity;
            else if (float.IsNegativeInfinity(value))
                return double.PositiveInfinity;
            else
                return double.Parse(value.ToString()); // Potential underlying precision error.  Seemingly whole numbers can be slightly off and not equate properly...
        }

        public static readonly Optimized Numbers = new Optimized();

        /// <summary>
        /// Iterates the prime factors of the provided value.
        /// First multiple is always 0 or 1 (and for other overloads can be -1).
        /// </summary>
        /// <param name="value">The value to factorize.</param>
        /// <returns>An enumerable that contains the prime factors of the provided value starting with 0 or 1 for sign retention.</returns>
        public static IEnumerable<ulong> Factors(ulong value)
        {
            return Numbers.Factors(value);
        }

        /// <summary>
        /// Iterates the prime factors of the provided value.
        /// First multiple is always 0, 1 or -1.
        /// </summary>
        /// <param name="value">The value to factorize.</param>
        /// <returns>An enumerable that contains the prime factors of the provided value starting with 0, 1, or -1 for sign retention.</returns>
        public static IEnumerable<long> Factors(long value)
        {
            return Prime.Numbers.Factors(value);
        }

        /// <summary>
        /// Iterates the prime factors of the provided value.
        /// First multiple is always 0, 1 or -1.
        /// </summary>
        /// <param name="value">Value to factorize.</param>
        /// <returns>An enumerable that contains the prime factors of the provided value starting with 0, 1, or -1 for sign retention.</returns>
        public static IEnumerable<BigInteger> Factors(BigInteger value)
        {
            return Numbers.Big.Factors(value);
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
        public static IEnumerable<dynamic> Factors(double value)
        {
            if (double.IsNaN(value) || value == 0)
            {
                yield return value;
            }
            else
            {
                yield return value < 1 ? -1 : 1;
                if (value < 0) value = Math.Abs(value);
                if (value == 1L)
                    yield break;

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
                        yield break;
                    }
                    else
                    {
                        foreach (var b in Factors((BigInteger)value).Skip(1))
                            yield return b;
                        yield break;
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
        {
            return Factors(ToDouble(value));
        }

        /// <summary>
        /// Iterates the prime factors of the provided value.
        /// If omitOneAndValue==false, first multiple is always 0, 1 or -1.
        /// Else if the value is prime or not a whole number, then there will be no results.
        /// </summary>
        /// <param name="value">The value to factorize.</param>
        /// <param name="omitOneAndValue">If true, only positive integers greater than 1 and less than the number itself are returned.</param>
        public static IEnumerable<dynamic> Factors(double value, bool omitOneAndValue)
        {
            return omitOneAndValue
                ? Factors(value).Skip(1).TakeWhile(v => v != value)
                : Factors(value);
        }

        /// <summary>
        /// Iterates the prime factors of the provided value.
        /// If omitOneAndValue==false, first multiple is always 0, 1 or -1.
        /// Else if the value is prime or not a whole number, then there will be no results.
        /// </summary>
        /// <param name="value">The value to factorize.</param>
        /// <param name="omitOneAndValue">If true, only positive integers greater than 1 and less than the number itself are returned..</param>
        public static IEnumerable<dynamic> Factors(float value, bool omitOneAndValue)
        {
            return omitOneAndValue
                ? Factors(value).Skip(1).TakeWhile(v => v != value)
                : Factors(value);
        }

        /// <summary>
        /// Iterates the prime factors of the provided value.
        /// If omitOneAndValue==false, first multiple is always 0 or 1.
        /// Else if the value is prime, then there will be no results.
        /// </summary>
        /// <param name="value">The value to factorize.</param>
        /// <param name="omitOneAndValue">If true, only positive integers greater than 1 and less than the number itself are returned.</param>
        public static IEnumerable<ulong> Factors(ulong value, bool omitOneAndValue)
        {
            return omitOneAndValue
                ? Factors(value).Skip(1).TakeWhile(v => v != value)
                : Factors(value);
        }

        /// <summary>
        /// Iterates the prime factors of the provided value.
        /// If omitOneAndValue==false, first multiple is always 0, 1 or -1.
        /// Else if the value is prime, then there will be no results.
        /// </summary>
        /// <param name="value">The value to factorize.</param>
        /// <param name="omitOneAndValue">If true, only positive integers greater than 1 and less than the number itself are returned.</param>
        public static IEnumerable<long> Factors(long value, bool omitOneAndValue)
        {
            return omitOneAndValue
                ? Factors(value).Skip(1).TakeWhile(v => v != value)
                : Factors(value);
        }

        /// <summary>
        /// Iterates the prime factors of the provided value.
        /// If omitOneAndValue==false, first multiple is always 0, 1 or -1.
        /// Else if the value is prime, then there will be no results.
        /// </summary>
        /// <param name="value">The value to factorize.</param>
        /// <param name="omitOneAndValue">If true, only positive integers greater than 1 and less than the number itself are returned.</param>
        public static IEnumerable<BigInteger> Factors(BigInteger value, bool omitOneAndValue)
        {
            return omitOneAndValue
                ? Factors(value).Skip(1).TakeWhile(v => v != value)
                : Factors(value);
        }

    }

    /// <summary>
    /// Importing this namespace will expose prime extensions for most numeric value types.
    /// Or import Open.Numeric.Primes to access the static methods directly.
    /// </summary>
    namespace Extensions
    {
        public static class PrimeExtensions
        {
            /// <summary>
            /// Validates if a number is prime.
            /// </summary>
            /// <returns>True if the provided value is a prime number</returns>
            public static bool IsPrime(this ulong value)
            {
                return Number.IsPrime(value);
            }

            /// <summary>
            /// Validates if a number is prime.
            /// </summary>
            /// <returns>True if the provided value is a prime number</returns>
            public static bool IsPrime(this long value)
            {
                return Number.IsPrime(value);
            }

            /// <summary>
            /// Validates if a number is prime.
            /// </summary>
            /// <returns>True if the provided value is a prime number</returns>
            public static bool IsPrime(this int value)
            {
                return Number.IsPrime(value);
            }

            /// <summary>
            /// Validates if a number is prime.
            /// </summary>
            /// <returns>True if the provided value is a prime number</returns>
            public static bool IsPrime(this uint value)
            {
                return Number.IsPrime(value);
            }

            /// <summary>
            /// Validates if a number is prime.
            /// </summary>
            /// <returns>True if the provided value is a prime number</returns>
            public static bool IsPrime(this short value)
            {
                return Number.IsPrime(value);
            }

            /// <summary>
            /// Validates if a number is prime.
            /// </summary>
            /// <returns>True if the provided value is a prime number</returns>
            public static bool IsPrime(this ushort value)
            {
                return Number.IsPrime(value);
            }

            /// <summary>
            /// Validates if a number is prime.
            /// </summary>
            /// <returns>True if the provided value is a prime number</returns>
            public static bool IsPrime(this sbyte value)
            {
                return Number.IsPrime(value);
            }

            /// <summary>
            /// Validates if a number is prime.
            /// </summary>
            /// <returns>True if the provided value is a prime number</returns>
            public static bool IsPrime(this byte value)
            {
                return Number.IsPrime(value);
            }

            /// <summary>
            /// Validates if a number is prime.
            /// </summary>
            /// <returns>True if the provided value is a prime number</returns>
            public static bool IsPrime(this double value)
            {
                return Number.IsPrime(value);
            }

            /// <summary>
            /// Validates if a number is prime.
            /// </summary>
            /// <returns>True if the provided value is a prime number</returns>
            public static bool IsPrime(this decimal value)
            {
                return Number.IsPrime(value);
            }

            /// <summary>
            /// Validates if a number is prime.
            /// </summary>
            /// <returns>True if the provided value is a prime number</returns>
            public static bool IsPrime(this BigInteger value)
            {
                return Number.IsPrime(value);
            }

            /// <summary>
            /// Finds the next prime number after the number given.
            /// </summary>
            /// <returns>The next prime after the number provided.</returns>
            /// <exception cref="ArgumentException">Cannot coerce to a valid long value.</exception>
            public static ulong NextPrime(this ulong value)
            {
                return Prime.Numbers.Next(value);
            }

            /// <summary>
            /// Finds the next prime number after the number given.  If this number is negative, then the result will be the next greater magnitude value prime as negative number.
            /// </summary>
            /// <returns>The next prime after the number provided.</returns>
            /// <exception cref="ArgumentException">Cannot coerce to a valid long value.</exception>
            public static long NextPrime(this long value)
            {
                return Prime.Numbers.Next(value);
            }

            /// <summary>
            /// Finds the next prime number after the number given.  If this number is negative, then the result will be the next greater magnitude value prime as negative number.
            /// </summary>
            /// <returns>The next prime after the number provided.</returns>
            /// <exception cref="ArgumentException">Cannot coerce to a valid long value.</exception>
            public static BigInteger NextPrime(this float value)
            {
                return Prime.Numbers.Big.Next(value);
            }

            /// <summary>
            /// Finds the next prime number after the number given.  If this number is negative, then the result will be the next greater magnitude value prime as negative number.
            /// </summary>
            /// <returns>The next prime after the number provided.</returns>
            /// <exception cref="ArgumentException">Cannot coerce to a valid long value.</exception>
            public static BigInteger NextPrime(this double value)
            {
                return Prime.Numbers.Big.Next(value);
            }

            /// <summary>
            /// Finds the next prime number after the number given.  If this number is negative, then the result will be the next greater magnitude value prime as negative number.
            /// </summary>
            /// <returns>The next prime after the number provided.</returns>
            /// <exception cref="ArgumentException">Cannot coerce to a valid long value.</exception>
            public static BigInteger NextPrime(this BigInteger value)
            {
                return Prime.Numbers.Big.Next(value);
            }

            /// <summary>
            /// Iterates the prime factors of the provided value.
            /// If omitOneAndValue==false, first multiple is always 0 or 1.
            /// Else if the value is prime, then there will be no results.
            /// </summary>
            /// <param name="omitOneAndValue">If true, only positive integers greater than 1 and less than the number itself are returned.</param>
            public static IEnumerable<ulong> PrimeFactors(this ulong value, bool omitOneAndValue = false)
            {
                return Prime.Factors(value, omitOneAndValue);
            }

            /// <summary>
            /// Iterates the prime factors of the provided value.
            /// If omitOneAndValue==false, first multiple is always 0, 1 or -1.
            /// Else if the value is prime, then there will be no results.
            /// </summary>
            /// <param name="omitOneAndValue">If true, only positive integers greater than 1 and less than the number itself are returned.</param>
            public static IEnumerable<long> PrimeFactors(this long value, bool omitOneAndValue = false)
            {
                return Prime.Factors(value, omitOneAndValue);
            }

            /// <summary>
            /// Iterates the prime factors of the provided value.
            /// If omitOneAndValue==false, first multiple is always 0, 1 or -1.
            /// Else if the value is prime, then there will be no results.
            /// </summary>
            /// <param name="omitOneAndValue">If true, only positive integers greater than 1 and less than the number itself are returned.</param>
            public static IEnumerable<BigInteger> PrimeFactors(this BigInteger value, bool omitOneAndValue = false)
            {
                return Prime.Factors(value, omitOneAndValue);
            }

            /// <summary>
            /// Iterates the prime factors of the provided value.
            /// If omitOneAndValue==false, first multiple is always 0, 1 or -1.
            /// Else if the value is prime or not a whole number, then there will be no results.
            /// </summary>
            /// <param name="omitOneAndValue">If true, only positive integers greater than 1 and less than the number itself are returned.</param>
            public static IEnumerable<dynamic> PrimeFactors(this double value, bool omitOneAndValue = false)
            {
                return Prime.Factors(value, omitOneAndValue);
            }

            /// <summary>
            /// Iterates the prime factors of the provided value.
            /// If omitOneAndValue==false, first multiple is always 0, 1 or -1.
            /// Else if the value is prime or not a whole number, then there will be no results.
            /// </summary>
            /// <param name="omitOneAndValue">If true, only positive integers greater than 1 and less than the number itself are returned.</param>
            public static IEnumerable<dynamic> PrimeFactors(this float value, bool omitOneAndValue = false)
            {
                return Prime.Factors(value, omitOneAndValue);
            }
        }
    }
}

