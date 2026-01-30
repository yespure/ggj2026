using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

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
        //ï¿½ï¿½ï¿½Â®ï¿½ï¿½ï¿½ï¿½Ê?
        settingPanel.SetActive(false);
        //ï¿½ï¿½ï¿½Â°ï¿½ï¿½oï¿½ï¿½Ê¼
        //ÒÔÏÂ®‹Ãæ³õÊ¼
        settingPanel.SetActive(false);
        //ÒÔÏÂ°´âo³õÊ¼
        onlineButton.onClick.AddListener(() => OnOnlineButtonClicked());
        offlineButton.onClick.AddListener(() => OnOfflineButtonClicked());
        settingButton.onClick.AddListener(() => OnSettingButtonClicked());
        quitButton.onClick.AddListener(() => OnQuitButtonClicked());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            settingPanel.SetActive(false);
        }
    }
    //ï¿½ï¿½ï¿½Â¸ï¿½ï¿½Nï¿½ï¿½ï¿½oï¿½ï¿½ï¿½ÜŒï¿½ï¿½F
    void OnOnlineButtonClicked()
    {
        SceneManager.LoadScene("SampleScene");//ï¿½È´ï¿½ï¿½ï¿½ï¿½ï¿½
    }
    void OnOfflineButtonClicked()
    {
        SceneManager.LoadScene("SampleScene");//ï¿½È´ï¿½ï¿½ï¿½ï¿½ï¿½
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
