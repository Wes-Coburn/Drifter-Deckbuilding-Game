[System.Serializable]
public class GameData : SaveData
{
    public string[] UnlockedHeroes, UnlockedPowers;
    //public bool Achievement_BETAFinish;

    public GameData(string[] unlockedHeroes, string[] unlockedPowers) //, bool achievement_BETA_Finish)
    {
        // Unlocks
        UnlockedHeroes = unlockedHeroes;
        UnlockedPowers = unlockedPowers;

        // Achievements
        //Achievement_BETAFinish = achievement_BETA_Finish;

        // Progress
        // persistent augments
        // persistent starting cards
    }
}
