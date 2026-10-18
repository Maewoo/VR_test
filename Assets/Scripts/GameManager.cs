using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private float gameDuration = 30f;

    [Header("Target Settings")]
    [SerializeField] private Target[] targets;

    private float timeRemaining;
    private int targetsRemaining;

    private bool isPlaying;
    private bool timerExpired;
    private bool gameEnded;

    public float TimeRemaining => timeRemaining;
    public bool IsPlaying => isPlaying;
    public int TargetsRemaining => targetsRemaining;

    public event Action<float> TimeChanged;
    public event Action<bool> GameEnded;

    private ShotTracker shotTracker;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        timeRemaining = gameDuration;
    }

    private void Start()
    {
        targets = FindObjectsByType<Target>(
            FindObjectsSortMode.None
        );

        targetsRemaining = targets.Length;

        shotTracker = ShotTracker.Instance;

        if (shotTracker == null)
        {
            Debug.LogError("GameManager: ShotTracker not found!");
            return;
        }

        shotTracker.TargetLanded += OnTargetLanded;
        shotTracker.ShotEnded += OnShotEnded;

        Debug.Log($"Targets found: {targetsRemaining}");

        StartGame();
    }

    private void Update()
    {
        if (!isPlaying || timerExpired)
            return;

        timeRemaining -= Time.deltaTime;
        timeRemaining = Mathf.Max(0f, timeRemaining);

        TimeChanged?.Invoke(timeRemaining);

        if (timeRemaining <= 0f)
        {
            timerExpired = true;
            Debug.Log("Time is up!");

            EvaluateGameEnd();
        }
    }

    public void StartGame()
    {
        timeRemaining = gameDuration;
        timerExpired = false;
        isPlaying = true;
        gameEnded = false;

        // Reset shot tracker for new game
        if (shotTracker != null)
        {
            shotTracker.ResetTracker();
        }

        // Reset all targets for new game
        foreach (Target target in targets)
        {
            target.ResetTarget();
        }

        // Reset all balls for new game
        Ball[] balls = FindObjectsByType<Ball>(FindObjectsSortMode.None);
        foreach (Ball ball in balls)
        {
            ball.ResetBall();
        }

        TimeChanged?.Invoke(timeRemaining);

        Debug.Log("Game started!");
    }

    private void OnTargetLanded(int shotId)
    {
        if (gameEnded)
            return;

        targetsRemaining = Mathf.Max(0, targetsRemaining - 1);

        Debug.Log($"Targets remaining: {targetsRemaining}");

        if (targetsRemaining == 0)
        {
            EndGame(true);
        }
    }

    private void OnShotEnded(int shotId)
    {
        EvaluateGameEnd();
    }

    private void EvaluateGameEnd()
    {
        if (gameEnded || !isPlaying)
            return;

        // Winning always takes priority.
        if (targetsRemaining <= 0)
        {
            EndGame(true);
            return;
        }

        // Wait for the active throw and domino effects to settle.
        if (shotTracker != null && shotTracker.ShotActive)
            return;

        // Lose if time expired or all throws have been used.
        if (timerExpired ||
            (shotTracker != null && shotTracker.ShotsRemaining <= 0))
        {
            EndGame(false);
        }
    }

    private void EndGame(bool won)
    {
        if (gameEnded)
            return;

        gameEnded = true;
        isPlaying = false;

        Debug.Log(won ? "YOU WIN!" : "YOU LOSE!");

        GameEnded?.Invoke(won);
    }

    private void OnDestroy()
    {
        if (shotTracker != null)
        {
            shotTracker.TargetLanded -= OnTargetLanded;
            shotTracker.ShotEnded -= OnShotEnded;
        }

        if (Instance == this)
            Instance = null;
    }
}
