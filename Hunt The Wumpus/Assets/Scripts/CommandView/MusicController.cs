using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CommandView
{
    public class MusicController : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            SceneManager.activeSceneChanged += FadeAudio;
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
        }
        private IEnumerator FadeIn()
        {
            yield return new WaitWhile(() => AudioListener.volume > 0);
            
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
