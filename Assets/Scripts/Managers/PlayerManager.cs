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

    private CombatManager coMan;
    private EffectManager efMan;
    private UIManager uMan;
    private AudioManager auMan;

    private int aetherCells;
    private List<HeroAugment> heroAugments;
    private List<HeroItem> heroItems;

    private bool isMyTurn;
    private int playerHealth;
    private int energyPerTurn;
    private int energyLeft;
    private bool heroPowerUsed;
    private int heroUltimateProgress;

    private PlayerHeroDisplay HeroDisplay { get => coMan.PlayerHero.GetComponent<PlayerHeroDisplay>(); }

    public PlayerHero PlayerHero { get; set; }
    public List<HeroAugment> HeroAugments { get => heroAugments; }
    public List<HeroItem> HeroItems { get => heroItems; }
    public List<Card> PlayerDeckList { get; private set; }
    public List<Card> CurrentPlayerDeck { get; private set; }
    public bool IsMyTurn
    {
        get => isMyTurn;
        set
        {
            if (isMyTurn == value) return; // TESTING
            isMyTurn = value;
            uMan.UpdateEndTurnButton(isMyTurn);
        }
    }

    public int AetherCells
    {
        get => aetherCells;
        set
        {
            int previousCount = aetherCells;
            aetherCells = value;
            uMan.SetAetherCount(value, previousCount);
        }
    }
    public bool HeroPowerUsed
    {
        get => heroPowerUsed;
        set
        {
            heroPowerUsed = value;
            HeroDisplay.PowerUsedIcon.SetActive(value);
        }
    }
    public int HeroUltimateProgress_Direct
    {
        set
        {
            heroUltimateProgress = value;
            string progressText = heroUltimateProgress + "/" + GameManager.HERO_ULTMATE_GOAL + " Powers Used";
            HeroDisplay.UltimateProgressText = progressText;
        }
    }
    public int HeroUltimateProgress
    {
        get => heroUltimateProgress;
        set
        {
            int previousProgress = heroUltimateProgress;
            heroUltimateProgress = value;
            int heroUltimateGoal = GameManager.HERO_ULTMATE_GOAL;
            if (heroUltimateProgress <= heroUltimateGoal)
            {
                bool ultimateReady = false;
                string progressText;
                if (heroUltimateProgress == heroUltimateGoal)
                {
                    ultimateReady = true;
                    progressText = "ULTIMATE READY!";
                }
                else progressText = heroUltimateProgress + "/" + heroUltimateGoal + " Powers Used";
                HeroDisplay.UltimateProgressText = progressText;

                PlayerHeroDisplay phd = coMan.PlayerHero.GetComponent<PlayerHeroDisplay>();
                GameObject progressBar = phd.UltimateProgressBar;
                GameObject progressFill = phd.UltimateProgressFill;
                AnimationManager.Instance.SetProgressBar(AnimationManager.ProgressBarType.Ultimate,
                    previousProgress, heroUltimateProgress, ultimateReady, progressBar, progressFill);
            }
            else heroUltimateProgress = heroUltimateGoal;
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
    public int MaxPlayerHealth
    {
        get
        {
            int bonusHealth = 0;
            if (GetAugment("Kinetic Amplifier")) bonusHealth = 10;
            return GameManager.PLAYER_STARTING_HEALTH + bonusHealth;
        }
    }
    public int StartEnergy
    {
        get
        {
            int bonusEnergy = 0;
            if (GetAugment("Synaptic Stabilizer")) bonusEnergy = 2;
            return bonusEnergy;
        }
    }
    public int EnergyPerTurn
    {
        get => energyPerTurn;
        set
        {
            energyPerTurn = value;
            if (energyPerTurn > MaxEnergyPerTurn)
                energyPerTurn = MaxEnergyPerTurn;
            coMan.PlayerHero.GetComponent<PlayerHeroDisplay>().PlayerActions =
                energyLeft + "/" + EnergyPerTurn;
        }
    }
    public int MaxEnergyPerTurn
    {
        get
        {
            int bonusEnergy = 0;
            if (GetAugment("Inertial Catalyzer")) bonusEnergy = 2;
            return GameManager.MAXIMUM_ENERGY_PER_TURN + bonusEnergy;
        }
    }
    private int MaxEnergy
    {
        get => GameManager.MAXIMUM_ENERGY;
    }
    public int EnergyLeft
    {
        get => energyLeft;
        set
        {
            energyLeft = value;
            if (energyLeft > MaxEnergy) energyLeft = MaxEnergy;
            coMan.PlayerHero.GetComponent<PlayerHeroDisplay>().PlayerActions = 
                energyLeft + "/" + EnergyPerTurn;
            
            if (!efMan.EffectsResolving) coMan.SelectPlayableCards(); // TESTING
        }
    }

    private void Start()
    {
        coMan = CombatManager.Instance;
        efMan = EffectManager.Instance;
        uMan = UIManager.Instance;
        auMan = AudioManager.Instance;

        PlayerHero = null; // Unnecessary?
        PlayerDeckList = new List<Card>();
        CurrentPlayerDeck = new List<Card>();
        AetherCells = 0;
        IsMyTurn = true; // TESTING

        heroAugments = new List<HeroAugment>();
        heroItems = new List<HeroItem>();   
    }

    public void AddItem(HeroItem item, bool isNewItem = false)
    {
        if (GetItem(item.ItemName))
        {
            Debug.LogError("ITEM ALREADY EXISTS!");
            return;
        }
        heroItems.Add(item);
        uMan.CreateItemIcon(item, isNewItem);
        if (isNewItem) auMan.StartStopSound("SFX_BuyItem");
    }

    private bool GetItem(string itemName)
    {
        int itemIndex = heroItems.FindIndex(x => x.ItemName == itemName);
        if (itemIndex == -1) return false;
        else return true;
    }

    public void AddAugment(HeroAugment augment, bool isNewAugment = false)
    {
        if (GetAugment(augment.AugmentName))
        {
            Debug.LogError("AUGMENT ALREADY EXISTS!");
            return;
        }
        heroAugments.Add(augment);
        uMan.CreateAugmentIcon(augment, isNewAugment);
        if (isNewAugment) auMan.StartStopSound("SFX_AcquireAugment");
    }

    public bool GetAugment(string augmentName)
    {
        int augmentIndex = heroAugments.FindIndex(x => x.AugmentName == augmentName);
        if (augmentIndex == -1) return false;
        else return true;
    }

    public void UseHeroPower(bool isUltimate)
    {
        void ErrorSound() => auMan.StartStopSound("SFX_Error");

        if (isUltimate)
        {
            UseHeroUltimate();
            return;
        }

        if (HeroPowerUsed == true)
        {
            uMan.CreateFleetingInfoPopup("Hero power already used this turn!");
            ErrorSound();
        }
        else if (EnergyLeft < PlayerHero.HeroPower.PowerCost)
        {
            uMan.CreateFleetingInfoPopup("Not enough energy!");
            ErrorSound();
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
                efMan.StartEffectGroupList(groupList, coMan.PlayerHero.GetComponent<PlayerHeroDisplay>().HeroPower);
                EnergyLeft -= PlayerHero.HeroPower.PowerCost;
                HeroPowerUsed = true;
                PlayerPowerSounds();
            }
        }
    }

    public void UseHeroUltimate()
    {
        void ErrorSound() => auMan.StartStopSound("SFX_Error");

        List<EffectGroup> groupList = PlayerHero.HeroUltimate.EffectGroupList;
        if (HeroUltimateProgress < GameManager.HERO_ULTMATE_GOAL)
        {
            uMan.CreateFleetingInfoPopup("Ultimate not ready!");
            ErrorSound();
        }
        else if (EnergyLeft < PlayerHero.HeroUltimate.PowerCost)
        {
            uMan.CreateFleetingInfoPopup("Not enough energy!");
            ErrorSound();
        }
        else if (!efMan.CheckLegalTargets(groupList, coMan.PlayerHero, true))
        {
            uMan.CreateFleetingInfoPopup("You can't do that right now!");
            ErrorSound();
        }
        else
        {
            efMan.StartEffectGroupList(groupList, coMan.PlayerHero.GetComponent<PlayerHeroDisplay>().HeroUltimate);
            EnergyLeft -= PlayerHero.HeroUltimate.PowerCost;
            HeroDisplay.UltimateUsedIcon.SetActive(true); // TESTING
            PlayerPowerSounds(true);
        }
    }

    public void PlayerPowerSounds(bool isUltimate = false)
    {
        Sound[] soundList;
        if (isUltimate) soundList = PlayerHero.HeroUltimate.PowerSounds;
        else soundList = PlayerHero.HeroPower.PowerSounds;
        foreach (Sound s in soundList) AudioManager.Instance.StartStopSound(null, s);
    }
}
