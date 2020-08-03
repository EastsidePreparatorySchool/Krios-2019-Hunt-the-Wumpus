using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroVideoHandler : MonoBehaviour
{
    public VideoPlayer introVideo;
    public AudioSource introLoopMusic;

    public bool planetPlayIntro;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Time.timeScale = 0;
        StartCoroutine(WaitThenComplete());
        introLoopMusic.PlayDelayed((float) introVideo.length);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator WaitThenComplete()
    {
        yield return new WaitForSeconds(36.11666f);
        CompleteIntroVid();
    }

    private void CompleteIntroVid(bool forced = false)
    {
        SceneManager.LoadScene(1);
        introVideo.targetCameraAlpha = 0;

        planetPlayIntro = forced;
    }
}