[System.Serializable]
public class GameData : SaveData
{
    public string[] UnlockedHeroes;
    public string[] UnlockedPowers;
    //public bool Achievement_BETA_Finish;

    public GameData(string[] unlockedHeroes, string[] unlockedPowers)
    {
        UnlockedHeroes = (string[])unlockedHeroes.Clone();
        UnlockedPowers = (string[])unlockedPowers.Clone();
    }
}
