using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Open.Numeric.Primes;


class Program
{
    static void Main(string[] args)
    {
        var byDivision = new TrialDivision();
        foreach (var p in byDivision.Numbers().Take(100))
        {
            Console.Write("{0}, ",p);
        }
    }

            static TimeSpan Test(int skip, int iterations, int repeat, IEnumerable<ulong> series)
        {
            var sw = new Stopwatch();
            for (var i = 0; i < repeat; i++)
            {
                int s = 0;
                foreach (var p in series.Take(iterations + skip))
                {
                    if (++s > skip)
                        sw.Start();
                }
                sw.Stop();
            }
            return sw.Elapsed;
        }

}
