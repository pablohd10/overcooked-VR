using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Este script requiere que el GameObject tenga un componente XRSimpleInteractable.
[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
public class FoodSpawner : MonoBehaviour
{
    [Header("Prefab & Spawn Settings")]
    public GameObject foodPrefab; // Prefab del objeto de comida que se generará.
    public Transform spawnPoint; // Punto donde se generará el objeto. Si no se asigna, usará la posición del GameObject.
    public bool spawnOnce = false; // Si es true, el objeto solo se generará una vez.

    private bool hasSpawned = false; // Indica si ya se ha generado un objeto (para controlar spawnOnce).
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable button; // Referencia al componente XRSimpleInteractable.

    void Awake()
    {
        // Obtiene el componente XRSimpleInteractable del GameObject.
        button = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();

        // Agrega un listener al evento selectEntered, que se activa cuando el usuario interactúa con el botón.
        button.selectEntered.AddListener(OnPressed);
    }

    void OnDestroy()
    {
        // Elimina el listener del evento selectEntered cuando el objeto se destruye, para evitar errores.
        button.selectEntered.RemoveListener(OnPressed);
    }

    private void OnPressed(SelectEnterEventArgs args)
    {
        // Si ya se ha generado un objeto y spawnOnce es true, no hace nada.
        if (hasSpawned && spawnOnce) return;

        // Llama al método para generar el objeto de comida.
        SpawnFood();
    }

    public void SpawnFood()
    {
        // Si no se ha asignado un prefab, muestra una advertencia en la consola y no genera nada.
        if (foodPrefab == null)
        {
            Debug.LogWarning("FoodSpawner: No foodPrefab assigned.");
            return;
        }

        // Determina la posición y rotación donde se generará el objeto.
        Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position; // Usa el spawnPoint si está asignado, de lo contrario usa la posición del GameObject.
        Quaternion rot = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity; // Usa la rotación del spawnPoint o la rotación por defecto.

        // Genera el objeto de comida en la posición y rotación especificadas.
        Instantiate(foodPrefab, pos, rot);

        // Marca que ya se ha generado un objeto.
        hasSpawned = true;

        // Si spawnOnce es true, desactiva el botón para evitar más interacciones.
        if (spawnOnce)
            button.enabled = false;
    }
}
