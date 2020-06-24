using CommandView;
using System.Collections;
using System.Collections.Generic;
using Gui;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public MusicController musicController;
    Animator cameraAnimator;
    Animator lettersAnimator;

    public GameObject OptionsMenu;
    GameObject optionsPanel;

    public GameObject MainMenuCanvas;
    GameObject MainMenuPanel;

    Planet planet;
    MainMenuVars vars;

    void Start()
    {
        cameraAnimator = GameObject.Find("Main Camera").GetComponent<Animator>();
        lettersAnimator = GameObject.Find("Letters").GetComponent<Animator>();
        planet = GameObject.Find("Planet").GetComponent<Planet>();
        vars = GameObject.Find("Main Camera").GetComponent<MainMenuVars>();
        optionsPanel = OptionsMenu.transform.Find("Panel").gameObject;
        MainMenuPanel = MainMenuCanvas.transform.Find("MenuPanel").gameObject;
    }

    void Update()
    {
        if (planet.backFromMiniGame)
            backFromMiniGame();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionsPanel.activeSelf)
                optionsPanel.SetActive(false);
            else if (MainMenuPanel.activeSelf && planet.readyToPlay)
            {
                planet.readyToPlay = false;
                Resume();
            }
            else if (!MainMenuPanel.activeSelf && planet.readyToPause)
            {
                planet.readyToPause = false;
                Pause();
            }
        }
    }

    public void Resume()
    {
        if (!planet.GetStartGame())
        {
            planet.SetStartGame();
            musicController.FadeOut();
        }

        ZoomIn();
        HideMainMenu();
        vars.isPause = false;
        vars.firstLaunch = false;
    }

    public void NewGame()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadGame()
    {
        planet.Loadfunc();
        print("loaded");
        Resume();
    }

    public void Options()
    {
        optionsPanel.SetActive(!optionsPanel.activeSelf);
    }

    public void Save()
    {
        GameObject.Find("Planet").GetComponent<Planet>().Savefunc();
        print("saved");
    }

    public void Exit()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

    public void backFromMiniGame()
    {
        vars.firstLaunch = false;
        vars.isPause = false;
        animBackFromMiniGame();
        HideMainMenu();
        planet.backFromMiniGame = false;
    }

    void Pause()
    {
        ZoomOut();
        ShowMainMenu();
        vars.isPause = true;
        HideStoreTroopSelect();
        if (vars.firstLaunch == false)
            GameObject.Find("MenuPanel/NewGame/Text (TMP)").GetComponent<TextMeshProUGUI>().text = "Continue";
    }

    void HideMainMenu()
    {
        print("Hiding main menu");
        GameObject.Find("MainMenuCanvas").transform.GetChild(0).gameObject.SetActive(false);
    }

    void ShowMainMenu()
    {
        GameObject.Find("MainMenuCanvas").transform.GetChild(0).gameObject.SetActive(true);
    }

    void HideStoreTroopSelect()
    {
        GameObject troopSelector = GameObject.Find("TroopSelectorUI");
        if (troopSelector != null)
            GameObject.Find("Canvas").GetComponent<TroopSelection>().ActivateTroopSelector(0, true);

        GameObject store = GameObject.Find("StoreUI");
        for (int i = 0; i < store.transform.childCount; i++)
        {
            var child = store.transform.GetChild(i).gameObject;
            if (child != null)
                child.SetActive(false);
        }
    }

    void animBackFromMiniGame()
    {
        cameraAnimator.SetBool("BackFromMiniGame", true);
        lettersAnimator.SetBool("BackFromMiniGame", true);
        cameraAnimator.SetBool("MoveIn", false);
        cameraAnimator.SetBool("MoveOut", false);
        lettersAnimator.SetBool("MoveOut", false);
        lettersAnimator.SetBool("MoveIn", false);
    }

    void ZoomIn()
    {
        cameraAnimator.SetBool("MoveIn", true);
        cameraAnimator.SetBool("MoveOut", false);
        lettersAnimator.SetBool("MoveOut", true);
        lettersAnimator.SetBool("MoveIn", false);
    }

    void ZoomOut()
    {
        cameraAnimator.SetBool("MoveIn", false);
        cameraAnimator.SetBool("MoveOut", true);
        lettersAnimator.SetBool("MoveOut", false);
        lettersAnimator.SetBool("MoveIn", true);
    }
}