using System;

namespace Prototype
{
    [Flags]
    public enum TestFlags : int
    {
        NoFlags = 0,
        Flag1 = 1,
        Flag2 = 2,
        Flag4 = 4,
        AllFlags = 7,
    }
}