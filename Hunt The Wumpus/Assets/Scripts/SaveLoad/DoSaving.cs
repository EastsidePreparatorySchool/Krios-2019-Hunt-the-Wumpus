using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CommandView;

public static class DoSaving
{
    public static void DoTheSaving (Planet planet)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/DONOTOPENTHIS.NOTHINGIMPORTANTHERE";
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveData data = new SaveData(planet);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    /*public static SaveData LoadTheData ()
    {
        string path = Application.persistentDataPath + "/DONOTOPENTHIS.NOTHINGIMPORTANTHERE";
    }*/
}
