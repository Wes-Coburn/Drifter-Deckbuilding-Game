using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerManager : HeroManager
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

    private int aetherCells;
    private List<HeroAugment> heroAugments;
    private List<HeroItem> heroItems;

    private bool heroPowerUsed;
    private int heroUltimateProgress;

    public override string HERO_TAG => "PlayerHero";
    public override string CARD_TAG => "PlayerCard";
    public override string HAND_ZONE_TAG => "PlayerHand";
    public override string PLAY_ZONE_TAG => "PlayerZone";
    public override string ACTION_ZONE_TAG => "PlayerActionZone";
    public override string DISCARD_ZONE_TAG => "PlayerDiscard";
    public override string HERO_POWER_TAG => "HeroPower";
    public override string HERO_ULTIMATE_TAG => "HeroUltimate";

    public override Hero HeroScript { get => heroScript; set { heroScript = value; } }
    public List<HeroAugment> HeroAugments { get => heroAugments; }
    public List<HeroItem> HeroItems { get => heroItems; }

    public override bool IsMyTurn
    {
        get => isMyTurn;
        set
        {
            isMyTurn = value;
            if (UIManager.Instance != null) Managers.U_MAN.UpdateEndTurnButton();
        }
    }

    public override int TurnNumber { get => turnNumber; set { turnNumber = value; } }

    public int AetherCells
    {
        get => aetherCells;
        set
        {
            int previousCount = aetherCells;
            aetherCells = value;
            int valueChange = aetherCells - previousCount;

            AnimationManager.CountingTextObject.ClearCountingTexts();
            var acpd = FindObjectOfType<AetherCellPopupDisplay>();
            if (acpd != null)
            {
                new AnimationManager.CountingTextObject(acpd.TotalAetherObject.GetComponent<TextMeshProUGUI>(),
                    valueChange, Color.red, acpd.TotalAether_Additional);

                new AnimationManager.CountingTextObject(acpd.AetherQuantityObject.GetComponent<TextMeshProUGUI>(),
                    valueChange, Color.red, acpd.AetherQuantity_Additional);
            }

            if (UIManager.Instance != null) Managers.U_MAN.SetAetherCount(valueChange);
        }
    }
    public bool HeroPowerUsed
    {
        get => heroPowerUsed;
        set
        {
            heroPowerUsed = value;
            GameObject powerReadyIcon = HeroObject.GetComponent<PlayerHeroDisplay>().PowerReadyIcon;
            powerReadyIcon.SetActive(!heroPowerUsed);
        }
    }
    public int HeroUltimateProgress
    {
        get => heroUltimateProgress;
        set
        {
            heroUltimateProgress = value;
            int heroUltimateGoal = GameManager.HERO_ULTMATE_GOAL;

            var phd = HeroObject.GetComponent<PlayerHeroDisplay>();
            phd.UltimateProgressValue = heroUltimateProgress;
            GameObject ultimateReadyIcon = phd.UltimateReadyIcon;
            GameObject ultimateButton = phd.UltimateButton;

            if (heroUltimateProgress >= heroUltimateGoal) ultimateReadyIcon.SetActive(true);
            else ultimateReadyIcon.SetActive(false);

            if (heroUltimateProgress == heroUltimateGoal)
            {
                Managers.AU_MAN.StartStopSound("SFX_HeroUltimateReady");
                AnimationManager.Instance.AbilityTriggerState(ultimateButton);
            }
        }
    }
    public override int MaxHealth => GameManager.PLAYER_STARTING_HEALTH;

    protected override void Start()
    {
        base.Start();
        heroAugments = new List<HeroAugment>();
        heroItems = new List<HeroItem>();
        HeroScript = null; // Unnecessary?
        AetherCells = 0;
        IsMyTurn = false; // Needs to be false to disable DragDrop outside of combat
    }

    public void AddItem(HeroItem item, bool isNewItem = false)
    {
        if (GetItem(item.ItemName))
        {
            Debug.LogError("ITEM ALREADY EXISTS!");
            return;
        }

        heroItems.Add(item);
        Managers.U_MAN.CreateItemIcon(item, isNewItem);
        if (isNewItem) Managers.AU_MAN.StartStopSound("SFX_BuyItem");
        Managers.U_MAN.UpdateItemsCount();
    }

    private bool GetItem(string itemName)
    {
        int itemIndex = heroItems.FindIndex(x => x.ItemName == itemName);
        return itemIndex != -1;
    }

    public void AddAugment(HeroAugment augment, bool isNewAugment = false)
    {
        if (GetAugment(augment.AugmentName))
        {
            Debug.LogError("AUGMENT ALREADY EXISTS!");
            return;
        }

        heroAugments.Add(augment);
        Managers.U_MAN.CreateAugmentIcon(augment, isNewAugment);
        if (isNewAugment) Managers.AU_MAN.StartStopSound("SFX_AcquireAugment");
    }

    public bool GetAugment(string augmentName)
    {
        int augmentIndex = heroAugments.FindIndex(x => x.AugmentName == augmentName);
        return augmentIndex != -1;
    }

    public bool UseHeroPower(bool isUltimate, bool isPreCheck = false)
    {
        static void ErrorSound() => Managers.AU_MAN.StartStopSound("SFX_Error");

        if (isUltimate) return UseHeroUltimate(isPreCheck);

        if (HeroPowerUsed)
        {
            if (isPreCheck) return false;
            Managers.U_MAN.CreateFleetingInfoPopup("Hero power already used this turn!");
            ErrorSound();
        }
        else if (CurrentEnergy < HeroScript.HeroPower.PowerCost)
        {
            if (isPreCheck) return false;
            Managers.U_MAN.CreateFleetingInfoPopup("Not enough energy!");
            ErrorSound();
        }
        else
        {
            GameObject heroPower = HeroObject.GetComponent<PlayerHeroDisplay>().HeroPower;
            var groupList = HeroScript.HeroPower.EffectGroupList;

            if (!Managers.EF_MAN.CheckLegalTargets(groupList, heroPower, true))
            {
                if (isPreCheck) return false;
                Managers.U_MAN.CreateFleetingInfoPopup("You can't do that right now!");
                ErrorSound();
            }
            else
            {
                if (isPreCheck) return true;
                Managers.EF_MAN.StartEffectGroupList(groupList, heroPower);
                CurrentEnergy -= HeroScript.HeroPower.PowerCost;
                HeroPowerUsed = true;
                PlayerPowerSounds();
                ParticleBurst(heroPower);
            }
        }

        return true;
    }

    private bool UseHeroUltimate(bool isPreCheck)
    {
        static void ErrorSound() => Managers.AU_MAN.StartStopSound("SFX_Error");

        GameObject heroUltimate = HeroObject.GetComponent<PlayerHeroDisplay>().HeroUltimate;
        var groupList = (HeroScript as PlayerHero).HeroUltimate.EffectGroupList;

        if (HeroUltimateProgress < GameManager.HERO_ULTMATE_GOAL)
        {
            if (isPreCheck) return false;
            Managers.U_MAN.CreateFleetingInfoPopup("Ultimate not ready!");
            ErrorSound();
        }
        else if (CurrentEnergy < GetUltimateCost(out _))
        {
            if (isPreCheck) return false;
            Managers.U_MAN.CreateFleetingInfoPopup("Not enough energy!");
            ErrorSound();
        }
        else if (!Managers.EF_MAN.CheckLegalTargets((HeroScript as PlayerHero).HeroUltimate.EffectGroupList, heroUltimate, true))
        {
            if (isPreCheck) return false;
            Managers.U_MAN.CreateFleetingInfoPopup("You can't do that right now!");
            ErrorSound();
        }
        else
        {
            if (isPreCheck) return true;
            Managers.EF_MAN.StartEffectGroupList(groupList, heroUltimate);
            CurrentEnergy -= GetUltimateCost(out _);
            PlayerPowerSounds(true);
            ParticleBurst(heroUltimate);
        }

        return true;
    }

    public int GetMaxItems(out bool hasBonus)
    {
        int bonusItems = 0;
        hasBonus = false;

        if (GetAugment("Kinetic Reinforcer"))
        {
            hasBonus = true;
            bonusItems = 3;
        }
        return GameManager.MAXIMUM_ITEMS + bonusItems;
    }

    public int GetUltimateCost(out Color ultimateColor)
    {
        int cost = (HeroScript as PlayerHero).HeroUltimate.PowerCost;

        if (Managers.G_MAN.GetReputationTier(GameManager.ReputationType.Techs) > 2)
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

    private void ParticleBurst(GameObject parent) =>
        AnimationManager.Instance.CreateParticleSystem(parent, ParticleSystemHandler.ParticlesType.ButtonPress, 1);

    public void PlayerPowerSounds(bool isUltimate = false)
    {
        Sound[] soundList;
        if (isUltimate) soundList = (HeroScript as PlayerHero).HeroUltimate.PowerSounds;
        else soundList = HeroScript.HeroPower.PowerSounds;
        foreach (Sound s in soundList) AudioManager.Instance.StartStopSound(null, s);
    }
}
