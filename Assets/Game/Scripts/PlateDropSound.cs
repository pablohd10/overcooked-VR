using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable), typeof(Rigidbody), typeof(AudioSource))]
public class PlateDropSound : MonoBehaviour
{
    [Header("Drop Sound Settings")]
    [Tooltip("Sonido que se reproduce al caer el plato.")]
    public AudioClip dropClip;

    [Tooltip("Velocidad mínima al colisionar para disparar el sonido.")]
    public float impactVelocityThreshold = 1.5f;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    private AudioSource audioSource;
    private bool playOnNextHit = false;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        // Configura AudioSource
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    void OnEnable()
    {
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnDisable()
    {
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        // Al soltarlo, activamos la escucha de colisión
        playOnNextHit = true;
        Debug.Log("[PlateDropSound] Plate released; will play sound on next impact.");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!playOnNextHit) return;

        // Comprobamos la velocidad del impacto
        float impactSpeed = rb.velocity.magnitude;
        Debug.Log($"[PlateDropSound] Collision detected with speed={impactSpeed}");

        if (impactSpeed >= impactVelocityThreshold)
        {
            if (dropClip != null)
            {
                audioSource.PlayOneShot(dropClip);
                Debug.Log("[PlateDropSound] Drop sound played.");
            }
            else
            {
                Debug.LogWarning("[PlateDropSound] No dropClip asignado en el Inspector.");
            }

            // Desactivamos hasta la próxima vez que lo sueltes
            playOnNextHit = false;
        }
    }
}
