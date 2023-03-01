[System.Serializable]
public class PlayerData : SaveData
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
    public string[] RecruitUnits;
    public string[] ShopActions;
    public int RecruitLoyalty;
    public int ActionShopLoyalty;
    public int ShopLoyalty;

    public int Reputation_Mages;
    public int Reputation_Mutants;
    public int Reputation_Rogues;
    public int Reputation_Techs;
    public int Reputation_Warriors;

    public bool Achievement_BETA_Finish;
    
    public PlayerData(int currentHour, string currentNarrative, string playerHero, string[] deckList,
        string[] augments, string[] items, int aetherCells,
        string[,] npcsAndClips, string[,] locationsNPCsObjectives, string[] visitedLocations,
        string[] shopItems, string[] recruitUnits, string[] shopActions,
        int recruitLoyalty, int actionShopLoyalty, int shopLoyalty,
        int reputationMages, int reputationMutants, int reputationRogues, int reputationTechs, int reputationWarriors)
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
        RecruitUnits = (string[])recruitUnits.Clone();
        ShopActions = (string[])shopActions.Clone();

        RecruitLoyalty = recruitLoyalty;
        ActionShopLoyalty = actionShopLoyalty;
        ShopLoyalty = shopLoyalty;

        Reputation_Mages = reputationMages;
        Reputation_Mutants = reputationMutants;
        Reputation_Rogues = reputationRogues;
        Reputation_Techs = reputationTechs;
        Reputation_Warriors = reputationWarriors;
    }
}
