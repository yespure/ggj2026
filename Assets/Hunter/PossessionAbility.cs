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
    private ObjectController previousHoveredTarget;

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

        //DetectObj();
        FindNearestTarget();
        HandleInput();
        HandleHighlight();
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

    void HandleHighlight()
    {
        if (hoveredTarget != previousHoveredTarget)
        {
            if (previousHoveredTarget != null)
            {
                var outline = previousHoveredTarget.GetComponent<Outline>();
                if (outline != null) outline.enabled = false;
            }

            if (hoveredTarget != null)
            {
                var outline = hoveredTarget.GetComponent<Outline>();
                if (outline != null) outline.enabled = true;
            }

            previousHoveredTarget = hoveredTarget;
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

    void FindNearestTarget()
    {
        if (playerBrain.IsPossessing)
        {
            hoveredTarget = null;
            return;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, possessRadius, possessLayer);
        
        float nearestDistance = Mathf.Infinity;
        ObjectController nearestTarget = null;

        foreach (Collider hit in hits)
        {
            float distance = Vector3.Distance(transform.position, hit.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = hit.GetComponentInParent<ObjectController>();
            }
        }

        hoveredTarget = nearestTarget;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, possessRadius);
    }
}