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
        public override IEnumerable<ulong> Numbers()
        {
            return LazyInitializer
                .EnsureInitialized(ref Memoized,
                    () => NumbersMemoizable().Memoize());
        }

        protected static readonly IReadOnlyList<ulong> Known
            = (new List<ulong>() { 2, 3, 5, 7, 11, 13, 17, 19, 23 })
                .AsReadOnly();

        protected IEnumerable<ulong> NumbersMemoizable()
        {
            ulong last = 1;
            foreach (var n in Known)
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

        public override IEnumerable<ulong> Numbers(ulong startingAt)
        {
            return Numbers()
                .SkipWhile(n=>n<startingAt);
        }

    }
}