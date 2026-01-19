using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    // [Header("Data")]
    // [HideInInspector]
    public static bool inCombat = false;
    public static bool isGamePaused = false;
    [SerializeField] private PlayerStats runtimeStats;
    public static PlayerStats runStats => Instance.runtimeStats;



    // set things up (before the game starts)
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        runtimeStats = ScriptableObject.CreateInstance<PlayerStats>();
    }

    // initialize things once
    void Start()
    {

    }

    // runs every frame
    void Update()
    {
        if (inCombat)
        {
            runtimeStats.Corruption += (runtimeStats.passiveGainRate / 100f) * Time.deltaTime;
        }
    }

    // runs every physics step
    // void FixedUpdate()
    // {
    // }

    // void LateUpdate()
    // {

    // }
}
