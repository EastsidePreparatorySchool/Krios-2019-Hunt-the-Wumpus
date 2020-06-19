using CommandView;
using UnityEngine;

namespace SaveLoad
{
    [System.Serializable]
    public class SaveData
    {
        public GameObject[] faces;

        public int turnsElapsed;
        public int money;
        public int nukes;
        public int sensors;

        public int wumpusLocation;

        public bool[,] state = new bool[4, 30];
        public int[] biomeNum = new int[30];
        public bool[] isColonized = new bool[30];
        public int[] hazardType = new int[30];

        public int[] troopType;
        public string[] troopName;
        public bool[] isExausted;

        public SaveData(Planet planet, bool[,] States, int[] BiomeNum, bool[] IsColonized, int[] HazardType, int[] TroopType, string[] TroopName, bool[] IsExausted, int NumOfTroops)
        {
            turnsElapsed = planet.GetComponent<GameMeta>().turnsElapsed;
            money = planet.GetComponent<GameMeta>().money;
            nukes = planet.GetComponent<GameMeta>().nukes;
            sensors = planet.GetComponent<GameMeta>().sensorTowers;

            wumpusLocation = planet.wumpus.location.GetTileNumber();

            int i = 0;
            for (i = 0; i < 30; i++)
                for (int j = 0; j < 4; j++)
                    state[j, i] = States[j, i];

            i = 0;
            foreach (int BN in BiomeNum)
            {
                biomeNum[i] = BN;
                i++;
            }

            i = 0;
            foreach (bool IC in IsColonized)
            {
                isColonized[i] = IC;
                i++;
            }

            i = 0;
            foreach (int HT in HazardType)
            {
                hazardType[i] = HT;
                i++;
            }

            troopType = new int[NumOfTroops];
            troopName = new string[NumOfTroops];
            isExausted = new bool[NumOfTroops];

            for (i = 0; i < NumOfTroops; i++)
            {
                troopType[i] = TroopType[i];
                troopName[i] = TroopName[i];
                isExausted[i] = IsExausted[i];
            }
        }
    }
}