using UnityEngine;
using UnityEngine.SceneManagement;

namespace CommandView
{
    public class MusicController : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            SceneManager.activeSceneChanged += FadeOut;
        }

        private void FadeOut(Scene current, Scene next)
        {
            print("CommandView Music: cur: "+current.name+"; next: "+next.name);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
