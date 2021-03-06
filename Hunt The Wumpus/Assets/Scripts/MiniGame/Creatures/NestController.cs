﻿using UnityEngine;

namespace MiniGame.Creatures
{
    public class NestController : MonoBehaviour
    {
        public GameObject wumpling;
        public ParticleSystem indicatorPs;

        public float timeBetweenSpawns = 25f;
        private float _timeDiffCounter;
        public int wumpsPerSpawn = 1;

        // Start is called before the first frame update
        void Start()
        {
            SpawnWumpling();
        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(Vector3.up, Time.deltaTime * 10);
        }

        private void FixedUpdate()
        {
            if (_timeDiffCounter < timeBetweenSpawns)
            {
                _timeDiffCounter += Time.fixedDeltaTime;
            }
            else
            {
                for (int i = 0; i < wumpsPerSpawn; i++)
                {
                    SpawnWumpling();
                }

                _timeDiffCounter = 0;
            }
        }

        private void SpawnWumpling()
        {
            Vector3 spawnLoc = transform.position;

            Instantiate(wumpling, spawnLoc, Quaternion.identity);
            //newThing.transform.parent = transform; //set this object as the parent for the new wump
        }

        public void ShowIndicatorPs()
        {
            indicatorPs.gameObject.SetActive(true);
        }
    }
}