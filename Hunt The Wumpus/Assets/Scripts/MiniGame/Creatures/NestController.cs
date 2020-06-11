using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NestController : MonoBehaviour
{
    public GameObject wumpling;
    
    public float timeBetweenSpawns = 25;
    private float _nextSpawnTime = 0;
    public int wumpsPerSpawn = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        spawnWumplings();
    }

    void spawnWumplings()
    {
        if (Time.time >= _nextSpawnTime)
        {
            for (int i = 0; i < wumpsPerSpawn; i++)
            {
                {
                    SpawnWumpling();
                }

                _nextSpawnTime += timeBetweenSpawns;
            }
        }
    }
    
    private void SpawnWumpling()
    {
        Vector3 spawnLoc = transform.position;

        GameObject newThing = Instantiate(wumpling, spawnLoc, Quaternion.identity);
        //newThing.transform.parent = transform; //set this object as the parent for the new wump
    }
}
