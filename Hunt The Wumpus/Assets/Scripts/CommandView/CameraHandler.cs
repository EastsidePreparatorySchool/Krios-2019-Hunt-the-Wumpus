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
        public MainMenuVars menuVars;
        public CanvasGroup otherUi;
        public VideoPlayer introVideo;


        public float beginningDistance = 30.0f;
        public float targetDistance = 17.0f; //will be at this most of the game
        public float zoomSpeed = 5.0f; //how fast it zooms in at the beginning of the game

        public float rotationSpeed = 80.0f; //player controlled
        public float distance = 17.0f;

        public AudioSource ambientMusic;
        public AudioSource introMusicStart;
        public AudioSource introMusicLoop;

        // Start is called before the first frame update
        private void Awake()
        {
            // PlayerPrefs.DeleteKey("needPlayIntroVid");
            // PlayerPrefs.Save();
            if (!PlayerPrefs.HasKey("needPlayIntroVid"))
            {
                otherUi.alpha = 0;
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
            introVideo.SetDirectAudioVolume(0, planetHandler.volume);
            introVideo.gameObject.GetComponent<AudioSource>().PlayDelayed((float) introVideo.clip.length); // music loop
            yield return new WaitForSeconds(36.8f);
            PlayerPrefs.SetInt("needPlayIntroVid", 0);
            PlayerPrefs.Save();
            introVideo.targetCameraAlpha = 0;
            otherUi.alpha = 1;
        }

        // Update is called once per frame
        void Update()
        {
            // print(planetHandler.GetStartGame()+", "+planetHandler.isFadingMusic);
            if (!ambientMusic.isPlaying && !planetHandler.isFadingMusic && !introVideo.isPlaying &&
                !menuVars.firstLaunch)
            {
                print("Playing ambient");
                AudioListener.volume = planetHandler.volume;
                ambientMusic.Play();
                if (introMusicStart.isPlaying || introMusicLoop.isPlaying)
                {
                    StartCoroutine(FadeIntroMusic());
                }
            }

            if (!(introMusicStart.isPlaying && introMusicLoop.isPlaying) && !ambientMusic.isPlaying &&
                !introVideo.isPlaying && !introVideo.isPlaying && menuVars.firstLaunch)
            {
                AudioListener.volume = planetHandler.volume;
                introMusicStart.Play();
                introMusicLoop.PlayDelayed(introMusicStart.clip.length);
            }

            if (ambientMusic.isPlaying && introVideo.isPlaying)
            {
                ambientMusic.Stop();
            }

            if (ambientMusic.isPlaying && Math.Abs(AudioListener.volume) < 0.01f && !planetHandler.isFadingMusic)
            {
                AudioListener.volume = planetHandler.volume;
            }

            PushInAnim();
        }

        private void PushInAnim()
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

        private IEnumerator FadeIntroMusic()
        {
            float elapsedTime = 0.5f;
            float curVol = introMusicStart.volume;
            float dur = 1f;

            while (elapsedTime < dur)
            {
                elapsedTime += Time.deltaTime;
                float lerpValue = Mathf.Lerp(curVol, 0, elapsedTime / dur);
                introMusicStart.volume = lerpValue;
                introMusicLoop.volume = lerpValue;
                yield return null;
            }

            introMusicStart.Stop();
            introMusicLoop.Stop();
        }
    }
}