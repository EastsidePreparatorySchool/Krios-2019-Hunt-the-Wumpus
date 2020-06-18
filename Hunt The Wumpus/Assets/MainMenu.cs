using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommandView;

public class MainMenu : MonoBehaviour
{
    Animator cameraAnimator;
    Animator lettersAnimator;

    MainMenuVars vars;
    Planet planet;

    void Start()
    {
        cameraAnimator = GameObject.Find("Main Camera").GetComponent<Animator>();
        lettersAnimator = GameObject.Find("Letters").GetComponent<Animator>();

        vars = GameObject.Find("Main Camera").GetComponent<MainMenuVars>();
        planet = GameObject.Find("Planet").GetComponent<Planet>();
    }

    void Update()
    {
        if (planet.backFromMiniGame == true)
            backFromMiniGame();

        if (Input.GetKeyDown(KeyCode.Escape))
            Pause();
    }

    public void NewGame()
    {
        ZoomIn();
        HideMainMenu();
        vars.isPause = false;
        vars.firstLaunch = false;
    }

    public void LoadGame()
    {

    }

    public void Options()
    {

    }

    public void Save()
    {

    }

    public void Exit()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

    public void backFromMiniGame()
    {
        HideMainMenu();
        vars.firstLaunch = false;
        vars.isPause = false;
        animBackFromMiniGame();
        planet.backFromMiniGame = false;

    }
    void Pause()
    {
        ZoomOut();
        ShowMainMenu();
        vars.isPause = true;
        if (vars.firstLaunch == false)
        {
            GameObject.Find("MenuPanel/NewGame/Text (TMP)").GetComponent<TextMeshProUGUI>().text = "Continue";
        }
    }

    void HideMainMenu()
    {
        GameObject.Find("MainMenuCanvas").transform.GetChild(0).gameObject.SetActive(false);
    }
    void ShowMainMenu()
    {
        GameObject.Find("MainMenuCanvas").transform.GetChild(0).gameObject.SetActive(true);
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
