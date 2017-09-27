using System;
using System.Collections.Generic;
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
        static bool IsPrimeInternal(ulong value)
        {
            if (value < 380000)
            {
                // This method is faster up until a point.
                double squared = Math.Sqrt(value);
                ulong flooredAndSquared = Convert.ToUInt64(Math.Floor(squared));

                for (ulong idx = 3; idx <= flooredAndSquared; idx++)
                {
                    if (value % idx == 0)
                    {
                        return false;
                    }
                }
            }
            else
            {
                ulong divisor = 6;
                while (divisor * divisor - 2 * divisor + 1 <= value)
                {

                    if (value % (divisor - 1) == 0)
                        return false;

                    if (value % (divisor + 1) == 0)
                        return false;

                    divisor += 6;
                }
            }



            return true;
        }

        static bool IsPrimeInternal(BigInteger value)
        {
            BigInteger divisor = 6;
            while (divisor * divisor - 2 * divisor + 1 <= value)
            {

                if (value % (divisor - 1) == 0)
                    return false;

                if (value % (divisor + 1) == 0)
                    return false;

                divisor += 6;
            }

            return true;
        }

        /// <summary>
        /// Validates if a number is prime.
        /// </summary>
        /// <param name="value">Value to verify.</param>
        /// <returns>True if the provided value is a prime number</returns>
        public static bool IsPrime(ulong value)
        {
            switch (value)
            {
                // 0 and 1 are not prime numbers
                case 0:
                case 1:
                    return false;

                case 2:
                case 3:
                    return true;

                default:
                    return value % 2 != 0
                        && value % 3 != 0
                        && IsPrimeInternal(value);
            }

        }

        /// <summary>
        /// Validates if a number is prime.
        /// </summary>
        /// <param name="value">Value to verify.</param>
        /// <returns>True if the provided value is a prime number</returns>
        public static bool IsPrime(BigInteger value)
        {
            value = BigInteger.Abs(value);
            if (value == 0 || value == 1)
                return false;
            if (value == 2 || value == 3)
                return true;

            return value % 2 != 0
                && value % 3 != 0
                && IsPrimeInternal(value);
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
            if(Math.Floor(value) != value)
                return false;

            value = Math.Abs(value);
            if(value<=ulong.MaxValue)
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

        internal static IEnumerable<ulong> ValidPrimeTests(ulong staringAt = 2)
        {
            var n = staringAt;
            if (n > 2)
            {
                if (n % 2 == 0)
                    n++;
            }
            else
            {
                yield return 2;
                n = 3;
            }

            for (; n < ulong.MaxValue - 1; n += 2)
                yield return n;
        }

        internal static IEnumerable<long> ValidPrimeTests(long staringAt)
        {
            var sign = staringAt < 0 ? -1 : 1;
            var n = Math.Abs(staringAt);

            if (n > 2)
            {
                if (n % 2 == 0)
                    n++;
            }
            else
            {
                yield return sign * 2;
                n = 3;
            }

            for (; n < long.MaxValue - 1; n += 2)
                yield return sign * n;
        }

        internal static IEnumerable<BigInteger> ValidPrimeTestsBig(BigInteger? staringAt = null)
        {
            var sign = staringAt?.Sign ?? 1;
            if(sign==0) sign = 1;
            var n = BigInteger.Abs(staringAt ?? BigInteger.One);

            if (n > 2)
            {
                if (n % 2 == 0)
                    n++;
            }
            else
            {
                yield return sign * 2;
                n = 3;
            }

            while(true)
            {
                yield return sign * n;
                n += 2;
            }
        }

        /// <summary>
        /// Returns an enumerable that will iterate every prime starting at the starting value.
        /// </summary>
        /// <param name="staringAt">Allows for skipping ahead any integer before checking for inclusive and subsequent primes.</param>
        /// <returns>An enumerable that will iterate every prime starting at the starting value</returns>
        public static IEnumerable<BigInteger> NumbersBig(BigInteger? staringAt = null)
        {
            if(staringAt>=ulong.MaxValue)
            {
                return ValidPrimeTestsBig(staringAt)
                    .Where(v => Number.IsPrime(v));
            }

            // Avoid potential 'big' math up until ulong.MaxValue.
            return ValidPrimeTests((ulong)staringAt)
                .Where(v => Number.IsPrime(v))
                .Cast<BigInteger>()
                .Concat(NumbersBig(ulong.MaxValue));
        }

        /// <summary>
        /// Returns an enumerable that will iterate every prime starting at the starting value.
        /// </summary>
        /// <param name="staringAt">Allows for skipping ahead any integer before checking for inclusive and subsequent primes.</param>
        /// <returns>An enumerable that will iterate every prime starting at the starting value</returns>
        public static IEnumerable<ulong> Numbers(ulong staringAt = 2)
        {
            return ValidPrimeTests(staringAt)
                .Where(v => Number.IsPrime(v));
        }

        /// <summary>
        /// Returns an enumerable that will iterate every prime starting at the starting value.
        /// </summary>
        /// <param name="staringAt">Allows for skipping ahead any integer before checking for inclusive and subsequent primes.  Passing a negative number here will produce a negative set of prime numbers.</param>
        /// <returns>An enumerable that will iterate every prime starting at the starting value</returns>
        public static IEnumerable<long> Numbers(long staringAt)
        {
            return ValidPrimeTests(staringAt)
                .Where(v => Number.IsPrime(v));
        }

        /// <summary>
        /// Returns an enumerable of key-value pairs that will iterate every prime starting at the starting value where the key is the count (index starting at 1) of the set.
        /// So the first entry is always {Key=1, Value=2}.
        /// </summary>
        public static IEnumerable<KeyValuePair<BigInteger, BigInteger>> NumbersIndexedBig()
        {
            var count = BigInteger.Zero;
            foreach(var n in NumbersBig())
            {
                count++;
                yield return new KeyValuePair<BigInteger, BigInteger>(count, n);
            }
        }

        /// <summary>
        /// Returns an enumerable of key-value pairs that will iterate every prime starting at the starting value where the key is the count (index starting at 1) of the set.
        /// So the first entry is always {Key=1, Value=2}.
        /// </summary>
        public static IEnumerable<KeyValuePair<ulong, ulong>> NumbersIndexed()
        {
            ulong count = 0L;
            foreach (var n in Numbers())
            {
                count++;
                yield return new KeyValuePair<ulong, ulong>(count, n);
            }
        }

        /// <summary>
        /// Returns a parallel enumerable that will iterate every prime starting at the starting value.
        /// </summary>
        /// <param name="staringAt">Allows for skipping ahead any integer before checking for inclusive and subsequent primes.</param>
        /// <param name="degreeOfParallelism">Operates in parallel unless 1 is specified.</param>
        /// <returns></returns>
        public static ParallelQuery<ulong> NumbersInParallel(ulong staringAt = 2, ushort? degreeOfParallelism = null)
        {
            var tests = ValidPrimeTests(staringAt)
                .AsParallel().AsOrdered();

            if (degreeOfParallelism.HasValue)
                tests = tests.WithDegreeOfParallelism(degreeOfParallelism.Value);

            return tests.Where(v => Number.IsPrime(v));
        }

        /// <summary>
        /// Returns a parallel enumerable that will iterate every prime starting at the starting value.
        /// </summary>
        /// <param name="staringAt">Allows for skipping ahead any integer before checking for inclusive and subsequent primes.</param>
        /// <param name="degreeOfParallelism">Operates in parallel unless 1 is specified.</param>
        /// <returns></returns>
        public static ParallelQuery<long> NumbersInParallel(long staringAt, ushort? degreeOfParallelism = null)
        {
            var tests = ValidPrimeTests(staringAt)
                .AsParallel().AsOrdered();

            if (degreeOfParallelism.HasValue)
                tests = tests.WithDegreeOfParallelism(degreeOfParallelism.Value);

            return tests.Where(v => Number.IsPrime(v));
        }

        /// <summary>
        /// Returns a parallel enumerable that will iterate every prime starting at the starting value.
        /// </summary>
        /// <param name="staringAt">Allows for skipping ahead any integer before checking for inclusive and subsequent primes.</param>
        /// <param name="degreeOfParallelism">Operates in parallel unless 1 is specified.</param>
        /// <returns></returns>
        public static ParallelQuery<BigInteger> NumbersBigInParallel(BigInteger? staringAt = null, ushort? degreeOfParallelism = null)
        {
            var s = staringAt ?? BigInteger.One;
            if (s >= ulong.MaxValue)
            {
                var testsBig = ValidPrimeTestsBig(s)
                    .AsParallel().AsOrdered();

                if (degreeOfParallelism.HasValue)
                    testsBig = testsBig.WithDegreeOfParallelism(degreeOfParallelism.Value);

                return testsBig.Where(v => Number.IsPrime(v));
            }

            var tests = ValidPrimeTests((ulong)s)
                    .AsParallel().AsOrdered();

            if (degreeOfParallelism.HasValue)
                tests = tests.WithDegreeOfParallelism(degreeOfParallelism.Value);

            return tests
                .Where(v => Number.IsPrime(v))
                .Cast<BigInteger>()
                .Concat(NumbersBigInParallel(ulong.MaxValue));
        }

        /// <summary>
        /// Finds the next prime number after the number given.
        /// </summary>
        /// <param name="after">The excluded lower boundary to start with.</param>
        /// <returns>The next prime after the number provided.</returns>
        public static ulong Next(ulong after)
        {
            return Numbers(after + 1).First();
        }

        /// <summary>
        /// Finds the next prime number after the number given.
        /// </summary>
        /// <param name="after">The excluded lower boundary to start with.</param>
        /// <returns>The next prime after the number provided.</returns>
        public static BigInteger Next(BigInteger after)
        {
            return NumbersBig(after + BigInteger.One).First();
        }

        /// <summary>
        /// Finds the next prime number after the number given.
        /// </summary>
        /// <param name="after">The excluded lower boundary to start with.  If this number is negative, then the result will be the next greater magnitude value prime as negative number.</param>
        /// <returns>The next prime after the number provided.</returns>
        public static long Next(long after)
        {
            var sign = after < 0 ? -1 : 1;
            after = Math.Abs(after);
            return sign * Numbers(after + 1).First();
        }

        /// <summary>
        /// Finds the next prime number after the number given.
        /// </summary>
        /// <param name="after">The excluded lower boundary to start with.  If this number is negative, then the result will be the next greater magnitude value prime as negative number.</param>
        /// <returns>The next prime after the number provided.</returns>
        /// <exception cref="ArgumentException">Cannot coerce to a valid long value.</exception>
        public static BigInteger Next(double after)
        {
            return Next((BigInteger)after);
        }

        /// <summary>
        /// Finds the next prime number after the number given.
        /// </summary>
        /// <param name="after">The excluded lower boundary to start with.  If this number is negative, then the result will be the next greater magnitude value prime as negative number.</param>
        /// <returns>The next prime after the number provided.</returns>
        /// <exception cref="ArgumentException">Cannot coerce to a valid integer value.</exception>
        public static BigInteger Next(float after)
        {
            return Next((BigInteger)after); // Any problematic precision error is negated by the conversion to a whole number.
        }

        /// <summary>
        /// Finds the next prime number after the number given.
        /// </summary>
        /// <param name="after">The excluded lower boundary to start with.  If this number is negative, then the result will be the next greater magnitude value prime as negative number.</param>
        /// <returns>The next prime after the number provided.</returns>
        public static BigInteger Next(decimal after)
        {
            return Next((BigInteger)after);
        }


        /// <summary>
        /// Iterates the prime factors of the provided value.
        /// First multiple is always 0 or 1 (and for other overloads can be -1).
        /// </summary>
        /// <param name="value">The value to factorize.</param>
        /// <returns>An enumerable that contains the prime factors of the provided value starting with 0 or 1 for sign retention.</returns>
        public static IEnumerable<ulong> Factors(ulong value)
        {
            if (value != 0UL)
            {
                yield return 1;
                ulong last = 1;

                // For larger numbers, a quick prime check can prevent large iterations.
                if(!Number.IsPrime(value)) foreach (var p in Numbers())
                {
                    ulong stop = value / last; // The list of possibilities shrinks for each test.
                    if (p > stop) break; // Exceeded possibilities? 
                    while ((value % p) == 0)
                    {
                        value /= p;
                        yield return p;
                        if (value == 1) yield break;
                    }
                    last = p;
                }
            }

            yield return value;
        }

        /// <summary>
        /// Iterates the prime factors of the provided value.
        /// First multiple is always 0, 1 or -1.
        /// </summary>
        /// <param name="value">The value to factorize.</param>
        /// <returns>An enumerable that contains the prime factors of the provided value starting with 0, 1, or -1 for sign retention.</returns>
        public static IEnumerable<long> Factors(long value)
        {
            if (value != 0L)
            {
                yield return value < 0L ? -1L : 1L;
                if (value < 0L) value = Math.Abs(value);
                if (value == 1L)
                    yield break;

                long last = 1L;

                // For larger numbers, a quick prime check can prevent large iterations.
                if (!Number.IsPrime(value)) foreach (var p in Numbers(2L))
                {
                    long stop = value / last; // The list of possibilities shrinks for each test.
                    if (p > stop) break; // Exceeded possibilities? 
                    while ((value % p) == 0)
                    {
                        value /= p;
                        yield return p;
                        if (value == 1) yield break;
                    }
                    last = p;
                }
            }
            yield return value;
        }

        /// <summary>
        /// Iterates the prime factors of the provided value.
        /// First multiple is always 0, 1 or -1.
        /// </summary>
        /// <param name="value">Value to factorize.</param>
        /// <returns>An enumerable that contains the prime factors of the provided value starting with 0, 1, or -1 for sign retention.</returns>
        public static IEnumerable<BigInteger> Factors(BigInteger value)
        {
            if (value != BigInteger.Zero)
            {
                yield return value < BigInteger.Zero ? BigInteger.MinusOne : BigInteger.One;
                value = BigInteger.Abs(value);
                if (value == BigInteger.One)
                    yield break;

                if(value<=ulong.MaxValue)
                {
                    // Use more efficient ulong instead.
                    foreach (var n in Factors((ulong)value).Skip(1))
                        yield return n;
                    yield break;
                }
                else
                {
                    BigInteger last = BigInteger.One;

                    // For larger numbers, a quick prime check can prevent large iterations.
                    if (!Number.IsPrime(value)) foreach (var p in NumbersBig())
                    {
                        BigInteger stop = value / last; // The list of possibilities shrinks for each test.
                        if (p > stop) break; // Exceeded possibilities? 
                        while ((value % p) == 0)
                        {
                            value /= p;
                            yield return p;
                            if (value == 1) yield break;
                        }
                        last = p;
                    }
                }


            }

            yield return value;
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
            if (double.IsNaN(value) || value==0)
            {
                yield return value;
            }
            else
            {
                yield return value < 1 ? -1 : 1;
                if (value < 0) value = Math.Abs(value);
                if (value == 1L)
                    yield break;

                if(value!=Math.Floor(value) || double.IsInfinity(value))
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
                return Prime.Next(value);
            }

            /// <summary>
            /// Finds the next prime number after the number given.  If this number is negative, then the result will be the next greater magnitude value prime as negative number.
            /// </summary>
            /// <returns>The next prime after the number provided.</returns>
            /// <exception cref="ArgumentException">Cannot coerce to a valid long value.</exception>
            public static long NextPrime(this long value)
            {
                return Prime.Next(value);
            }

            /// <summary>
            /// Finds the next prime number after the number given.  If this number is negative, then the result will be the next greater magnitude value prime as negative number.
            /// </summary>
            /// <returns>The next prime after the number provided.</returns>
            /// <exception cref="ArgumentException">Cannot coerce to a valid long value.</exception>
            public static BigInteger NextPrime(this float value)
            {
                return Prime.Next(value);
            }

            /// <summary>
            /// Finds the next prime number after the number given.  If this number is negative, then the result will be the next greater magnitude value prime as negative number.
            /// </summary>
            /// <returns>The next prime after the number provided.</returns>
            /// <exception cref="ArgumentException">Cannot coerce to a valid long value.</exception>
            public static BigInteger NextPrime(this double value)
            {
                return Prime.Next(value);
            }

            /// <summary>
            /// Finds the next prime number after the number given.  If this number is negative, then the result will be the next greater magnitude value prime as negative number.
            /// </summary>
            /// <returns>The next prime after the number provided.</returns>
            /// <exception cref="ArgumentException">Cannot coerce to a valid long value.</exception>
            public static BigInteger NextPrime(this BigInteger value)
            {
                return Prime.Next(value);
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

