using System.Diagnostics;
using System;

namespace Shayan.Caching
{

    /*
    * Testing class for NWayCache. 
    * Recursively computes fibonacci with and without caching and compares performance.
    * Also tests other API calls like Dump and Clear.
    */

    class TestProgram
    {
        static NWayCache<int, int> cache = new NWayCache<int, int>(3, 5, "MRU");

        static int fib(int n, bool useCache)
        {
            if (n == 0 || n == 1)
                return 1;
            if (useCache && cache.Fetch(n))
                return cache.Retrieve();
            int sum = fib(n - 1, useCache) + fib(n - 2, useCache);
            if (useCache)
                cache.Add(n, sum);
            return sum;
        }

        static void Main(string[] args)
        {
            var n = 35;

            var watchCached = Stopwatch.StartNew();
            int ansCached = fib(n, true);
            watchCached.Stop();

            Console.WriteLine(string.Format("Fibonacci of {0} with cache: Time spent {1} ms, answer {2}", n, watchCached.ElapsedMilliseconds, ansCached));

            var watchNoCache = Stopwatch.StartNew();
            int ansNoCache = fib(n, false);
            watchNoCache.Stop();

            Console.WriteLine(string.Format("Fibonacci of {0} no cache: Time spent {1} ms, answer {2}", n, watchNoCache.ElapsedMilliseconds, ansNoCache));

            Console.WriteLine();
            Console.WriteLine("Dump of cache");
            cache.Dump();
            Console.WriteLine();
            Console.WriteLine("Changing policy, clearing cache, and dumping contents");
            cache.Policy = "LRU";
            cache.Clear();
            cache.Dump();
            Console.ReadLine();
        }
    }
}
