using System.Diagnostics;
using System;

namespace Shayan.Caching
{

    /*
    * Testing class for NWayCache.
    */

    class TestProgram
    {
        static NWayCache<int, int> fib_cache = new NWayCache<int, int>(100, 5, "MRU");

        static int fib(int n, bool useCache)
        {
            if (n == 0 || n == 1)
                return 1;
            if (useCache && fib_cache.Fetch(n))
                return fib_cache.Retrieve();
            int sum = fib(n - 1, useCache) + fib(n - 2, useCache);
            if (useCache)
                fib_cache.Add(n, sum);
            return sum;
        }
        

        static void Main(string[] args)
        {

            var cache = new NWayCache<int, int>(8, 5, "MRU");
            int val;

            cache.Add(34, 68);
            cache.Add(22, 44);
            cache.Add(11, 22);
            cache.Add(9, 18);
            cache.Add(55, 110);
            cache.Add(142, 42);
            cache.Add(34, 15);

            Debug.Assert(cache.TryGetValue(34, out val));

            Debug.Assert(val == 15);

            cache.Add(2, 12);
            cache.TryGetValue(34, out val);

            Debug.Assert(val == 15);

            cache.Add(156, 32);
            cache.Add(11, 16);
            cache.Add(21, 3);
            cache.Add(39, 5);

            cache.Dump();

            var cache2 = new NWayCache<string, string>(4, 2);
            cache2.Add("foobar", "barfoo");
            cache2.Fetch("foobar");
            Debug.Assert(cache2.Retrieve() == "barfoo");

            Console.WriteLine();
            cache2.Dump();

            var cache3 = new NWayCache<int, string>(7, 3);
            string val3;
            cache3.Add(3, "hello");
            cache3.Add(6, "hello1");
            cache3.Add(8, "hello2");
            cache3.Add(9, "hello3");
            cache3.Add(2, "hello4");
            cache3.Add(4, "hello5");
            cache3.Policy = "MRU";
            cache3.Add(8, "hello6");
            cache3.Add(23, "hello7");
            cache3.Add(7, "hello8");
            cache3.TryGetValue(3, out val3);
            Debug.Assert(val3 == "hello");

            cache3.TryGetValue(8, out val3);

            Debug.Assert(val3 != "hello6"); //this is False because key 23 overwrites key 8 in the third set. 8 % 3 == 23 % 3
            cache3.TryGetValue(4, out val3);
            Debug.Assert(val3 == "hello5");

            Console.WriteLine();
            cache3.Dump();
            cache3.Clear();
            Console.WriteLine();
            Console.WriteLine("Cache3 after clear.");
            cache3.Dump();

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
