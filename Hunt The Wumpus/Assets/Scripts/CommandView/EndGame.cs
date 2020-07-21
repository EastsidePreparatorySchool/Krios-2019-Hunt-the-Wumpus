using CommandView;
using UnityEngine;
using TMPro;

public class EndGame : MonoBehaviour
{
    private GameObject _actualPlanet;
    private Planet _planet;
    private MainMenu _mainMenu;
    public TextMeshProUGUI endText;
    public TextMeshProUGUI turnsText;
    public TextMeshProUGUI button;

    private bool _stop;

    // Start is called before the first frame update
    void Start()
    {
        _actualPlanet = GameObject.Find("Planet");
        _planet = GameObject.Find("Planet").GetComponent<Planet>();
        _mainMenu = GameObject.Find("Main Camera").GetComponent<MainMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_planet.curGameStatus != GameStatus.InPlay && !_stop)
            EndTheGame(_planet.curGameStatus);
    }

    void EndTheGame(GameStatus status)
    {
        _stop = true;
        transform.GetChild(0).gameObject.SetActive(true);
        //TextMeshProUGUI  endText = endTextP.GetComponent<TextMeshProUGUI>();
        //TextMeshProUGUI turnsText = turnsTextP.GetComponent<TextMeshProUGUI>();
        _stop = true;
        switch (status)
        {
            case GameStatus.Win:
                endText.text = "Mission Accomplished\nYou Have Killed The Wumpus Queen";
                break;
            case GameStatus.RanOutOfResources:
                endText.text = "Mission Failed\nOut Of Moves";
                break;
            case GameStatus.LostSentTroopToWumpling:
                endText.text = "The Troops You Sent Have Been Murdered By The Wumpus";
                button.text = "Continue";
                break;
            default:
                print(status);
                break;
        }

        turnsText.text = "Turns: " + _planet.GetComponent<GameMeta>().turnsElapsed;
    }

    public void Button()
    {
        if (_planet.curGameStatus.Equals(GameStatus.LostSentTroopToWumpling))
        {
            transform.GetChild(0).gameObject.SetActive(false);
            _planet.curGameStatus = 0;
            _stop = false;
        }
        else
        {
            _planet.curGameStatus = 0;
            Destroy(_actualPlanet);
            _mainMenu.NewGame();
        }
    }
}