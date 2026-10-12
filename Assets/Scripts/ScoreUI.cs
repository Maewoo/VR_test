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
            Debug.LogError("ScoreUI: ShotTracker.Instance tidak ditemukan!");
            return;
        }

        if (scoreText == null)
        {
            Debug.LogError("ScoreUI: Score Text belum dihubungkan di Inspector!");
            return;
        }

        shotTracker.ScoreChanged += UpdateScore;
        UpdateScore(shotTracker.Score);

        Debug.Log("ScoreUI berhasil terhubung ke ShotTracker.");
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
        Debug.Log($"ScoreUI menerima skor: {score}");
    }
}
