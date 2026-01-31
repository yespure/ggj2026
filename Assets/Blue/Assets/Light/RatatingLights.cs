using UnityEngine;

public class RotatingRainbowLights : MonoBehaviour
{
    [Header("旋转设置")]
    public Vector3 rotateAxis = Vector3.up; // 旋转轴
    public float rotateSpeed = 50f;         // 旋转速度

    [Header("颜色设置")]
    public float colorChangeSpeed = 0.5f;   // 颜色变换速度

    private Light[] lights;

    void Start()
    {
        // 获取该物体下所有的灯光组件
        lights = GetComponentsInChildren<Light>();
    }

    void Update()
    {
        // 1. 处理旋转：绕着中心物体旋转
        transform.Rotate(rotateAxis, rotateSpeed * Time.deltaTime);

        // 2. 处理颜色：使用 HSV 变换实现彩虹效果
        for (int i = 0; i < lights.Length; i++)
        {
            // 为每个灯光计算一个略微不同的偏移量，让它们的颜色不同步
            float offset = (float)i / lights.Length;
            float hue = Mathf.Repeat(Time.time * colorChangeSpeed + offset, 1f);

            lights[i].color = Color.HSVToRGB(hue, 1f, 1f);
        }
    }
}