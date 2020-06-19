using CommandView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    Animator cameraAnimator;
    Animator lettersAnimator;

    MainMenuVars vars;

    void Start()
    {
        cameraAnimator = GameObject.Find("Main Camera").GetComponent<Animator>();
        lettersAnimator = GameObject.Find("Letters").GetComponent<Animator>();
        vars = GameObject.Find("Main Camera").GetComponent<MainMenuVars>();
    }

    void Update()
    {
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
        GameObject.Find("Planet").GetComponent<Planet>().Loadfunc();
        print("loaded");
        NewGame();
    }

    public void Options()
    {

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
        animBackFromMiniGame();
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
        GameObject.Find("MainMenuCanvas").transform.GetChild(0).gameObject.SetActive(false);
    }
    void ShowMainMenu()
    {
        GameObject.Find("MainMenuCanvas").transform.GetChild(0).gameObject.SetActive(true);
    }
      void HideStoreTroopSelect()
      {
          GameObject TroopSelector = GameObject.Find("TroopSelectorUI");
          if (TroopSelector != null)
              TroopSelector.SetActive(false);
  
          GameObject store = GameObject.Find("StoreUI");
          for (int i = 0; i<store.transform.childCount; i++)
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
