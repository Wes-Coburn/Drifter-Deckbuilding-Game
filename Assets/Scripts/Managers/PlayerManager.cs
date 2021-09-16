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

    private UIManager uMan;
    private AudioManager auMan;
    private PlayerHero playerHero;
    private int playerHealth;
    private int playerActionsLeft;

    public int AetherCells { get; set; }
    public List<HeroAugment> HeroAugments { get; private set; }
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
            if (PlayerDeckList == null || CurrentPlayerDeck == null) return;
            PlayerDeckList.Clear();
            if (value == null)
            {
                CurrentPlayerDeck.Clear();
                return;
            }
            AetherCells = 0; // TESTING
            CardManager cm = CardManager.Instance;
            for (int i = 0; i < GameManager.PLAYER_START_FOLLOWERS; i++)
                foreach (UnitCard uc in cm.PlayerStartUnits)
                    cm.AddCard(uc, GameManager.PLAYER);
            foreach (SkillCard skill in PlayerHero.HeroStartSkills)
                for (int i = 0; i < GameManager.PLAYER_START_SKILLS; i++)
                    cm.AddCard(skill, GameManager.PLAYER);
        }
    }
    public int PlayerHealth
    {
        get => playerHealth;
        set
        {
            playerHealth = value;
            CombatManager.Instance.PlayerHero.GetComponent<HeroDisplay>().HeroHealth = playerHealth;
        }
    }
    public int PlayerActionsLeft
    {
        get => playerActionsLeft;
        set
        {
            playerActionsLeft = value;
            if (playerActionsLeft > GameManager.MAXIMUM_ACTIONS) playerActionsLeft = GameManager.MAXIMUM_ACTIONS;
            CombatManager.Instance.PlayerHero.GetComponent<PlayerHeroDisplay>().PlayerActions = 
                playerActionsLeft + "/" + ActionsPerTurn;
        }
    }

    private void Start()
    {
        uMan = UIManager.Instance;
        auMan = AudioManager.Instance;
        PlayerDeckList = new List<Card>();
        CurrentPlayerDeck = new List<Card>();
        HeroAugments = new List<HeroAugment>();
        HeroPowerUsed = false;
    }

    public void UseHeroPower()
    {
        if (HeroPowerUsed == true)
        {
            uMan.CreateFleetinInfoPopup("Hero power already used this turn!");
            ErrorSound();
            return;
        }
        else if (PlayerActionsLeft < playerHero.HeroPower.PowerCost)
        {
            uMan.CreateFleetinInfoPopup("Not enough actions!");
            ErrorSound();
            return;
        }
        else
        {
            EffectManager em = EffectManager.Instance;
            CombatManager coMan = CombatManager.Instance;
            List<EffectGroup> groupList = PlayerHero.HeroPower.EffectGroupList;

            if (!em.CheckLegalTargets(groupList, coMan.PlayerHero, true))
            {
                uMan.CreateFleetinInfoPopup("You can't do that right now!");
                ErrorSound();
                return;
            }
            em.StartEffectGroupList(groupList, coMan.PlayerHero);

            PlayerActionsLeft -= playerHero.HeroPower.PowerCost;
            HeroPowerUsed = true;

            foreach (Sound s in PlayerHero.HeroPower.PowerSounds)
                AudioManager.Instance.StartStopSound(null, s);
        }

        void ErrorSound() => auMan.StartStopSound("SFX_Error");
    }

    public bool GetAugment(string augment)
    {
        int augmentIndex = HeroAugments.FindIndex(x => x.AugmentName == augment);
        if (augmentIndex == -1) return false;
        else return true;
    }
}
