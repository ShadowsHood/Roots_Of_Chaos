using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    // [Header("Data")]
    // [HideInInspector]
    public static PlayerStats runtimeStats;

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
    // void Update() {
    // }

    // runs every physics step
    // void FixedUpdate()
    // {
    // }

    // void LateUpdate()
    // {

    // }
}
