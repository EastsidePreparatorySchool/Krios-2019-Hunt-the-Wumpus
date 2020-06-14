using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using CommandView;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiniGame
{
    public class MusicController : MonoBehaviour
    {
        public AudioSource startAudioSource;
        public AudioSource loopAudioSource;

        private Planet _planet;

        private AudioClip _startClip;

        private AudioClip _loopClip;
        
        // Start is called before the first frame update
        void Awake()
        {
            _planet = GameObject.Find("Planet").GetComponent<Planet>();
            FaceHandler battleFaceHandler = _planet.faces[_planet.GetFaceInBattle()].GetComponent<FaceHandler>();
            
            // Load audio clips
            String[] names = {"Caves", "Planes", "Desert", "Jungle"};

            int biomeIndex = (int) battleFaceHandler.biomeType;
            if (battleFaceHandler.GetHazardObject().Equals(HazardTypes.Pit))
            {
                biomeIndex = 0;
            }
            _startClip = Resources.Load<AudioClip>("Music/" + names[biomeIndex] + "Start");
            _loopClip = Resources.Load<AudioClip>("Music/" + names[biomeIndex] + "Loop");

            startAudioSource.clip = _startClip;
            loopAudioSource.clip = _loopClip;

            SceneManager.activeSceneChanged += FadeAudio;
            DontDestroyOnLoad(gameObject);
        }

        private void FadeAudio(Scene current, Scene next)
        {
            // print("MiniGame: cur: "+current.name+"; next: "+next.name);
            if (next.name.Equals("CommandView"))
            {
                print("MiniGame fading out");
                StartCoroutine(FadeOut());
            }
            else
            {
                print("MiniGame fading in");
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
            
            print("MiniGame faded out");
            Destroy(gameObject);
        }
        private IEnumerator FadeIn()
        {
            print("Begin wait for audioListener");
            yield return new WaitWhile(() => AudioListener.volume > 0);
            startAudioSource.Play();
            loopAudioSource.PlayDelayed(_startClip.length);
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
            print("MiniGame faded in");
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
