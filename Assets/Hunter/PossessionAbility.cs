using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class PossessionAbility : MonoBehaviour
{
    [Header("Settings")]
    public float possessRadius = 10f;
    public LayerMask possessLayer;

    [Header("Debug")]
    public ObjectController hoveredTarget;

    [Header("Events")]
    public UnityEvent OnPossessAction;
    public UnityEvent OnUnPossessAction;

    // Refs
    private MaskController playerBrain;

    void Awake()
    {
        playerBrain = GetComponent<MaskController>();
    }

    void Update()
    {
        if (!playerBrain.isLocalPlayer) return;

        DetectObj();
        HandleInput();
    }

    void HandleInput()
    {
        // When not possessing, press E to possess
        if (!playerBrain.IsPossessing && hoveredTarget != null && Input.GetKeyDown(KeyCode.E))
        {
            playerBrain.PossessTarget(hoveredTarget);
            OnPossessAction?.Invoke();
        }
        // When possessing, press Q to unpossess
        else if (playerBrain.IsPossessing && Input.GetKeyDown(KeyCode.Q))
        {
            playerBrain.UnPossessTarget();
            OnUnPossessAction?.Invoke();
        }
    }

    /// <summary>
    /// Ray detect possessable objects in the center of the screen
    /// </summary>
    void DetectObj()
    {
        if (playerBrain.IsPossessing)
        {
            hoveredTarget = null;
            return;
        }

        hoveredTarget = null;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, possessRadius, possessLayer))
        {
            hoveredTarget = hit.collider.GetComponent<ObjectController>();
        }
    }
}