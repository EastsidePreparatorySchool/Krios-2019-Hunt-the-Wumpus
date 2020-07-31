using System.Collections.Generic;
using CommandView;
using UnityEngine;

namespace Wumpus
{
    public class Wumpus : MonoBehaviour
    {
        public GameObject planetGameObject;

        private Planet _planet;
        // private GameMeta _ingameStat;

        // init variables
        public FaceHandler location;
        public bool status; //false = sleeping | true = awake

        void Awake()
        {
            _planet = planetGameObject.GetComponent<Planet>();
            // _ingameStat = _planet.GetComponent<GameMeta>();
        }

        // Start is called before the first frame update
        void Start()
        {
            // TODO: fix dependence on playerLocation (will not be using such)
            //pick random face
            // playerLocation = _planet.GetPlayerLocation();
            do
            {
                location = _planet.faceHandlers[Random.Range(0, 30)];
            } while (location.colonized || location.discovered || location.GetHazardObject() != HazardTypes.None);

            //make sure room doesn't conflict
            // if (location == playerLocation) {
            //     location = (location + 7) % 29;
            // }
            // TODO: no conflict with adjacent rooms

            print("Wumpus Location: " + location);
            print("Wumpus Status: " + status);
        }

        // Update is called once per frame
        void Update()
        {
        }

        // movement function (if player location == wumpus location, fight, then run)
        //takes adj faces of current location, th
        public void Move(int repeat)
        {
            //otherwise, we simply pick a random adj face and update the location
            for (int i = 0; i < repeat; i++)
            {
                List<int> adjFaces = _planet.FindAdjacentFaces(location.GetTileNumber());
                List<FaceHandler> potentialFaces = new List<FaceHandler> {location};
                foreach (var faceNum in adjFaces)
                {
                    FaceHandler face = _planet.faceHandlers[faceNum];
                    if (!face.colonized && face.GetHazardObject() == HazardTypes.None)
                    {
                        potentialFaces.Add(face);
                    }
                }

                location = potentialFaces[Random.Range(0, potentialFaces.Count)];
            }

            print("wumpus moved to " + location.GetTileNumber());
        }
    }
}