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
    private GameManager gMan;

    private int aetherCells;
    private List<HeroAugment> heroAugments;
    private List<HeroItem> heroItems;

    private bool isMyTurn;
    private int playerHealth;
    private int damageTaken_Turn;
    private int energyPerTurn;
    private int currentEnergy;
    private bool heroPowerUsed;
    private int heroUltimateProgress;

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
            isMyTurn = value;
            uMan.UpdateEndTurnButton();
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
            GameObject powerReadyIcon =
                coMan.PlayerHero.GetComponent<PlayerHeroDisplay>().PowerReadyIcon;
            powerReadyIcon.SetActive(!heroPowerUsed);
        }
    }
    public int HeroUltimateProgress
    {
        get => heroUltimateProgress;
        set
        {
            int heroUltimateGoal = GameManager.HERO_ULTMATE_GOAL;
            if (value > heroUltimateGoal) heroUltimateProgress = heroUltimateGoal;
            else heroUltimateProgress = value;

            PlayerHeroDisplay phd = coMan.PlayerHero.GetComponent<PlayerHeroDisplay>();
            phd.UltimateProgressValue = heroUltimateProgress;
            GameObject ultimateReadyIcon = phd.UltimateReadyIcon;
            GameObject ultimateButton = phd.UltimateButton;

            if (heroUltimateProgress == heroUltimateGoal)
            {
                auMan.StartStopSound("SFX_HeroUltimateReady");
                AnimationManager.Instance.AbilityTriggerState(ultimateButton);
                ultimateReadyIcon.SetActive(true);
            }
            else ultimateReadyIcon.SetActive(false);
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
    public int MaxPlayerHealth => GameManager.PLAYER_STARTING_HEALTH;
    public int DamageTaken_Turn
    {
        get => damageTaken_Turn;
        set
        {
            damageTaken_Turn = value;
            bool isWounded;
            if (damageTaken_Turn >= GameManager.WOUNDED_VALUE) isWounded = true;
            else
            {
                isWounded = false;
                uMan.DestroyTooltipPopup();
            }
            coMan.PlayerHero.GetComponent<HeroDisplay>().IsWounded = isWounded;
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
            coMan.PlayerHero.GetComponent<HeroDisplay>().SetHeroEnergy(CurrentEnergy, energyPerTurn);
        }
    }
    public int MaxEnergyPerTurn => GameManager.MAXIMUM_ENERGY_PER_TURN;
    private int MaxEnergy
    {
        get
        {
            int bonusEnergy = 0;
            if (GetAugment("Inertial Catalyzer")) bonusEnergy = 5;
            return GameManager.MAXIMUM_ENERGY + bonusEnergy;
        }
    }
    public int CurrentEnergy
    {
        get => currentEnergy;
        set
        {
            currentEnergy = value;
            if (currentEnergy > MaxEnergy) currentEnergy = MaxEnergy;
            coMan.PlayerHero.GetComponent<HeroDisplay>().SetHeroEnergy(CurrentEnergy, energyPerTurn);
        }
    }

    private void Start()
    {
        coMan = CombatManager.Instance;
        efMan = EffectManager.Instance;
        uMan = UIManager.Instance;
        auMan = AudioManager.Instance;
        gMan = GameManager.Instance;

        PlayerDeckList = new List<Card>();
        CurrentPlayerDeck = new List<Card>();

        heroAugments = new List<HeroAugment>();
        heroItems = new List<HeroItem>();

        PlayerHero = null; // Unnecessary?
        AetherCells = 0;
        IsMyTurn = true;
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
        uMan.UpdateItemsCount();
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

    public bool UseHeroPower(bool isUltimate, bool isPreCheck = false)
    {
        void ErrorSound() => auMan.StartStopSound("SFX_Error");

        if (isUltimate) return UseHeroUltimate(isPreCheck);

        if (HeroPowerUsed)
        {
            if (isPreCheck) return false;
            uMan.CreateFleetingInfoPopup("Hero power already used this turn!");
            ErrorSound();
        }
        else if (CurrentEnergy < PlayerHero.HeroPower.PowerCost)
        {
            if (isPreCheck) return false;
            uMan.CreateFleetingInfoPopup("Not enough energy!");
            ErrorSound();
        }
        else
        {
            GameObject heroPower = coMan.PlayerHero.GetComponent<PlayerHeroDisplay>().HeroPower;
            List<EffectGroup> groupList = PlayerHero.HeroPower.EffectGroupList;

            if (!efMan.CheckLegalTargets(PlayerHero.HeroPower.EffectGroupList, heroPower, true))
            {
                if (isPreCheck) return false;
                uMan.CreateFleetingInfoPopup("You can't do that right now!");
                ErrorSound();
            }
            else
            {
                if (isPreCheck) return true;
                efMan.StartEffectGroupList(groupList, heroPower);
                CurrentEnergy -= PlayerHero.HeroPower.PowerCost;
                HeroPowerUsed = true;
                PlayerPowerSounds();
                ParticleBurst(heroPower);
            }
        }

        return true;
    }

    private bool UseHeroUltimate(bool isPreCheck)
    {
        void ErrorSound() => auMan.StartStopSound("SFX_Error");

        GameObject heroUltimate = coMan.PlayerHero.GetComponent<PlayerHeroDisplay>().HeroUltimate;
        List<EffectGroup> groupList = PlayerHero.HeroUltimate.EffectGroupList;

        if (HeroUltimateProgress < GameManager.HERO_ULTMATE_GOAL)
        {
            if (isPreCheck) return false;
            uMan.CreateFleetingInfoPopup("Ultimate not ready!");
            ErrorSound();
        }
        else if (CurrentEnergy < GetUltimateCost(out _))
        {
            if (isPreCheck) return false;
            uMan.CreateFleetingInfoPopup("Not enough energy!");
            ErrorSound();
        }
        else if (!efMan.CheckLegalTargets(PlayerHero.HeroUltimate.EffectGroupList, heroUltimate, true))
        {
            if (isPreCheck) return false;
            uMan.CreateFleetingInfoPopup("You can't do that right now!");
            ErrorSound();
        }
        else
        {
            if (isPreCheck) return true;
            efMan.StartEffectGroupList(groupList, heroUltimate);
            CurrentEnergy -= GetUltimateCost(out _);
            PlayerPowerSounds(true);
            ParticleBurst(heroUltimate);
        }

        return true;
    }

    public int GetUltimateCost(out Color ultimateColor)
    {
        int cost = PlayerHero.HeroUltimate.PowerCost;
        if (gMan.GetReputationTier(GameManager.ReputationType.Techs) > 2)
        {
            if (cost > 0)
            {
                cost--;
                ultimateColor = Color.green;
            }
            else ultimateColor = Color.white;
        }
        else ultimateColor = Color.white;
        return cost;
    }

    private void ParticleBurst(GameObject parent)
    {
        AnimationManager.Instance.CreateParticleSystem
            (parent, ParticleSystemHandler.ParticlesType.ButtonPress, 1);
    }

    public void PlayerPowerSounds(bool isUltimate = false)
    {
        Sound[] soundList;
        if (isUltimate) soundList = PlayerHero.HeroUltimate.PowerSounds;
        else soundList = PlayerHero.HeroPower.PowerSounds;
        foreach (Sound s in soundList) AudioManager.Instance.StartStopSound(null, s);
    }
}
