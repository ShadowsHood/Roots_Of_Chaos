using UnityEngine;
public abstract class ItemEffect : ScriptableObject
{
    public abstract void Apply(PlayerStats stats);
}

[CreateAssetMenu(menuName = "Items/Effects/Health")]
public class HealthEffect : ItemEffect
{
    public int amount;
    public override void Apply(PlayerStats stats)
    {
        stats.Health += amount;
    }
}

[CreateAssetMenu(menuName = "Items/Effects/MaxHealth")]
public class MaxHealthEffect : ItemEffect
{
    public int amount;
    public override void Apply(PlayerStats stats)
    {
        stats.MaxHealth += amount;
    }
}

[CreateAssetMenu(menuName = "Items/Effects/Damage")]
public class DamageEffect : ItemEffect
{
    public int amount;
    public override void Apply(PlayerStats stats)
    {
        stats.Damage += amount;
    }
}

[CreateAssetMenu(menuName = "Items/Effects/Range")]
public class RangeEffect : ItemEffect
{
    public int amount;
    public override void Apply(PlayerStats stats)
    {
        stats.Range += amount;
    }
}

[CreateAssetMenu(menuName = "Items/Effects/Speed")]
public class SpeedEffect : ItemEffect
{
    public float amount;
    public override void Apply(PlayerStats stats)
    {
        stats.MoveSpeed += amount;
    }
}

[CreateAssetMenu(menuName = "Items/Effects/FireRate")]
public class FireRateEffect : ItemEffect
{
    public float amount;
    public override void Apply(PlayerStats stats)
    {
        stats.FireRate += amount;
    }
}

[CreateAssetMenu(menuName = "Items/Effects/Corruption")]
public class CorruptionEffect : ItemEffect
{
    public float amount;
    public override void Apply(PlayerStats stats)
    {
        stats.Corruption += amount;
    }
}

