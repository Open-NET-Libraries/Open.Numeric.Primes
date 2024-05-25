using BenchmarkDotNet.Running;
using Open.Numeric.Primes.Benchmarks;

//BenchmarkRunner.Run<SquareRootBenchmarks>();
//BenchmarkRunner.Run<IsPrimeBenchmarks>();
//BenchmarkRunner.Run<DiscoveryBenchmarks.TrialDivision>();
//BenchmarkRunner.Run<DiscoveryBenchmarks.Polynomial>();
BenchmarkRunner.Run<IsPrimePolynomialBenchmarks>();
//BenchmarkRunner.Run<IsPrimeTrialDivisionBenchmarks>();

//double n = 0;
//while (n + 1 != n) n++;
//Console.WriteLine(n - 1);
//Console.WriteLine(n);
//Console.WriteLine(n + 1);