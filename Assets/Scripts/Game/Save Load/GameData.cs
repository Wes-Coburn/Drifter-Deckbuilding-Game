[System.Serializable]
public class GameData
{
    public int CurrentHour;
    public string CurrentNarrative;
    public string PlayerHero;
    public string[] PlayerDeck;
    public string[] PlayerAugments;
    public string[] PlayerItems;
    public int AetherCells;
    public string[,] NPCSAndClips;
    public string[,] LocationsNPCsObjectives;
    public string[] VisitedLocations;
    public string[] ShopItems;
    public string[] RecruitMages;
    public string[] RecruitMutants;
    public string[] RecruitRogues;
    public string[] RecruitTechs;
    public string[] RecruitWarriors;
    public int RecruitLoyalty;
    public int ShopLoyalty;

    public int Reputation_Mages;
    public int Reputation_Mutants;
    public int Reputation_Rogues;
    public int Reputation_Techs;
    public int Reputation_Warriors;

    public bool Achievement_BETA_Finish;

    public GameData (int currentHour, string currentNarrative, string playerHero, string[] deckList,
        string[] augments, string[] items, int aetherCells,
        string[,] npcsAndClips, string[,] locationsNPCsObjectives, string[] visitedLocations, string[] shopItems,
        string[] recruitMages, string[] recruitMutants, string[] recruitRogues, string[] recruitTechs, string[] recruitWarriors,
        int recruitLoyalty, int shopLoyalty,
        int reputationMages, int reputationMutants, int reputationRogues, int reputationTechs, int reputationWarriors,
        bool achievement_BETA_Finish)
    {
        CurrentHour = currentHour;
        CurrentNarrative = currentNarrative;
        PlayerHero = playerHero;
        PlayerDeck = (string[])deckList.Clone();
        PlayerAugments = (string[])augments.Clone();
        PlayerItems = (string[])items.Clone();
        AetherCells = aetherCells;
        NPCSAndClips = (string[,])npcsAndClips.Clone();
        LocationsNPCsObjectives = (string[,])locationsNPCsObjectives.Clone();
        VisitedLocations = (string[])visitedLocations.Clone();
        ShopItems = (string[])shopItems.Clone();

        RecruitMages = (string[])recruitMages.Clone();
        RecruitMutants = (string[])recruitMutants.Clone();
        RecruitRogues = (string[])recruitRogues.Clone();
        RecruitTechs = (string[])recruitTechs.Clone();
        RecruitWarriors = (string[])recruitWarriors.Clone();

        RecruitLoyalty = recruitLoyalty;
        ShopLoyalty = shopLoyalty;

        Reputation_Mages = reputationMages;
        Reputation_Mutants = reputationMutants;
        Reputation_Rogues = reputationRogues;
        Reputation_Techs = reputationTechs;
        Reputation_Warriors = reputationWarriors;

        Achievement_BETA_Finish = achievement_BETA_Finish;
    }
}
