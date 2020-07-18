using CommandView;
using UnityEngine;

namespace MiniGame
{
    public class PauseMenu : MonoBehaviour
    {
        private GameObject _planet;

        private Planet _planetHandler;
        // private GameMeta _gameMeta;

        private static bool _isPaused;

        public GameObject pauseMenu;

        void Start()
        {
            _planet = GameObject.Find("Planet");
            _planetHandler = _planet.GetComponent<Planet>();
            // _gameMeta = _planet.GetComponent<GameMeta>();
        }


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_isPaused)
                    Resume();
                else
                    Pause();
            }
        }

        public void Resume()
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            _isPaused = false;
        }

        void Pause()
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            _isPaused = true;
        }

        public void Surrender()
        {
            _planetHandler.result.InGameTroops.Clear();
            GameObject resultParent = GameObject.Find("Minigame Main Camera");
            ResultHandler rehandle = resultParent.GetComponent<ResultHandler>();
            Resume();
            rehandle.EndMiniGame(false);
        }
    }
}