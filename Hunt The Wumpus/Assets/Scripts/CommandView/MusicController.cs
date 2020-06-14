using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CommandView
{
    public class MusicController : MonoBehaviour
    {
        public AudioSource ambientMusic;
        // Start is called before the first frame update
        void Awake()
        {
            SceneManager.activeSceneChanged += FadeAudio;
            DontDestroyOnLoad(gameObject);
            ambientMusic.Play();
        }
        
        private void FadeAudio(Scene current, Scene next)
        {
            // print("MiniGame: cur: "+current.name+"; next: "+next.name);
            if (next.name.Equals("MVPMiniGame"))
            {
                print("CommandView Fading out");
                StartCoroutine(FadeOut());
            }
            else
            {
                print("CommandView Fading in");
                StartCoroutine(FadeIn());
            }
        }

        private IEnumerator FadeOut()
        {
            float elapsedTime = 0;
            float curVol = AudioListener.volume;
            float dur = 1f;

            while (elapsedTime < dur)
            {
                elapsedTime += Time.deltaTime;
                AudioListener.volume = Mathf.Lerp(curVol, 0, elapsedTime / dur);
                yield return null;
            }
            
            print("CommandView Faded out");
            Destroy(gameObject);
        }
        private IEnumerator FadeIn()
        {
            print("Begin wait for audioListener");
            yield return new WaitWhile(() => AudioListener.volume > 0);
            ambientMusic.Play();
            print("End wait for audioListener");
            
            float elapsedTime = 0;
            float curVol = AudioListener.volume;
            float dur = 1f;

            while (elapsedTime < dur)
            {
                elapsedTime += Time.deltaTime;
                AudioListener.volume = Mathf.Lerp(curVol, 1, elapsedTime / dur);
                yield return null;
            }
            print("CommandView Faded in");
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
