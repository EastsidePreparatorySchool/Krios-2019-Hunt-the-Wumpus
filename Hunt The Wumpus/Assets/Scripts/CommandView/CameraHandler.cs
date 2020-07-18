using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

namespace CommandView
{
    public class CameraHandler : MonoBehaviour
    {
        public Planet planetHandler;
        public GameMeta meta;
        public GameStarter gameStarter;
        public CanvasGroup otherUI;
        public VideoPlayer introVideo;
        
        
        public float beginningDistance = 30.0f;
        public float targetDistance = 17.0f; //will be at this most of the game
        public float zoomSpeed = 5.0f; //how fast it zooms in at the beginning of the game

        public float rotationSpeed = 80.0f; //player controlled
        public float distance = 17.0f;

        public AudioSource ambientMusic;

        // Start is called before the first frame update
        private void Awake()
        {
            if (!PlayerPrefs.HasKey("needPlayIntroVid"))
            {
                PlayerPrefs.SetInt("needPlayIntroVid", 1);
                PlayerPrefs.Save();
                otherUI.alpha = 0;
                StartCoroutine(PlayIntroVideo());
            }
        }

        void Start()
        {
            distance = beginningDistance;
            transform.position = new Vector3(0, 0, -distance);
        }
        
        private IEnumerator PlayIntroVideo()
        {
            introVideo.Play();
            introVideo.gameObject.GetComponent<AudioSource>().PlayDelayed((float) introVideo.clip.length); // music loop
            yield return new WaitForSeconds(36.8f);
            introVideo.targetCameraAlpha = 0;
            otherUI.alpha = 1;
        }

        // Update is called once per frame
        void Update()
        {
            // print(planetHandler.GetStartGame()+", "+planetHandler.isFadingMusic);
            if (!ambientMusic.isPlaying && !planetHandler.isFadingMusic)
            {
                print("Playing ambient");
                ambientMusic.Play();
                /*
                if (gameStarter.introMusicStart.isPlaying)
                {
                    gameStarter.introMusicStart.Stop();
                }

                if (gameStarter.introMusicLoop.isPlaying)
                {
                    gameStarter.introMusicLoop.Stop();
                }
                */
                
                AudioListener.volume = planetHandler.volume;
                // print(AudioListener.volume);
            }

            if (ambientMusic.isPlaying && Math.Abs(AudioListener.volume) < 0.01f && !planetHandler.isFadingMusic)
            {
                AudioListener.volume = planetHandler.volume;
            }

            // if (!planetHandler.isFadingMusic)
            // {
            //     // ambientMusic.Play();
            //     AudioListener.volume = planetHandler.volume;
            // }
            // print(ambientMusic.isPlaying+", "+AudioListener.volume);

            PushInAnim();
        }

        void PushInAnim()
        {
            transform.Translate(Vector3.forward * distance);

            //zoom to targetDistance
            if (distance > targetDistance)
            {
                distance -= zoomSpeed * Time.deltaTime;
            }

            if (distance < targetDistance)
            {
                distance = targetDistance; //in case you go too far
            }

            transform.Translate(Vector3.back * distance);
        }
    }
}