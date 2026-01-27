using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Button onlineButton;
    public Button offlineButton;
    public Button settingButton;
    public Button quitButton;

    public GameObject settingPanel;
    // Start is called before the first frame update
    void Start()
    {
        //以下畫面初始
        settingPanel.SetActive(false);
        //以下按鈕初始
        onlineButton.onClick.AddListener(() => OnOnlineButtonClicked());
        offlineButton.onClick.AddListener(() => OnOfflineButtonClicked());
        settingButton.onClick.AddListener(() => OnSettingButtonClicked());
        quitButton.onClick.AddListener(() => OnQuitButtonClicked());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //以下各種按鈕功能實現
    void OnOnlineButtonClicked()
    {
        SceneManager.LoadScene("SampleScene");//等待更改
    }
    void OnOfflineButtonClicked()
    {
        SceneManager.LoadScene("SampleScene");//等待更改
    }
    void OnSettingButtonClicked()
    {
        settingPanel.SetActive(true);
    }
    void OnQuitButtonClicked()
    {
        Application.Quit();
    }


}
