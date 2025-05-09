using System.Collections;
using UnityEngine;

public class KnifeCutDetector : MonoBehaviour {
    [SerializeField] private float cutVelocityThreshold = 0.2f; // velocidad mínima para considerar que es un corte
    private Vector3 lastPosition;
    private Vector3 velocity;

    private void Update() {
        velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other) {
        if (velocity.magnitude >= cutVelocityThreshold) {
            // Detectamos colisión con objeto de cocina
            if (other.TryGetComponent(out CuttingCounter cuttingCounter)) {
                cuttingCounter.CutByKnife(); // Llamamos al nuevo método
            }
        }
    }
}
