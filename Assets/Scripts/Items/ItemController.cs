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

        if (nameText != null) nameText.text = item.itemName;
        if (priceText != null)
        {
            priceText.gameObject.SetActive(price > 0);
            if (price > 0)
            {
                priceText.text = price > 0 ? $"-{displayedCorruptionPrice}%" : "Free";
                priceText.color = price > 0 ? Color.magenta : Color.green;
            }
        }

        if (statsText != null)
        {
            string desc = "";
            foreach (var e in item.effects)
            {
                string color = e.amount >= 0 ? "green" : "red";
                desc += $"<color={color}>{e.type} : {e.amount}</color>\n";
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

        if (corruptionPrice > 0) GameManager.runStats.Corruption -= corruptionPrice;

        InventoryController.Instance.Pickup(item);
        Destroy(gameObject);
    }

}