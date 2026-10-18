using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ThrowModeController : MonoBehaviour
{
    [Header("Throw Mode")]
    [SerializeField] private bool useButtonThrow = false;

    [Header("Button Throw Settings")]
    [SerializeField] private float buttonThrowForce = 15f;
    [SerializeField] private float buttonThrowSpin = 5f;
    [SerializeField] private KeyCode throwButton = KeyCode.T;

    [Header("Velocity Throw Settings (when disabled)")]
    [SerializeField] private float velocityScale = 2.0f;
    [SerializeField] private float angularVelocityScale = 1.2f;

    [Header("Rigidbody Settings")]
    [SerializeField] private float mass = 0.6f;
    [SerializeField] private float drag = 0.12f;
    [SerializeField] private float angularDrag = 0.08f;

    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    private bool isGrabbed;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
        ApplyPhysicsSettings();
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnSelectEntered);
            grabInteractable.selectExited.AddListener(OnSelectExited);
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnSelectEntered);
            grabInteractable.selectExited.RemoveListener(OnSelectExited);
        }
    }

    private void Update()
    {
        if (useButtonThrow && isGrabbed && Input.GetKeyDown(throwButton))
        {
            ApplyButtonThrow();
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        isGrabbed = true;

        // if (useButtonThrow)
        // {
        //     // Disable velocity-based throwing
        //     grabInteractable.velocityScale = 0f;
        //     grabInteractable.angularVelocityScale = 0f;
        //     Debug.Log("Button Throw mode: Press T to throw");
        // }
        // else
        // {
        //     // Enable velocity-based throwing
        //     grabInteractable.velocityScale = velocityScale;
        //     grabInteractable.angularVelocityScale = angularVelocityScale;
        // }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        isGrabbed = false;
    }

    private void ApplyButtonThrow()
    {
        if (rb == null || grabInteractable == null || grabInteractable.interactorsSelecting.Count == 0)
            return;

        // Get throw direction (forward from controller)
        var interactor = grabInteractable.interactorsSelecting[0];
        Transform controllerTransform = interactor.GetAttachTransform(grabInteractable);
        Vector3 throwDirection = controllerTransform != null
            ? controllerTransform.forward
            : Camera.main.transform.forward;

        // Apply linear velocity
        rb.linearVelocity = throwDirection * buttonThrowForce;

        // Apply angular velocity (spin)
        rb.angularVelocity = Random.insideUnitSphere * buttonThrowSpin;

        isGrabbed = false;
        grabInteractable.interactionManager.SelectExit(interactor, grabInteractable);

        Debug.Log($"Button throw applied: Force={buttonThrowForce}, Direction={throwDirection}");
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
            // grabInteractable.velocityScale = useButtonThrow ? 0f : velocityScale;
            // grabInteractable.angularVelocityScale = useButtonThrow ? 0f : angularVelocityScale;

            // Always use velocity scale so ball follows hand while grabbed
            grabInteractable.velocityScale = velocityScale;
            grabInteractable.angularVelocityScale = angularVelocityScale;
        }
    }

    [ContextMenu("Toggle Throw Mode")]
    public void ToggleThrowMode()
    {
        useButtonThrow = !useButtonThrow;
        ApplyPhysicsSettings();
        string mode = useButtonThrow ? "BUTTON THROW (Press T)" : "VELOCITY THROW";
        Debug.Log($"Throw mode changed to: {mode}");
    }

    [ContextMenu("Apply Settings")]
    public void RefreshSettings()
    {
        ApplyPhysicsSettings();
        Debug.Log($"Settings applied. Mode: {(useButtonThrow ? "Button Throw" : "Velocity Throw")}");
    }
}
