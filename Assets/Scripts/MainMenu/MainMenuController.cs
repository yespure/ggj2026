using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Sequence = DG.Tweening.Sequence;
using Cinemachine;
using UnityEngine.Audio;

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
    public AudioMixer audioMixer;
    public string[] volParameter = { "BGMVol", "SFXVol", "MasterVol" };
    public Slider[] volSliders = new Slider[3]; // 0 - BGM, 1 - SFX, 2 - Master
    [Space]
    
    [Header("Join Game Panel")]
    public GameObject joinGamePanel;
    public Button joinInButton;
    public TextMeshProUGUI hostIP;
    private bool joinGamePanelIsOut = false;
    [Space]

    // Parameters
    public float yOffset = 100f;
    private float initialXSpeed;
    private float initialYSpeed;
    private bool switchToMain;
    private bool showExitButton = false;
    private bool hideAllButtons = false;
    



    void Awake()
    {    
        // 记录你在 Inspector 面板中预设的初始速度
        // initialXSpeed = freeLookCam.m_XAxis.m_MaxSpeed;
        // initialYSpeed = freeLookCam.m_YAxis.m_MaxSpeed;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            showExitButton = !showExitButton;
            if (hideAllButtons)
            {   
                OnEscKeyboardDown();
            }
            
        }
    }

    public void OnSettingButtonClicked()
    {
        settingPanelIsOut = !settingPanelIsOut;
        settingPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, settingPanelIsOut ? 0 : 2000), 0.6f).SetEase(Ease.OutBack);
    }   

    public void OnLogginButtonClicked()
    {
        switchToMain = false;
        SwitchMainButtons();

        settingPanelIsOut = false;
        settingPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, settingPanelIsOut ? 0 : 2000), 0.6f).SetEase(Ease.OutBack);


        // Debug.Log("Loggin Button Clicked");
        
    }

    public void OnHostGameButtonClicked()
    {
        // Enter the lobby as host
        OnHideAllButtons();


        Debug.Log("Host Game Button Clicked");
    }

    public void OnJoinGameButtonClicked()
    {
        // Show join game panel
        joinGamePanelIsOut = !joinGamePanelIsOut;
        joinGamePanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, joinGamePanelIsOut ? 0 : 2000), 0.6f).SetEase(Ease.OutBack);
        
    }

    public void OnJoinInButtonClicked()
    {
        // Join the game with the provided IP address
        string ipAddress = hostIP.text;
        Debug.Log("Joining game at IP: " + ipAddress);
    }

    public void OnBackButtonClicked()
    {
        switchToMain = true;
        SwitchMainButtons();

        joinGamePanelIsOut = false;
        joinGamePanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, joinGamePanelIsOut ? 0 : 2000), 0.6f).SetEase(Ease.OutBack);


        // Debug.Log("Exit Lobby Button Clicked");
    }

    public void OnExitMainButtonClicked()
    {
        // Exit the game
        Application.Quit();
    }

    public void SwitchMainButtons()
    {
        if (switchToMain)
        {
            // Hide online buttons, one by one
            Sequence animatedUI_HideOnline = DOTween.Sequence();
            for (int i = 0; i < onlineButtons.Length; i++)
            {
                if (onlineButtons[i] == null) return;

                float targetY = i * yOffset;
                Vector3 targetPos = new(-600, targetY, 0);

                animatedUI_HideOnline.Append(onlineButtons[i].GetComponent<RectTransform>().DOAnchorPos(targetPos, 0.8f)).SetEase(Ease.OutBack);
            }
            animatedUI_HideOnline.Play();

            // Switch UI group to main menu, one by one
            Sequence animatedUI_ShowMain = DOTween.Sequence();
            for (int i = 0; i < mainMenuButtons.Length; i++)
            {
                if (mainMenuButtons[i] == null) return;

                float targetY = i * yOffset;
                Vector3 targetPos = new(100, targetY, 0);

                animatedUI_ShowMain.Append(mainMenuButtons[i].GetComponent<RectTransform>().DOAnchorPos(targetPos, 0.8f)).SetEase(Ease.OutBack);
            }
            animatedUI_ShowMain.Play();
        }
        else
        {
            // Hide main menu buttons, one by one
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

            // Switch UI group to online menu, one by one
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
        }
        
    }

    public void OnHideAllButtons()
    {
        // // Hide main menu buttons, one by one
        // Sequence animatedUI_HideMain = DOTween.Sequence();
        // for (int i = 0; i < mainMenuButtons.Length; i++)
        // {
        //     if (mainMenuButtons[i] == null) return;

        //     float targetY = i * yOffset;
        //     Vector3 targetPos = new(-600, targetY, 0);

        //     animatedUI_HideMain.Append(mainMenuButtons[i].GetComponent<RectTransform>().DOAnchorPos(targetPos, 0.8f)).SetEase(Ease.OutBack);
        //     // animatedUI_HideMain.AppendInterval(-0.1f);
        // }
        // animatedUI_HideMain.Play();

        // Hide online buttons, one by one
        Sequence animatedUI_HideOnline = DOTween.Sequence();
        for (int i = 0; i < onlineButtons.Length; i++)
        {
            if (onlineButtons[i] == null) return;

            float targetY = i * yOffset;
            Vector3 targetPos = new(-600, targetY, 0);

            animatedUI_HideOnline.Append(onlineButtons[i].GetComponent<RectTransform>().DOAnchorPos(targetPos, 0.8f)).SetEase(Ease.OutBack);
        }
        animatedUI_HideOnline.Play();

        hideAllButtons = true;
    }

    public void OnEscKeyboardDown()
    {
        mainMenuButtons[2].GetComponent<RectTransform>().DOAnchorPos(new Vector2(showExitButton ? 100 : -600, -300), 0.8f).SetEase(Ease.OutBack);

    }

    public void UpdateBGM(float value)
    {
        float dB = Mathf.Log10(Mathf.Max(0.0001f, value)) * 20;
        
        audioMixer.SetFloat(volParameter[0], dB);
    }

    public void UpdateSFX(float value)
    {
        float dB = Mathf.Log10(Mathf.Max(0.0001f, value)) * 20;
        
        audioMixer.SetFloat(volParameter[1], dB);
    }

    public void UpdateMaster(float value)
    {
        float dB = Mathf.Log10(Mathf.Max(0.0001f, value)) * 20;
        
        audioMixer.SetFloat(volParameter[2], dB);
    }









}
