using CommandView;
using UnityEngine;

namespace SaveLoad
{
    [System.Serializable]
    public class SaveData
    {
        public GameObject[] faces;

        public int turnsElapsed;
        public bool didSomething;
        public int money;
        public int nukes;
        public int sensors;

        public int wumpusLocation;

        public bool[,] state = new bool[4, 30];
        public int[] biomeNum = new int[30];
        public bool[] isColonized = new bool[30];
        public int[] hazardType = new int[30];
        public bool[] showHint = new bool[30];

        public int[] troopType;
        public string[] troopName;
        public bool[] isExausted;
        public bool[] isHeld;
        public int[] heldLoc;

        public SaveData(Planet planet, bool[,] states, int[] biomeNum, bool[] isColonized, int[] hazardType, bool[] showHint, int[] troopType, string[] troopName, bool[] isExausted, bool[] isHeld, int[] heldLoc, int numOfTroops)
        {
            turnsElapsed = planet.GetComponent<GameMeta>().turnsElapsed;
            didSomething = planet.GetComponent<Planet>().didSomething;
            money = planet.GetComponent<GameMeta>().money;
            nukes = planet.GetComponent<GameMeta>().nukes;
            sensors = planet.GetComponent<GameMeta>().sensorTowers;

            wumpusLocation = planet.wumpus.location.GetTileNumber();

            int i;
            for (i = 0; i < 30; i++)
                for (int j = 0; j < 4; j++)
                    state[j, i] = states[j, i];

            i = 0;
            foreach (int bn in biomeNum)
            {
                this.biomeNum[i] = bn;
                i++;
            }

            i = 0;
            foreach (bool ic in isColonized)
            {
                this.isColonized[i] = ic;
                i++;
            }

            i = 0;
            foreach (int ht in hazardType)
            {
                this.hazardType[i] = ht;
                i++;
            }

            i = 0;
            foreach (bool sh in showHint)
            {
                this.showHint[i] = sh;
                i++;
            }

            this.troopType = new int[numOfTroops];
            this.troopName = new string[numOfTroops];
            this.isExausted = new bool[numOfTroops];
            this.isHeld = new bool[numOfTroops];
            this.heldLoc = new int[numOfTroops];

            for (i = 0; i < numOfTroops; i++)
            {
                this.troopType[i] = troopType[i];
                this.troopName[i] = troopName[i];
                this.isExausted[i] = isExausted[i];
                this.isHeld[i] = isHeld[i];
                this.heldLoc[i] = heldLoc[i];
            }
        }
    }
}