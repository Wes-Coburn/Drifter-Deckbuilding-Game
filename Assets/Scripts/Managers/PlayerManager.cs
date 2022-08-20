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
    private int energyLeft;
    private bool heroPowerUsed;
    private int heroUltimateProgress;

    private PlayerHeroDisplay HeroDisplay { get => coMan.PlayerHero.GetComponent<PlayerHeroDisplay>(); }
    public PlayerHero PlayerHero { get; set; }
    public List<HeroAugment> HeroAugments { get => heroAugments; }
    public List<HeroItem> HeroItems { get => heroItems; }
    public List<Card> PlayerDeckList { get; private set; }
    public List<Card> CurrentPlayerDeck { get; private set; }
    public int MainDeckCount
    {
        get
        {
            int count = 0;
            foreach (Card c in PlayerDeckList)
            {
                if (c is SkillCard) continue;
                else count++;
            }
            return count;
        }
    }
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
            HeroDisplay.PowerUsedIcon.SetActive(value);
        }
    }
    public int HeroUltimateProgress_Direct
    {
        set
        {
            heroUltimateProgress = value;
            string progressText = heroUltimateProgress + "/" +
                GameManager.HERO_ULTMATE_GOAL + " Powers Used";
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
                else progressText = heroUltimateProgress + "/" +
                        heroUltimateGoal + " Powers Used";
                
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
    public int MaxPlayerHealth => GameManager.PLAYER_STARTING_HEALTH;
    public int StartEnergy
    {
        get
        {
            int bonusEnergy = 0;
            if (GetAugment("Synaptic Stabilizer")) bonusEnergy = 1;
            return bonusEnergy;
        }
    }
    public int DamageTaken_Turn
    {
        get => damageTaken_Turn;
        set
        {
            damageTaken_Turn = value;
            bool isWounded;
            if (damageTaken_Turn >= GameManager.WOUNDED_VALUE) isWounded = true;
            else isWounded = false;
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
            coMan.PlayerHero.GetComponent<HeroDisplay>().HeroEnergy =
                energyLeft + "/" + EnergyPerTurn;
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
    public int EnergyLeft
    {
        get => energyLeft;
        set
        {
            energyLeft = value;
            if (energyLeft > MaxEnergy) energyLeft = MaxEnergy;
            coMan.PlayerHero.GetComponent<HeroDisplay>().HeroEnergy = 
                energyLeft + "/" + EnergyPerTurn;
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
        int augmentIndex =
            heroAugments.FindIndex(x => x.AugmentName == augmentName);
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
        else if (EnergyLeft < PlayerHero.HeroPower.PowerCost)
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
                EnergyLeft -= PlayerHero.HeroPower.PowerCost;
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
        else if (EnergyLeft < GetUltimateCost(out _))
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
            EnergyLeft -= GetUltimateCost(out _);
            HeroDisplay.UltimateUsedIcon.SetActive(true);
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
