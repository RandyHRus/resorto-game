using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Specialized datastructure 
 * Should be O(1) add, O(1) remove, O(1) random, O(1) search
 */
public class ArrayHashSet<T>: IEnumerable<T>
{
    List<T> elems;
    Dictionary<T, int> locations;

    /*
    public HashSet<T> ConvertToHashSet()
    {
        HashSet<T> hashSet = new HashSet<T>();
        foreach (T elem in elems)
            hashSet.Add(elem);

        return hashSet;
    }
    */
    public List<T> ToList()
    {
        return new List<T>(elems);
    }

    public int Count => elems.Count;

    /** Initialize your data structure here. */
    public ArrayHashSet()
    {
        elems = new List<T>();
        locations = new Dictionary<T, int>();
    }

    /** Adds a value to the set. Returns true if the set did not already contain the specified element. */
    public bool Add(T val)
    {
        bool contain = locations.ContainsKey(val);
        if (contain) return false;
        locations.Add(val, elems.Count);
        elems.Add(val);
        return true;
    }

    /** Removes a value from the set. Returns true if the set contained the specified element. */
    public bool Remove(T val)
    {
        bool contain = locations.ContainsKey(val);
        if (!contain) return false;
        int loc = locations[val];
        if (loc < elems.Count - 1)
        { // not the last one than swap the last one with this val
            T lastone = elems[elems.Count - 1];
            elems[loc] = lastone;

            locations.Remove(lastone);
            locations.Add(lastone, loc);
        }
        locations.Remove(val);
        elems.RemoveAt(elems.Count - 1);
        return true;
    }

    /** Get a random element from the set. */
    public T GetRandom()
    {
        if (elems.Count == 0)
            throw new System.Exception("No element found");
        else
            return elems[Random.Range(0, elems.Count)];
    }

    public bool Contains(T elem)
    {
        return locations.ContainsKey(elem);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return elems.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
