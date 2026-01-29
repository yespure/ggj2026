using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evacuation : MonoBehaviour
{
    public Collectable collectable;
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Possessable")&& collectable.isCollected)
        {
            Debug.Log("win!");
        }
    }
}
