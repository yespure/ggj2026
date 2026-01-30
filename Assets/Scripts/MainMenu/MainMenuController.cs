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
        //���®����ʼ
        settingPanel.SetActive(false);
        //���°��o��ʼ
        onlineButton.onClick.AddListener(() => OnOnlineButtonClicked());
        offlineButton.onClick.AddListener(() => OnOfflineButtonClicked());
        settingButton.onClick.AddListener(() => OnSettingButtonClicked());
        quitButton.onClick.AddListener(() => OnQuitButtonClicked());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //���¸��N���o���܌��F
    void OnOnlineButtonClicked()
    {
        SceneManager.LoadScene("SampleScene");//�ȴ�����
    }
    void OnOfflineButtonClicked()
    {
        SceneManager.LoadScene("SampleScene");//�ȴ�����
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
