[System.Serializable]
public class GameData
{
    public bool HideExplicitLanguage;
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
    public string[] RecruitRogues;
    public string[] RecruitTechs;
    public string[] RecruitWarriors;
    public bool Achievement_BETA_Finish;

    public GameData (bool hideExplicitLanguage, string currentNarrative, string playerHero, string[] deckList,
        string[] augments, string[] items, int aetherCells,
        string[,] npcsAndClips, string[,] locationsNPCsObjectives, string[] visitedLocations, string[] shopItems,
        string[] recruitMages, string[] recruitRogues, string[] recruitTechs, string[] recruitWarriors,
        bool achievement_BETA_Finish)
    {
        HideExplicitLanguage = hideExplicitLanguage;
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
        RecruitRogues = (string[])recruitRogues.Clone();
        RecruitTechs = (string[])recruitTechs.Clone();
        RecruitWarriors = (string[])recruitWarriors.Clone();
        Achievement_BETA_Finish = achievement_BETA_Finish;
    }
}
