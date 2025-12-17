using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private GameStats stats => GameManager.runtimeStats;
    private RoomManager roomManager => RoomManager.Instance;
    public LifeManager lifeManager;
    public TextMeshProUGUI scoreText;
    public MinimapController minimapController;

    // set things up (before the game starts)
    // void Awake()
    // {

    // }

    // initialize things once
    void Start()
    {
        stats.OnHealthChanged += UpdateHealthUI;
        stats.OnScoreChanged += UpdateScoreUI;
        roomManager.OnEnterRoom += UpdateMinimap;
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

    void UpdateMinimap()
    {
        minimapController.UpdateMinimap();
    }

    // runs every physics step
    // void FixedUpdate()
    // {
    // }

    // void LateUpdate()
    // {

    // }
}
