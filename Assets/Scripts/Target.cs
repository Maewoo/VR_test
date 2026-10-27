using UnityEngine;

public class Target : MonoBehaviour
{
    private bool hasLanded;

    public void ResetTarget()
    {
        hasLanded = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent<Ground>(out _))
            return;

        if (hasLanded)
            return;

        hasLanded = true;

        if (ShotTracker.Instance == null)
        {
            Debug.LogError("ShotTracker.Instance is null!");
            return;
        }

        int landedShotId = ShotTracker.Instance.CurrentShotId;

        Debug.Log($"Scoring target: {gameObject.name}");

        ShotTracker.Instance.RegisterTargetLanded(landedShotId);

        Debug.Log(
            $"{gameObject.name} landed during shot #{landedShotId}."
        );
        Destroy(gameObject);
    }
}
