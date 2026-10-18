using System;
using System.Collections;
using UnityEngine;
using System.Diagnostics;

public class ShotTracker : MonoBehaviour
{
    public static ShotTracker Instance { get; private set; }

    public event Action<int> TargetLanded;
    public event Action<int> ShotEnded;

    [Header("Shot Settings")]
    [SerializeField] private int maxShots = 3;

    private int currentShotId;
    private int shotsRemaining;

    private bool shotActive;

    public int CurrentShotId => currentShotId;
    public int ShotsRemaining => shotsRemaining;
    public bool ShotActive => shotActive;

    [Header("Shot Completion")]
    [SerializeField] private float minShotDuration = 0.75f;
    [SerializeField] private float quietTimeRequired = 1.25f;
    [SerializeField] private float velocityThreshold = 0.05f;
    [SerializeField] private float maxShotDuration = 4f;

    [Header("Scoring")]
    [SerializeField] private int pointsPerTarget = 10;

    private int score;
    private int targetsLandedThisShot;

    public int Score => score;
    public int TargetsLandedThisShot => targetsLandedThisShot;

    public event System.Action<int> ScoreChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        shotsRemaining = maxShots;
    }

    public void ResetTracker()
    {
        currentShotId = 0;
        shotsRemaining = maxShots;
        targetsLandedThisShot = 0;
        score = 0;
        shotActive = false;

        UnityEngine.Debug.Log("ShotTracker reset for new game.");
    }

    public bool BeginShot()
    {
        if (shotsRemaining <= 0)
        {
            UnityEngine.Debug.Log("No shots remaining.");
            return false;
        }

        if (shotActive)
        {
            UnityEngine.Debug.LogWarning("A shot is already active.");
            return false;
        }

        currentShotId++;
        shotsRemaining--;
        targetsLandedThisShot = 0;
        shotActive = true;

        UnityEngine.Debug.Log(
            $"Shot #{currentShotId} started. " +
            $"Shots remaining: {shotsRemaining}"
        );
        StartCoroutine(WaitForShotToSettle());

        return true;
    }


    public void RegisterTargetLanded(int shotId)
    {
        if (!shotActive || shotId != currentShotId)
            return;

        targetsLandedThisShot++;
        score += pointsPerTarget;

        UnityEngine.Debug.Log($"Score is now: {score}");

        ScoreChanged?.Invoke(score);
        TargetLanded?.Invoke(shotId);
    }


    public void EndShot()
    {
        if (!shotActive)
            return;

        shotActive = false;

        UnityEngine.Debug.Log($"Shot #{currentShotId} ended.");

        ShotEnded?.Invoke(currentShotId);
    }

    private IEnumerator WaitForShotToSettle()
    {
        float elapsedTime = 0f;

        yield return new WaitForSeconds(minShotDuration);
        elapsedTime += minShotDuration;

        float quietTimer = 0f;

        while (shotActive)
        {
            elapsedTime += Time.deltaTime;

            // Failsafe: akhiri shot jika melewati batas waktu.
            if (elapsedTime >= maxShotDuration)
            {
                UnityEngine.Debug.Log("Shot ended: maximum duration reached.");
                EndShot();
                yield break;
            }

            bool allQuiet = true;

            Rigidbody[] bodies = FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);

            foreach (Rigidbody body in bodies)
            {
                bool isRelevant =
                    body.GetComponent<Ball>() != null ||
                    body.GetComponent<Target>() != null;

                if (!isRelevant || body.isKinematic)
                    continue;

                if (body.linearVelocity.sqrMagnitude >
                        velocityThreshold * velocityThreshold ||
                    body.angularVelocity.sqrMagnitude >
                        velocityThreshold * velocityThreshold)
                {
                    allQuiet = false;
                    break;
                }
            }

            if (allQuiet)
            {
                quietTimer += Time.deltaTime;

                if (quietTimer >= quietTimeRequired)
                {
                    EndShot();
                    yield break;
                }
            }
            else
            {
                quietTimer = 0f;
            }

            yield return null;
        }
    }

    [ContextMenu("Debug/Begin Shot")]
    private void DebugBeginShot()
    {
        BeginShot();
    }
}
