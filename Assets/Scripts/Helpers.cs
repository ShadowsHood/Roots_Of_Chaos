using UnityEngine;
using System.Collections;

public static class Helpers
{
    public static void ShakeCamera(float duration, float magnitude)
    {
        // Implémentation fictive pour l'exemple
        Debug.Log($"Camera shake for {duration} seconds with magnitude {magnitude}");
    }

    public static void Knockback(Rigidbody2D rb, Vector2 direction, float force)
    {
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }
}
