using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class ItemController : MonoBehaviour
{
    public ItemData item;
    public float corruptionPrice = 0;
    private SpriteRenderer sr;

    [Header("UI References")]
    public GameObject descriptionPanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI priceText;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (descriptionPanel != null) descriptionPanel.SetActive(true);
    }
    public void SetItemData(ItemData newItem, float price = 0f)
    {
        item = newItem;
        corruptionPrice = price;
        int displayedCorruptionPrice = Mathf.CeilToInt(price * 100);

        if (sr != null && item != null) sr.sprite = item.icon;

        // UI : Name + Price
        if (nameText != null) nameText.text = item.itemName;
        if (priceText != null)
        {
            bool isPaid = price > 0;
            priceText.gameObject.SetActive(isPaid);
            priceText.text = isPaid ? $"-{displayedCorruptionPrice}% Corruption" : "Free";
            priceText.color = isPaid ? Color.magenta : Color.green;
        }

        // UI : Stats (Buffs + Nerfs)
        if (statsText != null)
        {
            string desc = "";

            foreach (var e in item.buffs)
            {
                desc += $"<color=green>+{e.amount} {e.type}</color>\n";
            }

            if (price > 0 && item.nerfs != null)
            {
                desc += "\n<color=yellow>SACRIFICE :</color>\n";
                foreach (var n in item.nerfs)
                {
                    desc += $"<color=red>-{n.amount} {n.type}</color>\n";
                }
            }

            statsText.text = desc;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (InventoryController.Instance != null)
            {
                TryPickup();
            }
        }
    }

    private void TryPickup()
    {
        if (InventoryController.Instance == null || item == null) return;
        bool wasPurchased = corruptionPrice > 0;

        if (wasPurchased) GameManager.runStats.Corruption -= corruptionPrice;

        InventoryController.Instance.Pickup(item, wasPurchased);
        Destroy(gameObject);
    }

}