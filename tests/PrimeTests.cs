using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Numerics;

namespace Open.Numeric.Primes.Tests
{
    [TestClass]
    public class PrimeNumbers
    {
        static readonly ulong[] FIRST_PRIMES = new ulong[]
        {
                2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37
        };

        [TestMethod]
        public void Primes_ULong()
        {

            var i = 0;
            foreach (var p in Prime.Numbers().Take(FIRST_PRIMES.Length))
            {
                Assert.AreEqual(FIRST_PRIMES[i++], p);
            }
        }

        [TestMethod]
        public void Primes_ULongParallel()
        {

            var i = 0;
            foreach (var p in Prime
                .NumbersInParallel()
                .Take(FIRST_PRIMES.Length))
            {
                Assert.AreEqual(FIRST_PRIMES[i++], p);
            }
        }

        [TestMethod]
        public void Primes_Long()
        {

            var i = 0;
            foreach (var p in Prime
                .Numbers(2L)
                .Take(FIRST_PRIMES.Length))
            {
                Assert.IsTrue((ulong)p == FIRST_PRIMES[i++]);
            }
        }

        [TestMethod]
        public void Primes_LongParallel()
        {

            var i = 0;
            foreach (var p in Prime
                .NumbersInParallel(2L)
                .Take(FIRST_PRIMES.Length))
            {
                Assert.IsTrue((ulong)p == FIRST_PRIMES[i++]);
            }
        }

        [TestMethod]
        public void Primes_BigInt()
        {

            var i = 0;
            foreach (var p in Prime
                .NumbersBig()
                .Take(FIRST_PRIMES.Length))
            {
                Assert.IsTrue(p == FIRST_PRIMES[i++]);
            }
        }

        [TestMethod]
        public void Primes_BigIntInParallel()
        {

            var i = 0;
            foreach (var p in Prime
                .NumbersBigInParallel()
                .Take(FIRST_PRIMES.Length))
            {
                Assert.IsTrue(p == FIRST_PRIMES[i++]);
            }
        }

        [TestMethod]
        public void PrimeFactors_ULong()
        {
            for(var i = 0UL; i<1000; i++)
            {
                Assert.AreEqual(i,
                    Prime
                        .Factors(i)
                        .Aggregate(1UL,(p,c)=>p*c)
                );
                Assert.IsTrue(
                    !Prime
                        .Factors(i, true)
                        .Any(p => !Number.IsPrime(p))
                );
            }
        }

        [TestMethod]
        public void PrimeFactors_Long()
        {
            for (var i = 0L; i < 1000; i++)
            {
                Assert.AreEqual(i,
                    Prime
                        .Factors(i)
                        .Aggregate(1L, (p, c) => p * c)
                );
                Assert.IsTrue(
                    !Prime
                        .Factors(i, true)
                        .Any(p => !Number.IsPrime(p))
                );
            }
        }

        [TestMethod]
        public void PrimeFactors_BigInt()
        {
            for (BigInteger i = 0; i < 1000; i++)
            {
                Assert.AreEqual(i,
                    Prime
                        .Factors(i)
                        .Aggregate(BigInteger.One, (p, c) => p * c)
                );
                Assert.IsTrue(
                    !Prime
                        .Factors(i, true)
                        .Any(p => !Number.IsPrime(p))
                );
            }
        }
    }
}
