using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class ObjectTrans : MonoBehaviour
{
    public Camera playerCamera;
    private float possessRadius = 10f;
    public LayerMask possessLayer;
    private ObjectController currentTarget;
    private ObjectController currentControlled;
    private ThirdPersonCamera third;
    // Start is called before the first frame update
    void Start()
    {
        third = Camera.main.GetComponent<ThirdPersonCamera>();
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
            

            third.target = currentControlled.transform;

            Debug.Log("∏Ω…Ì≥…π¶: " + currentControlled.name);

        }
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
} 


