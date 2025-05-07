using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FoodSpawner : MonoBehaviour
{
    [Header("Prefab & Spawn Settings")]
    [Tooltip("The food prefab to spawn when touched.")]
    public GameObject foodPrefab;

    [Tooltip("Optional transform to position spawned food. If null, spawns at this object's position.")]
    public Transform spawnPoint;

    [Tooltip("If true, only spawn once. If false, can spawn repeatedly.")]
    public bool spawnOnce = false;

    private bool hasSpawned = false;

    void Reset()
    {
        // Ensure collider is trigger by default
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        // You can change the tag to match your hand/controller tag
        if ((other.CompareTag("PlayerHand") || other.CompareTag("XRController")) && !hasSpawned)
        {
            SpawnFood();
        }
    }

    void SpawnFood()
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
        {
            // Optional: disable this script so it won\'t spawn again
            enabled = false;
        }
    }
}
