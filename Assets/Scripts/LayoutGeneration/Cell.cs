using UnityEngine;

public class Cell : MonoBehaviour
{
    public int index;
    public int value;
    public SpriteRenderer spriteRenderer;

    public void SetSpecialRoomSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
}
