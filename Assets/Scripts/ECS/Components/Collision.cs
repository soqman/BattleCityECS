using System;
using System.Collections.Generic;
using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[System.Serializable]
public struct Collision : IComponent
{
    public List<CollisionItem> collisions;
    public enum CollisionDirection
    {
        L = 0,
        R = 2,
        U = 4,
        D = 8,
    }

    public struct CollisionItem
    {
        public IEntity collideWith;
        public CollisionDirection direction;
    }
}