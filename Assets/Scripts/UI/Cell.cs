using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public int index;
    public Image specialIcon;
    // public SpriteRenderer spriteRenderer;
    // public SpriteRenderer roomSprite;

    public void SetSpecialRoomSprite(Sprite sprite)
    {
        specialIcon.enabled = true;
        specialIcon.sprite = sprite;
        specialIcon.color = Color.white;
    }

    // public void SetRoomSprite(Sprite sprite)
    // {
    //     roomSprite.sprite = sprite;
    // }

    // public void RotateCell(List<int> connectedCells)
    // {
    //     connectedCells.Sort();
    //     index = connectedCells[0];

    //     if (connectedCells.Contains(index + 1) && connectedCells.Contains(index + 10))
    //     {
    //         ApplyRotation(-90);
    //     }
    //     else if (connectedCells.Contains(index + 1) && connectedCells.Contains(index + 11))
    //     {
    //         ApplyRotation(180);
    //     }
    //     else if (connectedCells.Contains(index + 9) && connectedCells.Contains(index + 10))
    //     {
    //         ApplyRotation(90);
    //     }
    // }

    // public void ApplyRotation(float angle)
    // {
    //     transform.rotation = Quaternion.Euler(0, 0, angle);
    // }
}
