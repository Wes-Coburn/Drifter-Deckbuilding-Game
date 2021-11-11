using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveLoad
{
    private static readonly string path =
        Application.persistentDataPath + "/GameData.data";

    public static void SaveGame(GameData data)
    {
        Debug.LogWarning("SAVE GAME!");
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static GameData LoadGame()
    {
        if (File.Exists(path))
        {
            Debug.LogWarning("LOAD GAME!");
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("SAVE FILE NOT FOUND IN " + path);
            return null;
        }
    }
}

