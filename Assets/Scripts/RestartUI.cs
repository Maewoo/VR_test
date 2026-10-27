using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RestartUI : MonoBehaviour
{
    [SerializeField] private GameObject restartPanel;
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private Button restartButton;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;

        if (gameManager == null)
        {
            Debug.LogError("RestartUI: GameManager not found!");
            return;
        }

        if (restartPanel == null || gameOverText == null || restartButton == null)
        {
            Debug.LogError("RestartUI: Missing UI references in Inspector!");
            return;
        }

        gameManager.GameEnded += OnGameEnded;
        restartButton.onClick.AddListener(OnRestartClicked);

        restartPanel.SetActive(false);

        Debug.Log("RestartUI successfully connected to GameManager.");
    }

    private void OnGameEnded(bool won)
    {
        restartPanel.SetActive(true);
        gameOverText.text = won ? "YOU WIN!\nPress Restart to play again" : "YOU LOSE!\nPress Restart to try again";
    }

    public void OnRestartClicked()
    {
        restartPanel.SetActive(false);
        SceneManager.LoadScene("GameScene");
        gameManager.StartGame();
    }

    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.GameEnded -= OnGameEnded;
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(OnRestartClicked);
        }
    }
}
