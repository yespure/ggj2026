using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public bool isCollected = false;
    private Transform carryPoint;
    public ObjectTrans objectTrans;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isCollected)
        {
            carryPoint = objectTrans.currentControlled.transform.Find("CarryPoint");
            transform.SetParent(carryPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
            

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Possessable"))
        {
            isCollected = true;
            
        }
    }
}
