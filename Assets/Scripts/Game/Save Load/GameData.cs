[System.Serializable]
public class GameData : SaveData
{
    public string[] UnlockedHeroes, UnlockedPowers;
    public string[] HeroAugments;

    public int Reputation_Mages;
    public int Reputation_Mutants;
    public int Reputation_Rogues;
    public int Reputation_Techs;
    public int Reputation_Warriors;

    //public bool Achievement_BETAFinish;

    public bool TutorialActive_WorldMap;

    public GameData(string[] unlockedHeroes, string[] unlockedPowers, string[] heroAugments,
        int repMage, int repMutant, int repRogue, int repTech, int repWarrior,
        bool tutorWorldMap)
    {
        // Unlocks
        UnlockedHeroes = unlockedHeroes;
        UnlockedPowers = unlockedPowers;

        // Progress
        HeroAugments = heroAugments;
        // persistent starting cards ???

        // Reputation
        Reputation_Mages = repMage;
        Reputation_Mutants = repMutant;
        Reputation_Rogues = repRogue;
        Reputation_Techs = repTech;
        Reputation_Warriors = repWarrior;

        // Achievements
        //Achievement_BETAFinish = achievement_BETA_Finish;

        // Tutorials
        TutorialActive_WorldMap = tutorWorldMap;
    }
}
