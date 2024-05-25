using BenchmarkDotNet.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace Open.Numeric.Primes.Benchmarks;

[MemoryDiagnoser]
[SuppressMessage("Performance", "CA1822:Mark members as static")]
public class SquareRootBenchmarks
{
	const int Start = 12;
	const int Size = 2000000;

	[Benchmark]
	public void SquareRoot()
	{
		for (var i = Start; i < Size; i++)
		{
			_ = Math.Sqrt(i);
		}
	}

	//[Benchmark]
	public int SquareRootRecast()
	{
		int result = 0;
		for (var i = Start; i < Size; i++)
		{
			result = (int)Math.Sqrt(i);
		}
		return result;
	}

	static int IntSqrt(int x)
	{
		if (x <= 1) return x;
		int left = 0, right = x;
		while (left < right)
		{
			int mid = (left + right + 1) / 2;
			if (mid <= x / mid) left = mid;
			else right = mid - 1;
		}
		return left;
	}

	//[Benchmark]
	public void NearestInt()
	{
		for (var i = Start; i < Size; i++)
		{
			_ = IntSqrt(i);
		}
	}

	static int HeronSqrt(int n)
	{
		int root = n >> 1;
		while (root * root > n)
		{
			root = (root + n / root) >> 1;
		}
		return root;
	}

	[Benchmark]
	public void Heron()
	{
		for (var i = Start; i < Size; i++)
		{
			_ = HeronSqrt(i);
		}
	}

	static int BabylonianSqrt(int n)
	{
		double x = n;
		double y = 1;
		const double e = 0.000001;
		while (x - y > e)
		{
			x = (x + y) / 2;
			y = n / x;
		}
		return (int)x;
	}

	[Benchmark]
	public void Babylonian()
	{
		for (var i = Start; i < Size; i++)
		{
			_ = BabylonianSqrt(i);
		}
	}
}
