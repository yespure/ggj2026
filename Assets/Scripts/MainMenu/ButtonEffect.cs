using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonEffect : MonoBehaviour, 
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    [Header("���o�Ŵ�")]
    public float hoverScale = 1.1f;
    public float scaleSpeed = 10f;
    private Vector3 originalScale;
    private bool isHovering = false;

    [Header("�h��Ч��")]
    public float floatAmplitude = 5f; // Ư������
    public float floatFrequency = 1f; // Ư���ٶ�

    private Vector3 originalPosition;
    private float floatTimer = 0f;

    [Header("���o��")]
    private AudioSource audioSource;
    public AudioClip clickSound;
    public AudioClip HoverSound;


    private void Awake()
    {
        originalScale = transform.localScale;
        originalPosition = transform.localPosition;
    }
    void Start()
    {
        
    }

    void Update()
    {
        ButtonHover();
        ButtonFloat();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        //����M��
        isHovering = true;
        // audioSource.PlayOneShot(HoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //����˳�
        isHovering = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //����c��
        // audioSource.PlayOneShot(clickSound);
    }
    public void ButtonHover()
    {
        //���o�Ŵ����߉݋
        Vector3 targetScale = isHovering
          ? originalScale * hoverScale
          : originalScale;

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * scaleSpeed
            );
    }
    public void ButtonFloat()
    {
        //  Ʈ��Ч��
        if (isHovering)
        {
            floatTimer += Time.deltaTime * floatFrequency;

            
            float xOffset = Mathf.Sin(floatTimer * 2f) * floatAmplitude;
            float yOffset = Mathf.Cos(floatTimer * 3f) * floatAmplitude;

            transform.localPosition = originalPosition + new Vector3(xOffset, yOffset, 0f);
        }
        else
        {
            floatTimer = 0f;
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * scaleSpeed);
        }
    }
}
