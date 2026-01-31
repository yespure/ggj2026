using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Sequence = DG.Tweening.Sequence;

public class MainMenuController : MonoBehaviour
{
    [Header("Main Menu")]
    public Button[] mainMenuButtons = new Button[2];    //0-Loggin, 1-Nothing, 2-Nothing, 3-Exit. --Nothing means empty, the button is transparent.
    public Button[] onlineButtons = new Button[2];      //0-Host Game, 1-Join Game, 2-Nothing, 3-Exit.
    public Button settingButton;
    [Space]

    [Header("Setting Panel")]
    public GameObject settingPanel;
    private bool settingPanelIsOut = false;
    [Space]
    
    [Header("Join Game Panel")]
    public GameObject joinGamePanel;
    public Button joinInButton;
    public TextMeshProUGUI hostIP;
    private bool joinGamePanelIsOut = false;
    [Space]

    // Parameters
    public float yOffset = 100f;
    public float titleSpeed = 1f;



    

    void Start()
    {
        //This is Setting Panel

        //This is Buttons in main menu, hide when setting panel is active
        // logginButton.onClick.AddListener(() => OnLogginButtonClicked());
        // offlineButton.onClick.AddListener(() => OnOfflineButtonClicked());
        // settingButton.onClick.AddListener(() => OnSettingButtonClicked());
        // quitButton.onClick.AddListener(() => OnQuitButtonClicked());

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            settingPanel.SetActive(false);
        }
    }

    public void OnSettingButtonClicked()
    {
        settingPanelIsOut = !settingPanelIsOut;
        settingPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, settingPanelIsOut ? 0 : 2000), 0.6f).SetEase(Ease.OutBack);
    }   

    public void OnLogginButtonClicked()
    {
        // Hide main menu buttons
        Sequence animatedUI_HideMain = DOTween.Sequence();
        for (int i = 0; i < mainMenuButtons.Length; i++)
        {
            if (mainMenuButtons[i] == null) return;

            float targetY = i * yOffset;
            Vector3 targetPos = new(-600, targetY, 0);

            animatedUI_HideMain.Append(mainMenuButtons[i].GetComponent<RectTransform>().DOAnchorPos(targetPos, 0.8f)).SetEase(Ease.OutBack);
            // animatedUI_HideMain.AppendInterval(-0.1f);
        }
        animatedUI_HideMain.Play();

        // Switch UI group to online menu
        Sequence animatedUI_showOnline = DOTween.Sequence();
        for (int i = 0; i < onlineButtons.Length; i++)
        {
            if (onlineButtons[i] == null) return;

            float targetY = i * yOffset; // 目标位置的 Y 坐标，根据按钮索引调整间距
            Vector3 targetPos = new(100, targetY, 0); // 目标位置

            // 在每个动画之间加入xx秒的间隔（或者负数来重叠）
            animatedUI_showOnline.Append(onlineButtons[i].GetComponent<RectTransform>().DOAnchorPos(targetPos, 0.8f)).SetEase(Ease.OutBack);
            // animatedUI_showOnline.AppendInterval(-0.1f); // 这样下一个按钮会提前xx秒开始动
        }
        animatedUI_showOnline.Play();


        // Debug.Log("Loggin Button Clicked");
        
    }

    public void OnHostGameButtonClicked()
    {
        // Enter the lobby as host
        SceneManager.LoadScene("HostGameScene");
        Debug.Log("Host Game Button Clicked");
    }

    public void OnJoinInButtonClicked()
    {
        // Show join game panel
        joinGamePanelIsOut = !joinGamePanelIsOut;
        joinGamePanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, joinGamePanelIsOut ? 0 : 2000), 0.6f).SetEase(Ease.OutBack);
        
        // Get the IP address
        string ipAddress = hostIP.text;
    }

    public void OnBackButtonClicked()
    {
        // Hide online buttons
        Sequence animatedUI_HideOnline = DOTween.Sequence();
        for (int i = 0; i < onlineButtons.Length; i++)
        {
            if (onlineButtons[i] == null) return;

            float targetY = i * yOffset;
            Vector3 targetPos = new(-600, targetY, 0);

            animatedUI_HideOnline.Append(onlineButtons[i].GetComponent<RectTransform>().DOAnchorPos(targetPos, 0.8f)).SetEase(Ease.OutBack);
        }
        animatedUI_HideOnline.Play();

        // Switch UI group to main menu
        Sequence animatedUI_ShowMain = DOTween.Sequence();
        for (int i = 0; i < mainMenuButtons.Length; i++)
        {
            if (mainMenuButtons[i] == null) return;

            float targetY = i * yOffset;
            Vector3 targetPos = new(100, targetY, 0);

            animatedUI_ShowMain.Append(mainMenuButtons[i].GetComponent<RectTransform>().DOAnchorPos(targetPos, 0.8f)).SetEase(Ease.OutBack);
        }
        animatedUI_ShowMain.Play();



        // Debug.Log("Exit Lobby Button Clicked");
    }

    public void OnExitMainButtonClicked()
    {
        // Exit the game
        Application.Quit();
    }






}
