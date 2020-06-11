using CommandView;
using UnityEngine;

namespace Wumpus
{
    public class Wumpus : MonoBehaviour
    {
        public GameObject planetGameObject;
        private Planet _planet;
        private GameMeta _ingameStat;
    
        // init variables
        public int location;
        public bool status; //false = sleeping | true = awake
        private int _wumpTurn = 1;
        
        public int playerLocation;

        void Awake() {
            _planet = planetGameObject.GetComponent<Planet>();
            _ingameStat = _planet.GetComponent<GameMeta>();
        }
        
        // Start is called before the first frame update
        void Start()
        {
            // TODO: fix dependence on playerLocation (will not be using such)
            //pick random face
            // playerLocation = _planet.GetPlayerLocation();
            location = Mathf.RoundToInt(Random.Range(0, 29));

            //make sure room doesn't conflict
            // if (location == playerLocation) {
            //     location = (location + 7) % 29;
            // }
            // TODO: no conflict with adjacent rooms
            
            print("Wumpus Location: "+location);
            print("Wumpus Status: "+status);
        }

        // Update is called once per frame
        void Update()
        {
            //if a turn has passed, then the wumpus moves then update the turn counter
            if(_ingameStat.turnsElapsed > _wumpTurn) {
                Move();
                _wumpTurn++;
            }

        }

        // movement function (if player location == wumpus location, fight, then run)
        //takes adj faces of current location, th
        private void Move() {
            //if the wumpus is asleep it won't move
            if(status == false) return;
            //otherwise, we simply pick a random adj face and update the location
            int[] adjFaces;
            adjFaces = _planet.FindAdjacentFaces(location).ToArray();
            // adjFaces = _planet.FindOpenAdjacentFaces(location).ToArray();
            location = adjFaces[Mathf.RoundToInt(Random.Range(0, 3))];
            print("wumpus moevd to " + location);
        }
    }
}
