using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    private ShotTracker shotTracker;

    private void Start()
    {
        shotTracker = ShotTracker.Instance;

        if (shotTracker == null)
        {
            Debug.LogError("ScoreUI: ShotTracker.Instance not found!");
            return;
        }

        if (scoreText == null)
        {
            Debug.LogError("ScoreUI: Score Text not assigned in Inspector!");
            return;
        }

        shotTracker.ScoreChanged += UpdateScore;
        UpdateScore(shotTracker.Score);

        Debug.Log("ScoreUI successfully connected to ShotTracker.");
    }

    private void OnDestroy()
    {
        if (shotTracker != null)
        {
            shotTracker.ScoreChanged -= UpdateScore;
        }
    }

    private void UpdateScore(int score)
    {
        if (scoreText == null)
            return;

        scoreText.text = $"Score: {score}";
    }
}
