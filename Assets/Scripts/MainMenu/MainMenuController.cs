using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenuController : MonoBehaviour
{
    public Button logginButton;
    public Button offlineButton;
    public Button settingButton;
    public Button quitButton;
    
    public GameObject settingPanel;

    // Parameters
    public float buttonSpeed = 0.5f;
    public float titleSpeed = 1f;
    bool panelIsOut = false;


    

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
    //���¸��N���o���܌��F
    public void OnLogginButtonClicked()
    {
        SceneManager.LoadScene("SampleScene");// Lobby? Else?
    }
    public void OnOfflineButtonClicked()
    {
        SceneManager.LoadScene("SampleScene");//�ȴ�����
    }
    public void OnSettingButtonClicked()
    {
        panelIsOut = !panelIsOut;
        settingPanel.transform.DOMove(new Vector3(0, panelIsOut ? 1440 : 0, 0), 1);
    }
    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }


}
