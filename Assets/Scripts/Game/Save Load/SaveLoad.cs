using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

public static class SaveLoad
{
    private static readonly string playerSavePath = Path.Combine(Application.persistentDataPath, "PlayerData.data");
    private static readonly string gameSavePath = Path.Combine(Application.persistentDataPath, "GameData.data");

    public enum SaveType
    {
        Player,
        Game
    }

    private static string GetFilePath(SaveType saveType)
    {
        switch (saveType)
        {
            case SaveType.Player:
                return playerSavePath;
            case SaveType.Game:
                return gameSavePath;
            default:
                Debug.LogError("Invalid save type!");
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
            SaveData data = saveType switch
            {
                SaveType.Player => formatter.Deserialize(stream) as PlayerData,
                SaveType.Game => formatter.Deserialize(stream) as GameData,
                _ => null,
            };

            if (data == null)
            {
                Debug.LogError("Failed to deserialize save data!");
            }

            return data;
        }
        else
        {
            Debug.LogWarning($"Save file not found at path: {filePath}");
            return null;
        }
    }

    public static void DeleteGameData() => DeleteSaveData(GetFilePath(SaveType.Game));
    public static void DeletePlayerData() => DeleteSaveData(GetFilePath(SaveType.Player));
    
    private static void DeleteSaveData(string filePath)
    {
        if (File.Exists(filePath)) File.Delete(filePath);
        else Debug.LogWarning($"Save file not found at path: {filePath}");
    }
}
