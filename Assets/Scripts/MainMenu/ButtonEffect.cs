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
    [Header("按鈕放大")]
    public float hoverScale = 1.1f;
    public float scaleSpeed = 10f;
    private Vector3 originalScale;
    private bool isHovering = false;

    [Header("飄浮效果")]
    public float floatAmplitude = 5f; // 漂浮幅度
    public float floatFrequency = 1f; // 漂浮速度

    private Vector3 originalPosition;
    private float floatTimer = 0f;

    [Header("按鈕聲音")]
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
        //鼠標進入
        isHovering = true;
        audioSource.PlayOneShot(HoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //鼠標退出
        isHovering = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //鼠標點擊
        audioSource.PlayOneShot(clickSound);
    }
    public void ButtonHover()
    {
        //按鈕放大基本邏輯
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
        //  飘浮效果
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
