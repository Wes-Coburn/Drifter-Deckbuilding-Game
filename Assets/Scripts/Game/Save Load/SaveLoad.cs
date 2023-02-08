using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveLoad
{
    private static readonly string path =
        Application.persistentDataPath + "/GameData.data";

    public static void SaveGame(GameData data)
    {
        BinaryFormatter formatter = new();
        FileStream stream = new(path, FileMode.Create);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static GameData LoadGame()
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new();
            FileStream stream = new(path, FileMode.Open);
            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogWarning("SAVE FILE NOT FOUND IN " + path);
            return null;
        }
    }
}

