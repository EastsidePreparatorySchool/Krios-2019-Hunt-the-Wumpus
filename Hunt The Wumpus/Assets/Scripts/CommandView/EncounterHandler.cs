using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;


namespace CommandView
{
    public class EncounterHandler : MonoBehaviour
    {
        public VideoPlayer encounterVid;
        public bool resumeGame = true;
        private Planet _planetHandler;

        // Start is called before the first frame update
        void Start()
        {
            _planetHandler = GameObject.Find("Planet").GetComponent<Planet>();

            if (resumeGame)
            {
                _planetHandler.didSomething = true;
                _planetHandler.wumpus.Move(30);
                _planetHandler.backFromMiniGame = true;
                _planetHandler.SetFaceInBattle(-1);
            }
            else
            {
                _planetHandler.curGameStatus = GameStatus.RanOutOfResources;
            }

            StartCoroutine(SwitchBack());
        }

        private IEnumerator SwitchBack()
        {
            yield return new WaitUntil(() => encounterVid.isPlaying);
            yield return new WaitWhile(() => encounterVid.isPlaying);

            SceneManager.LoadScene(0);
        }
    }
}