using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using CommandView;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiniGame
{
    public class MusicControllerMg : MonoBehaviour
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

            AudioListener.volume = 1;
            startAudioSource.Play();
            loopAudioSource.PlayDelayed(_startClip.length);
        }
        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
