using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Open.Numeric.Primes.Tests
{
	[TestClass]
	public class PrimeNumbers
	{
		static void PrimesTest<T>()
			where T : PrimalityBase<ulong>, new()
		{
			var i = 0;
			var instance = new T();
			foreach (var p in instance.Take(TrialDivision.FirstKnown.Count))
			{
				Assert.AreEqual(TrialDivision.FirstKnown[i++], p, "Numbers did not match for " + typeof(T));
			}

			i = 0;
			foreach (var p in instance.InParallel()
				.Take(TrialDivision.FirstKnown.Count))
			{
				Assert.AreEqual(TrialDivision.FirstKnown[i++], p, "Numbers in parallel did not match for " + typeof(T));
			}
		}

		static void PrimesTestBig<T>()
			where T : PrimalityBase<BigInteger>, new()
		{
			var i = 0;
			var instance = new T();
			foreach (var p in instance.Take(TrialDivision.FirstKnown.Count))
			{
				Debug.WriteLine($"PrimesTestBig: {i}");
				Assert.AreEqual(TrialDivision.FirstKnown[i++], p, "Numbers did not match for " + typeof(T));
			}

			i = 0;
			foreach (var p in instance.InParallel()
				.Take(TrialDivision.FirstKnown.Count))
			{
				Assert.AreEqual(TrialDivision.FirstKnown[i++], p, "Numbers in parallel did not match for " + typeof(T));
			}
		}

		[TestMethod]
		public void Primes_ULongByDivision()
		{
			PrimesTest<TrialDivision>();
		}

		[TestMethod]
		public void Primes_ULongFromPolynomial()
		{
			PrimesTest<Polynomial.U64>();
		}

		[TestMethod]
		public void Primes_ULongMillerRabin()
		{
			PrimesTest<MillerRabin.U64>();
		}

		[TestMethod]
		public void Primes_BigIntMillerRabin()
		{
			PrimesTestBig<MillerRabin.BigInt>();
		}

		[TestMethod]
		public void Primes_BigIntFromPolynomial()
		{
			PrimesTestBig<Polynomial.BigInt>();
		}


		[TestMethod]
		public void PrimeFactors_ULong()
		{
			for (var i = 0UL; i < 1000; i++)
			{
				Assert.AreEqual(i,
					Prime
						.Factors(i)
						.Aggregate(1UL, (p, c) => p * c)
				);
				Assert.IsTrue(
					Prime
						.Factors(i, true)
						.All(Number.IsPrime)
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
					Prime
						.Factors(i, true)
						.All(Prime.Numbers.Big.IsPrime)
				);
			}
		}


	}
}
