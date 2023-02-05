using BenchmarkDotNet.Attributes;

namespace Open.Numeric.Primes.Benchmarks;

[MemoryDiagnoser]
public class IsPrimeTrialDivSqrtCheck
{
	const int Size = 2000000;

	static bool IsPrimeUsingSqrt(int value)
	{
		var sqr = Math.Sqrt(value);
		for (var p = 5; p <= sqr; p += 2)
		{
			if (value % p == 0)
				return false;
		}

		return true;
	}

	[Benchmark]
	public void Sqrt()
	{
		for (var i = 7; i < Size; i++)
		{
			_ = IsPrimeUsingSqrt(i);
		}
	}

	static bool IsPrimeUsingInt(int value)
	{
		var p = 5;
	loop:
		if (value % p == 0)
			return false;

		// Get next value.
		p += 2;

		// Have we checked all possible values already?
		if (value / p < p)
			return true;

		goto loop;
	}

	[Benchmark]
	public void DivInt()
	{
		for (var i = 7; i < Size; i++)
		{
			_ = IsPrimeUsingInt(i);
		}
	}
}
