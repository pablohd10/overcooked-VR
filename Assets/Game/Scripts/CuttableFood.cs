using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CuttableFood : MonoBehaviour
{
    [Header("Cut Settings")]
    [Tooltip("Number of cuts required to replace with sliced prefab.")]
    public int requiredCuts = 3;
    [Tooltip("Prefab to instantiate when fully cut.")]
    public GameObject slicedPrefab;

    private int cutCount = 0;
    private bool isCut = false;

    private void Start()
    {
        // Ensure this collider is trigger
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
        Debug.Log($"[CuttableFood] Start: {gameObject.name} requires {requiredCuts} cuts.");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[CuttableFood] OnTriggerEnter: '{gameObject.name}' hit by '{other.gameObject.name}' (tag={other.tag}).");
        if (isCut)
        {
            Debug.Log($"[CuttableFood] {gameObject.name} is already cut. Ignoring.");
            return;
        }
        if (!other.CompareTag("Knife"))
        {
            Debug.Log($"[CuttableFood] Ignored collision with non-knife object '{other.gameObject.name}'.");
            return;
        }

        cutCount++;
        Debug.Log($"[CuttableFood] {gameObject.name} cut count: {cutCount}/{requiredCuts}.");

        if (cutCount >= requiredCuts)
        {
            DoSlice();
        }
    }

    private void DoSlice()
    {
        isCut = true;
        Debug.Log($"[CuttableFood] {gameObject.name} reached required cuts. Slicing now.");

        // Save transform data
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;
        Transform parent = transform.parent;

        // Destroy original
        Destroy(gameObject);

        // Instantiate sliced version
        GameObject sliced = Instantiate(slicedPrefab, pos, rot, parent);
        sliced.tag = "Cuttable";
        Debug.Log($"[CuttableFood] Spawned sliced prefab '{sliced.name}' at {pos}.");
    }
}
