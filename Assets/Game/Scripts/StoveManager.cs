using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveManager : MonoBehaviour
{
    public GameObject stoveVisual; // Parte visual de la estufa que se enciende
    public bool isOn = false;

    // Método para encender la estufa
    public void TurnOn()
    {
        // Solo enciende la estufa si no está ya encendida
        if (!isOn)
        {
            isOn = true;
            stoveVisual.SetActive(true);
            Debug.Log("Estufa encendida");
        }
        else
        {
            Debug.Log("La estufa ya está encendida.");
        }
    }

    // Método para apagar la estufa
    public void TurnOff()
    {
        // Solo apaga la estufa si no está ya apagada
        if (isOn)
        {
            isOn = false;
            stoveVisual.SetActive(false);
            Debug.Log("Estufa apagada");
        }
        else
        {
            Debug.Log("La estufa ya está apagada.");
        }
    }
}
