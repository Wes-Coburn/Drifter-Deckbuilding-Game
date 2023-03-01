using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveLoad
{
    private static readonly string playerSave_Path = Application.persistentDataPath + "/PlayerData.data";
    private static readonly string gameSave_Path = Application.persistentDataPath + "/GameData.data";

    public enum SaveType
    {
        PlayerSave,
        GameSave,
    }
    private static string GetFilePath(SaveType saveType)
    {
        switch (saveType)
        {
            case SaveType.PlayerSave: return playerSave_Path;
            case SaveType.GameSave: return gameSave_Path;
            default:
                Debug.LogError("INVALID SAVETYPE!");
                return null;
        }
    }
    public static void SaveGame(SaveData data, SaveType saveType)
    {
        BinaryFormatter formatter = new();
        using FileStream stream = new(GetFilePath(saveType), FileMode.Create);
        formatter.Serialize(stream, data);
    }

    public static SaveData LoadGame(SaveType saveType)
    {
        string filePath = GetFilePath(saveType);

        if (File.Exists(filePath))
        {
            BinaryFormatter formatter = new();
            using FileStream stream = new(filePath, FileMode.Open);
            SaveData data;
            if (saveType is SaveType.PlayerSave) data = formatter.Deserialize(stream) as PlayerData;
            else if (saveType is SaveType.GameSave) data = formatter.Deserialize(stream) as GameData;
            else
            {
                Debug.LogError("INVALID SAVEDATA TYPE!");
                return null;
            }
            return data;
        }
        else
        {
            Debug.LogWarning("SAVE FILE NOT FOUND IN " + filePath);
            return null;
        }
    }
}

