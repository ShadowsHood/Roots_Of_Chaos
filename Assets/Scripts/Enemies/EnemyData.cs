using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Data/Enemy")]
public class EnemyData : ScriptableObject
{
    public float moveSpeed = 4f;
    public int maxHealth = 6;
    public float attackSpeed = 2f;
    public float detectionRange = 5f;
    public int damage = 1;
    public GameObject prefab;
}
