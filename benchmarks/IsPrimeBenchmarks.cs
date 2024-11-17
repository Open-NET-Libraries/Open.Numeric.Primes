using BenchmarkDotNet.Attributes;
using Open.Numeric.Primes.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Open.Numeric.Primes.Benchmarks;

[MemoryDiagnoser]
public class IsPrimeBenchmarks
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

	[Benchmark]
	public void IntIsPrime()
	{
		var n = IntNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			_ = n[i].IsPrime();
		}
	}

	[Benchmark]
	public void UIntIsPrime()
	{
		var n = UIntNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			_ = n[i].IsPrime();
		}
	}

	[Benchmark]
	public void LongIsPrime()
	{
		var n = LongNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			_ = n[i].IsPrime();
		}
	}

	[Benchmark]
	public void ULongIsPrime()
	{
		var n = ULongNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			_ = n[i].IsPrime();
		}
	}

	[Benchmark]
	public void ULongRefIsPrime()
	{
		var n = ULongNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			ref readonly var v = ref n[i];
			_ = Prime.Numbers.IsPrime(in v);
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
	public void BigIntRefIsPrime()
	{
		var n = BigIntNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			ref readonly var v = ref n[i];
			_ = Prime.Numbers.Big.IsPrime(in v);
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
