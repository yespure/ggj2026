using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShutiaoEndMove : MonoBehaviour
{
    private float rotateSpeed = 180f;
    private float scaleTime = 0.5f;
    private float t = 0f;
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        if(t < 1f)
        {
            t += Time.deltaTime / scaleTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
        }
    }
}
