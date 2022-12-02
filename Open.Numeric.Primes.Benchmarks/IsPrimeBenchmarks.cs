using BenchmarkDotNet.Attributes;
using Open.Numeric.Primes.Extensions;
using System.Numerics;
using System.Diagnostics.CodeAnalysis;

namespace Open.Numeric.Primes.Benchmarks;

[MemoryDiagnoser]
[SuppressMessage("Performance", "CA1822:Mark members as static")]
public class IsPrimeBenchmarks
{
	const int Size = 10000000;
	static readonly IEnumerable<int> Values = Enumerable.Range(0, Size);

	static readonly ReadOnlyMemory<int> IntNumbers = Values.ToArray();
	static readonly ReadOnlyMemory<uint> UIntNumbers = Values.Select(i => (uint)i).ToArray();
	static readonly ReadOnlyMemory<long> LongNumbers = Values.Select(i => (long)i).ToArray();
	static readonly ReadOnlyMemory<ulong> ULongNumbers = Values.Select(i => (ulong)i).ToArray();
	static readonly ReadOnlyMemory<double> DoubleNumbers = Values.Select(i => (double)i).ToArray();
	static readonly ReadOnlyMemory<decimal> DecimalNumbers = Values.Select(i => (decimal)i).ToArray();
	static readonly ReadOnlyMemory<BigInteger> BigIntNumbers = Values.Select(i => (BigInteger)i).ToArray();

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
		for (var i = 0; i < len; i++)
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
	public void LongRefIsPrime()
	{
		var n = LongNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			ref readonly var v = ref n[i];
			_ = Numbers.IsPrime(in v);
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
	public void ULongRefIsPrime()
	{
		var n = ULongNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			ref readonly var v = ref n[i];
			_ = Numbers.IsPrime(in v);
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

	[Benchmark]
	public void LongPolynomialIsPrime()
	{
		var n = LongNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			_ = Polynomial.IsPrime(n[i]);
		}
	}

	[Benchmark]
	public void LongRefPolynomialIsPrime()
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
	public void DecimalPolynomialIsPrime()
	{
		var n = DecimalNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			_ = Polynomial.IsPrime(in n[i]);
		}
	}

	[Benchmark]
	public void DecimalRefPolynomialIsPrime()
	{
		var n = DecimalNumbers.Span;
		var len = n.Length;
		for (var i = 0; i < len; i++)
		{
			ref readonly var v = ref n[i];
			_ = Polynomial.IsPrime(in v);
		}
	}
}
