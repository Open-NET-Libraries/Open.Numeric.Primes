using BenchmarkDotNet.Attributes;

namespace Open.Numeric.Primes.Benchmarks;

public static class DiscoveryBenchmarks
{
	const int Max = 1000000;

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
		[Params(false, true)]
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

		[Benchmark]
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

		[Benchmark]
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

		[Benchmark]
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
