using System;
using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[System.Serializable]
public struct Collision : IComponent
{
    public IEntity colliderWith;
    public enum CollisionDirection
    {
        L = 0,
        R = 2,
        U = 4,
        D = 8,
        LU = 16,
        UR = 32,
        RD = 64,
        DL = 128
    }
}