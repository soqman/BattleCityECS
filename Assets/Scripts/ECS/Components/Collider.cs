using Morpeh;
using Sirenix.OdinInspector.Editor.Drawers;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[System.Serializable]
public struct Collider : IComponent
{
    public float xSize;
    public float ySize;
    public float xOffset;
    public float yOffset;
    public LayerMask mask;
    public int layer;
}