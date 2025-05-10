using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
public class FoodSpawner : MonoBehaviour
{
    [Header("Prefab & Spawn Settings")]
    public GameObject foodPrefab;
    public Transform spawnPoint;
    public bool spawnOnce = false;

    private bool hasSpawned = false;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable button;

    void Awake()
    {
        button = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        button.selectEntered.AddListener(OnPressed);
    }

    void OnDestroy()
    {
        button.selectEntered.RemoveListener(OnPressed);
    }

    private void OnPressed(SelectEnterEventArgs args)
    {
        if (hasSpawned && spawnOnce) return;
        SpawnFood();
    }

    public void SpawnFood()
    {
        if (foodPrefab == null)
        {
            Debug.LogWarning("FoodSpawner: No foodPrefab assigned.");
            return;
        }

        Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
        Quaternion rot = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        Instantiate(foodPrefab, pos, rot);

        hasSpawned = true;
        if (spawnOnce)
            button.enabled = false;
    }
}
