using Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using UnityEngine.UI;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[System.Serializable]
public struct AreaView : IComponent
{
    public Transform Transform;
    public SpriteRenderer spriteRenderer;
    public Sprite wholeSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftUpSprite;
    public Sprite rightUpSprite;
    public Sprite rightDownSprite;
    public Sprite leftDownSprite;
}