using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private StatsManager stats => GameManager.runStats;
    private RoomManager roomManager => RoomManager.Instance;
    public LifeManager lifeManager;
    public MinimapController minimapController;

    [Header("Stats Texts")]
    public TextMeshProUGUI moveSpeedText;
    public TextMeshProUGUI fireRateText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI rangeText;
    public TextMeshProUGUI corruptionGainRateText;
    public TextMeshProUGUI corruptionHitPenaltyText;
    public TextMeshProUGUI spreadAngleText;
    public TextMeshProUGUI shotSpeedText;
    public TextMeshProUGUI luckText;
    public Image corruptionBar;

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
        stats.OnSpreadAngleChanged += UpdateSpreadAngleUI;
        stats.OnShotSpeedChanged += UpdateShotSpeedUI;
        stats.OnLuckChanged += UpdateLuckUI;
        stats.OnCorruptionGainRateChanged += UpdateCorruptionGainRateUI;
        stats.OnCorruptionHitPenaltyChanged += UpdateCorruptionHitPenaltyUI;
        roomManager.OnEnterRoom += UpdateMinimap;

        // Initial UI update
        UpdateHealthUI();
        UpdateMoveSpeedUI();
        UpdateFireRateUI();
        UpdateDamageUI();
        UpdateRangeUI();
        UpdateCorruptionUI();
        UpdateSpreadAngleUI();
        UpdateShotSpeedUI();
        UpdateLuckUI();
        UpdateCorruptionGainRateUI();
        UpdateCorruptionHitPenaltyUI();
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null && GameManager.runStats != null)
        {
            GameManager.runStats.OnHealthChanged -= UpdateHealthUI;
            GameManager.runStats.OnMoveSpeedChanged -= UpdateMoveSpeedUI;
            GameManager.runStats.OnFireRateChanged -= UpdateFireRateUI;
            GameManager.runStats.OnDamageChanged -= UpdateDamageUI;
            GameManager.runStats.OnRangeChanged -= UpdateRangeUI;
            GameManager.runStats.OnCorruptionChanged -= UpdateCorruptionUI;
            GameManager.runStats.OnSpreadAngleChanged -= UpdateSpreadAngleUI;
            GameManager.runStats.OnShotSpeedChanged -= UpdateShotSpeedUI;
            GameManager.runStats.OnLuckChanged -= UpdateLuckUI;
            GameManager.runStats.OnCorruptionGainRateChanged -= UpdateCorruptionGainRateUI;
            GameManager.runStats.OnCorruptionHitPenaltyChanged -= UpdateCorruptionHitPenaltyUI;
        }

        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.OnEnterRoom -= UpdateMinimap;
        }
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
        corruptionBar.fillAmount = stats.Corruption;
    }
    void UpdateSpreadAngleUI()
    {
        spreadAngleText.text = $"Spread Angle: {stats.SpreadAngle}";
    }
    void UpdateShotSpeedUI()
    {
        shotSpeedText.text = $"Shot Speed: {stats.ShotSpeed}";
    }
    void UpdateLuckUI()
    {
        luckText.text = $"Luck: {stats.Luck}";
    }
    void UpdateCorruptionGainRateUI()
    {
        corruptionGainRateText.text = $"Corruption Gain Rate: {stats.CorruptionGainRate}";
    }
    void UpdateCorruptionHitPenaltyUI()
    {
        corruptionHitPenaltyText.text = $"Corruption Hit Penalty: {stats.CorruptionHitPenalty}";
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
