using CommandView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{
    private Planet _planet;
    private MainMenu _mainMenu;
    public TextMeshProUGUI endText;
    public TextMeshProUGUI turnsText;
    public TextMeshProUGUI button;

    private bool stop = false;

    // Start is called before the first frame update
    void Start()
    {
        _planet = GameObject.Find("Planet").GetComponent<Planet>();
        _mainMenu = GameObject.Find("Main Camera").GetComponent<MainMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_planet.GameStatus != 0 && stop == false)
            EndTheGame(_planet.GameStatus);
    }

    void EndTheGame(int status)
    {
        stop = true;
        transform.GetChild(0).gameObject.SetActive(true);
        //TextMeshProUGUI  endText = endTextP.GetComponent<TextMeshProUGUI>();
        //TextMeshProUGUI turnsText = turnsTextP.GetComponent<TextMeshProUGUI>();
        stop = true;
        if (status == 1)
            endText.text = "You Have Killed The Wumpus";
        else if (status == 2)
            endText.text = "Out Of Moves";
        else if (status == 3)
        {
            endText.text = "The Troops You Sent Have Been Murdured By The Wumpus";
            button.text = "Continue";
        }
            

        turnsText.text = "Turns: " + _planet.GetComponent<GameMeta>().turnsElapsed;
    }

    public void Button()
    {
        if (_planet.GameStatus == 3)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            _planet.GameStatus = 0;
            stop = false;
        }
        else
        {
            _planet.GameStatus = 0;
            _mainMenu.NewGame();
        }
    }
}
