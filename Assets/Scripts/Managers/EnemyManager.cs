using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : HeroManager
{
    /* SINGELTON_PATTERN */
    public static EnemyManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    
    public override string HERO_TAG => "EnemyHero";
    public override string CARD_TAG => "EnemyCard";
    public override string HAND_ZONE_TAG => "EnemyHand";
    public override string PLAY_ZONE_TAG => "EnemyZone";
    public override string ACTION_ZONE_TAG => "EnemyActionZone";
    public override string DISCARD_ZONE_TAG => "EnemyDiscard";
    public override string HERO_POWER_TAG => "EnemyHeroPower";
    public override string HERO_ULTIMATE_TAG => "UNDEFINED";

    public override Hero HeroScript
    {
        get => heroScript;
        set
        {
            heroScript = value;
            if (DeckList == null || CurrentDeck == null) return;
            DeckList.Clear();

            if (heroScript == null)
            {
                CurrentDeck.Clear();
                return;
            }
            if ((heroScript as EnemyHero).Reinforcements[ReinforcementGroup] == null)
            {
                Debug.LogError("REINFORCEMENTS NOT FOUND!");
                return;
            }

            var reinforcements = (heroScript as EnemyHero).Reinforcements[ReinforcementGroup];
            var refoUnits = reinforcements.ReinforcementUnits;
            var refoActions = reinforcements.ReinforcementActions;

            foreach (var unit in refoUnits) Managers.CA_MAN.AddCard(unit, Managers.EN_MAN);
            foreach (var action in refoActions) Managers.CA_MAN.AddCard(action, Managers.EN_MAN);
        }
    }

    public override bool IsMyTurn { get; set; }
    public override int TurnNumber
    {
        get => turnNumber;
        set
        {
            turnNumber = value;
            if (Managers.G_MAN.IsTutorial) return;
            SurgeProgress();
        }
    }

    public void SurgeProgress(bool displayOnly = false)
    {
        var ehd = HeroObject.GetComponent<EnemyHeroDisplay>();
        int surgeDelay = Managers.G_MAN.GetSurgeDelay(Managers.CO_MAN.DifficultyLevel);
        int turnsLeft = surgeDelay - (TurnNumber % surgeDelay);
        int surgeValue = TurnNumber / surgeDelay;

        if (TurnNumber > 0)
        {
            ehd.DisplaySurgeProgress(turnsLeft, surgeValue + 1, surgeDelay);

            if (!displayOnly && TurnNumber % surgeDelay == 0)
            {
                for (int i = 0; i < surgeValue; i++)
                    Managers.EV_MAN.NewDelayedAction(() => Surge(), 0.5f, true);

                Managers.EV_MAN.NewDelayedAction(() => SurgePopup(), 0.5f, true);
            }
        }
        else ehd.DisplaySurgeProgress(surgeDelay, 1, surgeDelay);

        void SurgePopup()
        {
            Managers.U_MAN.CreateFleetingInfoPopup($"ENEMY SURGE!\n[{surgeValue}x]");
            Managers.AN_MAN.CreateParticleSystem(null, ParticleSystemHandler.ParticlesType.Explosion, 2);
        }
        void Surge()
        {
            var surgeCard = Managers.CA_MAN.DrawCard(this);
            if (surgeCard != null)
                surgeCard.GetComponent<CardDisplay>().ChangeCurrentEnergyCost(-surgeValue);
        }
    }

    public int ReinforcementGroup { get; set; }
    public override int MaxHealth
    {
        get
        {
            if (Managers.G_MAN.IsTutorial) return GameManager.TUTORIAL_STARTING_HEALTH;
            else
            {
                int levelToInt = (int)(HeroScript as EnemyHero).EnemyLevel;
                if (levelToInt < 1)
                {
                    Debug.LogError("UNEXPECTED BEHVAIOR!");
                    levelToInt = 1;
                }

                return GameManager.ENEMY_STARTING_HEALTH + (5 * (levelToInt - 1));
            }
        }
    }

    public override void ResetForCombat()
    {
        base.ResetForCombat();
        DeckList.Clear();
        CurrentDeck.Clear();
        ReinforcementGroup = 0; // FOR TESTING ONLY?
    }

    public void Mulligan()
    {
        List<GameObject> cardsToReplace = new();

        foreach (var card in HandZoneCards)
        {
            var cd = card.GetComponent<CardDisplay>();
            if (cd.CurrentEnergyCost > 2) cardsToReplace.Add(card);
        }

        foreach (var card in cardsToReplace) Managers.CA_MAN.DiscardCard(card);
        for (int i = 0; i < cardsToReplace.Count; i++)
            Managers.EV_MAN.NewDelayedAction(() =>
            Managers.CA_MAN.DrawCard(this), 0.5f, true);
    }

    public void StartEnemyTurn()
    {
        TurnNumber++;

        Managers.EV_MAN.NewDelayedAction(() => ReplenishEnergy(), 0);

        Managers.EV_MAN.NewDelayedAction(() =>
        Managers.CA_MAN.TriggerPlayedUnits(CardManager.TRIGGER_TURN_START, this), 0);

        Managers.EV_MAN.NewDelayedAction(() => TurnDraw(), 1);

        Managers.EV_MAN.NewDelayedAction(() => CreateActionSchedule(), 0);
        
        void ReplenishEnergy()
        {
            int startEnergy = CurrentEnergy;
            if (EnergyPerTurn < MaxEnergy) EnergyPerTurn++;
            CurrentEnergy = EnergyPerTurn;
            int energyChange = CurrentEnergy - startEnergy;
            Managers.AN_MAN.ModifyHeroEnergyState(energyChange, HeroObject);
        }
        void TurnDraw()
        {
            if (HandZoneCards.Count >= GameManager.MAX_HAND_SIZE)
            {
                Managers.U_MAN.CreateFleetingInfoPopup("Enemy hand is full!");
                Debug.Log("ENEMY HAND IS FULL!");
                return;
            }
            Managers.CA_MAN.DrawCard(this);
        }
    }

    public void ContinueEnemyTurn() => CreateActionSchedule();

    void CreateActionSchedule()
    {
        if (HasActionsRemaining())
        {
            Managers.EV_MAN.NewDelayedAction(() => CreatePlayCardSchedule(), 0);
            Managers.EV_MAN.NewDelayedAction(() => CreateAttackSchedule(), 0);
            Managers.EV_MAN.NewDelayedAction(() => CreateActionSchedule(), 0);
        }
        else Managers.EV_MAN.NewDelayedAction(() => Managers.CO_MAN.EndCombatTurn(this), 1);

        bool HasActionsRemaining()
        {
            foreach (var card in HandZoneCards)
                if (Managers.CA_MAN.IsPlayable(card, true)) return true;

            foreach (var unit in PlayZoneCards)
                if (Managers.CO_MAN.CanAttack(unit, null, true, true) &&
                    FindDefender(unit) != null) return true;

            return false;
        }
    }

    private void CreatePlayCardSchedule()
    {
        List<GameObject> actionCards = new();
        var priorityCards = FindPriorityCards();

        foreach (var card in priorityCards)
        {
            if (CombatManager.IsUnitCard(card)) SchedulePlayCard(card);
            else actionCards.Add(card);
        }
        foreach (var card in actionCards) SchedulePlayCard(card);

        List<GameObject> FindPriorityCards()
        {
            int totalCost = 0;
            int cardsToPlay = 0;

            List<GameObject> priorityCards = new();
            List<GameObject> highestCostCards = new();
            foreach (var card in HandZoneCards)
                highestCostCards.Add(card);

            highestCostCards.Sort((c1, c2) =>
            c1.GetComponent<CardDisplay>().CurrentEnergyCost -
            c2.GetComponent<CardDisplay>().CurrentEnergyCost);

            AddPriorityCards();
            int cardsToPlay_1 = cardsToPlay;
            GetNewPriorityCards();
            int cardsToPlay_2 = cardsToPlay;
            if (cardsToPlay_1 > cardsToPlay_2) GetNewPriorityCards();

            priorityCards.Reverse();
            return priorityCards;

            void AddPriorityCards()
            {
                foreach (var card in highestCostCards)
                    AddPriorityCard(card);
            }
            void AddPriorityCard(GameObject card)
            {
                var cd = card.GetComponent<CardDisplay>();
                int cost = cd.CurrentEnergyCost;
                if ((totalCost + cost) > CurrentEnergy || !Managers.CA_MAN.IsPlayable(card, true)) return;
                priorityCards.Add(card);
                totalCost += cost;
                cardsToPlay++;
            }
            void GetNewPriorityCards()
            {
                ClearPriorityCards();
                highestCostCards.Reverse();
                AddPriorityCards();
            }
            void ClearPriorityCards()
            {
                priorityCards.Clear();
                totalCost = 0;
                cardsToPlay = 0;
            }
        }

        void SchedulePlayCard(GameObject card, float delay = 1)
        {
            if (Managers.CA_MAN.IsPlayable(card, true))
                Managers.EV_MAN.NewDelayedAction(() => PlayCard(card), delay, true);
        }

        void PlayCard(GameObject card)
        {
            if (Managers.CA_MAN.IsPlayable(card, true))
                Managers.CA_MAN.PlayCard(card);
        }
    }

    private void CreateAttackSchedule()
    {
        // Strongest enemies attack first (to avoid 'wasting' attacks, sacrificing units unnecessarily)
        var sortHighPower = PlayZoneCards.ToList();
        sortHighPower.Sort((c1, c2) => CombatManager.GetUnitDisplay(c1).CurrentPower -
        CombatManager.GetUnitDisplay(c2).CurrentPower);
        
        foreach (var ally in sortHighPower)
        {
            var ucd = CombatManager.GetUnitDisplay(ally);
            if (CanAttack(ally) && FindDefender(ally) != null) SchedulePreAttack(ally);
        }

        void SchedulePreAttack(GameObject attacker) =>
            Managers.EV_MAN.NewDelayedAction(() => ScheduleAttack(attacker), 0, true);

        void ScheduleAttack(GameObject attacker)
        {
            if (CanAttack(attacker))
            {
                if (FindDefender(attacker) != null)
                {
                    Managers.EV_MAN.NewDelayedAction(() =>
                    ResolveAttack(attacker), 0.5f, true);
                }
                else Debug.LogError("NO VALID DEFENDERS!");
            }
        }

        void ResolveAttack(GameObject attacker)
        {
            if (CanAttack(attacker))
            {
                var defender = FindDefender(attacker);
                if (defender != null) Managers.CO_MAN.Attack(attacker, defender);
                else Debug.LogError("NO VALID DEFENDERS!");
            }
        }

        bool CanAttack(GameObject unit)
        {
            if (unit == null)
            {
                Debug.Log("ENEMY ATTACKER IS NULL!");
                return false;
            }

            if (!PlayZoneCards.Contains(unit))
            {
                Debug.Log("ENEMY ATTACKER HAS MOVED!");
                return false;
            }

            var ucd = CombatManager.GetUnitDisplay(unit);
            if (ucd.IsExhausted) return false;
            if (ucd.CurrentPower < 1 || ucd.CurrentHealth < 1) return false;

            return true;
        }
    }

    private int TotalEnemyPower()
    {
        int totalPower = 0;
        foreach (var unit in PlayZoneCards)
        {
            var ucd = CombatManager.GetUnitDisplay(unit);
            if (!ucd.IsExhausted) totalPower += ucd.CurrentPower;
        }
        return totalPower;
    }

    private int TotalPlayerPower()
    {
        int totalPower = 0;
        foreach (var unit in Managers.P_MAN.PlayZoneCards)
            totalPower += CombatManager.GetUnitDisplay(unit).CurrentPower;
        return totalPower;
    }

    private GameObject FindDefender(GameObject attacker)
    {
        bool playerHasDefender = false;
        int playerHealth = Managers.P_MAN.CurrentHealth;
        List<GameObject> legalDefenders = new();
        var attackerDisplay = attacker.GetComponent<UnitCardDisplay>();

        foreach (var unit in Managers.P_MAN.PlayZoneCards)
        {
            var ucd = CombatManager.GetUnitDisplay(unit);
            if (ucd.CurrentHealth < 1) continue;

            if (CardManager.GetAbility(unit, CardManager.ABILITY_DEFENDER) &&
                !CardManager.GetAbility(unit, CardManager.ABILITY_STEALTH))
            {
                playerHasDefender = true;
                break;
            }
        }

        foreach (var playerUnit in Managers.P_MAN.PlayZoneCards)
        {
            var ucd = CombatManager.GetUnitDisplay(playerUnit);
            if (ucd.CurrentHealth < 1) continue;

            if (playerHasDefender && !CardManager.GetAbility(playerUnit,
                CardManager.ABILITY_DEFENDER)) continue;

            if (CardManager.GetAbility(playerUnit,
                CardManager.ABILITY_STEALTH)) continue;

            legalDefenders.Add(playerUnit);

        }

        // If player hero is <LETHALLY THREATENED>
        if (!playerHasDefender && TotalEnemyPower() >= playerHealth) return Managers.P_MAN.HeroObject;

        // If enemy hero is <LESS THREATENED>
        if (CurrentHealth > (MaxHealth * 0.5f) && TotalPlayerPower() < (CurrentHealth * 0.5f))
        {
            /*
             * > If player hero is <SEVERELY THREATENED>
             * OR 
             * > The attacker has Infiltrate
             * OR
             * > The attacker has an 'enemy hero becomes Wounded' trigger and the player hero is not wounded
             * 
             * ---> Attack the player hero.
             * ---> Otherwise, attack a player unit.
             */
            if (!playerHasDefender)
            {
                if (TotalEnemyPower() >= playerHealth * 0.5f || CardManager.GetTrigger(attacker, CardManager.TRIGGER_INFILTRATE) ||
                    (CardManager.GetModifier(attacker, ModifierAbility.TriggerType.EnemyHeroWounded) && !Managers.P_MAN.IsWounded()))
                    return Managers.P_MAN.HeroObject;
            }
        }
        // If enemy hero is <MORE THREATENED>
        else if (CurrentHealth > (MaxHealth * 0.3f) && TotalPlayerPower() < (CurrentHealth * 0.5f))
        {
            // Attack an enemy unless there are no good attacks
            return GetBestDefender(!playerHasDefender); // Return good attacks only if player does not have defender
        }

        return GetBestDefender(false);

        GameObject GetBestDefender(bool goodAttacksOnly)
        {
            GameObject defender = null;
            int highestPriority = -99;
            int attackerPower = attackerDisplay.CurrentPower;

            foreach (var legalDefender in legalDefenders)
            {
                var ucd = CombatManager.GetUnitDisplay(legalDefender);
                int priority = 0;
                int defenderPower = ucd.CurrentPower;
                int defenderHealth = ucd.CurrentHealth;
                priority += defenderPower + (defenderPower / 3) - (defenderHealth / 2);

                bool defender_Armored =
                    CardManager.GetAbility(legalDefender, CardManager.ABILITY_ARMORED);
                if (defender_Armored && attackerPower < 2) continue;

                if (CardManager.GetAbility(legalDefender,
                    CardManager.ABILITY_RANGED)) priority += 2; // Defender has Ranged

                if (CardManager.GetAbility(legalDefender,
                    CardManager.ABILITY_POISONED)) priority -= 2; // Defender is Poisoned

                if (CardManager.GetTrigger(legalDefender,
                    CardManager.TRIGGER_REVENGE)) priority -= 2; // Defender has Revenge

                if (CardManager.GetAbility(legalDefender, CardManager.ABILITY_FORCEFIELD))
                {
                    if (goodAttacksOnly) continue;
                    priority -= 2;
                }
                else
                {
                    int modHealth = defenderHealth;
                    if (defender_Armored)
                    {
                        if (attackerPower < 2) continue; // Attacker will not deal damage
                        modHealth++;
                    }
                    if (attackerPower >= modHealth) priority += 6; // Defender will be destroyed
                    else if (goodAttacksOnly) continue;
                }

                if (!CardManager.GetAbility(attacker, CardManager.ABILITY_RANGED) &&
                    !CardManager.GetAbility(attacker, CardManager.ABILITY_FORCEFIELD))
                {
                    int modHealth = attackerDisplay.CurrentHealth;
                    if (CardManager.GetAbility(attacker, CardManager.ABILITY_ARMORED)) modHealth++;
                    if (modHealth > defenderPower) priority += 3; // Attacker will survive
                }

                if (priority > highestPriority)
                {
                    highestPriority = priority;
                    defender = legalDefender;
                }
            }
            if (defender == null && !playerHasDefender) return Managers.P_MAN.HeroObject;
            return defender;
        }
    }

    public void UseHeroPower()
    {
        var groupList = HeroScript.CurrentHeroPower.EffectGroupList;
        var heroPower = HeroObject.GetComponent<EnemyHeroDisplay>().HeroPower;
        Managers.EF_MAN.StartEffectGroupList(groupList, heroPower);

        Sound[] soundList;
        soundList = HeroScript.CurrentHeroPower.PowerSounds;
        foreach (var s in soundList) AudioManager.Instance.StartStopSound(null, s);
        Managers.AN_MAN.TriggerHeroPower(heroPower);
    }
}
