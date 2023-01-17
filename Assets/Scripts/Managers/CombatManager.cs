using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    /* SINGELTON_PATTERN */
    public static CombatManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    #region FIELDS
    private GameManager gMan;
    private CardManager caMan;
    private AudioManager auMan;
    private EffectManager efMan;
    private EventManager evMan;
    private UIManager uMan;
    private AnimationManager anMan;
    private PlayerManager pMan;
    private EnemyManager enMan;

    // Shared Zones
    public const string CARD_ZONE = "CardZone";

    // Player Zones
    public const string PLAYER_HERO = "PlayerHero";
    public const string PLAYER_HAND = "PlayerHand";
    public const string PLAYER_ZONE = "PlayerZone";
    public const string PLAYER_ACTION_ZONE = "PlayerActionZone";
    public const string PLAYER_DISCARD = "PlayerDiscard";
    public const string HERO_POWER = "HeroPower";
    public const string HERO_ULTIMATE = "HeroUltimate";

    // Enemy Zones
    public const string ENEMY_HERO = "EnemyHero";
    public const string ENEMY_HAND = "EnemyHand";
    public const string ENEMY_ZONE = "EnemyZone";
    public const string ENEMY_ACTION_ZONE = "EnemyActionZone";
    public const string ENEMY_DISCARD = "EnemyDiscard";
    public const string ENEMY_HERO_POWER = "EnemyHeroPower";
    #endregion

    #region PROPERTIES
    /* GAME ZONES */
    public GameObject CardZone { get; private set; }

    private static string DIFFICULTY_LEVEL = "DifficultyLevel";
    public int DifficultyLevel
    {
        get => PlayerPrefs.GetInt(DIFFICULTY_LEVEL, 1);
        set => PlayerPrefs.SetInt(DIFFICULTY_LEVEL, value);
    }
    #endregion

    #region METHODS
    #region UTILITY
    private void Start()
    {
        gMan = GameManager.Instance;
        caMan = CardManager.Instance;
        auMan = AudioManager.Instance;
        efMan = EffectManager.Instance;
        evMan = EventManager.Instance;
        uMan = UIManager.Instance;
        anMan = AnimationManager.Instance;
        pMan = PlayerManager.Instance;
        enMan = EnemyManager.Instance;
    }

    /******
     * *****
     * ****** START_COMBAT_SCENE
     * *****
     *****/
    public void StartCombatScene()
    {
        // GAME ZONES
        // ALL CARDS
        CardZone = GameObject.Find(CARD_ZONE);
        // PLAYER
        pMan.HandZone = GameObject.Find(PLAYER_HAND);
        pMan.PlayZone = GameObject.Find(PLAYER_ZONE);
        pMan.ActionZone = GameObject.Find(PLAYER_ACTION_ZONE);
        pMan.DiscardZone = GameObject.Find(PLAYER_DISCARD);
        pMan.HeroObject = GameObject.Find(PLAYER_HERO);
        // ENEMY
        enMan.HandZone = GameObject.Find(ENEMY_HAND);
        enMan.PlayZone = GameObject.Find(ENEMY_ZONE);
        enMan.ActionZone = GameObject.Find(ENEMY_ACTION_ZONE);
        enMan.DiscardZone = GameObject.Find(ENEMY_DISCARD);
        enMan.HeroObject = GameObject.Find(ENEMY_HERO);

        uMan.SelectTarget(pMan.HeroObject, UIManager.SelectionType.Disabled);
        uMan.SelectTarget(pMan.HeroObject, UIManager.SelectionType.Disabled);
    }

    public static HeroManager GetSourceHero(GameObject sourceObject, bool getEnemy = false)
    {
        if (sourceObject == null)
        {
            Debug.LogError("SOURCE IS NULL!");
            return null;
        }

        if (sourceObject.CompareTag(CardManager.ENEMY_CARD) ||
            sourceObject.CompareTag(ENEMY_HERO) ||
            sourceObject.CompareTag(ENEMY_HERO_POWER))
        {
            if (!getEnemy) return EnemyManager.Instance;
            else return PlayerManager.Instance;
        }

        if (sourceObject.CompareTag(CardManager.PLAYER_CARD) ||
            sourceObject.CompareTag(PLAYER_HERO) ||
            sourceObject.CompareTag(HERO_POWER) ||
            sourceObject.CompareTag(HERO_ULTIMATE) ||
            sourceObject.CompareTag("HeroItem"))
        {
            if (!getEnemy) return PlayerManager.Instance;
            else return EnemyManager.Instance;
        }

        Debug.LogError("INVALID TAG!");
        return null;
    }
    public static UnitCardDisplay GetUnitDisplay(GameObject card)
    {
        if (!IsUnitCard(card))
        {
            Debug.LogError("OBJECT IS NOT UNIT CARD!");
            return null;
        }

        return card.GetComponent<UnitCardDisplay>();
    }
    public static bool IsUnitCard(GameObject target)
    {
        if (target == null)
        {
            Debug.LogWarning("CARD IS NULL!");
            return false;
        }
        return target.TryGetComponent<UnitCardDisplay>(out _);
    }
    public static bool IsActionCard(GameObject target)
    {
        if (target == null)
        {
            Debug.LogWarning("CARD IS NULL!");
            return false;
        }
        return target.TryGetComponent<ActionCardDisplay>(out _);
    }
    public static bool IsDamaged(GameObject unitCard)
    {
        if (!unitCard.TryGetComponent(out UnitCardDisplay ucd))
        {
            Debug.LogError("TARGET IS NOT UNIT CARD!");
            return false;
        }
        return ucd.CurrentHealth < ucd.MaxHealth;
    }
    /*
    public static bool PowerIsBuffed(GameObject unitCard)
    {
        UnitCardDisplay ucd = unitCard.GetComponent<UnitCardDisplay>();
        return ucd.CurrentPower > ucd.UnitCard.StartPower;
    }
    public static bool PowerIsDebuffed(GameObject unitCard)
    {
        UnitCardDisplay ucd = unitCard.GetComponent<UnitCardDisplay>();
        return ucd.CurrentPower < ucd.UnitCard.StartPower;
    }
    public static bool HealthIsBuffed(GameObject unitCard)
    {
        UnitCardDisplay ucd = unitCard.GetComponent<UnitCardDisplay>();
        return ucd.CurrentHealth > ucd.UnitCard.StartHealth;
    }
    */

    /******
     * *****
     * ****** GET_LOWEST_HEALTH_UNIT
     * *****
     *****/
    public GameObject GetLowestHealthUnit(List<GameObject> unitList, bool targetsEnemy)
    {
        if (unitList.Count < 1) return null;
        int lowestHealth = int.MaxValue;
        List<GameObject> lowestHealthUnits = new List<GameObject>();

        foreach (GameObject unit in unitList)
        {
            if (!IsUnitCard(unit)) continue;

            if (targetsEnemy && CardManager.GetAbility(unit, CardManager.ABILITY_WARD)) continue;

            int health = GetUnitDisplay(unit).CurrentHealth;
            if (health < 1 || efMan.UnitsToDestroy.Contains(unit)) continue;

            if (health < lowestHealth)
            {
                lowestHealth = health;
                lowestHealthUnits.Clear();
                lowestHealthUnits.Add(unit);
            }
            else if (health == lowestHealth) lowestHealthUnits.Add(unit);
        }
        if (lowestHealthUnits.Count < 1) return null;
        if (lowestHealthUnits.Count > 1)
        {
            int randomIndex = Random.Range(0, lowestHealthUnits.Count);
            return lowestHealthUnits[randomIndex];
        }
        else return lowestHealthUnits[0];
    }

    /******
     * *****
     * ****** GET_STRONGEST_UNIT
     * *****
     *****/
    public GameObject GetStrongestUnit(List<GameObject> unitList, bool targetsEnemy)
    {
        if (unitList.Count < 1) return null;
        int highestPower = -1;
        List<GameObject> highestPowerUnits = new List<GameObject>();

        foreach (GameObject unit in unitList)
        {
            if (!IsUnitCard(unit)) continue;

            if (targetsEnemy && CardManager.GetAbility(unit, CardManager.ABILITY_WARD)) continue;

            int health = GetUnitDisplay(unit).CurrentHealth;
            if (health < 1 || efMan.UnitsToDestroy.Contains(unit)) continue;

            int power = GetUnitDisplay(unit).CurrentPower;
            if (power > highestPower)
            {
                highestPower = power;
                highestPowerUnits.Clear();
                highestPowerUnits.Add(unit);
            }
            else if (power == highestPower) highestPowerUnits.Add(unit);
        }

        if (highestPowerUnits.Count < 1) return null;

        if (highestPowerUnits.Count > 1)
        {
            List<GameObject> highestHealthUnits = new List<GameObject>();
            int highestHealth = 0;

            foreach (GameObject unit in highestPowerUnits)
            {
                int health = GetUnitDisplay(unit).CurrentHealth;
                if (health > highestHealth)
                {
                    highestHealth = health;
                    highestHealthUnits.Clear();
                    highestHealthUnits.Add(unit);
                }
                else if (health == highestHealth) highestHealthUnits.Add(unit);
            }

            if (highestHealthUnits.Count > 1)
            {
                int randomIndex = Random.Range(0, highestHealthUnits.Count);
                return highestHealthUnits[randomIndex];
            }
            else return highestHealthUnits[0];
        }
        else return highestPowerUnits[0];
    }

    /******
     * *****
     * ****** GET_WEAKEST_UNIT
     * *****
     *****/
    public GameObject GetWeakestUnit(List<GameObject> unitList, bool targetsEnemy)
    {
        if (unitList.Count < 1) return null;
        int lowestPower = 999;
        List<GameObject> lowestPowerUnits = new List<GameObject>();

        foreach (GameObject unit in unitList)
        {
            if (!IsUnitCard(unit)) continue;

            if (targetsEnemy && CardManager.GetAbility(unit, CardManager.ABILITY_WARD)) continue;

            int health = GetUnitDisplay(unit).CurrentHealth;
            if (health < 1 || efMan.UnitsToDestroy.Contains(unit)) continue;

            int power = GetUnitDisplay(unit).CurrentPower;
            if (power < lowestPower)
            {
                lowestPower = power;
                lowestPowerUnits.Clear();
                lowestPowerUnits.Add(unit);
            }
            else if (power == lowestPower) lowestPowerUnits.Add(unit);
        }

        if (lowestPowerUnits.Count < 1) return null;

        if (lowestPowerUnits.Count > 1)
        {
            List<GameObject> lowestHealthUnits = new List<GameObject>();
            int lowestHealth = 999;

            foreach (GameObject unit in lowestPowerUnits)
            {
                int health = GetUnitDisplay(unit).CurrentHealth;
                if (health < lowestHealth)
                {
                    lowestHealth = health;
                    lowestHealthUnits.Clear();
                    lowestHealthUnits.Add(unit);
                }
                else if (health == lowestHealth) lowestHealthUnits.Add(unit);
            }

            if (lowestHealthUnits.Count > 1)
            {
                int randomIndex = Random.Range(0, lowestHealthUnits.Count);
                return lowestHealthUnits[randomIndex];
            }
            else return lowestHealthUnits[0];
        }
        else return lowestPowerUnits[0];
    }
    /******
     * *****
     * ****** CAN_ATTACK
     * *****
     *****/
    public bool CanAttack(GameObject attacker, GameObject defender, bool preCheck = true, bool ignoreDefender = false)
    {
        bool defenderIsUnit = false;
        if (defender != null) defenderIsUnit = IsUnitCard(defender);

        if (defender != null)
        {
            if (attacker.CompareTag(defender.tag)) return false;
            if (attacker.CompareTag(CardManager.PLAYER_CARD) && defender == pMan.HeroObject) return false;
        }
        else
        {
            if (!preCheck && !ignoreDefender)
            {
                Debug.LogError("DEFENDER IS NULL!");
                return false;
            }
        }

        // TUTORIAL!
        if (!preCheck && gMan.IsTutorial && pMan.EnergyPerTurn == 2)
        {
            if (!defenderIsUnit) return false;
        }

        UnitCardDisplay atkUcd = GetUnitDisplay(attacker);
        if (atkUcd.IsExhausted)
        {
            if (preCheck && !ignoreDefender)
            {
                uMan.CreateFleetingInfoPopup("Exhausted units can't attack!");
                SFX_Error();
            }
            return false;
        }
        else if (atkUcd.CurrentPower < 1)
        {
            if (preCheck && !ignoreDefender)
            {
                uMan.CreateFleetingInfoPopup("Units with 0 power can't attack!");
                SFX_Error();
            }
            return false;
        }

        if (defender == null) return true; // For DragDrop() and SelectPlayableCards()

        if (defender.TryGetComponent(out UnitCardDisplay defUcd))
        {
            if (enMan.HandZoneCards.Contains(defender)) return false; // Unnecessary, already checked in CardSelect
            if (defUcd.CurrentHealth < 1 || efMan.UnitsToDestroy.Contains(defender)) return false; // Destroyed units that haven't left play yet
            if (CardManager.GetAbility(defender, CardManager.ABILITY_STEALTH))
            {
                if (!preCheck)
                {
                    uMan.CreateFleetingInfoPopup("Units with Stealth can't be attacked!");
                    SFX_Error();
                }
                return false;
            }
        }

        if (!(defenderIsUnit && CardManager.GetAbility(defender, CardManager.ABILITY_DEFENDER))) // TESTING
        {
            List<GameObject> enemyZone;
            if (attacker.CompareTag(CardManager.PLAYER_CARD)) enemyZone = enMan.PlayZoneCards;
            else if (attacker.CompareTag(CardManager.ENEMY_CARD)) enemyZone = pMan.PlayZoneCards;
            else
            {
                Debug.LogError("INVALID ZONE!");
                return false;
            }
            foreach (GameObject unit in enemyZone)
            {
                if (unit == defender) continue;

                if (CardManager.GetAbility(unit, CardManager.ABILITY_DEFENDER) &&
                    !CardManager.GetAbility(unit, CardManager.ABILITY_STEALTH))
                {
                    if (!preCheck)
                    {
                        uMan.CreateFleetingInfoPopup("Enemies with Defender must be attacked!");
                        SFX_Error();
                    }
                    return false;
                }
            }
        }

        return true;

        void SFX_Error() => auMan.StartStopSound("SFX_Error");
    }
    #endregion

    #region BASIC COMBAT
    /******
     * *****
     * ****** REFRESH_ALL_UNITS
     * *****
     *****/
    public void RefreshAllUnits()
    {
        List<List<GameObject>> cardZoneList = new List<List<GameObject>>
        {
            pMan.PlayZoneCards,
            enMan.PlayZoneCards,
        };

        foreach (List<GameObject> cards in cardZoneList)
            foreach (GameObject card in cards)
                GetUnitDisplay(card).IsExhausted = false;
    }
    #endregion

    #region DAMAGE HANDLING
    /******
     * *****
     * ****** ATTACK
     * *****
     *****/
    public void Attack(GameObject attacker, GameObject defender)
    {
        evMan.PauseDelayedActions(true);

        string logEntry = "";
        if (attacker.CompareTag(CardManager.PLAYER_CARD))
        {
            logEntry += "<b><color=\"green\">";

            if (gMan.IsTutorial && pMan.EnergyPerTurn == 2) // TUTORIAL!
            {
                if (pMan.HeroPowerUsed) gMan.Tutorial_Tooltip(6);
                else return;
            }
        }
        else logEntry += "<b><color=\"red\">";
        logEntry += GetUnitDisplay(attacker).CardName + "</b></color> ";
        
        logEntry += "attacked ";
        if (IsUnitCard(defender))
        {
            if (defender.CompareTag(CardManager.PLAYER_CARD)) logEntry += "<b><color=\"green\">";
            else logEntry += "<b><color=\"red\">";
            logEntry += GetUnitDisplay(defender).CardName + "</b></color>.";
        }
        else
        {
            if (attacker.CompareTag(CardManager.PLAYER_CARD)) logEntry += "the enemy hero.";
            else logEntry += "your hero.";
        }
        uMan.CombatLogEntry(logEntry);

        GetUnitDisplay(attacker).IsExhausted = true;
        if (CardManager.GetAbility(attacker, CardManager.ABILITY_STEALTH))
            GetUnitDisplay(attacker).RemoveCurrentAbility(CardManager.ABILITY_STEALTH);

        if (!CardManager.GetAbility(attacker, CardManager.ABILITY_RANGED))
            anMan.UnitAttack(attacker, defender, IsUnitCard(defender));
        else
        {
            auMan.PlayAttackSound(attacker);
            efMan.CreateEffectRay(attacker.transform.position, defender,
                () => RangedAttackRay(), efMan.DamageRayColor, EffectRay.EffectRayType.RangedAttack);
        }

        if (pMan.IsMyTurn) evMan.NewDelayedAction(() => caMan.SelectPlayableCards(), 0);

        void RangedAttackRay()
        {
            Strike(attacker, defender, true, false);
            evMan.PauseDelayedActions(false);
        }
    }

    /******
     * *****
     * ****** STRIKE
     * *****
     *****/
    public void Strike(GameObject striker, GameObject defender, bool isCombat, bool isMelee)
    {
        bool strikerDestroyed;
        //bool defenderDealtDamage; // No current use

        // COMBAT
        if (isCombat)
        {
            DealDamage(striker, defender, 
                out bool strikerDealtDamage, out bool defenderDestroyed, isMelee);

            if (IsUnitCard(defender))
            {
                if (!CardManager.GetAbility(striker, CardManager.ABILITY_RANGED))
                    DealDamage(defender, striker, out _, out strikerDestroyed, false);
                else
                {
                    //defenderDealtDamage = false;
                    strikerDestroyed = false;
                }

                if (!strikerDestroyed && CardManager.GetTrigger
                    (striker, CardManager.TRIGGER_DEATHBLOW))
                {
                    if (defenderDestroyed)
                        caMan.TriggerUnitAbility(striker, CardManager.TRIGGER_DEATHBLOW);
                }
                if (!defenderDestroyed && CardManager.GetTrigger
                    (defender, CardManager.TRIGGER_DEATHBLOW))
                {
                    if (strikerDestroyed)
                        caMan.TriggerUnitAbility(defender, CardManager.TRIGGER_DEATHBLOW);
                }
            }
            else if (!defenderDestroyed && strikerDealtDamage)
            {
                string player;
                if (striker.CompareTag(CardManager.PLAYER_CARD)) player = GameManager.ENEMY;
                else player = GameManager.PLAYER;
                caMan.TriggerPlayedUnits(CardManager.TRIGGER_RETALIATE, player);

                // Trigger Infiltrate BEFORE Retaliate, can cause Retaliate sources to be destroyed before triggering.
                if (CardManager.GetTrigger(striker, CardManager.TRIGGER_INFILTRATE))
                    caMan.TriggerUnitAbility(striker, CardManager.TRIGGER_INFILTRATE);
            }

            if (!(!IsUnitCard(defender) && defenderDestroyed))
                evMan.NewDelayedAction(() => uMan.UpdateEndTurnButton(), 0);
        }
        // STRIKE EFFECTS // no current use
        /*
        else
        {
            DealDamage(striker, defender,
                out bool attackerDealtDamage, out bool defenderDestroyed);

            // delay
            if (IsUnitCard(defender))
            {
                if (defenderDestroyed &&
                CardManager.GetTrigger(striker, CardManager.TRIGGER_DEATHBLOW))
                    DeathblowTrigger(striker);
            }
            else if (attackerDealtDamage &&
                CardManager.GetTrigger(striker, CardManager.TRIGGER_INFILTRATE))
                InfiltrateTrigger(striker);
        }
        */

        void DealDamage(GameObject striker, GameObject defender,
            out bool dealtDamage, out bool defenderDestroyed, bool isMeleeAttacker)
        {
            UnitCardDisplay ucd = GetUnitDisplay(striker);
            int power = ucd.CurrentPower;

            TakeDamage(defender, power, out dealtDamage, out defenderDestroyed, isMeleeAttacker);

            // Poisonous
            if (IsUnitCard(defender))
            {
                UnitCardDisplay defUcd = GetUnitDisplay(defender);
                if (dealtDamage && !defenderDestroyed)
                {
                    if (CardManager.GetAbility(striker, CardManager.ABILITY_POISONOUS))
                        defUcd.AddCurrentAbility(efMan.PoisonAbility);
                }
            }
        }
    }

    /******
     * *****
     * ****** TAKE_DAMAGE
     * *****
     *****/
    public void TakeDamage(GameObject target, int damageValue, out bool wasDamaged,
        out bool wasDestroyed, bool isMeleeAttacker)
    {
        wasDamaged = false;
        wasDestroyed = false;

        if (damageValue < 1) return;

        anMan.ShakeCamera(AnimationManager.Bump_Light);
        //anMan.CreateParticleSystem(target, ParticleSystemHandler.ParticlesType.Damage, 1); // TESTING

        int targetValue;
        int newTargetValue;
        if (IsUnitCard(target)) targetValue = GetUnitDisplay(target).CurrentHealth;
        else if (target == pMan.HeroObject) targetValue = pMan.CurrentHealth;
        else if (target == enMan.HeroObject) targetValue = enMan.CurrentHealth;
        else
        {
            Debug.LogError("INVALID TARGET!");
            return;
        }

        if (targetValue < 1) return; // Don't deal damage to targets with 0 health
        newTargetValue = targetValue - damageValue;

        // Damage to heroes
        if (target == pMan.HeroObject)
        {
            pMan.DamageTakenTurn += damageValue;
            pMan.CurrentHealth = newTargetValue;

            anMan.ModifyHeroHealthState(target, -damageValue);
            wasDamaged = true;
        }
        else if (target == enMan.HeroObject)
        {
            enMan.DamageTakenTurn += damageValue;
            enMan.CurrentHealth = newTargetValue;

            anMan.ModifyHeroHealthState(target, -damageValue);
            wasDamaged = true;
        }
        // Damage to Units
        else
        {
            if (CardManager.GetAbility(target, CardManager.ABILITY_FORCEFIELD))
            {
                GetUnitDisplay(target).AbilityTriggerState(CardManager.ABILITY_FORCEFIELD);
                GetUnitDisplay(target).RemoveCurrentAbility(CardManager.ABILITY_FORCEFIELD);
                return; // Unnecessary?
            }
            else
            {
                if (CardManager.GetAbility(target, CardManager.ABILITY_ARMORED))
                {
                    GetUnitDisplay(target).AbilityTriggerState(CardManager.ABILITY_ARMORED);
                    int newDamage = damageValue - 1;
                    if (newDamage < 1) return;
                    newTargetValue = targetValue - newDamage;
                }

                int newHealth = newTargetValue;
                if (newHealth < 0) newHealth = 0;
                int damageTaken = targetValue - newHealth;

                GetUnitDisplay(target).CurrentHealth = newTargetValue;
                anMan.UnitTakeDamageState(target, damageTaken, isMeleeAttacker);
                wasDamaged = true;
            }
        }

        if (newTargetValue < 1)
        {
            wasDestroyed = true;

            if (IsUnitCard(target)) DestroyUnit(target);
            else
            {
                anMan.ShakeCamera(EZCameraShake.CameraShakePresets.Earthquake);
                anMan.SetAnimatorBool(target, "IsDestroyed", true);
                bool playerWins;
                if (target == pMan.HeroObject) playerWins = false;
                else playerWins = true;
                gMan.EndCombat(playerWins);
            }
        }
    }

    /******
     * *****
     * ****** HEAL_DAMAGE
     * *****
     *****/
    public void HealDamage(GameObject target, HealEffect healEffect)
    {
        int healingValue = healEffect.Value;
        int targetValue;
        int maxValue;
        int newTargetValue;

        if (target == pMan.HeroObject)
        {
            targetValue = pMan.CurrentHealth;
            maxValue = pMan.MaxHealth;
        }
        else if (target == enMan.HeroObject)
        {
            targetValue = enMan.CurrentHealth;
            maxValue = enMan.MaxHealth;
        }
        else
        {
            targetValue = GetUnitDisplay(target).CurrentHealth;
            maxValue = GetUnitDisplay(target).MaxHealth;
        }

        if (targetValue < 1) return; // Don't heal destroyed units or heroes

        if (healEffect.HealFully) newTargetValue = maxValue;
        else
        {
            newTargetValue = targetValue + healingValue;
            if (newTargetValue > maxValue) newTargetValue = maxValue;

            if (newTargetValue < targetValue)
            {
                Debug.LogError("NEW HEALTH < PREVIOUS HEALTH!");
                return;
            }
        }

        auMan.StartStopSound("SFX_StatPlus");
        int healthChange = newTargetValue - targetValue;

        if (IsUnitCard(target))
        {
            GetUnitDisplay(target).CurrentHealth = newTargetValue;
            anMan.UnitStatChangeState(target, 0, healthChange, true);
        }
        else
        {
            if (target == pMan.HeroObject) pMan.CurrentHealth = newTargetValue;
            else if (target == enMan.HeroObject) enMan.CurrentHealth = newTargetValue;
            anMan.ModifyHeroHealthState(target, healthChange);
        }
    }

    /******
     * *****
     * ****** DESTROY_UNIT [PLAY >>> DISCARD]
     * *****
     *****/
    public void DestroyUnit(GameObject card)
    {
        if (card == null)
        {
            Debug.LogError("CARD IS NULL!");
            return;
        }

        HeroManager sourceHMan = GetSourceHero(card);
        HeroManager enemyHMan = GetSourceHero(card, true);

        if (!efMan.UnitsToDestroy.Contains(card)) efMan.UnitsToDestroy.Add(card);
        DestroyFX();

        // Remove from lists immediately for PlayCard effects triggered from Revenge
        sourceHMan.PlayZoneCards.Remove(card);

        evMan.NewDelayedAction(() => Destroy(), 0.75f, true);
        evMan.NewDelayedAction(() => DestroyTriggers(), 0, true);

        void DestroyFX()
        {
            if (card == null)
            {
                Debug.LogError("CARD IS NULL!");
                return;
            }
            Sound deathSound = GetUnitDisplay(card).UnitCard.UnitDeathSound;
            anMan.DestroyUnitCardState(card);
        }
        void DestroyTriggers()
        {
            if (card == null)
            {
                Debug.LogError("CARD IS NULL!");
                return;
            }

            if (sourceHMan == null || enemyHMan == null)
            {
                Debug.LogError("MANAGERS ARE NULL!");
                return;
            }

            // Revenge
            caMan.TriggerUnitAbility(card, CardManager.TRIGGER_REVENGE);

            // Marked
            if (CardManager.GetAbility(card, CardManager.ABILITY_MARKED))
                evMan.NewDelayedAction(() => MarkedTrigger(), 0.5f, true);

            // Ally Destroyed
            efMan.TriggerModifiers_SpecialTrigger
                (ModifierAbility.TriggerType.AllyDestroyed, sourceHMan.PlayZoneCards);

            // Enemy Destroyed
            efMan.TriggerModifiers_SpecialTrigger
                (ModifierAbility.TriggerType.EnemyDestroyed, enemyHMan.PlayZoneCards);

            // Marked Enemy Destroyed
            if (CardManager.GetAbility(card, CardManager.ABILITY_MARKED))
            {
                efMan.TriggerModifiers_SpecialTrigger
                    (ModifierAbility.TriggerType.MarkedEnemyDestroyed, enemyHMan.PlayZoneCards);
            }
            // Poisoned Enemy Destroyed
            if (CardManager.GetAbility(card, CardManager.ABILITY_POISONED))
            {
                efMan.TriggerModifiers_SpecialTrigger
                    (ModifierAbility.TriggerType.PoisonedEnemyDestroyed, enemyHMan.PlayZoneCards);
            }

            void MarkedTrigger()
            {
                GetUnitDisplay(card).AbilityTriggerState(CardManager.ABILITY_MARKED);
                caMan.DrawCard(GetSourceHero(card, true));
            }
        }
        void Destroy()
        {
            if (card == null)
            {
                Debug.LogError("CARD IS NULL!");
                return;
            }

            if (sourceHMan == null || enemyHMan == null)
            {
                Debug.LogError("MANAGERS ARE NULL!");
                return;
            }

            efMan.UnitsToDestroy.Remove(card);

            UnitCard unitCard = card.GetComponent<CardDisplay>().CardScript as UnitCard;
            card.GetComponent<CardZoom>().DestroyZoomPopups();
            AudioManager.Instance.StartStopSound(null, unitCard.UnitDeathSound);

            DiscardCard(sourceHMan.DiscardZoneCards);

            if (pMan.IsMyTurn) caMan.SelectPlayableCards();

            void DiscardCard(List<Card> cardZone)
            {
                if (unitCard.BanishAfterPlay) caMan.HideCard(card);
                else
                {
                    card.GetComponent<CardDisplay>().ResetCard();
                    cardZone.Add(caMan.HideCard(card));
                }
            }
        }
    }
    #endregion
    #endregion
}
