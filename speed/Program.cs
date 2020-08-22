using Open.Numeric.Primes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

internal class Program
{
	const ulong START_NUMBER_DEFAULT = 10000;
	const ulong TIMES_DEFAULT = 1000;
	const int PRIMES_TO_FIND = 3;

	static void Main()
	{
		Console.WriteLine();

		foreach (var s in Tests().OrderBy(t => t))
			Console.WriteLine(s);

		Console.WriteLine();
		Console.WriteLine("Press enter to continue.");
		Console.ReadLine();
	}

	static IEnumerable<string> Tests()
	{
		yield return string.Format("{0} TrialDivision", Test<TrialDivision>(out _));
		yield return string.Format("{0} TrialDivisionMemoized", Test<TrialDivisionMemoized>(out _));

		var tdm = new TrialDivisionMemoized();
		Test(tdm, START_NUMBER_DEFAULT, 1);
		yield return string.Format("{0} TrialDivisionMemoized Cached", Test(tdm));
		yield return string.Format("{0} Polynomial", Test<Polynomial.U64>(out _));
		yield return string.Format("{0} MillerRabin", Test<MillerRabin.U64>(out _));
		yield return string.Format("{0} Optimized", Test<Optimized>(out _));
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
