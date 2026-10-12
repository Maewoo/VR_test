
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;

        if (gameManager == null)
        {
            Debug.LogError("TimerUI: GameManager tidak ditemukan!");
            return;
        }

        if (timerText == null)
        {
            Debug.LogError("TimerUI: Timer Text belum dihubungkan!");
            return;
        }

        gameManager.TimeChanged += UpdateTimer;
        UpdateTimer(gameManager.TimeRemaining);

        Debug.Log("TimerUI berhasil terhubung ke GameManager.");
    }

    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.TimeChanged -= UpdateTimer;
        }
    }

    private void UpdateTimer(float timeRemaining)
    {
        int seconds = Mathf.CeilToInt(timeRemaining);
        timerText.text = $"Time: {seconds}";
    }
}
