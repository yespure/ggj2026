using UnityEngine;
using Mirror;
using kcp2k; // 默认 KCP 传输
using UnityEngine.UI;
using TMPro; // 如果使用 TMP 输入框

public class NetworkUIController : MonoBehaviour
{
    public TMP_InputField addressInput; // 拖入用于输入 IP 的输入框

    // 绑定到 "Host" 按钮
    public void StartHost()
    {
        if (!NetworkClient.active)
        {
            NetworkManager.singleton.StartHost();
        }
    }

    // 绑定到 "Join" 按钮
    public void JoinGame()
    {
        string input = addressInput.text; // 用户输入的字符串，如 "127.0.0.1:1234"

        if (string.IsNullOrEmpty(input)) input = "localhost:7777";

        if (input.Contains(":"))
        {
            // 按照冒号拆分
            string[] parts = input.Split(':');

            // 设置 IP 部分
            NetworkManager.singleton.networkAddress = parts[0];

            // 设置 端口 部分
            if (ushort.TryParse(parts[1], out ushort port))
            {
                if (Transport.active is KcpTransport kcp)
                {
                    kcp.Port = port;
                }
                // 如果你用的是 Telepathy (TCP)
                // else if (Transport.active is Telepathy.TelepathyTransport tele)
                // {
                //     tele.port = port;
                // }
            }
        }
        else
        {
            // 如果没有冒号，只设置 IP，使用 Transport 面板上的默认端口
            NetworkManager.singleton.networkAddress = input;
        }

        // 启动连接
        NetworkManager.singleton.StartClient();
    }

    // 绑定到 "Stop" 按钮（可选）
    public void StopGame()
    {
        // 如果是 Host，停止 Server 和 Client
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        // 如果只是 Client，则断开连接
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }
    }
}