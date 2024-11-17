using BenchmarkDotNet.Attributes;

namespace Open.Numeric.Primes.Benchmarks;

public static class DiscoveryBenchmarks
{
	const int Max = 6_542;

	static void Run<T>(IEnumerable<T> source)
	{
		foreach (var _ in source.Take(Max))
		{
		}
	}

	public enum MemoizeMode
	{
		None,
		Instance,
		Singleton
	}

	public abstract class BenchBase
	{
		//[Params(false, true)]
		public bool Parallel { get; set; }

		[Params(MemoizeMode.None)]
		public virtual MemoizeMode Mode { get; set; }
	}

	public abstract class MemoizableBenchBase : BenchBase
	{
		[Params(MemoizeMode.None, MemoizeMode.Instance, MemoizeMode.Singleton)]
		public override MemoizeMode Mode { get; set; }
	}

	[MemoryDiagnoser]
	public class TrialDivision : MemoizableBenchBase
	{
		readonly Primes.TrialDivision.U32 PrimalityU32 = new();
		Primes.TrialDivision.U32.Memoized PrimalityMemoizedU32 = new();
		readonly Primes.TrialDivision.U32.Memoized PrimalitySingletonU32 = new();

		readonly Primes.TrialDivision.U64 PrimalityU64 = new();
		Primes.TrialDivision.U64.Memoized PrimalityMemoizedU64 = new();
		readonly Primes.TrialDivision.U64.Memoized PrimalitySingletonU64 = new();

		[IterationSetup]
		public void IterationSetup()
		{
			PrimalityMemoizedU32 = new();
			PrimalityMemoizedU64 = new();
		}

		[Benchmark]
		public void U32()
		{
			PrimalityU32Base primality = Mode switch
			{
				MemoizeMode.Instance => PrimalityMemoizedU32,
				MemoizeMode.Singleton => PrimalitySingletonU32,
				_ => PrimalityU32,
			};

			if (Parallel)
			{
				Run(primality.InParallel());
			}
			else
			{
				Run(primality);
			}
		}

		//[Benchmark]
		public void U64()
		{
			PrimalityU64Base primality = Mode switch
			{
				MemoizeMode.Instance => PrimalityMemoizedU64,
				MemoizeMode.Singleton => PrimalitySingletonU64,
				_ => PrimalityU64,
			};

			if (Parallel)
			{
				Run(primality.InParallel());
			}
			else
			{
				Run(primality);
			}
		}
	}

	[MemoryDiagnoser]
	public class SieveOfEratosthenes
	{
		[Benchmark]
		public void U32() => Run(SieveOfEratosthenesUpto(65_536));

		public static IEnumerable<uint> SieveOfEratosthenesUpto(uint lessThan)
		{
			yield return 2;

			// Implement the sieve of Eratosthenes
			var sieve = new bool[lessThan / 2 - 1];
			for (uint n = 3; n < lessThan; n += 2)
			{
				uint i = n / 2 - 1;
				// If the is flagged as composite, skip it.
				if (sieve[i]) continue;

				// If it hasn't been flagged, then it must be prime.
				yield return n;

				// Flag all multiples of the prime as composite.
				for (uint j = n * n; j < lessThan; j += n)
				{
					if ((j & 1) == 0) continue;
					sieve[j / 2 - 1] = true;
				}
			}
		}
	}

	[MemoryDiagnoser]
	public class Polynomial : BenchBase
	{
		readonly Primes.Polynomial.U32 PrimalityU32 = new();
		readonly Primes.Polynomial.U64 PrimalityU64 = new();
		readonly Primes.Polynomial.BigInt PrimalityBigInt = new();

		[Benchmark]
		public void U32()
		{
			if (Parallel)
			{
				Run(PrimalityU32.InParallel());
			}
			else
			{
				Run(PrimalityU32);
			}
		}

		//[Benchmark]
		public void U64()
		{
			if (Parallel)
			{
				Run(PrimalityU64.InParallel());
			}
			else
			{
				Run(PrimalityU64);
			}
		}

		//[Benchmark]
		public void BigInt()
		{
			if (Parallel)
			{
				Run(PrimalityBigInt.InParallel());
			}
			else
			{
				Run(PrimalityBigInt);
			}
		}
	}
}
