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
        public Animator cameraAnimator;
        public Animator lettersAnimator;


        public float beginningDistance = 30.0f;
        public float targetDistance = 17.0f; //will be at this most of the game
        public float zoomSpeed = 5.0f; //how fast it zooms in at the beginning of the game

        public float rotationSpeed = 80.0f; //player controlled
        public float distance = 17.0f;

        public AudioSource ambientMusic;
        public AudioSource introMusicStart;
        public AudioSource introMusicLoop;
        private static readonly int IntroVideoComplete = Animator.StringToHash("IntroVideoComplete");
        private static readonly int IntroTitles = Animator.StringToHash("IntroTitles");
        private static readonly int CamIntroFast = Animator.StringToHash("CamIntroFast");

        // Start is called before the first frame update
        private void Awake()
        {
            // PlayerPrefs.DeleteKey("needPlayIntroVid");
            // PlayerPrefs.Save();
            if (!PlayerPrefs.HasKey("needPlayIntroVid"))
            {
                otherUi.alpha = 0;
                ambientMusic.Stop();
                introMusicStart.Stop();
                introMusicLoop.Stop();
                
                StartCoroutine(PlayIntroVideo());
            }
            else
            {
                introVideo.targetCameraAlpha = 0f;
                cameraAnimator.SetBool(CamIntroFast, true);
                lettersAnimator.SetBool(IntroTitles, true);
            }
        }

        void Start()
        {
            distance = beginningDistance;
            transform.position = new Vector3(0, 0, -distance);
        }

        private IEnumerator PlayIntroVideo()
        {
            introVideo.Prepare();
            yield return new WaitUntil(() => introVideo.isPrepared);

            // introVideo.time = 34f;
            introVideo.Play();
            introVideo.SetDirectAudioVolume(0, planetHandler.volume);
            introMusicLoop.PlayDelayed((float) introVideo.clip.length);
            yield return new WaitForSeconds(36.11666f);
            // yield return new WaitForSeconds(2.11666f);
            PlayerPrefs.SetInt("needPlayIntroVid", 0);
            PlayerPrefs.Save();
            
            cameraAnimator.SetBool(IntroVideoComplete, true);
            
            introVideo.targetCameraAlpha = 0;

            yield return new WaitForSeconds(0.81667f); // 0.88334f - (4/60)
            lettersAnimator.SetBool(IntroTitles, true);
            
            yield return new WaitForSeconds(2.06666f); // 2f + (4/60)
            float lerpStart = Time.time;
            while (true)
            {
                var progress = Time.time - lerpStart;
                float duration = 0.2f;
                otherUi.alpha = Mathf.Lerp(0, 1, progress / duration);
                if (duration < progress)
                {
                    break;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        // Update is called once per frame
        void Update()
        {
            // print(planetHandler.GetStartGame()+", "+planetHandler.isFadingMusic);
            if (introVideo.isPlaying || !PlayerPrefs.HasKey("needPlayIntroVid"))
            {
                ambientMusic.Stop();
                introMusicStart.Stop();
                introMusicLoop.Stop();
            }
            else if (!ambientMusic.isPlaying && !planetHandler.isFadingMusic &&
                     !menuVars.firstLaunch)
            {
                print("Playing ambient");
                AudioListener.volume = planetHandler.volume;
                ambientMusic.Play();
                introMusicStart.Stop();
                introMusicLoop.Stop();
                // if (introMusicStart.isPlaying && introMusicStart.volume > 0.01f || introMusicLoop.isPlaying && introMusicLoop.volume > 0.01f)
                // {
                //     StartCoroutine(FadeIntroMusic());
                // }
            }
            else if (!introMusicStart.isPlaying && !introMusicLoop.isPlaying && !ambientMusic.isPlaying &&
                     menuVars.firstLaunch)
            {
                AudioListener.volume = planetHandler.volume;
                introMusicStart.Play();
                introMusicLoop.PlayDelayed(introMusicStart.clip.length);
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