using System.Collections.Generic;
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
    
    private EventManager evMan;
    private AnimationManager anMan;
    private CardManager caMan;
    private PlayerManager pMan;

    public override bool IsMyTurn { get; set; }
    public override int TurnNumber
    {
        get => turnNumber;
        set
        {
            turnNumber = value;

            int surgeDelay = GameManager.Instance.GetSurgeDelay(coMan.DifficultyLevel);
            if (TurnNumber > 0 && TurnNumber % surgeDelay == 0)
            {
                int surgeCount = TurnNumber / surgeDelay;
                if (surgeCount < 1)
                {
                    Debug.LogError("SURGE COUNT < 1!");
                    return;
                }

                for (int i = 0; i < surgeCount; i++)
                    evMan.NewDelayedAction(() => Surge(), 0.5f, true);

                evMan.NewDelayedAction(() => SurgePopup(), 0.5f, true);

                void SurgePopup()
                {
                    UIManager.Instance.CreateFleetingInfoPopup($"ENEMY SURGE!\n[{surgeCount}x]");
                    anMan.CreateParticleSystem(null, ParticleSystemHandler.ParticlesType.Explosion, 2);
                }
                void Surge()
                {
                    GameObject surgeCard = caMan.DrawCard(this);
                    if (surgeCard != null)
                    {
                        surgeCard.GetComponent<CardDisplay>().ChangeCurrentEnergyCost(-surgeCount);
                    }
                }
            }
        }
    }
    public int ReinforcementGroup { get; set; }
    public EnemyHero EnemyHero { get => heroScript as EnemyHero; }
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

            Reinforcements reinforcements = (heroScript as EnemyHero).Reinforcements[ReinforcementGroup];
            List<UnitCard> refoUnits = reinforcements.ReinforcementUnits;
            List<ActionCard> refoActions = reinforcements.ReinforcementActions;

            foreach (UnitCard unit in refoUnits) caMan.AddCard(unit, GameManager.ENEMY);
            foreach (ActionCard action in refoActions) caMan.AddCard(action, GameManager.ENEMY);
        }
    }

    public override int MaxHealth
    {
        get
        {
            if (GameManager.Instance.IsTutorial)
                return GameManager.TUTORIAL_STARTING_HEALTH;
            return GameManager.ENEMY_STARTING_HEALTH;
        }
    }

    protected override void Start()
    {
        base.Start();
        evMan = EventManager.Instance;
        anMan = AnimationManager.Instance;
        caMan = CardManager.Instance;
        pMan = PlayerManager.Instance;
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
        List<GameObject> cardsToReplace = new List<GameObject>();

        foreach (GameObject card in HandZoneCards)
        {
            CardDisplay cd = card.GetComponent<CardDisplay>();
            if (cd.CurrentEnergyCost > 2) cardsToReplace.Add(card);
        }

        foreach (GameObject card in cardsToReplace) caMan.DiscardCard(card);
        for (int i = 0; i < cardsToReplace.Count; i++)
            evMan.NewDelayedAction(() =>
            caMan.DrawCard(this), 0.5f, true);
    }

    public void StartEnemyTurn()
    {
        TurnNumber++; // TESTING

        evMan.NewDelayedAction(() => ReplenishEnergy(), 0);

        evMan.NewDelayedAction(() =>
        caMan.TriggerPlayedUnits(CardManager.TRIGGER_TURN_START, GameManager.ENEMY), 0);

        evMan.NewDelayedAction(() => TurnDraw(), 1);

        evMan.NewDelayedAction(() => CreateActionSchedule(), 0);
        
        void CreateActionSchedule()
        {
            if (HasActionsRemaining())
            {
                evMan.NewDelayedAction(() => CreatePlayCardSchedule(), 0);
                evMan.NewDelayedAction(() => CreateAttackSchedule(), 0);
                evMan.NewDelayedAction(() => CreateActionSchedule(), 0);
            }
            else evMan.NewDelayedAction(() =>
            GameManager.Instance.EndCombatTurn(GameManager.ENEMY), 1);
        }
        bool HasActionsRemaining()
        {
            foreach (GameObject card in HandZoneCards)
                if (caMan.IsPlayable(card, true)) return true;

            foreach (GameObject unit in PlayZoneCards)
                if (coMan.CanAttack(unit, null, true, true) &&
                    FindDefender(unit) != null) return true;

            return false;
        }
        void ReplenishEnergy()
        {
            int startEnergy = CurrentEnergy;
            if (EnergyPerTurn < MaxEnergyPerTurn) EnergyPerTurn++;
            CurrentEnergy = EnergyPerTurn;
            int energyChange = CurrentEnergy - startEnergy;
            anMan.ModifyHeroEnergyState(energyChange, HeroObject);
        }
        void TurnDraw()
        {
            if (HandZoneCards.Count >= GameManager.MAX_HAND_SIZE)
            {
                UIManager.Instance.CreateFleetingInfoPopup("Enemy hand is full!");
                Debug.Log("ENEMY HAND IS FULL!");
                return;
            }
            caMan.DrawCard(this);
        }
    }

    private void CreatePlayCardSchedule()
    {
        List<GameObject> actionCards = new List<GameObject>();
        List<GameObject> priorityCards = FindPriorityCards();

        foreach (GameObject card in priorityCards)
        {
            if (CombatManager.IsUnitCard(card)) SchedulePlayCard(card);
            else actionCards.Add(card);
        }
        foreach (GameObject card in actionCards) SchedulePlayCard(card);

        List<GameObject> FindPriorityCards()
        {
            int totalCost = 0;
            int cardsToPlay = 0;

            List<GameObject> priorityCards = new List<GameObject>();

            List<GameObject> highestCostCards = new List<GameObject>();
            foreach (GameObject card in HandZoneCards)
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
                foreach (GameObject card in highestCostCards)
                    AddPriorityCard(card);
            }
            void AddPriorityCard(GameObject card)
            {
                CardDisplay cd = card.GetComponent<CardDisplay>();
                int cost = cd.CurrentEnergyCost;
                if ((totalCost + cost) > CurrentEnergy || !caMan.IsPlayable(card, true)) return;
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
            CardDisplay cd = card.GetComponent<CardDisplay>();
            evMan.NewDelayedAction(() => PlayCard(card), delay, true);
        }

        void PlayCard(GameObject card)
        {
            if (caMan.IsPlayable(card, true))
            {
                CardDisplay cd = card.GetComponent<CardDisplay>();
                caMan.PlayCard(card);
            }
        }
    }

    private void CreateAttackSchedule()
    {
        foreach (GameObject ally in PlayZoneCards)
        {
            UnitCardDisplay ucd = CombatManager.GetUnitDisplay(ally);
            if (CanAttack(ally) && FindDefender(ally) != null) SchedulePreAttack(ally);
        }
        
        void SchedulePreAttack(GameObject attacker) =>
            evMan.NewDelayedAction(() => ScheduleAttack(attacker), 0, true);

        void ScheduleAttack(GameObject attacker)
        {
            if (CanAttack(attacker))
            {
                if (FindDefender(attacker) != null)
                {
                    evMan.NewDelayedAction(() =>
                    ResolveAttack(attacker), 0.5f, true);
                }
                else Debug.LogError("NO VALID DEFENDERS!");
            }
        }

        void ResolveAttack(GameObject attacker)
        {
            if (CanAttack(attacker))
            {
                GameObject defender = FindDefender(attacker);
                if (defender != null) coMan.Attack(attacker, defender);
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

            UnitCardDisplay ucd = CombatManager.GetUnitDisplay(unit);
            if (ucd.IsExhausted) return false;
            if (ucd.CurrentPower < 1 || ucd.CurrentHealth < 1) return false;

            return true;
        }
    }
    
    private int TotalEnemyPower()
    {
        int totalPower = 0;
        foreach (GameObject unit in PlayZoneCards)
        {
            UnitCardDisplay ucd = CombatManager.GetUnitDisplay(unit);
            if (!ucd.IsExhausted) totalPower += ucd.CurrentPower;
        }
        return totalPower;
    }
    
    private int TotalPlayerPower()
    {
        int totalPower = 0;
        foreach (GameObject unit in pMan.PlayZoneCards)
            totalPower += CombatManager.GetUnitDisplay(unit).CurrentPower;
        return totalPower;
    }

    private GameObject FindDefender(GameObject attacker)
    {
        bool playerHasDefender = false;
        int playerHealth = PlayerManager.Instance.CurrentHealth;
        List<GameObject> legalDefenders = new List<GameObject>();
        UnitCardDisplay attackerDisplay = attacker.GetComponent<UnitCardDisplay>();

        foreach (GameObject unit in pMan.PlayZoneCards)
        {
            UnitCardDisplay ucd = CombatManager.GetUnitDisplay(unit);
            if (ucd.CurrentHealth < 1) continue;

            if (CardManager.GetAbility(unit, CardManager.ABILITY_DEFENDER) &&
                !CardManager.GetAbility(unit, CardManager.ABILITY_STEALTH))
            {
                playerHasDefender = true;
                break;
            }
        }

        foreach (GameObject playerUnit in pMan.PlayZoneCards)
        {
            UnitCardDisplay ucd = CombatManager.GetUnitDisplay(playerUnit);
            if (ucd.CurrentHealth < 1) continue;

            if (playerHasDefender && !CardManager.GetAbility(playerUnit,
                CardManager.ABILITY_DEFENDER)) continue;

            if (CardManager.GetAbility(playerUnit,
                CardManager.ABILITY_STEALTH)) continue;

            legalDefenders.Add(playerUnit);

        }

        // If player hero is <LETHALLY THREATENED>
        if (!playerHasDefender && TotalEnemyPower() >= playerHealth) return pMan.HeroObject;

        // If enemy hero is <UNTHREATENED>
        if (CurrentHealth > (MaxHealth * 0.65f) && TotalPlayerPower() < (CurrentHealth * 0.35f))
        {
            // Attack an enemy unless there are no good attacks
            return GetBestDefender(!playerHasDefender); // Return good attacks only if player does not have defender
        }
        // If enemy hero is <MILDLY THREATENED>
        else if (CurrentHealth > (MaxHealth * 0.5f) && TotalPlayerPower() < (CurrentHealth * 0.5f))
        {
            /*
             * If player hero is <SEVERELY THREATENED> or the attacker has Infiltrate,
             * attack the player hero. Otherwise, attack a player unit.
             */
            if (!playerHasDefender)
            {
                if (TotalEnemyPower() >= playerHealth * 0.75f ||
                    CardManager.GetTrigger(attacker, CardManager.TRIGGER_INFILTRATE))
                    return pMan.HeroObject;
            }
        }

        return GetBestDefender(false);
        
        GameObject GetBestDefender(bool goodAttacksOnly)
        {
            GameObject defender = null;
            int highestPriority = -99;
            int attackerPower = attackerDisplay.CurrentPower;

            foreach (GameObject legalDefender in legalDefenders)
            {
                UnitCardDisplay ucd = CombatManager.GetUnitDisplay(legalDefender);
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
            if (defender == null && !playerHasDefender) return pMan.HeroObject;
            return defender;
        }
    }
    
    public void UseHeroPower()
    {
        List<EffectGroup> groupList = HeroScript.HeroPower.EffectGroupList;
        GameObject heroPower = HeroObject.GetComponent<EnemyHeroDisplay>().HeroPower;
        EffectManager.Instance.StartEffectGroupList(groupList, heroPower);

        Sound[] soundList;
        soundList = HeroScript.HeroPower.PowerSounds;
        foreach (Sound s in soundList) AudioManager.Instance.StartStopSound(null, s);
        AnimationManager.Instance.TriggerHeroPower(heroPower);
    }
}
