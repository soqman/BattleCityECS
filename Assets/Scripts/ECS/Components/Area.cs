using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[System.Serializable]
public struct Area : IComponent
{
    public int x;
    public int y;
    public DamageType State;
}
public enum DamageType{Whole,Left,Right,Up,Down,LeftUp,LeftDown,RightUp,RightDown,Destroyed}