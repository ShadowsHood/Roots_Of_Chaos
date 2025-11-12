using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private GameStats stats => GameManager.runtimeStats;
    public LifeManager lifeManager;
    public TextMeshProUGUI scoreText;

    // set things up (before the game starts)
    // void Awake()
    // {
        
    // }

    // initialize things once
    void Start() {
        stats.OnHealthChanged += UpdateHealthUI;
        stats.OnScoreChanged += UpdateScoreUI;
    }

    // runs every frame
    // void Update()
    // {
    // }
    
    void UpdateHealthUI()
    {
        // healthText.text = $"HP: {stats.Health}";
        lifeManager.UpdateLifeUI(stats.MaxHealth, stats.Health);
    }
    void UpdateScoreUI()
    {
        scoreText.text = $"Score: {stats.Score}";
    }

    // runs every physics step
    // void FixedUpdate()
    // {
    // }

    // void LateUpdate()
    // {

    // }
}
