using BenchmarkDotNet.Attributes;
using System.Numerics;
using static Open.Numeric.Primes.Benchmarks.DiscoveryBenchmarks;

namespace Open.Numeric.Primes.Benchmarks;

[MemoryDiagnoser]
public class IsPrimeTrialDivisionBenchmarks
{
	const int Size = 2000000;
	static readonly IEnumerable<int> Values = Enumerable.Range(0, Size);

	internal static readonly ReadOnlyMemory<int> IntNumbers = Values.ToArray();
	internal static readonly ReadOnlyMemory<uint> UIntNumbers = Values.Select(i => (uint)i).ToArray();
	internal static readonly ReadOnlyMemory<long> LongNumbers = Values.Select(i => (long)i).ToArray();
	internal static readonly ReadOnlyMemory<ulong> ULongNumbers = Values.Select(i => (ulong)i).ToArray();
	internal static readonly ReadOnlyMemory<double> DoubleNumbers = Values.Select(i => (double)i).ToArray();
	internal static readonly ReadOnlyMemory<decimal> DecimalNumbers = Values.Select(i => (decimal)i).ToArray();
	internal static readonly ReadOnlyMemory<BigInteger> BigIntNumbers = Values.Select(i => (BigInteger)i).ToArray();

	readonly TrialDivision.U32 PrimalityU32 = new();
	TrialDivision.U32.Memoized PrimalityMemoizedU32 = new();
	readonly TrialDivision.U32.Memoized PrimalitySingletonU32 = new();

	readonly TrialDivision.U64 PrimalityU64 = new();
	TrialDivision.U64.Memoized PrimalityMemoizedU64 = new();
	readonly TrialDivision.U64.Memoized PrimalitySingletonU64 = new();

	[Params(MemoizeMode.None, MemoizeMode.Instance, MemoizeMode.Singleton)]
	public MemoizeMode Mode { get; set; }

	[IterationSetup]
	public void IterationSetup()
	{
		PrimalityMemoizedU32 = new();
		PrimalityMemoizedU64 = new();
	}

	[Benchmark(Baseline = true)]
	public void UIntIsPrime()
	{
		PrimalityU32Base primality = Mode switch
		{
			MemoizeMode.Instance => PrimalityMemoizedU32,
			MemoizeMode.Singleton => PrimalitySingletonU32,
			_ => PrimalityU32,
		};

		var n = UIntNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			_ = primality.IsPrime(n[i]);
		}
	}

	[Benchmark]
	public void LongIsPrime()
	{
		PrimalityU64Base primality = Mode switch
		{
			MemoizeMode.Instance => PrimalityMemoizedU64,
			MemoizeMode.Singleton => PrimalitySingletonU64,
			_ => PrimalityU64,
		};

		var n = LongNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			_ = primality.IsPrime(n[i]);
		}
	}

	[Benchmark]
	public void LongRefIsPrime()
	{
		PrimalityU64Base primality = Mode switch
		{
			MemoizeMode.Instance => PrimalityMemoizedU64,
			MemoizeMode.Singleton => PrimalitySingletonU64,
			_ => PrimalityU64,
		};

		var n = LongNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			ref readonly var v = ref n[i];
			_ = primality.IsPrime(in v);
		}
	}

	[Benchmark]
	public void ULongIsPrime()
	{
		PrimalityU64Base primality = Mode switch
		{
			MemoizeMode.Instance => PrimalityMemoizedU64,
			MemoizeMode.Singleton => PrimalitySingletonU64,
			_ => PrimalityU64,
		};

		var n = ULongNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			_ = primality.IsPrime(n[i]);
		}
	}

	[Benchmark]
	public void ULongRefIsPrime()
	{
		PrimalityU64Base primality = Mode switch
		{
			MemoizeMode.Instance => PrimalityMemoizedU64,
			MemoizeMode.Singleton => PrimalitySingletonU64,
			_ => PrimalityU64,
		};

		var n = ULongNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			ref readonly var v = ref n[i];
			_ = primality.IsPrime(in v);
		}
	}
}
