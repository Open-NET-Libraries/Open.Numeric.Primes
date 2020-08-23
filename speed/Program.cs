using Open.Numeric.Primes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

internal class Program
{
	const uint START_NUMBER_DEFAULT = 10000;
	const uint TIMES_DEFAULT = 1000;
	const int PRIMES_TO_FIND = 3;

	static void Main()
	{
		Console.WriteLine("Singular Trial Division Tests:");

		const uint test32 = 2130706433;
		Console.WriteLine("{0} U32 Benchmark", TrialDivBenchmark(test32));
		Console.WriteLine("{0} U32", TrialDiv(test32));
		const ulong test64 = 8592868089022906369;
		Console.WriteLine("{0} U64 Benchmark", TrialDivBenchmark(test64));
		Console.WriteLine("{0} U64", TrialDiv(test64));

		Console.WriteLine();
		Console.WriteLine("Batch Tests:");

		foreach (var s in Tests().OrderBy(t => t))
			Console.WriteLine(s);

		Console.WriteLine();
		Console.WriteLine("Press enter to continue.");
		Console.ReadLine();
	}

	static IEnumerable<string> Tests()
	{
		yield return string.Format("{0} TrialDivision.U32", Test<TrialDivision.U32>(out _));
		yield return string.Format("{0} TrialDivision.U32.Memoized", Test<TrialDivision.U32.Memoized>(out _));
		yield return string.Format("{0} TrialDivision.U64", Test<TrialDivision.U64>(out _));
		yield return string.Format("{0} TrialDivision.U64.Memoized", Test<TrialDivision.U64.Memoized>(out _));

		var tdm = new TrialDivision.U64.Memoized();
		Test(tdm, START_NUMBER_DEFAULT, 1);
		yield return string.Format("{0} TrialDivision.U64.Memoized Cached", Test(tdm));
		yield return string.Format("{0} Polynomial.U64", Test<Polynomial.U64>(out _));
		yield return string.Format("{0} Polynomial.U32", Test<Polynomial.U32>(out _));
		yield return string.Format("{0} MillerRabin.U64", Test<MillerRabin.U64>(out _));
		yield return string.Format("{0} Optimized", Test<Optimized>(out _));
	}

	static TimeSpan TrialDivBenchmark(uint value)
	{
		var sw = Stopwatch.StartNew();
		var max = Math.Sqrt(value);
		for (var i = 3U; i < max; i += 2U)
		{
			if (value % i == 0) throw new Exception("Is not prime.");
		}
		sw.Stop();
		return sw.Elapsed;
	}

	static TimeSpan TrialDivBenchmark(ulong value)
	{
		var sw = Stopwatch.StartNew();
		var max = Math.Sqrt(value);
		for (var i = 3UL; i < max; i += 2UL)
		{
			if (value % i == 0) throw new Exception("Is not prime.");
		}
		sw.Stop();
		return sw.Elapsed;
	}

	static TimeSpan TrialDiv(uint value)
	{
		var t = new TrialDivision.U32();
		t.IsPrime(value); // first run.
		var sw = Stopwatch.StartNew();
		if (!t.IsPrime(value)) throw new Exception("Is not prime.");
		sw.Stop();
		return sw.Elapsed;
	}

	static TimeSpan TrialDiv(ulong value)
	{
		var t = new TrialDivision.U64();
		t.IsPrime(value); // first run.
		var sw = Stopwatch.StartNew();
		if (!t.IsPrime(value)) throw new Exception("Is not prime.");
		sw.Stop();
		return sw.Elapsed;
	}

	static TimeSpan Test<T>(out List<uint> found, uint number = START_NUMBER_DEFAULT, uint times = TIMES_DEFAULT)
		where T : PrimalityBase<uint>, new()
	{
		var sw = Stopwatch.StartNew();
		found = new List<uint>(PRIMES_TO_FIND);
		for (ulong t = 0; t < times; t++)
		{
			found.Clear();
			var prime = new T();
			for (var i = 0; i < PRIMES_TO_FIND; i++)
			{
				number = prime.Next(in number);
				found.Add(number);
			}
		}
		sw.Stop();
		return sw.Elapsed;
	}

	static TimeSpan Test<T>(out List<ulong> found, ulong number = START_NUMBER_DEFAULT, ulong times = TIMES_DEFAULT)
		where T : PrimalityBase<ulong>, new()
	{
		var sw = Stopwatch.StartNew();
		found = new List<ulong>(PRIMES_TO_FIND);
		for (ulong t = 0; t < times; t++)
		{
			found.Clear();
			var prime = new T();
			for (var i = 0; i < PRIMES_TO_FIND; i++)
			{
				number = prime.Next(in number);
				found.Add(number);
			}
		}
		sw.Stop();
		return sw.Elapsed;
	}

	static TimeSpan Test(PrimalityBase<ulong> prime, ulong number = START_NUMBER_DEFAULT, ulong times = TIMES_DEFAULT)
	{
		var sw = Stopwatch.StartNew();
		for (ulong t = 0; t < times; t++)
		{
			for (var i = 0; i < PRIMES_TO_FIND; i++)
			{
				number = prime.Next(number);
			}
		}
		sw.Stop();
		return sw.Elapsed;
	}



}
