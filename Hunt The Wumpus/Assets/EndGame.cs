using CommandView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{
    private Planet _planet;
    private GameObject _WLCanvas;
    private MainMenu _mainMenu;

    // Start is called before the first frame update
    void Start()
    {
        _planet = GameObject.Find("Planet").GetComponent<Planet>();
        _WLCanvas = GameObject.Find("WinLoseCanvas");
        _mainMenu = GameObject.Find("Main Camera").GetComponent<MainMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_planet.GameStatus != 0)
            EndTheGame(_planet.GameStatus);
    }

    void EndTheGame(int status)
    {
        _WLCanvas.transform.GetChild(0).gameObject.SetActive(true);
        TextMeshProUGUI endText = _WLCanvas.transform.Find("EndGameText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI turnsText = _WLCanvas.transform.Find("Turns").GetComponent<TextMeshProUGUI>();
        if (status == 1)
            endText.text = "You Have Killed The Wumpus";
        else if (status == 2)
            endText.text = "Out Of Moves";
        else if (status == 3)
        {
            endText.text = "The Troops You Sent Have Been Murdured By The Wumpus";
            _WLCanvas.transform.Find("Button/Text (TMP)").GetComponent<TextMeshProUGUI>().text = "Continue";
        }
            

        turnsText.text = "Turns: " + _planet.GetComponent<GameMeta>().turnsElapsed;
    }

    public void Button()
    {
        if (_planet.GameStatus == 3)
        {
            _WLCanvas.transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            _WLCanvas.transform.GetChild(0).gameObject.SetActive(false);
            _mainMenu.NewGame();
        }
    }
}
