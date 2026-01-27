using UnityEngine;
using Mirror;

public class HostControl : MonoBehaviour
{
    public void StartGame()
    {
        // 只有 Host (服务器) 有权切换场景
        if (NetworkServer.active)
        {
            // 切换到名为 "GameScene" 的场景
            // Mirror 会自动把所有连接的玩家带过去
            NetworkManager.singleton.ServerChangeScene("GameScene");
        }
    }
}