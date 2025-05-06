using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveManager : MonoBehaviour
{
    public GameObject stoveVisual; // parte que se enciende
    public bool isOn = false;

    public void TurnOn()
    {
        isOn = true;
        stoveVisual.SetActive(true);
        Debug.Log("Estufa encendida");
    }

    public void TurnOff()
    {
        isOn = false;
        stoveVisual.SetActive(false);
        Debug.Log("Estufa apagada");
    }
}

