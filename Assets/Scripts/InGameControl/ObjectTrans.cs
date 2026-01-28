using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class ObjectTrans : MonoBehaviour
{
    public Camera playerCamera;
    private float possessRadius = 5f;
    public LayerMask possessLayer;
    private PossessableOBJ currentTarget;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DetectObj();

        if (currentTarget != null && Input.GetKeyDown(KeyCode.E))
        {
            currentTarget.OnPossessed();
            Debug.Log("∏Ω…Ì≥…π¶");
        }
    }
    void DetectObj()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, possessRadius, possessLayer))
        {
            currentTarget = hit.collider.GetComponent<PossessableOBJ>();
        }
    }
} 


