using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CheckCompare<T>
{
    public static bool Compare(Func<T, T, bool> func, T arg1, T arg2)
    {
        if (func(arg1, arg2))
            return true;
        else
            return false;
    }

    static Func<T, T, bool> Greater<T>()
    where T : IComparable<T>
    {
        return delegate (T lhs, T rhs) { return lhs.CompareTo(rhs) > 0; };
    }

    static Func<T, T, bool> Less<T>()
        where T : IComparable<T>
    {
        return delegate (T lhs, T rhs) { return lhs.CompareTo(rhs) < 0; };
    }
}
