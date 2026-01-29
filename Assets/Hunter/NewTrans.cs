using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NewTrans : MonoBehaviour
{
    [Header("---Possess---")]
    private float possessRadius = 10f;
    public LayerMask possessLayer;
    [Header("Debug")]
    public ObjectController currentTarget;
    public ObjectController currentControlled;

    [Header("---Event---")]
    public UnityEvent OnPossessed;
    public UnityEvent OnUnPossessed;

    void Update()
    {
        DetectObj();

        if (currentTarget != null && Input.GetKeyDown(KeyCode.E))
        {
            currentControlled = currentTarget;
            currentControlled.OnPossessed();
            Possess(currentControlled);
            OnPossessed?.Invoke();
            Debug.Log("∏Ω…Ì≥…π¶: " + currentControlled.name);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentControlled.OnUnPossessed();
            UnPossess(currentControlled);
            currentControlled = null;
            OnUnPossessed?.Invoke();
        }
    }
    void DetectObj()
    {
        currentTarget = null;

        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, possessRadius, possessLayer))
        {
            currentTarget = hit.collider.GetComponent<ObjectController>();
        }
    }

    private void Possess(ObjectController possessObj)
    {
        transform.parent = possessObj.transform.Find("MaskSlot");
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        FindFirstObjectByType<FakePlayer>().TakeoverControl(possessObj.transform); // TODO: need test
    }

    private void UnPossess(ObjectController possessObj)
    {
        transform.parent = null;
    }
}
