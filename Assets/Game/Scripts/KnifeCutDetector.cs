using System.Collections;
using UnityEngine;

public class KnifeCutDetector : MonoBehaviour {
    [SerializeField] private float cutVelocityThreshold = 0.2f; // Velocidad mínima para considerar que es un corte
    private Vector3 lastPosition;
    private Vector3 velocity;

    private void Update() {
        // Calcula la velocidad del cuchillo
        velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other) {
        // Verifica si la velocidad del cuchillo supera el umbral
        if (velocity.magnitude >= cutVelocityThreshold) {
            // Detecta si el objeto tiene un componente "CuttableItem" (representa la comida)
            if (other.TryGetComponent(out CuttableItem cuttableItem)) {
                // Llama al método para cortar la comida
                CutFood(cuttableItem);
            }
        }
    }

    private void CutFood(CuttableItem cuttableItem) {
        // Verifica si la comida tiene un prefab de versión cortada
        if (cuttableItem.cuttedPrefab != null) {
            // Reemplaza el objeto actual por su versión cortada
            Instantiate(cuttableItem.cuttedPrefab, cuttableItem.transform.position, cuttableItem.transform.rotation);
            Destroy(cuttableItem.gameObject); // Destruye el objeto original
        }
    }
}
