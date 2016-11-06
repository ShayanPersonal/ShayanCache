using System;

namespace Shayan.Caching
{
    /*
     * An N-way set associative cache represented by OrderedDictionaries. 
     * Lookup s and additions are done in O(n) where n is the size of the sets.
     * 
     * NWayCache takes 3 parameters:
     *      n is the size of the sets. Default 10.
     *      setCount is the number of sets in the cache. Default 10.
     *      policy is a string representing the replacement policy. Default LRU.
     *      
     *      Capacity is automatically computed as setCount * n.
     *      All of these attributes can be read through their corresponding property. Policy can be modified after initialization.
     *      
     *  Public API:
     *      void Add(key, value) inserts a key value pair to the cache. If cache is full, evicts the most recently used or least recently 
     *          used item depending on replacement policy.
     *          
     *      bool Fetch(key) checks if a key-value pair exists in the cache for the key. If it exists, it's prepped for retrieval and returns
     *          True. Otherwise returns False.
     *          
     *      TValue Retrieve() returns the last value that was fetched by Fetch.
     *      
     *      void Dump() prints statistics about the cache including capacity, set size, set count, policy, and cache contents.
     *      
     *      void Clear() clears all entries in the cache.
     *      
     *      Properties include N, Capacity, SetCount, and Policy. Policy can be modified, the others are read-only.
     *      
     */      

    class NWayCache<TKey, TValue>
    {
        //Private variables and methods
        private NWayCacheSet[] _sets;
        private TValue _lastFetched;
        private string _policy;

        private int GetHash(TKey key)
        {
            return key.GetHashCode() % SetCount;
        }

        //Properties
        public int N { get; }
        public int Capacity { get; }
        public int SetCount { get; }
        public string Policy
        {
            get { return _policy; }
            set
            {
                if (value != "MRU" && value != "LRU")
                    throw new ArgumentException("Policy must be MRU or LRU", "policy");
                _policy = value;
            }
        }

        //Public methods
        public NWayCache(int n = 10, int setCount = 10, string policy = "LRU")
        {
            if (setCount < 1)
                throw new ArgumentException("setCount must be a positive integer", "setCount");
            if (n < 1)
                throw new ArgumentException("Set size must be a positive integer", "n");
            Policy = policy;
            Capacity = setCount * n;
            N = n;
            SetCount = setCount;
            _sets = new NWayCacheSet[SetCount];
            for (int i = 0; i < SetCount; i++)
                _sets[i] = new NWayCacheSet(N);
        }

        public bool Fetch(TKey key)
        {
            object value = _sets[GetHash(key)].Get(key);
            if (value == null)
                return false;
            _lastFetched = (TValue)value;
            return true;
        }

        public TValue Retrieve()
        {
            return _lastFetched;
        }

        public void Add(TKey key, TValue value)
        {
            _sets[GetHash(key)].Add(key, value, Policy);
        }

        public void Dump()
        {
            Console.WriteLine(string.Format("Set size N: {0}", N));
            Console.WriteLine(string.Format("SetCount number of sets: {0}", SetCount));
            Console.WriteLine(string.Format("Capacity: {0}", Capacity));
            Console.WriteLine(string.Format("Policy: {0}", Policy));
            int count = 0;
            foreach (NWayCacheSet set in _sets)
            {
                Console.WriteLine(string.Format("Set {0}", count));
                set.Dump();
                count++;
            }
        }

        public void Clear()
        {
            foreach (NWayCacheSet set in _sets)
            {
                set.Clear();
            }
        }

    }
}
