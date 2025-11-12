using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Data/Projectile")]
public class ProjectileData : ScriptableObject
{
    public float speed = 5f;
    public float lifetime = 1f;
    public int damage;
    public string targetTag;
    public GameObject prefab;
}
