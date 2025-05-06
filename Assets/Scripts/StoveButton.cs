using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveButton : MonoBehaviour
{
    public StoveManager stove;
    public AudioClip clickSound;
    public bool isGreenButton = true; // Cambia esto según sea el verde o rojo

    public void OnButtonPressed()
    {
        if (clickSound != null)
        {
            // Reproducir el sonido en el AudioSource adjunto al objeto
            AudioSource.PlayClipAtPoint(clickSound, transform.position);
        }

        if (isGreenButton)
            stove.TurnOn();
        else
            stove.TurnOff();
    }
}
