using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ThrowPhysicsController : MonoBehaviour
{
    [Header("Launch Force Settings")]
    [SerializeField] private float velocityScale = 1.5f;
    [SerializeField] private float angularVelocityScale = 1f;

    [Header("Rigidbody Settings")]
    [SerializeField] private float mass = 0.5f;
    [SerializeField] private float drag = 0.1f;
    [SerializeField] private float angularDrag = 0.05f;

    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        ApplyPhysicsSettings();
    }

    private void ApplyPhysicsSettings()
    {
        if (rb != null)
        {
            rb.mass = mass;
            rb.linearDamping = drag;
            rb.angularDamping = angularDrag;
        }

        if (grabInteractable != null)
        {
            grabInteractable.velocityScale = velocityScale;
            grabInteractable.angularVelocityScale = angularVelocityScale;
        }
    }

    [ContextMenu("Apply Physics Settings")]
    public void RefreshPhysicsSettings()
    {
        ApplyPhysicsSettings();
        Debug.Log($"Physics applied: VelocityScale={velocityScale}, Mass={mass}, Drag={drag}");
    }
}
