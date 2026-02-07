using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Data/Player")]
public class PlayerData : ScriptableObject
{
    public float moveSpeed = 4f;
    public int baseHealth = 6;
    public float fireRate = 2f;
    public int damage = 2;
    public int range = 4;
    public float spreadAngle = 5f;
    public float shotSpeed = 5f;
    public int luck = 0;
    public float corruptionGainRate = 1f;
    public float corruptionHitPenalty = 5f;
    public GameObject prefab;
}
