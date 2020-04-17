using System.Collections.Generic;
using Morpeh;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[System.Serializable]
public struct Collision : IComponent
{
    public List<CollisionItem> collisions;
    public struct CollisionItem
    {
        public IEntity collideWith;
        public Direction direction;
    }
}