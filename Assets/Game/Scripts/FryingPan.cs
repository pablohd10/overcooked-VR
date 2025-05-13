using System.Collections.Generic; // Para usar listas
using UnityEngine; // Acceso a componentes de Unity

public class FryingPan : MonoBehaviour
{
    // 🔥 Referencia a la estufa sobre la que puede colocarse esta sartén
    public StoveManager stove;

    // ✨ Sistema de partículas para el efecto de cocinado (chisporroteo)
    public GameObject sizzlingParticles;

    // 🔊 Sonido de cocinado que se reproducirá si hay comida y la estufa está encendida
    public AudioSource sizzlingSound;

    // 🍖 Lista de los objetos de comida que actualmente están dentro de la sartén
    private List<GameObject> contents = new List<GameObject>();

    // 📍 Indica si la sartén está actualmente colocada sobre una estufa encendida
    private bool isOverStove = false;

    void Start()
    {
        // Al iniciar, desactivamos las partículas si están asignadas
        if (sizzlingParticles != null)
            sizzlingParticles.SetActive(false);
    }

    void Update()
    {
        // ✅ Condición para activar partículas y sonido:
        // Si la sartén está sobre la estufa, la estufa está encendida, y hay comida dentro
        if (stove != null && stove.isOn && isOverStove && contents.Count > 0)
        {
            // Activamos partículas si no están activas
            if (!sizzlingParticles.activeSelf)
                sizzlingParticles.SetActive(true);

            // Reproducimos sonido si no está ya sonando
            if (!sizzlingSound.isPlaying)
                sizzlingSound.Play();
            
            foreach (GameObject food in contents)
            {
                CookableFood cookable = food.GetComponent<CookableFood>();
                if (cookable != null)
                {
                    cookable.StartCooking();
                }
            }
        }
        else
        {
            // Si no se cumple la condición, apagamos partículas y sonido
            if (sizzlingParticles.activeSelf)
                sizzlingParticles.SetActive(false);

            if (sizzlingSound.isPlaying)
                sizzlingSound.Stop();
            
            foreach (GameObject food in contents)
            {
                CookableFood cookable = food.GetComponent<CookableFood>();
                if (cookable != null)
                {
                    cookable.StopCooking();
                }
            }
        }
    }

    // ✅ Detectamos colisiones con objetos usando trigger
    private void OnTriggerEnter(Collider other)
    {
        // Si el objeto que entra tiene el tag "Food", lo añadimos a la lista
        if (other.CompareTag("Food"))
        {
            contents.Add(other.gameObject);
        }
        // Si entra en la zona con tag "StoveZone", consideramos que está sobre la estufa
        else if (other.CompareTag("StoveZone"))
        {
            isOverStove = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Si un objeto de comida sale de la sartén, lo quitamos de la lista
        if (other.CompareTag("Food"))
        {
            contents.Remove(other.gameObject);
        }
        // Si sale de la zona "StoveZone", ya no está sobre la estufa
        else if (other.CompareTag("StoveZone"))
        {
            isOverStove = false;
        }
    }
}
