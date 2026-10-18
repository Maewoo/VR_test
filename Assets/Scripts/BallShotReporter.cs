
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Ball))]
public class BallShotReporter : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private Ball ball;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        ball = GetComponent<Ball>();
    }

    private void OnEnable()
    {
        grabInteractable.selectExited.AddListener(OnSelectExited);

        if (ShotTracker.Instance != null)
        {
            ShotTracker.Instance.ShotEnded += OnShotEnded;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameEnded += OnGameEnded;
        }
    }

    private void OnDisable()
    {
        grabInteractable.selectExited.RemoveListener(OnSelectExited);

        if (ShotTracker.Instance != null)
        {
            ShotTracker.Instance.ShotEnded -= OnShotEnded;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameEnded -= OnGameEnded;
        }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        if (ShotTracker.Instance == null)
            return;

        bool shotStarted = ShotTracker.Instance.BeginShot();

        if (shotStarted)
        {
            grabInteractable.enabled = false;
            Debug.Log($"{gameObject.name} thrown.");
        }
    }

    private void OnShotEnded(int shotId)
    {
        ball.ResetBall();
        grabInteractable.enabled = true;
        Debug.Log($"{gameObject.name} ready for next throw.");
    }

    private void OnGameEnded(bool won)
    {
        ball.ResetBall();
        grabInteractable.enabled = true;
        Debug.Log($"{gameObject.name} reset for game over.");
    }
}
