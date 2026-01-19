using UnityEngine;
using System.Collections;

public static class Helpers
{
    public static void ShakeCamera(float duration, float magnitude)
    {
        // Implémentation fictive pour l'exemple
        Debug.Log($"Camera shake for {duration} seconds with magnitude {magnitude}");
    }

    public static Color Opacity(Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }
}
