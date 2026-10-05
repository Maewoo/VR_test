using UnityEngine;

public class Target : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Target hit by: " + collision.gameObject.name);
    }
}

