using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Open.Collections;

namespace Open.Numeric.Primes
{
    public class TrialDivision : PrimalityU64Base
    {

        LazyList<ulong> Memoized;

        /// <summary>
        /// Returns a memoized enumerable that will iterate every prime starting at the starting value.
        /// </summary>
        /// <returns>A memoized enumerable that will iterate every prime starting at the starting value</returns>
        public override IEnumerable<ulong> Numbers()
        {
            return LazyInitializer
                .EnsureInitialized(ref Memoized,
                    () => NumbersMemoizable().Memoize());
        }

        /// <summary>
        /// Returns a parallel enumerable that will iterate every prime starting at the starting value.
        /// </summary>
        /// <param name="staringAt">Allows for skipping ahead any integer before checking for inclusive and subsequent primes.</param>
        /// <param name="degreeOfParallelism">Operates in parallel unless 1 is specified.</param>
        /// <returns></returns>
        public override ParallelQuery<ulong> NumbersInParallel(ulong staringAt, ushort? degreeOfParallelism = null)
        {
            var tests = Numbers().SkipWhile(v=>v<staringAt)
                .AsParallel().AsOrdered();

            if (degreeOfParallelism.HasValue)
                tests = tests.WithDegreeOfParallelism(degreeOfParallelism.Value);

            return tests.Where(v => IsPrime(v));
        }

        protected static readonly IReadOnlyList<ulong> FirstKnown
            = (new List<ulong>() { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199, 211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293, 307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397, 401, 409, 419, 421, 431, 433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491, 499, 503, 509, 521, 523, 541, })
                .AsReadOnly();

        protected IEnumerable<ulong> NumbersMemoizable()
        {
            ulong last = 1;
            foreach (var n in FirstKnown)
            {
                yield return n;
                last = n;
            }

            /*
             * Note: here is where things start to recurse but should work perfectly
             * as the next primes can only be discovered by their predecessors.
             */
            foreach (var n in ValidPrimeTests(last + 1).Where(p => IsPrime(p)))
            {
                yield return n;
            }
        }


        public override bool IsPrime(ulong value)
        {
            return !Factors(value).Skip(2).Any();
        }

        /// <summary>
        /// Returns an enumerable that will iterate every prime starting at the starting value.
        /// </summary>
        /// <param name="staringAt">Allows for skipping ahead any integer before checking for inclusive and subsequent primes.</param>
        /// <returns>An enumerable that will iterate every prime starting at the starting value</returns>
        public override IEnumerable<ulong> Numbers(ulong startingAt)
        {
            return Numbers()
                .SkipWhile(n=>n<startingAt);
        }

    }
}