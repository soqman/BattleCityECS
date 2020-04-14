﻿using UnityEngine;

[CreateAssetMenu(fileName = "new AreaType", menuName = "ScriptableObjects/AreaType")]
public class AreaType : ScriptableObject
{
    public Sprite wholeSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftUpSprite;
    public Sprite rightUpSprite;
    public Sprite rightDownSprite;
    public Sprite leftDownSprite;
    public bool isWalkable;
    public bool isBulletTransparent;
    public int health;
}
