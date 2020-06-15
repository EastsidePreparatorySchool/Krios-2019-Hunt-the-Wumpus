using UnityEngine;

namespace CommandView
{
    public class MusicControllerCv : MonoBehaviour
    {
        public AudioSource ambientMusic;
        // Start is called before the first frame update
        void Start()
        {
            AudioListener.volume = 1f;
            ambientMusic.Play();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
