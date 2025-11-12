using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Data/Player")]
public class PlayerData : ScriptableObject
{
    public float moveSpeed = 4f;
    public int baseHealth = 6;
    public float fireRate = 2f;
}
