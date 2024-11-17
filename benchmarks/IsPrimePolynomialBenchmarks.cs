using BenchmarkDotNet.Attributes;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Open.Numeric.Primes.Benchmarks;

[MemoryDiagnoser]
public class IsPrimePolynomialBenchmarks
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

	[Benchmark(Baseline = true)]
	public void UIntIsPrime()
	{
		var n = UIntNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			_ = Polynomial.IsPrime(n[i]);
		}
	}

	[Benchmark]
	public void LongIsPrime()
	{
		var n = LongNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			_ = Polynomial.IsPrime(n[i]);
		}
	}

	[Benchmark]
	public void LongRefIsPrime()
	{
		var n = LongNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			ref readonly var v = ref n[i];
			_ = Polynomial.IsPrime(in v);
		}
	}

	[Benchmark]
	public void DecimalToBigIntIsPrime()
	{
		var n = DecimalNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			_ = Polynomial.IsPrime((BigInteger)n[i]);
		}
	}

#if NET7_0_OR_GREATER
	[Benchmark]
	public void DecimalIsPrime()
	{
		var n = DecimalNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			_ = Polynomial.IsPrime(in n[i]);
		}
	}

	[Benchmark]
	public void DecimalRefIsPrime()
	{
		var n = DecimalNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			ref readonly var v = ref n[i];
			_ = Polynomial.IsPrime(in v);
		}
	}
#endif
}
