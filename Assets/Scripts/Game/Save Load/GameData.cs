[System.Serializable]
public class GameData
{
    public bool HideExplicitLanguage;
    public string PlayerHero;
    public string[] PlayerDeck;
    public string[] PlayerAugments;
    public int AetherCells;
    public string[,] NPCSAndClips;
    public string[,] LocationsNPCsObjectives;
    public string[] RecruitMages;
    public string[] RecruitRogues;
    public string[] RecruitTechs;
    public string[] RecruitWarriors;

    public GameData (bool hideExplicitLanguage, string playerHero, string[] deckList, string[] augments, int aetherCells, string[,] npcsAndClips, string[,] locationsNPCsObjectives,
        string[] recruitMages, string[] recruitRogues, string[] recruitTechs, string[] recruitWarriors)
    {
        HideExplicitLanguage = hideExplicitLanguage;
        PlayerHero = playerHero;
        PlayerDeck = (string[])deckList.Clone();
        PlayerAugments = (string[])augments.Clone();
        AetherCells = aetherCells;
        NPCSAndClips = (string[,])npcsAndClips.Clone();
        LocationsNPCsObjectives = (string[,])locationsNPCsObjectives.Clone();
        RecruitMages = (string[])recruitMages.Clone();
        RecruitRogues = (string[])recruitRogues.Clone();
        RecruitTechs = (string[])recruitTechs.Clone();
        RecruitWarriors = (string[])recruitWarriors.Clone();
    }
}
