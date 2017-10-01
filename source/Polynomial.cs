using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace Open.Numeric.Primes
{
    public static class Polynomial
    {
        const ulong MAX_ULONG_DIVISOR = 25043747693UL;

        public static bool IsPrime(ulong value)
        {
            if (value == 0 || value == 1)
                return false;
            if (value == 2 || value == 3)
                return true;

            if (value % 2 == 0 || value % 3 == 0)
                return false;

            ulong divisor = 6;
            while (divisor * divisor - 2 * divisor + 1 <= value)
            {
                if (value % (divisor - 1) == 0)
                    return false;

                if (value % (divisor + 1) == 0)
                    return false;

                divisor += 6;

                if (divisor > MAX_ULONG_DIVISOR)
                    return IsPrime(value, divisor);
            }
            return true;
        }

        public static bool IsPrime(BigInteger value)
        {
            if (value.IsZero || value.IsOne)
                return false;
            value = BigInteger.Abs(value);
            if (value.IsOne)
                return false;

            if (value <= ulong.MaxValue)
                return IsPrime((ulong)value);

            return value % 2 != 0
                && value % 3 != 0
                && IsPrime(value, 6);
        }

        internal static bool IsPrime(BigInteger value, BigInteger divisor)
        {
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

        public class U64 : PrimalityU64Base
        {
            public override bool IsPrime(ulong value)
            {
                return Polynomial.IsPrime(value);
            }
        }

        public class BigInt : PrimalityBigIntBase
        {
            public override bool IsPrime(BigInteger value)
            {
                return Polynomial.IsPrime(value);
            }
        }
    }

}