using System;
using System.Collections.Generic;
using System.Linq;

namespace Open.Numeric.Primes
{
    public class TrialDivision : PrimalityU64Base
    {
        public override bool IsPrime(ulong value)
        {
            return !Factors(value).Skip(2).Any();
        }

        public override IEnumerable<ulong> Numbers()
        {
            return NumbersMemoized();
        }

    }
}