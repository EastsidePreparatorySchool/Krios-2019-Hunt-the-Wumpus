using System.Collections;
using System.Collections.Generic;
using CommandView;
using MiniGame.Creatures.DeathHandlers;
using MiniGame.Terrain;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MiniGame
{
    public class SpawnManager : MonoBehaviour
    {
        private Planet _planet;
        private MapGenerator _mapGen;

        private List<TroopMeta> _troops;

        // public int[] waves; //number of wumps for each 'wave' since it would be annoying to have all the wumps at once
        // public float xSpawnRangeMin = 15;
        // public float xSpawnRangeMax = 35;
        // public float zSpawnRangeMin = 15;
        // public float zSpawnRangeMax = 35;
        // public float ySpawnLoc = 1;
        //
        // public bool spawnsWumplings;
        //
        // public float timeBetweenWaves = 100;
        // private float _nextWaveTime;
        //
        // private int _currentWave;
        // public int waveForDisplay;
        //
        // private bool _isDone;

    
        // Start is called before the first frame update
        void Start()
        {
        
            _planet = GameObject.Find("Planet").GetComponent<Planet>();
            _troops = _planet.result.inGameTroops;

            _mapGen = GameObject.Find("Minigame Main Camera").GetComponent<MapGenerator>();
            _mapGen.GenerateMaze();

            // _currentWave = 0;
            // if (spawnsWumplings)
            // {
            //     waves = planet.GetComponent<Planet>().GetWumplingWaveArray();
            //     Debug.Log(waves);
            // }
            // else
            // {
            //     waves = planet.GetComponent<Planet>().GetPlayerWaveArray();
            //     Debug.Log(waves);
            // }

            SpawnEntities();
        }

        // Update is called once per frame
        void Update()
        {
            //if(_planet.result.inGameTroops.)

        }

        private void SpawnEntities()
        {
            for (int i = 0; i < MapGenerator.nodeRows; i++)
            {
                for (int j = 0; j < MapGenerator.nodeCols; j++)
                {
                    Node node = _mapGen.GetNodeMap()[i, j];
                    if (_mapGen.isStartNode(node))
                    {
                        SpawnTroops(node);
                    }
                    else
                    {
                        PopulateWumplingRoom(node);
                    }
                }
            }
        }

        private IEnumerator WaitForNodes()
        {
            while (_mapGen.GetNodeMap() != null)
            {
                yield return true;
            }
        }
        
        private void SpawnCharacter()
        {
            // //I don't want Wumps to spawn in the middle, so I pick 2 random values one on each side and then use another random to choose between them.
            // float xLoc1 = Random.Range(xSpawnRangeMin, xSpawnRangeMax);
            // float xLoc2 = Random.Range(-xSpawnRangeMin, -xSpawnRangeMax);
            //
            // float zLoc1 = Random.Range(zSpawnRangeMin, zSpawnRangeMax);
            // float zLoc2 = Random.Range(-zSpawnRangeMin, -zSpawnRangeMax);
            //
            // float xLocR = (Random.value > 0.5f ? xLoc1 : xLoc2);
            // float zLocR = (Random.value > 0.5f ? zLoc1 : zLoc2);
            //
            //
            // Vector3 spawnLoc = new Vector3(xLocR, ySpawnLoc, zLocR);
            //
            // GameObject newThing = Instantiate(resourceString, spawnLoc, Quaternion.identity);
            // newThing.transform.parent = transform; //set this object as the parent for the new wump
        }

        private void SpawnTroops(Node node)
        {
            foreach (var troop in _troops)
            {
                GameObject soldier = (GameObject) Instantiate(Resources.Load(troop.resourceString), _mapGen.GetRandomPositionInNodeFromNode(node), Quaternion.identity);
                soldier.GetComponent<SoldierDeathHandler>().troopMeta = troop;
            }
        }

        private void PopulateWumplingRoom(Node node)
        {
            
        }
    }
}
