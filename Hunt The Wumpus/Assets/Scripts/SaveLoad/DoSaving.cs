using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CommandView;
using UnityEngine;

namespace SaveLoad
{
    public static class DoSaving
    {
        public static void DoTheSaving(Planet planet, bool[,] states, int[] biomeNum, bool[] isColonized, int[] hazardType, bool[] showHint, int[] troopType, string[] troopName, bool[] isExausted, bool[] isHeld, int[] heldLoc, int numOfTroops)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/DONOTOPENTHIS.NOTHINGIMPORTANTHERE";
            FileStream stream = new FileStream(path, FileMode.Create);

            SaveData data = new SaveData(planet, states, biomeNum, isColonized, hazardType, showHint, troopType, troopName, isExausted, isHeld, heldLoc, numOfTroops);

            formatter.Serialize(stream, data);
            stream.Close();
        }

        public static SaveData LoadTheData ()
        {
            string path = Application.persistentDataPath + "/DONOTOPENTHIS.NOTHINGIMPORTANTHERE";
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                SaveData data = formatter.Deserialize(stream) as SaveData;
                stream.Close();

                return data;
            }
            else
            {
                Debug.LogError("Savegame not found");
                return null;
            }
        }
    }
}
