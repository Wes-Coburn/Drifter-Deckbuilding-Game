[System.Serializable]
public class GameData
{
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
    public bool Achievement_BETA_Finish;

    public GameData (string currentNarrative, string playerHero, string[] deckList,
        string[] augments, string[] items, int aetherCells,
        string[,] npcsAndClips, string[,] locationsNPCsObjectives, string[] visitedLocations, string[] shopItems,
        string[] recruitMages, string[] recruitMutants, string[] recruitRogues, string[] recruitTechs, string[] recruitWarriors,
        int recruitLoyalty, int shopLoyalty,
        bool achievement_BETA_Finish)
    {
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
        Achievement_BETA_Finish = achievement_BETA_Finish;
    }
}
