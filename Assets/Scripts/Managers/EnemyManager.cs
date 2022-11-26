using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
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
    
    private CombatManager coMan;
    private EventManager evMan;
    private AnimationManager anMan;
    private CardManager caMan;

    private EnemyHero enemyHero;
    private int enemyHealth;
    private int damageTaken_Turn;
    private int energyPerTurn;
    private int currentEnergy;
    public bool IsMyTurn { get; set; }
    public List<Card> EnemyDeckList { get; private set; }
    public List<Card> CurrentEnemyDeck { get; private set; }
    public int ReinforcementGroup { get; set; }

    public EnemyHero EnemyHero
    {
        get => enemyHero;
        set
        {
            enemyHero = value;
            if (EnemyDeckList == null || CurrentEnemyDeck == null) return;
            EnemyDeckList.Clear();

            if (value == null)
            {
                CurrentEnemyDeck.Clear();
                return;
            }
            if (EnemyHero.Reinforcements[ReinforcementGroup] == null)
            {
                Debug.LogError("REINFORCEMENTS NOT FOUND!");
                return;
            }

            Reinforcements reinforcements = enemyHero.Reinforcements[ReinforcementGroup];
            List<UnitCard> refoUnits = reinforcements.ReinforcementUnits;
            List<ActionCard> refoActions = reinforcements.ReinforcementActions;

            foreach (UnitCard unit in refoUnits) caMan.AddCard(unit, GameManager.ENEMY);
            foreach (ActionCard action in refoActions) caMan.AddCard(action, GameManager.ENEMY);
        }
    }
    public int EnemyHealth
    {
        get => enemyHealth;
        set
        {
            enemyHealth = value;
            coMan.EnemyHero.GetComponent<HeroDisplay>().HeroHealth = enemyHealth;
        }
    }
    public int MaxEnemyHealth
    {
        get
        {
            if (GameManager.Instance.IsTutorial)
                return GameManager.TUTORIAL_STARTING_HEALTH;
            return GameManager.ENEMY_STARTING_HEALTH;
        }
    }
    public int DamageTaken_Turn
    {
        get => damageTaken_Turn;
        set
        {
            bool wasWounded;
            if (damageTaken_Turn >= GameManager.WOUNDED_VALUE) wasWounded = true;
            else wasWounded = false;

            damageTaken_Turn = value;
            bool isWounded;
            if (damageTaken_Turn >= GameManager.WOUNDED_VALUE) isWounded = true;
            else isWounded = false;
            coMan.EnemyHero.GetComponent<HeroDisplay>().IsWounded = isWounded;

            if (!wasWounded && isWounded)
            {
                EffectManager.Instance.TriggerModifiers_SpecialTrigger
                    (ModifierAbility.TriggerType.EnemyHeroWounded, coMan.PlayerZoneCards);
            }
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
            coMan.EnemyHero.GetComponent<HeroDisplay>().SetHeroEnergy(CurrentEnergy, energyPerTurn);
        }
    }
    public int MaxEnergyPerTurn => GameManager.MAXIMUM_ENERGY_PER_TURN;
    private int MaxEnergy => GameManager.MAXIMUM_ENERGY;
    public int CurrentEnergy
    {
        get => currentEnergy;
        set
        {
            currentEnergy = value;
            if (currentEnergy > MaxEnergy) currentEnergy = MaxEnergy;
            coMan.EnemyHero.GetComponent<HeroDisplay>().SetHeroEnergy(CurrentEnergy, energyPerTurn);

            if (currentEnergy < 0)
            {
                Debug.LogError("NEGATIVE ENERGY LEFT <" + currentEnergy + ">");
            }
        }
    }

    private void Start()
    {
        coMan = CombatManager.Instance;
        evMan = EventManager.Instance;
        anMan = AnimationManager.Instance;
        caMan = CardManager.Instance;
    }

    public void StartCombat()
    {
        EnemyDeckList = new List<Card>();
        CurrentEnemyDeck = new List<Card>();
        ReinforcementGroup = 0; // FOR TESTING ONLY?
    }

    public void Mulligan()
    {
        List<GameObject> cardsToReplace = new List<GameObject>();

        foreach (GameObject card in coMan.EnemyHandCards)
        {
            CardDisplay cd = card.GetComponent<CardDisplay>();
            if (cd.CurrentEnergyCost > 2) cardsToReplace.Add(card);
        }

        foreach (GameObject card in cardsToReplace) coMan.DiscardCard(card);
        for (int i = 0; i < cardsToReplace.Count; i++)
            evMan.NewDelayedAction(() =>
            coMan.DrawCard(GameManager.ENEMY), 0.5f, true);
    }

    public void StartEnemyTurn()
    {
        evMan.NewDelayedAction(() => ReplenishEnergy(), 0);

        evMan.NewDelayedAction(() =>
        caMan.TriggerPlayedUnits(CardManager.TRIGGER_TURN_START, GameManager.ENEMY), 0);

        evMan.NewDelayedAction(() => TurnDraw(), 1);

        evMan.NewDelayedAction(() => CreateActionSchedule(), 0); // TESTING
        
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
            foreach (GameObject card in coMan.EnemyHandCards)
                if (coMan.IsPlayable(card, true)) return true;

            foreach (GameObject unit in coMan.EnemyZoneCards)
                if (coMan.CanAttack(unit, null, true, true)) return true;

            return false;
        }
        void ReplenishEnergy()
        {
            int startEnergy = CurrentEnergy;
            if (energyPerTurn < MaxEnergyPerTurn) energyPerTurn++;
            CurrentEnergy = EnergyPerTurn;
            int energyChange = CurrentEnergy - startEnergy;
            anMan.ModifyHeroEnergyState(energyChange, coMan.EnemyHero);
        }
        void TurnDraw()
        {
            if (coMan.EnemyHandCards.Count >= GameManager.MAX_HAND_SIZE)
            {
                UIManager.Instance.CreateFleetingInfoPopup("Enemy hand is full!");
                Debug.Log("ENEMY HAND IS FULL!");
                return;
            }
            coMan.DrawCard(GameManager.ENEMY);
        }
    }

    private void CreatePlayCardSchedule()
    {
        List<GameObject> actionCards = new List<GameObject>();
        List<GameObject> priorityCards = FindPriorityCards();

        foreach (GameObject card in priorityCards)
        {
            if (coMan.IsUnitCard(card)) SchedulePlayCard(card);
            else actionCards.Add(card);
        }
        foreach (GameObject card in actionCards) SchedulePlayCard(card);

        List<GameObject> FindPriorityCards()
        {
            int totalCost = 0;
            int cardsToPlay = 0;

            List<GameObject> priorityCards = new List<GameObject>();

            List<GameObject> highestCostCards = new List<GameObject>();
            foreach (GameObject card in coMan.EnemyHandCards)
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
                if ((totalCost + cost) > CurrentEnergy || !coMan.IsPlayable(card, true)) return;
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
            if (coMan.IsPlayable(card, true))
            {
                CardDisplay cd = card.GetComponent<CardDisplay>();
                coMan.PlayCard(card);
            }
        }
    }

    private void CreateAttackSchedule()
    {
        foreach (GameObject ally in coMan.EnemyZoneCards)
        {
            UnitCardDisplay ucd = coMan.GetUnitDisplay(ally);
            if (CanAttack(ally)) SchedulePreAttack(ally);
        }
        
        void SchedulePreAttack(GameObject attacker) =>
            evMan.NewDelayedAction(() => ScheduleAttack(attacker), 0, true);

        void ScheduleAttack(GameObject attacker)
        {
            if (CanAttack(attacker)) evMan.NewDelayedAction(() =>
            ResolveAttack(attacker), 0.5f, true);
        }

        void ResolveAttack(GameObject attacker)
        {
            if (CanAttack(attacker))
            {
                GameObject defender = FindDefender(attacker);
                coMan.Attack(attacker, defender);
            }
        }

        bool CanAttack(GameObject unit)
        {
            if (unit == null)
            {
                Debug.Log("ENEMY ATTACKER IS NULL!");
                return false;
            }

            if (!coMan.EnemyZoneCards.Contains(unit))
            {
                Debug.Log("ENEMY ATTACKER HAS MOVED!");
                return false;
            }

            UnitCardDisplay ucd = coMan.GetUnitDisplay(unit);
            if (ucd.IsExhausted) return false;
            if (ucd.CurrentPower < 1 || ucd.CurrentHealth < 1) return false;

            return true;
        }
    }
    
    private int TotalEnemyPower()
    {
        int totalPower = 0;
        foreach (GameObject unit in coMan.EnemyZoneCards)
        {
            UnitCardDisplay ucd = coMan.GetUnitDisplay(unit);
            if (!ucd.IsExhausted) totalPower += ucd.CurrentPower;
        }
        return totalPower;
    }
    
    private int TotalPlayerPower()
    {
        int totalPower = 0;
        foreach (GameObject unit in coMan.PlayerZoneCards)
            totalPower += coMan.GetUnitDisplay(unit).CurrentPower;
        return totalPower;
    }

    private GameObject FindDefender(GameObject attacker)
    {
        bool playerHasDefender = false;
        int playerHealth = PlayerManager.Instance.PlayerHealth;
        List<GameObject> legalDefenders = new List<GameObject>();
        UnitCardDisplay attackerDisplay = attacker.GetComponent<UnitCardDisplay>();

        foreach (GameObject unit in coMan.PlayerZoneCards)
        {
            UnitCardDisplay ucd = coMan.GetUnitDisplay(unit);
            if (ucd.CurrentHealth < 1) continue;

            if (CardManager.GetAbility(unit, CardManager.ABILITY_DEFENDER) &&
                !CardManager.GetAbility(unit, CardManager.ABILITY_STEALTH))
            {
                playerHasDefender = true;
                break;
            }
        }

        foreach (GameObject playerUnit in coMan.PlayerZoneCards)
        {
            UnitCardDisplay ucd = coMan.GetUnitDisplay(playerUnit);
            if (ucd.CurrentHealth < 1) continue;

            if (playerHasDefender && !CardManager.GetAbility(playerUnit,
                CardManager.ABILITY_DEFENDER)) continue;

            if (CardManager.GetAbility(playerUnit,
                CardManager.ABILITY_STEALTH)) continue;

            legalDefenders.Add(playerUnit);

        }

        // If player hero is <LETHALLY THREATENED>
        if (!playerHasDefender && TotalEnemyPower() >= playerHealth) return coMan.PlayerHero;

        // If enemy hero is <UNTHREATENED>
        if (EnemyHealth > (MaxEnemyHealth * 0.65f) && TotalPlayerPower() < (EnemyHealth * 0.35f))
        {
            // Attack an enemy unless there are no good attacks
            return GetBestDefender(!playerHasDefender); // Return good attacks only if player does not have defender
        }
        // If enemy hero is <MILDLY THREATENED>
        else if (EnemyHealth > (MaxEnemyHealth * 0.5f) && TotalPlayerPower() < (EnemyHealth * 0.5f))
        {
            /*
             * If player hero is <SEVERELY THREATENED> or the attacker has Infiltrate,
             * attack the player hero. Otherwise, attack a player unit.
             */
            if (!playerHasDefender)
            {
                if (TotalEnemyPower() >= playerHealth * 0.75f ||
                    CardManager.GetTrigger(attacker, CardManager.TRIGGER_INFILTRATE))
                    return coMan.PlayerHero;
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
                UnitCardDisplay ucd = coMan.GetUnitDisplay(legalDefender);
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
            if (defender == null) return coMan.PlayerHero;
            return defender;
        }
    }

    public void UseHeroPower()
    {
        List<EffectGroup> groupList = EnemyHero.EnemyHeroPower.EffectGroupList;
        GameObject heroPower = coMan.EnemyHero.GetComponent<EnemyHeroDisplay>().HeroPower;
        EffectManager.Instance.StartEffectGroupList(groupList, heroPower);

        Sound[] soundList;
        soundList = EnemyHero.EnemyHeroPower.PowerSounds;
        foreach (Sound s in soundList) AudioManager.Instance.StartStopSound(null, s);
        AnimationManager.Instance.TriggerHeroPower(heroPower);
    }
}
