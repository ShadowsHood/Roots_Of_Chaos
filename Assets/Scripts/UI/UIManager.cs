using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private PlayerStats stats => GameManager.runtimeStats;
    private RoomManager roomManager => RoomManager.Instance;
    public LifeManager lifeManager;
    public MinimapController minimapController;

    [Header("Stats Texts")]
    public TextMeshProUGUI moveSpeedText;
    public TextMeshProUGUI fireRateText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI rangeText;
    public TextMeshProUGUI corruptionText;

    // set things up (before the game starts)
    // void Awake()
    // {

    // }

    // initialize things once
    void Start()
    {
        stats.OnHealthChanged += UpdateHealthUI;
        stats.OnMoveSpeedChanged += UpdateMoveSpeedUI;
        stats.OnFireRateChanged += UpdateFireRateUI;
        stats.OnDamageChanged += UpdateDamageUI;
        stats.OnRangeChanged += UpdateRangeUI;
        stats.OnCorruptionChanged += UpdateCorruptionUI;
        roomManager.OnEnterRoom += UpdateMinimap;

        // Initial UI update
        UpdateHealthUI();
        UpdateMoveSpeedUI();
        UpdateFireRateUI();
        UpdateDamageUI();
        UpdateRangeUI();
        UpdateCorruptionUI();
    }

    void OnDestroy()
    {
        stats.OnHealthChanged -= UpdateHealthUI;
        stats.OnMoveSpeedChanged -= UpdateMoveSpeedUI;
        stats.OnFireRateChanged -= UpdateFireRateUI;
        stats.OnDamageChanged -= UpdateDamageUI;
        stats.OnRangeChanged -= UpdateRangeUI;
        stats.OnCorruptionChanged -= UpdateCorruptionUI;
        roomManager.OnEnterRoom -= UpdateMinimap;
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
    void UpdateMoveSpeedUI()
    {
        moveSpeedText.text = $"Move Speed: {stats.MoveSpeed}";
    }
    void UpdateFireRateUI()
    {
        fireRateText.text = $"Fire Rate: {stats.FireRate}";
    }
    void UpdateDamageUI()
    {
        damageText.text = $"Damage: {stats.Damage}";
    }
    void UpdateRangeUI()
    {
        rangeText.text = $"Range: {stats.Range}";
    }
    void UpdateCorruptionUI()
    {
        corruptionText.text = $"Corruption: {stats.Corruption}";
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
