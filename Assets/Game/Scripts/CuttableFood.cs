using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class CuttableFood : MonoBehaviour
{
    [Header("Cut Settings")]
    [Tooltip("Número de cortes necesarios para reemplazar con la versión cortada.")]
    public int requiredCuts = 3;

    [Tooltip("Prefab que reemplaza a este cuando está completamente cortado.")]
    public GameObject slicedPrefab;

    [Header("Audio")]
    [Tooltip("Sonido que se reproduce cada vez que cortas el alimento.")]
    public AudioClip cutClip;

    private int cutCount = 0;
    private bool isCut = false;
    private AudioSource audioSource;

    private void Awake()
    {
        // Asegura que el collider es trigger
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        // Consigue o añade el AudioSource
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        if (cutClip != null) audioSource.clip = cutClip;

        Debug.Log($"[CuttableFood] Awake: {gameObject.name} needs {requiredCuts} cuts.");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[CuttableFood] OnTriggerEnter: '{gameObject.name}' hit by '{other.gameObject.name}' (tag={other.tag}).");
        if (isCut)
        {
            Debug.Log($"[CuttableFood] {gameObject.name} already cut. Ignoring.");
            return;
        }
        if (!other.CompareTag("Knife"))
        {
            Debug.Log($"[CuttableFood] Ignoring collision with '{other.gameObject.name}'.");
            return;
        }

        // Reproducir sonido de corte
        if (cutClip != null)
            audioSource.PlayOneShot(cutClip);

        cutCount++;
        Debug.Log($"[CuttableFood] {gameObject.name} cut count: {cutCount}/{requiredCuts}.");

        if (cutCount >= requiredCuts)
            DoSlice();
    }

    private void DoSlice()
    {
        isCut = true;
        Debug.Log($"[CuttableFood] {gameObject.name} reached required cuts. Slicing now.");

        // Guarda posición y rotación
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;
        Transform parent = transform.parent;

        // Destruye el original
        Destroy(gameObject);

        // Instancia la versión cortada
        var sliced = Instantiate(slicedPrefab, pos, rot, parent);
        sliced.tag = "Cuttable";
        Debug.Log($"[CuttableFood] Spawned sliced prefab '{sliced.name}'.");
    }
}
