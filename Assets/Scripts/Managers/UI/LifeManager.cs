using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LifeManager : MonoBehaviour
{
    public GameObject heartPrefab;
    public Transform heartsContainer;
    private List<GameObject> hearts = new List<GameObject>();

    public void UpdateLifeUI(int maxHealth, int currentHealth)
    {
        while (hearts.Count < maxHealth)
        {
            GameObject newHeart = Instantiate(heartPrefab, heartsContainer);
            newHeart.transform.localScale = Vector3.one;
            hearts.Add(newHeart);
        }

        while (hearts.Count > maxHealth)
        {
            Destroy(hearts[hearts.Count - 1]);
            hearts.RemoveAt(hearts.Count - 1);
        }

        for (int i = 0; i < hearts.Count; i++)
        {
            Image full = hearts[i].transform.Find("HeartFull").GetComponent<Image>();
            full.enabled = i < currentHealth;
        }
    }
}
