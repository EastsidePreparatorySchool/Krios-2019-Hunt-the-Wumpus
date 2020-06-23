﻿using UnityEngine;
using UnityEngine.SceneManagement;

namespace CommandView
{
    public class CameraHandler : MonoBehaviour
    {
        public Planet planetHandler;
        public float beginningDistance = 30.0f;
        public float targetDistance = 17.0f; //will be at this most of the game
        public float zoomSpeed = 5.0f; //how fast it zooms in at the beginning of the game

        public float rotationSpeed = 80.0f; //player controlled
        public float distance = 17.0f;

        public AudioSource ambientMusic;

        // Start is called before the first frame update
        void Start()
        {
            distance = beginningDistance;
            transform.position = new Vector3(0, 0, -distance);
        }

        // Update is called once per frame
        void Update()
        {
            if (!ambientMusic.isPlaying && planetHandler.startGame)
            {
                AudioListener.volume = 1f;
                ambientMusic.Play();
            }

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