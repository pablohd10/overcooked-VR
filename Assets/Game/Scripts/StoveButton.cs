using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
public class StoveButton : MonoBehaviour
{
    public StoveManager stove; // Referencia al script del StoveManager
    public AudioClip clickSound; // Sonido que se reproducirá al presionar el botón
    public bool isGreenButton = true; // Indica si el botón es verde (encender) o rojo (apagar)

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable button; // Referencia al componente XRSimpleInteractable

    private void Awake()
    {
        // Obtiene el componente XRSimpleInteractable del GameObject
        button = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();

        // Agrega un listener al evento selectEntered, que se activa cuando el usuario interactúa con el botón
        button.selectEntered.AddListener(OnButtonPressed);
    }

    private void OnDestroy()
    {
        // Elimina el listener del evento selectEntered cuando el objeto se destruye, para evitar errores
        button.selectEntered.RemoveListener(OnButtonPressed);
    }

    private void OnButtonPressed(SelectEnterEventArgs args)
    {
        // Reproducir el sonido al presionar el botón
        if (clickSound != null)
        {
            AudioSource.PlayClipAtPoint(clickSound, transform.position);
        }

        // Llama al método correspondiente según el tipo de botón
        if (isGreenButton)
            stove.TurnOn();
        else
            stove.TurnOff();
    }
}
