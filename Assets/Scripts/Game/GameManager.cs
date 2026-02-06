using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
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
            runtimeStats.Corruption += (runtimeStats.passiveGainRate / 100f) * Time.deltaTime;
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
        runtimeStats.MaxHealth = playerData.baseHealth;
        runtimeStats.Health = playerData.baseHealth;
        runtimeStats.MoveSpeed = playerData.moveSpeed;
        runtimeStats.FireRate = playerData.fireRate;
        runtimeStats.Damage = playerData.damage;
        runtimeStats.Range = playerData.range;

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

        if (floorGenerator != null)
        {
            floorGenerator.SetupFloor();
        }
        else
        {
            Debug.LogError("FloorGenerator not assigned !");
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) Instantiate(playerData.prefab, Vector3.zero, Quaternion.identity);

        floorGenerator.SetupFloor();

        MusicManager.Instance.PlayMusic("Floor");
    }

    // runs every physics step
    // void FixedUpdate()
    // {
    // }

    // void LateUpdate()
    // {

    // }
}
