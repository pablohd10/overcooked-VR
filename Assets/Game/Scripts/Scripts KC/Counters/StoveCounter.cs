using System;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : MonoBehaviour, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public enum State
    {
        Idle,
        Cooking,
        Cooked,
        Burned
    }

    [Header("Cooking Settings")]
    public float cookingTime = 5f;
    public float burnTime = 3f;

    [Header("References")]
    public Collider stoveZoneCollider;  // Trigger collider around pan area

    [Header("Prefabs")]
    public GameObject rawPrefab;
    public GameObject cookedPrefab;
    public GameObject burnedPrefab;

    private State state = State.Idle;
    private bool isFireOn = false;
    private bool isBurgerInZone = false;
    private GameObject currentBurger;
    private float timer = 0f;

    private void Start()
    {
        state = State.Idle;
        stoveZoneCollider.isTrigger = true;
        Debug.Log("StoveCounter Start: Initialized with Idle state");
    }

    private void Update()
    {
        Debug.Log($"Update: isFireOn={isFireOn}, isBurgerInZone={isBurgerInZone}, hasBurger={(currentBurger != null)}, state={state}, timer={timer}");
        if (!isFireOn || !isBurgerInZone || currentBurger == null)
            return;

        timer += Time.deltaTime;
        float duration = (state == State.Cooking ? cookingTime : burnTime);

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = Mathf.Clamp01(timer / duration)
        });

        if (state == State.Cooking && timer >= cookingTime)
        {
            Debug.Log("Cooking complete: swapping to cookedPrefab");
            SwapBurgerPrefab(cookedPrefab);
            state = State.Cooked;
            timer = 0f;
            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
        }
        else if (state == State.Cooked && timer >= burnTime)
        {
            Debug.Log("Burn complete: swapping to burnedPrefab");
            SwapBurgerPrefab(burnedPrefab);
            state = State.Burned;
            timer = 0f;
            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
        }
    }

    // Called by Start button
    public void ToggleFireOn()
    {
        Debug.Log("ToggleFireOn called");
        isFireOn = true;
        if (state == State.Idle && isBurgerInZone)
        {
            Debug.Log("Starting cooking phase");
            state = State.Cooking;
            timer = 0f;
            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
        }
    }

    // Called by Stop button
    public void ToggleFireOff()
    {
        Debug.Log("ToggleFireOff called");
        isFireOn = false;
        // Pause progression but keep state and prefab
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Burger")) return;

        Debug.Log($"OnTriggerEnter: {other.name} entered zone");
        isBurgerInZone = true;
        currentBurger = other.gameObject;
        timer = 0f;

        // Determine starting state based on burger prefab
        if (isFireOn)
        {
            // If prefab name contains cooked prefab name, start burn phase
            if (currentBurger.name.Contains(cookedPrefab.name))
            {
                Debug.Log("Cooked burger placed: starting burn phase");
                state = State.Cooked;
            }
            else if (currentBurger.name.Contains(burnedPrefab.name))
            {
                Debug.Log("Burned burger placed: no further cooking");
                state = State.Burned;
                // No timer
                return;
            }
            else
            {
                Debug.Log("Raw burger placed: starting cook phase");
                state = State.Cooking;
            }
            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
        }
        else
        {
            state = State.Idle;
            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject != currentBurger) return;
        if (!other.CompareTag("Burger")) return;

        Debug.Log("OnTriggerExit: Burger left zone");
        isBurgerInZone = false;
        state = State.Idle;
        timer = 0f;
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs { progressNormalized = 0f });
        currentBurger = null;
    }

    private void SwapBurgerPrefab(GameObject newPrefab)
    {
        if (currentBurger == null)
        {
            Debug.LogWarning("SwapBurgerPrefab called but no burger present");
            return;
        }

        Vector3 pos = currentBurger.transform.position;
        Quaternion rot = currentBurger.transform.rotation;
        Debug.Log($"Swapping burger to {newPrefab.name} at position {pos}");
        Destroy(currentBurger);
        currentBurger = Instantiate(newPrefab, pos, rot);
        currentBurger.tag = "Burger";
    }

    // State queries
    public bool IsCooking() => state == State.Cooking;
    public bool IsCooked() => state == State.Cooked;
    public bool IsBurned() => state == State.Burned;
}
