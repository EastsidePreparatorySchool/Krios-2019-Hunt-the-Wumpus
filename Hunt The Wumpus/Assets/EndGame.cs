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

    // Start is called before the first frame update
    void Start()
    {
        _planet = GameObject.Find("Planet").GetComponent<Planet>();
        _WLCanvas = GameObject.Find("WinLoseCanvas");
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
        if (status == 0)
            endText.text = "You Have Killed The Wumpus";
        else if (status == 1)
            endText.text = "Out Of Moves";
        else if (status == 2)
        {
            endText.text = "You Have Killed The Wumpus";
        }
    }
}
