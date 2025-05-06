using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveButton : MonoBehaviour
{
    public StoveManager stove;
    public bool isGreenButton = true; // cambiar esto según sea el verde o rojo

    public void OnButtonPressed()
    {
        if (isGreenButton)
            stove.TurnOn();
        else
            stove.TurnOff();
    }
}

