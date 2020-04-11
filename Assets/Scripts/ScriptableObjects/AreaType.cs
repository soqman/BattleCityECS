using UnityEngine;

[CreateAssetMenu(fileName = "new AreaType", menuName = "ScriptableObjects/AreaType")]
public class AreaType : ScriptableObject
{
    public Sprite sprite;
    public bool isWalkable;
    public bool isBulletTransparent;
    public int health;
}
