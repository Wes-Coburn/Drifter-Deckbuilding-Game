using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    /* SINGELTON_PATTERN */
    public static PlayerManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    
    private CardManager caMan;
    private CombatManager coMan;
    private EffectManager efMan;
    private UIManager uMan;
    private AudioManager auMan;
    private PlayerHero playerHero;
    private int playerHealth;
    private int playerActionsLeft;
    private List<HeroAugment> heroAugments;
    private int aetherCells;

    public int AetherCells
    {
        get => aetherCells;
        set
        {
            aetherCells = value;
            uMan.SetAetherCount(value); // TESTING
        }
    }
    public List<HeroAugment> HeroAugments { get => heroAugments; }
    public List<Card> PlayerDeckList { get; private set; }
    public List<Card> CurrentPlayerDeck { get; private set; }
    public bool IsMyTurn { get; set; }
    public int ActionsPerTurn { get; set; }
    public bool HeroPowerUsed { get; set; }

    public PlayerHero PlayerHero
    {
        get => playerHero;
        set
        {
            playerHero = value;
            // TESTING
            PlayerDeckList.Clear();
            CurrentPlayerDeck.Clear();
            HeroAugments.Clear();
            AetherCells = 0;
            if (value == null) return;

            foreach (UnitCard uc in caMan.PlayerStartUnits)
                for (int i = 0; i < GameManager.PLAYER_START_FOLLOWERS; i++)
                    caMan.AddCard(uc, GameManager.PLAYER);
            foreach (SkillCard skill in PlayerHero.HeroStartSkills)
                for (int i = 0; i < GameManager.PLAYER_START_SKILLS; i++)
                    caMan.AddCard(skill, GameManager.PLAYER);
        }
    }
    public int PlayerHealth
    {
        get => playerHealth;
        set
        {
            playerHealth = value;
            coMan.PlayerHero.GetComponent<HeroDisplay>().HeroHealth = playerHealth;
        }
    }
    public int PlayerActionsLeft
    {
        get => playerActionsLeft;
        set
        {
            playerActionsLeft = value;
            if (playerActionsLeft > GameManager.MAXIMUM_ACTIONS) 
                playerActionsLeft = GameManager.MAXIMUM_ACTIONS;
            coMan.PlayerHero.GetComponent<PlayerHeroDisplay>().PlayerActions = 
                playerActionsLeft + "/" + ActionsPerTurn;
        }
    }

    private void Start()
    {
        caMan = CardManager.Instance;
        coMan = CombatManager.Instance;
        efMan = EffectManager.Instance;
        uMan = UIManager.Instance;
        auMan = AudioManager.Instance;
        PlayerDeckList = new List<Card>();
        CurrentPlayerDeck = new List<Card>();
        heroAugments = new List<HeroAugment>();
        HeroPowerUsed = false;
    }

    public void AddAugment(HeroAugment augment)
    {
        heroAugments.Add(augment);
        uMan.CreateAugmentIcon(augment); // TESTING
    }

    public bool GetAugment(string augmentName)
    {
        int augmentIndex = heroAugments.FindIndex(x => x.AugmentName == augmentName);
        if (augmentIndex == -1) return false;
        else return true;
    }

    public void UseHeroPower()
    {
        void ErrorSound() => auMan.StartStopSound("SFX_Error");

        if (PlayerActionsLeft < playerHero.HeroPower.PowerCost)
        {
            uMan.CreateFleetinInfoPopup("Not enough actions!");
            ErrorSound();
        }
        else if (HeroPowerUsed == true)
        {
            uMan.CreateFleetinInfoPopup("Hero power already used this turn!");
            ErrorSound();
            return;
        }
        else
        {
            List<EffectGroup> groupList = PlayerHero.HeroPower.EffectGroupList;
            if (!efMan.CheckLegalTargets(groupList, coMan.PlayerHero, true))
            {
                uMan.CreateFleetinInfoPopup("You can't do that right now!");
                ErrorSound();
            }
            else
            {
                efMan.StartEffectGroupList(groupList, coMan.PlayerHero);
                PlayerActionsLeft -= playerHero.HeroPower.PowerCost;
                HeroPowerUsed = true;
                foreach (Sound s in PlayerHero.HeroPower.PowerSounds)
                    AudioManager.Instance.StartStopSound(null, s);
            }
        }
    }
}
