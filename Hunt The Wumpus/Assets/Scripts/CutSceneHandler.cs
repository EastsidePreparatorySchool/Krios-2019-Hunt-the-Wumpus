using System.Collections;
using CommandView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CutSceneHandler : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public bool resumeGame = true;
    public bool isLogos;
    private Planet _planetHandler;

    // Start is called before the first frame update
    void Start()
    {
        if (!isLogos)
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
                _planetHandler.curGameStatus = GameStatus.Finished;
            }
        }

        StartCoroutine(SwitchBack());
    }

    private IEnumerator SwitchBack()
    {
        yield return new WaitUntil(() => videoPlayer.isPlaying);
        yield return new WaitWhile(() => videoPlayer.isPlaying);

        SceneManager.LoadScene(0);
    }
}