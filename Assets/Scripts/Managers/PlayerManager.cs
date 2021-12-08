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

    private int aetherCells;
    private List<HeroAugment> heroAugments;
    private List<HeroItem> heroItems;
    private int playerHealth;
    private int energyPerTurn;
    private int playerEnergyLeft;
    private bool heroPowerUsed;

    public PlayerHero PlayerHero { get; set; }
    public List<HeroAugment> HeroAugments { get => heroAugments; }
    public List<HeroItem> HeroItems { get => heroItems; }
    public List<Card> PlayerDeckList { get; set; }
    public List<Card> CurrentPlayerDeck { get; private set; }
    public bool IsMyTurn { get; set; }

    public int AetherCells
    {
        get => aetherCells;
        set
        {
            aetherCells = value;
            uMan.SetAetherCount(value);
        }
    }
    public bool HeroPowerUsed
    {
        get => heroPowerUsed;
        set
        {
            heroPowerUsed = value;
            PlayerHeroDisplay phd = coMan.PlayerHero.GetComponent<PlayerHeroDisplay>();
            phd.PowerUsedIcon.SetActive(value);
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

    private int MaximumEnergy
    {
        get
        {
            int maxEnergy = GameManager.MAXIMUM_ENERGY;
            if (GetAugment("Inertial Catalyzer")) maxEnergy++;
            return maxEnergy;
        }
    }
    public int EnergyPerTurn
    {
        get => energyPerTurn;
        set
        {
            energyPerTurn = value;
            if (energyPerTurn > MaximumEnergy)
                energyPerTurn = MaximumEnergy;
        }
    }
    public int PlayerEnergyLeft
    {
        get => playerEnergyLeft;
        set
        {
            playerEnergyLeft = value;
            if (playerEnergyLeft > EnergyPerTurn)
            {
                Debug.LogError("ENERGY LEFT > ENERGY PER TURN!");
                playerEnergyLeft = EnergyPerTurn;
            }

            if (playerEnergyLeft > MaximumEnergy) 
                playerEnergyLeft = MaximumEnergy;
            coMan.PlayerHero.GetComponent<PlayerHeroDisplay>().PlayerActions = 
                playerEnergyLeft + "/" + EnergyPerTurn;
        }
    }

    private void Start()
    {
        caMan = CardManager.Instance;
        coMan = CombatManager.Instance;
        efMan = EffectManager.Instance;
        uMan = UIManager.Instance;
        auMan = AudioManager.Instance;

        PlayerHero = null; // TESTING
        PlayerDeckList = new List<Card>();
        CurrentPlayerDeck = new List<Card>();
        AetherCells = 0;
        //AetherCells = 10; // FOR TESTING ONLY

        heroAugments = new List<HeroAugment>();
        heroItems = new List<HeroItem>();   
    }

    public void AddAugment(HeroAugment augment)
    {
        heroAugments.Add(augment);
        uMan.CreateAugmentIcon(augment);
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

        if (PlayerEnergyLeft < PlayerHero.HeroPower.PowerCost)
        {
            uMan.CreateFleetingInfoPopup("Not enough energy!");
            ErrorSound();
        }
        else if (HeroPowerUsed == true)
        {
            uMan.CreateFleetingInfoPopup("Hero power already used this turn!");
            ErrorSound();
            return;
        }
        else
        {
            List<EffectGroup> groupList = PlayerHero.HeroPower.EffectGroupList;
            if (!efMan.CheckLegalTargets(groupList, coMan.PlayerHero, true))
            {
                uMan.CreateFleetingInfoPopup("You can't do that right now!");
                ErrorSound();
            }
            else
            {
                efMan.StartEffectGroupList(groupList, coMan.PlayerHero);
                PlayerEnergyLeft -= PlayerHero.HeroPower.PowerCost;
                HeroPowerUsed = true;
                foreach (Sound s in PlayerHero.HeroPower.PowerSounds)
                    AudioManager.Instance.StartStopSound(null, s);
                caMan.TriggerPlayedUnits(CardManager.TRIGGER_RESEARCH);
            }
        }
    }
}
