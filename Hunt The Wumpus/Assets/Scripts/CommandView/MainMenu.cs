using Gui;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.IO;

namespace CommandView
{
    public class MainMenu : MonoBehaviour
    {
        public EndGame EndGame;

        public MusicController musicController;
        private Animator _cameraAnimator;
        private Animator _lettersAnimator;

        public GameObject optionsMenu;
        private GameObject _optionsPanel;

        public GameObject mainMenuCanvas;
        private GameObject _mainMenuPanel;

        public SettingsMenu settings;

        public GameObject ContinueBtn;

        public VideoPlayer introVideo;

        private Planet _planet;
        private MainMenuVars _vars;
        private static readonly int FromMiniGame = Animator.StringToHash("BackFromMiniGame");
        private static readonly int MoveIn = Animator.StringToHash("MoveIn");
        private static readonly int MoveOut = Animator.StringToHash("MoveOut");
        private static readonly int IntroVideoComplete = Animator.StringToHash("IntroVideoComplete");
        private static readonly int IntroTitles = Animator.StringToHash("IntroTitles");
        private static readonly int CamIntroFast = Animator.StringToHash("CamIntroFast");

        void Start()
        {
            _cameraAnimator = GameObject.Find("Main Camera").GetComponent<Animator>();
            _lettersAnimator = GameObject.Find("Letters").GetComponent<Animator>();
            _planet = GameObject.Find("Planet").GetComponent<Planet>();
            _vars = GameObject.Find("Main Camera").GetComponent<MainMenuVars>();
            _optionsPanel = optionsMenu.transform.Find("Panel").gameObject;
            _mainMenuPanel = mainMenuCanvas.transform.Find("MenuPanel").gameObject;

            string path = Application.persistentDataPath + "/DONOTOPENTHIS.NOTHINGIMPORTANTHERE";
            if (File.Exists(path))
                ContinueBtn.SetActive(true);
            if (PlayerPrefs.GetInt("AutoStartNewGame") == 1 ? true : false)
            {
                PlayerPrefs.SetInt("AutoStartNewGame", false ? 1 : 0);
                Resume();
            }

        }

        void Update()
        {
            if (_planet.backFromMiniGame)
                BackFromMiniGame();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (settings.inCredits)
                    settings.Credits();

                else if (_optionsPanel.activeSelf)
                    _optionsPanel.SetActive(false);

                else if (GameObject.Find("BuyTroopsPanel") != null)
                {
                    GameObject store = GameObject.Find("StoreUI");
                    for (int i = 0; i < store.transform.childCount; i++)
                    {
                        var child = store.transform.GetChild(i).gameObject;
                        if (child != null)
                            child.SetActive(false);
                    }
                }

                else if (_mainMenuPanel.activeSelf && _planet.readyToPlay && !_vars.firstLaunch)
                {
                    _planet.readyToPlay = false;
                    Resume();
                }

                else if (!_mainMenuPanel.activeSelf && _planet.readyToPause)
                {
                    _planet.readyToPause = false;
                    Pause();
                }
            }
        }

        public void Continue()
        {
            LoadGame();
        }

        public void NewGameBtn()
        {
            if (_vars.firstLaunch)
                Resume();
            else
            {
                PlayerPrefs.SetInt("AutoStartNewGame", true ? 1 : 0);
                PlayerPrefs.Save();
                _planet.curGameStatus.Equals(GameStatus.RanOutOfResources);
                EndGame.Button();
            }
        }

        public void Resume()
        {
            if (!_planet.GetStartGame())
            {
                _planet.SetStartGame();
                musicController.FadeOut();
            }

            ZoomIn();
            HideMainMenu();

            _vars.isPause = false;
            _vars.firstLaunch = false;
        }

        public void NewGame()
        {
            SceneManager.LoadScene(0);
        }

        public void LoadGame()
        {
            _planet.Loadfunc();
            print("loaded");
            _vars.firstLaunch = false;
            Resume();
        }

        public void Options()
        {
            _optionsPanel.SetActive(!_optionsPanel.activeSelf);
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

        private void BackFromMiniGame()
        {
            _vars.firstLaunch = false;
            _vars.isPause = false;
            AnimBackFromMiniGame();
            HideMainMenu();
            _planet.backFromMiniGame = false;
        }

        private void Pause()
        {
            ZoomOut();
            ShowMainMenu();
            _vars.isPause = true;
            HideStoreTroopSelect();
            if (_vars.firstLaunch == false)
                ContinueBtn.SetActive(true);
                //GameObject.Find("MenuPanel/NewGame/Text (TMP)").GetComponent<TextMeshProUGUI>().text = "Continue";
        }

        private void HideMainMenu()
        {
            print("Hiding main menu");
            GameObject.Find("MainMenuCanvas").transform.GetChild(0).gameObject.SetActive(false);
        }

        private void ShowMainMenu()
        {
            GameObject.Find("MainMenuCanvas").transform.GetChild(0).gameObject.SetActive(true);
        }

        private void HideStoreTroopSelect()
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

        private void AnimBackFromMiniGame()
        {
            _cameraAnimator.SetBool(FromMiniGame, true);
            _lettersAnimator.SetBool(FromMiniGame, true);
            _cameraAnimator.SetBool(MoveIn, false);
            _cameraAnimator.SetBool(MoveOut, false);
            _lettersAnimator.SetBool(MoveOut, false);
            _lettersAnimator.SetBool(MoveIn, false);
        }

        private void ZoomIn()
        {
            _cameraAnimator.SetBool(IntroVideoComplete, false);
            _cameraAnimator.SetBool(CamIntroFast, false);
            _cameraAnimator.SetBool(MoveIn, true);
            _cameraAnimator.SetBool(MoveOut, false);
            _lettersAnimator.SetBool(IntroTitles, false);
            _lettersAnimator.SetBool(MoveOut, true);
            _lettersAnimator.SetBool(MoveIn, false);
        }

        private void ZoomOut()
        {
            _cameraAnimator.SetBool(MoveIn, false);
            _cameraAnimator.SetBool(MoveOut, true);
            _lettersAnimator.SetBool(MoveOut, false);
            _lettersAnimator.SetBool(MoveIn, true);
        }
    }
}