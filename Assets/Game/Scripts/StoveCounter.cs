using System;
using UnityEngine;
using static CuttingCounter;

[RequireComponent(typeof(AudioSource))]
public class StoveCounter : MonoBehaviour, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs { public State state; }

    public enum State { Idle, Cooking, Cooked, Burned }

    [Header("Cooking Settings")]
    public float cookingTime = 5f;
    public float burnTime = 3f;

    [Header("References")]
    public Collider stoveZoneCollider;

    [Header("Prefabs")]
    public GameObject rawPrefab, cookedPrefab, burnedPrefab;

    [Header("Audio")]
    [Tooltip("Sizzle clip to play while cooking.")]
    public AudioClip cookingClip;
    private AudioSource cookingAudioSource;

    private State state = State.Idle;
    private bool isFireOn = false;
    private bool isBurgerInZone = false;
    private GameObject currentBurger;
    private float timer = 0f;

    private void Start()
    {
        // 1) Trigger collider
        if (stoveZoneCollider != null)
            stoveZoneCollider.isTrigger = true;

        // 2) AudioSource guaranteed by RequireComponent
        cookingAudioSource = GetComponent<AudioSource>();
        Debug.Assert(cookingAudioSource != null, "[StoveCounter] AudioSource missing!");

        if (cookingClip != null)
        {
            cookingAudioSource.clip = cookingClip;
            cookingAudioSource.loop = true;
            cookingAudioSource.playOnAwake = false;
            // Para pruebas en 2D (asegúrate de oírlo fácil)
            cookingAudioSource.spatialBlend = 0f;
            cookingAudioSource.volume = 1f;
        }
        else
        {
            Debug.LogWarning("[StoveCounter] cookingClip no asignado en el Inspector.");
        }

        Debug.Log("[StoveCounter] Start complete: Idle state, AudioSource ready.");
    }

    private void Update()
    {
        // --- AUDIO LOOPING ---
        if (cookingAudioSource != null && cookingClip != null)
        {
            bool shouldPlay = isFireOn && isBurgerInZone;
            if (shouldPlay && !cookingAudioSource.isPlaying)
                cookingAudioSource.Play();
            else if (!shouldPlay && cookingAudioSource.isPlaying)
                cookingAudioSource.Stop();
        }

        // --- COOKING LOGIC ---
        Debug.Log($"[Update] fire={isFireOn}, inZone={isBurgerInZone}, hasBurger={currentBurger != null}, state={state}, timer={timer}");
        if (!isFireOn || !isBurgerInZone || currentBurger == null)
            return;

        timer += Time.deltaTime;
        float duration = (state == State.Cooking) ? cookingTime : burnTime;
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = Mathf.Clamp01(timer / duration)
        });

        if (state == State.Cooking && timer >= cookingTime)
        {
            Debug.Log("[StoveCounter] Cooking complete");
            SwapBurger(cookedPrefab, State.Cooked);
        }
        else if (state == State.Cooked && timer >= burnTime)
        {
            Debug.Log("[StoveCounter] Burn complete");
            SwapBurger(burnedPrefab, State.Burned);
        }
    }

    public void ToggleFireOn()
    {
        Debug.Log("[StoveCounter] ToggleFireOn called");
        isFireOn = true;
        if (state == State.Idle && isBurgerInZone)
        {
            state = State.Cooking;
            timer = 0f;
            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
        }
    }

    public void ToggleFireOff()
    {
        Debug.Log("[StoveCounter] ToggleFireOff called");
        isFireOn = false;
        if (cookingAudioSource.isPlaying)
            cookingAudioSource.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Burger")) return;
        Debug.Log($"[StoveCounter] OnTriggerEnter: {other.name}");
        isBurgerInZone = true;
        currentBurger = other.gameObject;
        timer = 0f;

        if (isFireOn)
        {
            // Decide fase según prefab
            if (currentBurger.name.Contains(burnedPrefab.name)) state = State.Burned;
            else if (currentBurger.name.Contains(cookedPrefab.name)) state = State.Cooked;
            else state = State.Cooking;
            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
        }
        else
        {
            state = State.Idle;
            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject != currentBurger || !other.CompareTag("Burger")) return;
        Debug.Log("[StoveCounter] OnTriggerExit");
        isBurgerInZone = false;
        state = State.Idle;
        timer = 0f;
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
        currentBurger = null;
    }

    private void SwapBurger(GameObject newPrefab, State newState)
    {
        if (currentBurger == null) return;
        Vector3 pos = currentBurger.transform.position;
        Quaternion rot = currentBurger.transform.rotation;
        Destroy(currentBurger);
        currentBurger = Instantiate(newPrefab, pos, rot);
        currentBurger.tag = "Burger";
        state = newState;
        timer = 0f;
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
    }

    // Consultas de estado
    public bool IsCooking() => state == State.Cooking;
    public bool IsCooked() => state == State.Cooked;
    public bool IsBurned() => state == State.Burned;
}
