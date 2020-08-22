using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Xunit;

namespace Open.Numeric.Primes.Tests
{
	public static class PrimeNumbers
	{


		[Theory]
		[InlineData(2130706433)]
		public static void TrialDivision32Test(uint value)
		{
			var instance = new TrialDivision.U32();
			Assert.True(instance.IsPrime(Convert.ToUInt32(value)));
		}


		[Theory]
		[InlineData(2130706433)]
		[InlineData(8592868089022906369)]
		public static void TrialDivision64Test(ulong value)
		{
			var instance = new TrialDivision.U64();
			Assert.True(instance.IsPrime(value));
		}


		static void PrimesTest32<T>()
			where T : PrimalityBase<uint>, new()
		{
			var i = 0;
			var instance = new T();
			foreach (var p in instance.Take(TrialDivision.FirstKnown.Length))
			{
				Assert.Equal(TrialDivision.FirstKnown[i++], p);//, "Numbers did not match for " + typeof(T));
			}

			i = 0;
			foreach (var p in instance.InParallel()
				.Take(TrialDivision.FirstKnown.Length))
			{
				Assert.Equal(TrialDivision.FirstKnown[i++], p);//, "Numbers in parallel did not match for " + typeof(T));
			}
		}

		static void PrimesTest64<T>()
			where T : PrimalityBase<ulong>, new()
		{
			var i = 0;
			var instance = new T();
			foreach (var p in instance.Take(TrialDivision.FirstKnown.Length))
			{
				Assert.Equal(TrialDivision.FirstKnown[i++], p);//, "Numbers did not match for " + typeof(T));
			}

			i = 0;
			foreach (var p in instance.InParallel()
				.Take(TrialDivision.FirstKnown.Length))
			{
				Assert.Equal(TrialDivision.FirstKnown[i++], p);//, "Numbers in parallel did not match for " + typeof(T));
			}
		}

		static void PrimesTestBig<T>()
			where T : PrimalityBase<BigInteger>, new()
		{
			var i = 0;
			var instance = new T();
			foreach (var p in instance.Take(TrialDivision.FirstKnown.Length))
			{
				Debug.WriteLine($"PrimesTestBig: {i}");
				Assert.Equal(TrialDivision.FirstKnown[i++], p);//, "Numbers did not match for " + typeof(T));
			}

			i = 0;
			foreach (var p in instance.InParallel()
				.Take(TrialDivision.FirstKnown.Length))
			{
				Assert.Equal(TrialDivision.FirstKnown[i++], p);//, "Numbers in parallel did not match for " + typeof(T));
			}
		}

		[Fact]
		public static void Primes_ULongByDivision() => PrimesTest64<TrialDivision.U64>();

		[Fact]
		public static void Primes_UIntFromPolynomial() => PrimesTest32<Polynomial.U32>();

		[Fact]
		public static void Primes_ULongFromPolynomial() => PrimesTest64<Polynomial.U64>();

		[Fact]
		public static void Primes_ULongMillerRabin() => PrimesTest64<MillerRabin.U64>();

		[Fact]
		public static void Primes_BigIntMillerRabin() => PrimesTestBig<MillerRabin.BigInt>();

		[Fact]
		public static void Primes_BigIntFromPolynomial() => PrimesTestBig<Polynomial.BigInt>();


		[Fact]
		public static void PrimeFactors_ULong()
		{
			for (var i = 0UL; i < 1000; i++)
			{
				Assert.Equal(i,
					Prime
						.Factors(i)
						.Aggregate(1UL, (p, c) => p * c)
				);
				Assert.True(
					Prime
						.Factors(i, true)
						.All(Number.IsPrime)
				);
			}
		}

		[Fact]
		public static void CommonFactors_ULong()
		{
			Assert.Equal(3UL,
				Prime
					.CommonFactors(9UL, 3UL, 3UL)
					.Aggregate(1UL, (p, c) => p * c)
			);

			Assert.Equal(3UL,
				Prime
					.CommonFactors(3UL, 3UL, 9UL)
					.Aggregate(1UL, (p, c) => p * c)
			);

			Assert.Equal(12UL,
				Prime
					.CommonFactors(84UL, 756UL, 108UL)
					.Aggregate(1UL, (p, c) => p * c)
			);

			Assert.Equal(3UL,
				Prime
					.CommonFactors(84UL, 756UL, 108UL, 3UL * 7UL * 13UL)
					.Aggregate(1UL, (p, c) => p * c)
			);

			Assert.Equal(4UL,
				Prime
					.CommonFactors(84UL, 756UL, 108UL, 4UL * 7UL * 17UL)
					.Aggregate(1UL, (p, c) => p * c)
			);
		}

		[Fact]
		public static void CommonFactors_BigInt()
		{
			Assert.Equal(new BigInteger(12),
				Prime
					.CommonFactors(new BigInteger(84), new BigInteger(756), new BigInteger(108))
					.Aggregate(BigInteger.One, (p, c) => p * c)
			);

			Assert.Equal(new BigInteger(3),
				Prime
					.CommonFactors(new BigInteger(84), new BigInteger(756), new BigInteger(108), new BigInteger(3 * 7 * 13))
					.Aggregate(BigInteger.One, (p, c) => p * c)
			);
		}

		[Fact]
		public static void PrimeFactors_BigInt()
		{
			for (BigInteger i = 0; i < 1000; i++)
			{
				Assert.Equal(i,
					Prime
						.Factors(i)
						.Aggregate(BigInteger.One, (p, c) => p * c)
				);
				Assert.True(
					Prime
						.Factors(i, true)
						.All(Prime.Numbers.Big.IsPrime)
				);
			}
		}
	}
}
