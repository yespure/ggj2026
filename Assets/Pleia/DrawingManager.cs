using UnityEngine;

public class BoardPainter : MonoBehaviour
{
    [Header("设置")]
    public GameObject linePrefab; // 拖入 LineRenderer 的预制体
    public LayerMask boardLayer;  // 设置为画板所在的 Layer
    public float minDistance = 0.01f; // 最小绘制距离（优化点数）

    private LineRenderer currentLine;
    private Transform currentBoard;   // 当前正在画的那个画板
    private int positionCount = 0;
    private Vector3 lastLocalPos;

    void Update()
    {
        // 1. 鼠标按下：开始画线
        if (Input.GetMouseButtonDown(0))
        {
            StartLine();
        }
        // 2. 鼠标按住：持续绘制
        else if (Input.GetMouseButton(0))
        {
            UpdateLine();
        }
        // 3. 鼠标抬起：结束绘制
        else if (Input.GetMouseButtonUp(0))
        {
            currentLine = null;
        }
    }

    void StartLine()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, boardLayer))
        {
            // A. 记录当前画板
            currentBoard = hit.transform;

            // B. 生成线条实例
            GameObject lineObj = Instantiate(linePrefab);

            // --- 关键步骤：设置父子关系 ---
            // 将线条设为画板的子物体，这样画板移动时，线条会自动跟随
            lineObj.transform.SetParent(currentBoard);

            // 重置线条的局部变换（确保它没有奇怪的偏移）
            lineObj.transform.localPosition = Vector3.zero;
            lineObj.transform.localRotation = Quaternion.identity;
            lineObj.transform.localScale = Vector3.one;

            currentLine = lineObj.GetComponent<LineRenderer>();

            // --- 关键设置：必须使用局部坐标 ---
            currentLine.useWorldSpace = false;

            positionCount = 0;
            currentLine.positionCount = 0;

            // 添加起始点
            AddPoint(hit.point, hit.normal);
        }
    }

    void UpdateLine()
    {
        if (currentLine == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // 必须检测是否还在同一个画板上
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, boardLayer))
        {
            if (hit.transform != currentBoard) return; // 如果滑到了另一个物体上，停止绘制

            AddPoint(hit.point, hit.normal);
        }
    }

    void AddPoint(Vector3 worldPoint, Vector3 normal)
    {
        // 1. 防止 Z-Fighting (闪烁)
        // 将点沿法线方向稍微抬起一点点，避免和画板重面
        Vector3 offsetPoint = worldPoint + (normal * 0.005f);

        // 2. --- 核心公式：世界转局部 ---
        // 使用画板(父物体)的 transform 将世界坐标转为局部坐标
        Vector3 localPos = currentBoard.InverseTransformPoint(offsetPoint);

        // 3. 距离检测（优化）
        if (positionCount > 0 && Vector3.Distance(localPos, lastLocalPos) < minDistance)
            return;

        // 4. 赋值给 LineRenderer
        positionCount++;
        currentLine.positionCount = positionCount;
        currentLine.SetPosition(positionCount - 1, localPos);

        lastLocalPos = localPos;
    }
}