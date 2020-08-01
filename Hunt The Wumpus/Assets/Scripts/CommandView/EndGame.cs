using SaveLoad;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CommandView
{
    public class EndGame : MonoBehaviour
    {
        private GameObject _planet;
        private Planet _planetHandler;
        public MainMenu mainMenu;
        public TextMeshProUGUI endText;
        public TextMeshProUGUI turnsText;
        public TextMeshProUGUI button;

        private bool _stop;

        // Start is called before the first frame update
        void Start()
        {
            _planet = GameObject.Find("Planet");
            _planetHandler = GameObject.Find("Planet").GetComponent<Planet>();
            mainMenu = GameObject.Find("Main Camera").GetComponent<MainMenu>();
        }

        // Update is called once per frame
        void Update()
        {
            if (_planetHandler.curGameStatus != GameStatus.InPlay && !_stop)
                EndTheGame(_planetHandler.curGameStatus);
        }

        void EndTheGame(GameStatus status)
        {
            _stop = true;
            transform.GetChild(0).gameObject.SetActive(true);
            switch (status)
            {
                case GameStatus.Win:
                    endText.text = "Mission Accomplished\nYou Have Killed The Wumpus Queen";
                    DoSaving.DeleteSave();
                    break;
                case GameStatus.RanOutOfResources:
                    // endText.text = "Mission Failed\nOut Of Moves";
                    DoSaving.DeleteSave();
                    Button();
                    break;
                case GameStatus.LostSentTroopToWumpling:
                    endText.text = "The Troops You Sent Have Been Murdered By The Wumpus";
                    button.text = "Continue";
                    break;
                default:
                    print(status);
                    break;
            }

            turnsText.text = "Turns: " + _planetHandler.GetComponent<GameMeta>().turnsElapsed;
        }

        public void Button()
        {
            if (_planetHandler.curGameStatus.Equals(GameStatus.LostSentTroopToWumpling))
            {
                transform.GetChild(0).gameObject.SetActive(false);
                _planetHandler.curGameStatus = 0;
                _stop = false;
            }
            else
            {
                _planetHandler.curGameStatus = 0;
                Destroy(_planet);
                mainMenu.NewGame();
            }
        }
    }
}