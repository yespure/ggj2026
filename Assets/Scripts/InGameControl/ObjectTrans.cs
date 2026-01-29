using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ObjectTrans : MonoBehaviour
{
    public Camera playerCamera;
    private float possessRadius = 10f;
    public LayerMask possessLayer;
    private ObjectController currentTarget;
    private ObjectController currentControlled;
    private ThirdPersonCamera third;

    [Header("MaskOn")]
    public GameObject mask;
    public Transform maskSlot;
    private FakeFakePlayer fakeplayer;
    // Start is called before the first frame update
    void Start()
    {
        third = Camera.main.GetComponent<ThirdPersonCamera>();
        third.target = mask.transform;
        fakeplayer = mask.GetComponent<FakeFakePlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        DetectObj();

        if (currentTarget != null && Input.GetKeyDown(KeyCode.E))
        {
            if (currentControlled != null)
            {
                currentControlled.OnUnPossessed();
            }
            currentControlled = currentTarget;
            currentControlled.OnPossessed();

            fakeplayer.isControlled = false;

            third.target = currentControlled.transform;
            maskSlot = currentControlled.transform.Find("MaskSlot");

            Debug.Log("∏Ω…Ì≥…π¶: " + currentControlled.name);

            

        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentControlled.OnUnPossessed();
            currentControlled = null;
            third.target = mask.transform;
            fakeplayer.isControlled = true;
        }
        UpdateMask();
    }
    void DetectObj()
    {
        currentTarget = null;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, possessRadius, possessLayer))
        {
            currentTarget = hit.collider.GetComponent<ObjectController>();
        }
    }

    void UpdateMask()
    {
        if (currentControlled == null) return;
        if (mask == null || maskSlot == null) return;

        mask.transform.position = maskSlot.position;
        mask.transform.rotation = maskSlot.rotation;
    }
} 


