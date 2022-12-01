using BenchmarkDotNet.Attributes;
using Open.Numeric.Primes.Extensions;
using System.Numerics;

namespace Open.Numeric.Primes.Benchmarks;

[MemoryDiagnoser]
public class IsPrimeBenchmarks
{
	static readonly ReadOnlyMemory<int> IntNumbers = Enumerable.Range(0, 1000000).ToArray();
	static readonly ReadOnlyMemory<uint> UIntNumbers = Enumerable.Range(0, 1000000).Select(i => (uint)i).ToArray();
	static readonly ReadOnlyMemory<long> LongNumbers = Enumerable.Range(0, 1000000).Select(i => (long)i).ToArray();
	static readonly ReadOnlyMemory<ulong> ULongNumbers = Enumerable.Range(0, 1000000).Select(i => (ulong)i).ToArray();
	static readonly ReadOnlyMemory<double> DoubleNumbers = Enumerable.Range(0, 1000000).Select(i => (double)i).ToArray();
	static readonly ReadOnlyMemory<decimal> DecimalNumbers = Enumerable.Range(0, 1000000).Select(i => (decimal)i).ToArray();
	static readonly ReadOnlyMemory<BigInteger> BigIntNumbers = Enumerable.Range(0, 1000000).Select(i => (BigInteger)i).ToArray();

	static readonly Optimized Numbers = Prime.Numbers;

	[Benchmark]
	public void IntIsPrime()
	{
		var n = IntNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			_ = Numbers.IsPrime(n[i]);
		}
	}

	[Benchmark]
	public void UIntIsPrime()
	{
		var n = UIntNumbers.Span;
		var len = n.Length;
		for(var i = 0; i < len; i++)
		{
			_ = Numbers.IsPrime(n[i]);
		}
	}

	[Benchmark]
	public void LongIsPrime()
	{
		var n = LongNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			_ = Numbers.IsPrime(n[i]);
		}
	}

	[Benchmark]
	public void ULongIsPrime()
	{
		var n = ULongNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			_ = Numbers.IsPrime(n[i]);
		}
	}

	[Benchmark]
	public void ULongInIsPrime()
	{
		var n = ULongNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			_ = Numbers.IsPrime(in n[i]);
		}
	}

	[Benchmark]
	public void DoubleIsPrime()
	{
		var n = DoubleNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			_ = n[i].IsPrime();
		}
	}

	[Benchmark]
	public void DecimalIsPrime()
	{
		var n = DecimalNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			_ = n[i].IsPrime();
		}
	}

	[Benchmark]
	public void BigIntIsPrime()
	{
		var n = BigIntNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			_ = n[i].IsPrime();
		}
	}
}
