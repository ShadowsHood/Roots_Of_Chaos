using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemEffect[] effects;

    public void Apply(PlayerStats stats)
    {
        foreach (var e in effects)
            e.Apply(stats);
    }
}
