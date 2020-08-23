# Open.Numeric.Primes

Methods and extensions for prime number detection and discovery.

[![NuGet](https://img.shields.io/nuget/v/Open.Numeric.Primes.svg)](https://www.nuget.org/packages/Open.Numeric.Primes/)

## Examples

### Importing

```cs
using Open.Numeric.Primes;
```

### Primality Test

```cs
Number.IsPrime(8592868089022906369) // true
```

### Factors

```cs
Prime.Factors(12) // 2, 2, 3
```

### Common Factors

```cs
Prime.CommonFactors(84, 756, 108) // 2, 2, 3
```

### Greatest Factor

```cs
Prime.GreatestFactor(84, 756, 108) // 12
```

### Prime Discovery

```cs
// Will list the first 1000 primes.
foreach(var prime in Prime.Numbers.Take(1000))
{
    Console.Write(prime);
}
```

or

```cs
// Will list the first 1000 primes greater than (or equal to) 10,000.
foreach(var prime in Prime.Numbers.StartingAt(10000).Take(100))
{
    Console.Write(prime);
}
```
