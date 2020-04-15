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
    public DamagedState State;
}
public enum DamagedState{Whole,Left,Right,Up,Down,LeftUp,LeftDown,RightUp,RightDown,Destroyed}