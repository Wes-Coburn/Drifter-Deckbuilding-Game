[System.Serializable]
public class GameData
{
    public bool HideExplicitLanguage;
    public string PlayerHero;
    public string[] PlayerDeck;
    public string[] PlayerAugments;
    public string[] PlayerItems;
    public int AetherCells;
    public string[,] NPCSAndClips;
    public string[,] LocationsNPCsObjectives;
    public string[] ShopItems;
    public string[] RecruitMages;
    public string[] RecruitRogues;
    public string[] RecruitTechs;
    public string[] RecruitWarriors;

    public GameData (bool hideExplicitLanguage, string playerHero, string[] deckList, string[] augments, string[] items,
        int aetherCells, string[,] npcsAndClips, string[,] locationsNPCsObjectives, string[] shopItems,
        string[] recruitMages, string[] recruitRogues, string[] recruitTechs, string[] recruitWarriors)
    {
        HideExplicitLanguage = hideExplicitLanguage;
        PlayerHero = playerHero;
        PlayerDeck = (string[])deckList.Clone();
        PlayerAugments = (string[])augments.Clone();
        PlayerItems = (string[])items.Clone();
        AetherCells = aetherCells;
        NPCSAndClips = (string[,])npcsAndClips.Clone();
        LocationsNPCsObjectives = (string[,])locationsNPCsObjectives.Clone();
        ShopItems = (string[])shopItems.Clone();
        RecruitMages = (string[])recruitMages.Clone();
        RecruitRogues = (string[])recruitRogues.Clone();
        RecruitTechs = (string[])recruitTechs.Clone();
        RecruitWarriors = (string[])recruitWarriors.Clone();
    }
}
