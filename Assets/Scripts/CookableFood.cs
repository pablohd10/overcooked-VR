using UnityEngine;
using UnityEngine.UI;

public class CookableFood : MonoBehaviour
{
    public GameObject cookedVersion;  // Prefab de la carne cocinada
    public GameObject burnedVersion;  // Prefab de la carne quemada

    public float cookTime = 5f;       // Tiempo hasta que se cocina
    public float burnTime = 10f;      // Tiempo total hasta que se quema

    private float currentCookTimer = 0f;
    private bool isCooking = false;
    private bool isBurning = false;

    public GameObject progressBarPrefab;  // Prefab de la barra de progreso
    private GameObject progressBarInstance;  // Instancia de la barra
    private Image fillImage;  // Imagen de la barra

    // Colores para el progreso
    public Color cookingColor = Color.green;    // Color cuando se cocina
    public Color burningColor = Color.red;      // Color cuando se quema

    public void StartCooking()
    {
        if (!isCooking)
        {
            isCooking = true;
            CreateProgressBar();  // Crear la barra de progreso
        }
    }

    public void StopCooking()
    {
        isCooking = false;
    }

    private void CreateProgressBar()
    {
        if (progressBarPrefab == null) return;

        // Crear la barra de progreso en el mundo
        progressBarInstance = Instantiate(progressBarPrefab, transform.position + Vector3.up * 0.2f, Quaternion.identity);
        progressBarInstance.transform.SetParent(transform); // Seguirá el objeto 3D
        fillImage = progressBarInstance.GetComponentInChildren<Image>();
    }

    void Update()
    {
        if (!isCooking) return;

        currentCookTimer += Time.deltaTime;

        if (fillImage != null)
        {
            // Cambiar el valor del fillAmount basado en el tiempo
            float total = isBurning ? burnTime : cookTime;
            fillImage.fillAmount = Mathf.Clamp01(currentCookTimer / total);

            // Cambiar el color de la barra dependiendo del progreso
            if (currentCookTimer < cookTime)
            {
                // Color durante la cocción (verde, amarillo, etc.)
                fillImage.color = cookingColor;
            }
            else if (currentCookTimer < burnTime)
            {
                // Color durante el proceso de quemado (amarillo o naranja)
                fillImage.color = Color.Lerp(cookingColor, burningColor, (currentCookTimer - cookTime) / (burnTime - cookTime));
            }
            else
            {
                // Color cuando está completamente quemado (rojo)
                fillImage.color = burningColor;
            }
        }

        if (currentCookTimer >= burnTime)
        {
            Burn();
        }
        else if (currentCookTimer >= cookTime && !isBurning)
        {
            Cook();
        }
    }

    private void Cook()
    {
        if (cookedVersion != null)
        {
            // Reemplaza la carne cruda con la carne cocinada
            Instantiate(cookedVersion, transform.position, transform.rotation);
            Destroy(gameObject);  // Destruye la carne cruda

            // Establece que ya no estamos quemando, solo cocinando
            isBurning = false;
        }
    }

    private void Burn()
    {
        if (burnedVersion != null)
        {
            // Reemplaza la carne cocinada con la carne quemada
            Instantiate(burnedVersion, transform.position, transform.rotation);
            Destroy(gameObject);  // Destruye el objeto original

            // Destruye la barra de progreso
            if (progressBarInstance != null)
                Destroy(progressBarInstance);
        }
    }
}
