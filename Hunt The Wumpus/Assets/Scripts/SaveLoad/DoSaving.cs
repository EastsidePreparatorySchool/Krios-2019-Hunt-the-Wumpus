using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CommandView;
using SaveLoad;
using UnityEngine.SceneManagement;

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

    public static SaveData LoadTheData ()
    {
        SceneManager.LoadScene("CommanView", LoadSceneMode.Single);


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
