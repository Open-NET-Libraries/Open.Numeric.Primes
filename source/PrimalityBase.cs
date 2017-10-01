using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using Open.Collections;

namespace Open.Numeric.Primes
{
    public abstract class PrimalityBase<T>
        where T : struct
    {

        protected abstract IEnumerable<T> ValidPrimeTests(T startingAt);

        public abstract bool IsPrime(T value);

        public virtual IEnumerable<T> Numbers(T startingAt)
        {
            return ValidPrimeTests(startingAt)
                .Where(v => IsPrime(v));
        }
        public abstract IEnumerable<T> Numbers();

        public abstract IEnumerable<KeyValuePair<T, T>> NumbersIndexed();

        public abstract ParallelQuery<T> NumbersInParallel(T staringAt, ushort? degreeOfParallelism = null);

        public abstract ParallelQuery<T> NumbersInParallel(ushort? degreeOfParallelism = null);

        public abstract IEnumerable<T> Factors(T value);
    }

    public abstract class PrimalityU64Base : PrimalityBase<ulong>
    {
        protected override IEnumerable<ulong> ValidPrimeTests(ulong staringAt = 2)
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

        public override IEnumerable<ulong> Numbers()
        {
            return Numbers(2);
        }


        /// <summary>
        /// Returns an enumerable of key-value pairs that will iterate every prime starting at the starting value where the key is the count (index starting at 1) of the set.
        /// So the first entry is always {Key=1, Value=2}.
        /// </summary>
        public override IEnumerable<KeyValuePair<ulong, ulong>> NumbersIndexed()
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
        public override ParallelQuery<ulong> NumbersInParallel(ulong staringAt, ushort? degreeOfParallelism = null)
        {
            var tests = ValidPrimeTests(staringAt)
                .AsParallel().AsOrdered();

            if (degreeOfParallelism.HasValue)
                tests = tests.WithDegreeOfParallelism(degreeOfParallelism.Value);

            return tests.Where(v => Number.IsPrime(v));
        }

        /// <summary>
        /// Returns a parallel enumerable that will iterate every prime.
        /// </summary>
        /// <param name="degreeOfParallelism">Operates in parallel unless 1 is specified.</param>
        /// <returns></returns>
        public override ParallelQuery<ulong> NumbersInParallel(ushort? degreeOfParallelism = null)
        {
            return NumbersInParallel(2, degreeOfParallelism);
        }


        /// <summary>
        /// Iterates the prime factors of the provided value.
        /// First multiple is always 0 or 1 (and for other overloads can be -1).
        /// </summary>
        /// <param name="value">The value to factorize.</param>
        /// <returns>An enumerable that contains the prime factors of the provided value starting with 0 or 1 for sign retention.</returns>
        public override IEnumerable<ulong> Factors(ulong value)
        {
            if (value != 0UL)
            {
                yield return 1;
                ulong last = 1;

                // For larger numbers, a quick prime check can prevent large iterations.
                foreach (var p in Numbers())
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

    }

    public abstract class PrimalityBigIntBase : PrimalityBase<BigInteger>
    {
        protected override IEnumerable<BigInteger> ValidPrimeTests(BigInteger staringAt)
        {
            var sign = staringAt.Sign;
            if (sign == 0) sign = 1;
            var n = BigInteger.Abs(staringAt);

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

            while (true)
            {
                yield return sign * n;
                n += 2;
            }
        }
        public override IEnumerable<BigInteger> Numbers()
        {
            return Numbers(BigInteger.One);
        }
        protected IEnumerable<BigInteger> ValidPrimeTests()
        {
            return ValidPrimeTests(BigInteger.One);
        }

        /// <summary>
        /// Returns an enumerable of key-value pairs that will iterate every prime starting at the starting value where the key is the count (index starting at 1) of the set.
        /// So the first entry is always {Key=1, Value=2}.
        /// </summary>
        public override IEnumerable<KeyValuePair<BigInteger, BigInteger>> NumbersIndexed()
        {
            var count = BigInteger.Zero;
            foreach (var n in Numbers())
            {
                count++;
                yield return new KeyValuePair<BigInteger, BigInteger>(count, n);
            }
        }

        /// <summary>
        /// Returns a parallel enumerable that will iterate every prime starting at the starting value.
        /// </summary>
        /// <param name="staringAt">Allows for skipping ahead any integer before checking for inclusive and subsequent primes.</param>
        /// <param name="degreeOfParallelism">Operates in parallel unless 1 is specified.</param>
        /// <returns></returns>
        public override ParallelQuery<BigInteger> NumbersInParallel(BigInteger staringAt, ushort? degreeOfParallelism = null)
        {
            if (staringAt >= ulong.MaxValue)
            {
                var testsBig = ValidPrimeTests(staringAt)
                    .AsParallel().AsOrdered();

                if (degreeOfParallelism.HasValue)
                    testsBig = testsBig.WithDegreeOfParallelism(degreeOfParallelism.Value);

                return testsBig.Where(v => Number.IsPrime(v));
            }

            var tests = ValidPrimeTests((ulong)staringAt)
                    .AsParallel().AsOrdered();

            if (degreeOfParallelism.HasValue)
                tests = tests.WithDegreeOfParallelism(degreeOfParallelism.Value);

            return tests
                .Where(v => Number.IsPrime(v))
                .Select(v => (BigInteger)v)
                .Concat(NumbersInParallel(ulong.MaxValue));
        }

        /// <summary>
        /// Returns a parallel enumerable that will iterate every prime.
        /// </summary>
        /// <param name="degreeOfParallelism">Operates in parallel unless 1 is specified.</param>
        /// <returns></returns>
        public override ParallelQuery<BigInteger> NumbersInParallel(ushort? degreeOfParallelism = null)
        {
            return NumbersInParallel(2, degreeOfParallelism);
        }


        /// <summary>
        /// Iterates the prime factors of the provided value.
        /// First multiple is always 0, 1 or -1.
        /// </summary>
        /// <param name="value">Value to factorize.</param>
        /// <returns>An enumerable that contains the prime factors of the provided value starting with 0, 1, or -1 for sign retention.</returns>
        public override IEnumerable<BigInteger> Factors(BigInteger value)
        {
            if (value != BigInteger.Zero)
            {
                yield return value < BigInteger.Zero ? BigInteger.MinusOne : BigInteger.One;
                value = BigInteger.Abs(value);
                if (value == BigInteger.One)
                    yield break;

                BigInteger last = BigInteger.One;

                // For larger numbers, a quick prime check can prevent large iterations.
                foreach (var p in Numbers())
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

            yield return value;
        }

    }
}