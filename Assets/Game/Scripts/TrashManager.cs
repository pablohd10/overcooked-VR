using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashManager : MonoBehaviour
{
    public AudioClip trashSound; // Arrastra el sonido desde el Inspector
    public float volume = 1.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            // Reproducir sonido en la posición de la basura
            if (trashSound != null)
            {
                AudioSource.PlayClipAtPoint(trashSound, transform.position, volume);
            }

            // Destruir el objeto
            Destroy(other.gameObject);
        }
    }
}