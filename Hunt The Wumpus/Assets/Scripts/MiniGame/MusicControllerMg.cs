﻿using System;
using CommandView;
using UnityEngine;

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
            String[] names = {"Caves", "Plains", "Desert", "Jungle"};

            int biomeIndex = (int) battleFaceHandler.biomeType;
            if (battleFaceHandler.GetHazardObject().Equals(HazardTypes.Pit))
            {
                biomeIndex = 0;
            }

            _startClip = Resources.Load<AudioClip>("Music/" + names[biomeIndex] + "Start");
            _loopClip = Resources.Load<AudioClip>("Music/" + names[biomeIndex] + "Loop");

            startAudioSource.clip = _startClip;
            loopAudioSource.clip = _loopClip;

            startAudioSource.Play();
            loopAudioSource.PlayDelayed(_startClip.length);
        }

        // Update is called once per frame
        void Update()
        {
            if (AudioListener.volume < _planet.volume)
            {
                AudioListener.volume = _planet.volume;
            }

            // print(AudioListener.volume+", "+_planet.volume);
        }
    }
}