using System.Collections.Specialized;

namespace Shayan.Caching
{

    /*
    * Internal class representing sets in NWayCache. Employs OrderedDictionary as its data structure
    */

    internal class NWayCacheSet
    {
        private int _capacity;
        private OrderedDictionary _internalData;

        public NWayCacheSet(int capacity)
        {
            _capacity = capacity;
            _internalData = new OrderedDictionary(_capacity);
        }

        public object Get(object key)
        {
            if (!_internalData.Contains(key))
                return null;
            object value = _internalData[key];
            //Remove the key-value pair and insert it back up at the front as the most recently accessed.
            _internalData.Remove(key);
            _internalData.Insert(0, key, value);
            return value;
        }

        public void Add(object key, object value, string policy)
        {
            if (policy == "MRU")
            {
                while (_internalData.Count >= _capacity)
                    _internalData.RemoveAt(0);
            }
            if (policy == "LRU")
            {
                while (_internalData.Count >= _capacity)
                    _internalData.RemoveAt(_internalData.Count - 1);
            }
            _internalData.Remove(key);
            _internalData.Insert(0, key, value);
        }

        public void Dump()
        {
            foreach (System.Collections.DictionaryEntry o in _internalData)
            {
                System.Console.WriteLine(string.Format("Key: {0}, Value: {1}", o.Key.ToString(), o.Value.ToString()));
            }
        }

        public void Clear()
        {
            _internalData.Clear();
        }

    }
}
