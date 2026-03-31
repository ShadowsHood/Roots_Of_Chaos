using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    // [Header("Data")]
    // [HideInInspector]
    public FloorGenerator floorGenerator;
    public static bool inCombat = false;
    public static bool isGamePaused = false;
    public PlayerData playerData;
    [SerializeField] private StatsManager runtimeStats;
    public static StatsManager runStats => Instance.runtimeStats;
    public Transform projectilesRoot;
    public Transform itemsRoot;
    private bool isTakingCorruptionDamage = false;

    // set things up (before the game starts)
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        runtimeStats = ScriptableObject.CreateInstance<StatsManager>();
    }

    // initialize things once
    void Start()
    {
        StartGame();
    }

    // runs every frame
    void Update()
    {
        if (inCombat)
        {
            runtimeStats.Corruption += (runtimeStats.CorruptionGainRate / 100f) * Time.deltaTime;
            if (runStats.IsCorrupted && !isTakingCorruptionDamage)
            {
                StartCoroutine(CorruptionDamageRoutine());
            }
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
            StartGame();
    }

    void StartGame()
    {
        if (playerData == null)
        {
            Debug.LogError("PlayerData missing !");
            return;
        }
        // Reset stats
        runtimeStats.MaxHealth = playerData.baseHealth;
        runtimeStats.Health = playerData.baseHealth;
        runtimeStats.MoveSpeed = playerData.moveSpeed;
        runtimeStats.FireRate = playerData.fireRate;
        runtimeStats.Damage = playerData.damage;
        runtimeStats.Range = playerData.range;
        runtimeStats.SpreadAngle = playerData.spreadAngle;
        runtimeStats.ShotSpeed = playerData.shotSpeed;
        runtimeStats.Luck = playerData.luck;
        runtimeStats.CorruptionGainRate = playerData.corruptionGainRate;
        runtimeStats.CorruptionHitPenalty = playerData.corruptionHitPenalty;
        runtimeStats.Corruption = 0f;

        // Reset sacrifices
        runtimeStats.maxHealthSacrifice = 0;
        runtimeStats.damageSacrifice = 0;
        runtimeStats.speedSacrifice = 0;
        runtimeStats.rangeSacrifice = 0;
        runtimeStats.fireRateSacrifice = 0;
        runtimeStats.corruptionGainRateSacrifice = 0;
        runtimeStats.corruptionHitPenaltySacrifice = 0;
        runtimeStats.spreadAngleSacrifice = 0;
        runtimeStats.shotSpeedSacrifice = 0;
        runtimeStats.luckSacrifice = 0;

        if (projectilesRoot != null)
        {
            foreach (Transform child in projectilesRoot) Destroy(child.gameObject);
        }

        if (itemsRoot != null)
        {
            foreach (Transform child in itemsRoot) Destroy(child.gameObject);
        }

        if (InventoryController.Instance != null)
        {
            InventoryController.Instance.inventory.Clear();
            InventoryController.Instance.RecalculateStats();
        }

        if (PlayerController.Instance == null) Instantiate(playerData.prefab, Vector3.zero, Quaternion.identity);

        if (floorGenerator != null)
        {
            floorGenerator.SetupFloor();
        }
        else
        {
            Debug.LogError("FloorGenerator not assigned !");
        }

        MusicManager.Instance.PlayMusic("Floor", 0.5f, 1.15f);
    }

    IEnumerator CorruptionDamageRoutine()
    {
        isTakingCorruptionDamage = true;

        while (runStats.IsCorrupted)
        {
            if (PlayerController.Instance != null) PlayerController.Instance.TakeDamage(1, PlayerController.Instance.transform.position);
            yield return new WaitForSeconds(runStats.CorruptionDamageInterval);
        }

        isTakingCorruptionDamage = false;
    }

    // runs every physics step
    // void FixedUpdate()
    // {
    // }

    // void LateUpdate()
    // {

    // }
}
