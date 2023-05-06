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
    [Header("MULLIGAN EFFECT"), SerializeField]
    private EffectGroup mulliganEffect;
    [Header("COGNITIVE MAGNIFIER EFFECT"), SerializeField]
    private EffectGroup cognitiveMagnifierEffect;
    [Header("CRYOCHAMBER EFEFCT"), SerializeField]
    private EffectGroup cryochamberEffect;

    public const string CARD_ZONE = "CardZone";
    #endregion

    #region PROPERTIES
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
    public static UnitCardDisplay GetUnitDisplay(GameObject card)
    {
        if (card == null)
        {
            Debug.LogWarning("CARD IS NULL!");
            return null;
        }

        if (!card.TryGetComponent(out UnitCardDisplay ucd))
        {
            Debug.LogError("TARGET IS NOT UNIT CARD!");
            return null;
        }

        return ucd;
    }
    public static bool IsUnitCard(GameObject target)
    {
        if (target == null)
        {
            Debug.LogWarning("TARGET IS NULL!");
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
    public static bool IsDamaged(GameObject target)
    {
        if (target.TryGetComponent(out UnitCardDisplay ucd))
            return ucd.CurrentHealth < ucd.MaxHealth;

        if (target.TryGetComponent(out HeroDisplay _))
        {
            var hMan = HeroManager.GetSourceHero(target);
            return hMan.CurrentHealth < hMan.MaxHealth;
        }

        Debug.LogError("INVALID TARGET!");
        return false;
    }
    public void RefreshAllUnits()
    {
        List<List<GameObject>> cardZoneList = new()
        {
            Managers.P_MAN.PlayZoneCards,
            Managers.EN_MAN.PlayZoneCards,
        };

        foreach (var cards in cardZoneList)
            foreach (var card in cards)
                GetUnitDisplay(card).IsExhausted = false;
    }

    /******
     * *****
     * ****** GET_LOWEST_HEALTH_UNIT
     * *****
     *****/
    public GameObject GetLowestHealthUnit(List<GameObject> unitList, bool targetsEnemy)
    {
        if (unitList.Count < 1) return null;
        int lowestHealth = int.MaxValue;
        List<GameObject> lowestHealthUnits = new();

        foreach (var unit in unitList)
        {
            if (!IsUnitCard(unit)) continue;

            if (targetsEnemy && CardManager.GetAbility(unit, CardManager.ABILITY_WARD)) continue;

            int health = GetUnitDisplay(unit).CurrentHealth;
            if (health < 1 || Managers.EF_MAN.UnitsToDestroy.Contains(unit)) continue;

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
        int highestPower = int.MinValue;
        List<GameObject> highestPowerUnits = new();

        foreach (var unit in unitList)
        {
            if (!IsUnitCard(unit)) continue;

            if (targetsEnemy && CardManager.GetAbility(unit, CardManager.ABILITY_WARD)) continue;

            int health = GetUnitDisplay(unit).CurrentHealth;
            if (health < 1 || Managers.EF_MAN.UnitsToDestroy.Contains(unit)) continue;

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
            List<GameObject> highestHealthUnits = new();
            int highestHealth = int.MinValue;

            foreach (var unit in highestPowerUnits)
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
        int lowestPower = int.MaxValue;
        List<GameObject> lowestPowerUnits = new();

        foreach (var unit in unitList)
        {
            if (!IsUnitCard(unit)) continue;

            if (targetsEnemy && CardManager.GetAbility(unit, CardManager.ABILITY_WARD)) continue;

            int health = GetUnitDisplay(unit).CurrentHealth;
            if (health < 1 || Managers.EF_MAN.UnitsToDestroy.Contains(unit)) continue;

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
            List<GameObject> lowestHealthUnits = new();
            int lowestHealth = int.MaxValue;

            foreach (var unit in lowestPowerUnits)
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
     * ****** GET_HIGHEST_COST
     * *****
     *****/
    public GameObject GetHighestCostTarget(List<GameObject> targets)//, bool targetsEnemy)
    {
        if (targets.Count < 1) return null;
        int highestCost = int.MinValue;
        List<GameObject> highestCostTargets = new();

        foreach (var target in targets)
        {
            var cd = target.GetComponent<CardDisplay>();

            /* Unnecessary, method is only used on cards in hand
            if (cd is UnitCardDisplay ucd)
            {
                if ((targetsEnemy && CardManager.GetAbility(target, CardManager.ABILITY_WARD)) ||
                    (ucd.CurrentHealth < 1) || Managers.EF_MAN.UnitsToDestroy.Contains(target)) continue;
            }
            */

            int cost = cd.CurrentEnergyCost;

            if (cost > highestCost)
            {
                highestCost = cost;
                highestCostTargets.Clear();
                highestCostTargets.Add(target);
            }
            else if (cost == highestCost) highestCostTargets.Add(target);
        }

        if (highestCostTargets.Count < 1) return null;

        if (highestCostTargets.Count > 1)
        {
            int randomIndex = Random.Range(0, highestCostTargets.Count);
            return highestCostTargets[randomIndex];
        }
        else return highestCostTargets[0];
    }

    /******
     * *****
     * ****** GET_LOWEST_COST
     * *****
     *****/
    public GameObject GetLowestCostTarget(List<GameObject> targets)//, bool targetsEnemy)
    {
        if (targets.Count < 1) return null;
        int lowestCost = int.MaxValue;
        List<GameObject> lowestCostTargets = new();

        foreach (var target in targets)
        {
            var cd = target.GetComponent<CardDisplay>();

            /* Unnecessary, method is only used on cards in hand
            if (cd is UnitCardDisplay ucd)
            {
                if ((targetsEnemy && CardManager.GetAbility(target, CardManager.ABILITY_WARD)) ||
                    (ucd.CurrentHealth < 1) || Managers.EF_MAN.UnitsToDestroy.Contains(target)) continue;
            }
            */

            int cost = cd.CurrentEnergyCost;

            if (cost < lowestCost)
            {
                lowestCost = cost;
                lowestCostTargets.Clear();
                lowestCostTargets.Add(target);
            }
            else if (cost == lowestCost) lowestCostTargets.Add(target);
        }

        if (lowestCostTargets.Count < 1) return null;

        if (lowestCostTargets.Count > 1)
        {
            int randomIndex = Random.Range(0, lowestCostTargets.Count);
            return lowestCostTargets[randomIndex];
        }
        else return lowestCostTargets[0];
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
            if (attacker.CompareTag(defender.tag) || 
                defender == HeroManager.GetSourceHero(attacker)) return false;
        }
        else
        {
            if (!preCheck && !ignoreDefender)
            {
                Debug.LogError("DEFENDER IS NULL!");
                return false;
            }
        }

        // TUTORIAL! <<< WATCH >>>
        if (Managers.G_MAN.IsTutorial && Managers.P_MAN.EnergyPerTurn == 2 && !preCheck &&
            (!Managers.P_MAN.HeroPowerUsed || !defenderIsUnit)) return false;

        var atkUcd = GetUnitDisplay(attacker);
        if (atkUcd.IsExhausted)
        {
            if (preCheck && !ignoreDefender)
            {
                Managers.U_MAN.CreateFleetingInfoPopup("Exhausted units can't attack!");
                SFX_Error();
            }
            return false;
        }
        else if (atkUcd.CurrentPower < 1)
        {
            if (preCheck && !ignoreDefender)
            {
                Managers.U_MAN.CreateFleetingInfoPopup("Units with 0 power can't attack!");
                SFX_Error();
            }
            return false;
        }

        if (defender == null) return true; // For DragDrop() and SelectPlayableCards()

        if (defender.TryGetComponent(out UnitCardDisplay defUcd))
        {
            if (Managers.EN_MAN.HandZoneCards.Contains(defender)) return false; // Unnecessary, already checked in CardSelect
            if (defUcd.CurrentHealth < 1 || Managers.EF_MAN.UnitsToDestroy.Contains(defender)) return false; // Destroyed units that haven't left play yet
            if (CardManager.GetAbility(defender, CardManager.ABILITY_STEALTH))
            {
                if (!preCheck)
                {
                    Managers.U_MAN.CreateFleetingInfoPopup("Units with Stealth can't be attacked!");
                    SFX_Error();
                }
                return false;
            }
        }

        if (!(defenderIsUnit && CardManager.GetAbility(defender, CardManager.ABILITY_DEFENDER))) // TESTING
        {
            HeroManager.GetSourceHero(attacker, out HeroManager hMan_Enemy);

            foreach (var unit in hMan_Enemy.PlayZoneCards)
            {
                if (unit == defender) continue;

                if (CardManager.GetAbility(unit, CardManager.ABILITY_DEFENDER) &&
                    !CardManager.GetAbility(unit, CardManager.ABILITY_STEALTH))
                {
                    if (!preCheck)
                    {
                        Managers.U_MAN.CreateFleetingInfoPopup("Enemies with Defender must be attacked!");
                        SFX_Error();
                    }
                    return false;
                }
            }
        }

        return true;

        static void SFX_Error() => Managers.AU_MAN.StartStopSound("SFX_Error");
    }
    #endregion

    #region BASIC COMBAT
    /******
     * *****
     * ****** START_COMBAT
     * *****
     *****/
    public void StartCombat()
    {
        var pm = Managers.P_MAN;
        var em = Managers.EN_MAN;
        var um = Managers.U_MAN;
        var vm = Managers.EV_MAN;
        var cm = Managers.CA_MAN;
        var gm = Managers.G_MAN;

        var playerData = PlayerData.SavedPlayerData;
        if (playerData != null) PlayerData.SavedPlayerData = null;

        um.StartCombatScene();
        CardZone = GameObject.Find(CARD_ZONE);
        if (CardZone == null) Debug.LogError("CARD ZONE IS NULL!");

        foreach (var hMan in new List<HeroManager>() { pm, em })
        {
            if (playerData == null) hMan.ResetForCombat(); // !!! Load From Save !!!

            hMan.HandZone = SetZone(hMan.HAND_ZONE_TAG);
            hMan.PlayZone = SetZone(hMan.PLAY_ZONE_TAG);
            hMan.ActionZone = SetZone(hMan.ACTION_ZONE_TAG);
            hMan.HeroObject = SetZone(hMan.HERO_TAG);

            static GameObject SetZone(string tag)
            {
                var zone = GameObject.Find(tag);
                if (zone == null) Debug.LogError($"{tag} IS NULL!");
                return zone;
            }

            um.SelectTarget(hMan.HeroObject, UIManager.SelectionType.Disabled);

            if (playerData != null) // !!! Load From Save !!!
            {
                // HandZone
                PlaceCards(hMan.HandZoneCards, hMan.HandZone, false);
                // PlayZone
                PlaceCards(hMan.PlayZoneCards, hMan.PlayZone, true);

                continue;

                void PlaceCards(List<GameObject> cards, GameObject zone, bool isPlayed)
                {
                    foreach (var card in cards)
                    {
                        if (card == null)
                        {
                            Debug.LogError("LOADED CARD IS NULL!");
                            continue;
                        }

                        // Place card object
                        card.transform.SetParent(CardZone.transform);
                        // Place container object
                        cm.ChangeCardZone(card, zone, CardManager.ZoneChangeType.LoadFromSave);
                        // Set IsPlayed in DragDrop
                        card.GetComponent<DragDrop>().IsPlayed = isPlayed;
                    }
                }
            }

            var hd = hMan.HeroObject.GetComponent<HeroDisplay>();
            hd.HeroBase.SetActive(false);
            hd.HeroStats.SetActive(false);
            hd.HeroNameObject.SetActive(false);
        }

        if (playerData != null) // !!! Load From Save !!!
        {
            // Enemy Hero
            int activeEHIndex = Managers.G_MAN.ActiveNPCHeroes.FindIndex(x => x.HeroName == playerData.EnemyHero);
            if (activeEHIndex != -1) em.HeroScript = Managers.G_MAN.ActiveNPCHeroes[activeEHIndex];
            else Debug.LogError("ENEMY HERO NOT FOUND!");

            // Hero Turn
            bool isPlayerTurn;
            if (playerData.HeroTurnCombat == PlayerData.TURN_PLAYER) isPlayerTurn = true;
            else if (playerData.HeroTurnCombat == PlayerData.TURN_ENEMY) isPlayerTurn = false;
            else
            {
                Debug.LogError("INVALID HERO TURN!");
                isPlayerTurn = false;
            }
            pm.IsMyTurn = isPlayerTurn;
            em.IsMyTurn = !isPlayerTurn;

            // Current Health
            pm.CurrentHealth = playerData.CurrentHealth_Player;
            em.CurrentHealth = playerData.CurrentHealth_Enemy;

            // Energy Per Turn
            pm.EnergyPerTurn = playerData.EnergyPerTurn_Player;
            em.EnergyPerTurn = playerData.EnergyPerTurn_Enemy;

            // Current Energy
            pm.CurrentEnergy = playerData.CurrentEnergy_Player;
            em.CurrentEnergy = playerData.CurrentEnergy_Enemy;

            // Player Hero Ultimate Progress
            pm.HeroUltimateProgress = playerData.HeroUltimateProgress;

            // Enemy Hero Surge Progress
            em.SurgeProgress(true); // TESTING

            // Damage Taken
            pm.DamageTaken_ThisTurn = playerData.DamageTaken_ThisTurn_Player;
            em.DamageTaken_ThisTurn = playerData.DamageTaken_ThisTurn_Enemy;

            // Display Heroes
            DisplayHeroes();

            // Display Skybar
            StartCoroutine(Managers.AN_MAN.ShowSkyBarChildren(1));

            // Turn Popup
            CombatAudio();
            if (pm.IsMyTurn) um.CreateTurnPopup(true);

            // Select Playable Cards
            cm.SelectPlayableCards(em.IsMyTurn);

            // CONTINUE ENEMY TURN
            if (em.IsMyTurn) em.ContinueEnemyTurn();

            return;
        }

        um.EndTurnButton.SetActive(false);
        um.CombatLog.SetActive(false);

        // ENEMY MANAGER
        var enemyHero = Managers.D_MAN.EngagedHero as EnemyHero;
        if (enemyHero == null)
        {
            Debug.LogError("ENEMY HERO IS NULL!");
            return;
        }

        // ENEMY HERO
        em.HeroScript = enemyHero;
        em.CurrentHealth = gm.IsTutorial ? GameManager.TUTORIAL_STARTING_HEALTH : em.MaxHealth;

        int energyPerTurn = gm.IsTutorial ? 0 : gm.GetEnemyStartingEnergy(DifficultyLevel);
        em.EnergyPerTurn = energyPerTurn;
        em.CurrentEnergy = 0;
        em.DamageTaken_ThisTurn = 0;
        em.AlliesDestroyed_ThisTurn = 0;
        em.TurnNumber = 0;

        CombatAudio(); // enemyMan.HeroScript needs to be set first

        // PLAYER MANAGER
        pm.CurrentHealth = pm.CurrentHealth; // Display current health
        pm.EnergyPerTurn = 0;
        pm.CurrentEnergy = 0;
        pm.HeroUltimateProgress = 0;
        pm.DamageTaken_ThisTurn = 0;
        pm.AlliesDestroyed_ThisTurn = 0;
        foreach (var item in pm.HeroItems) item.IsUsed = false;
        pm.TurnNumber = 0;

        // UPDATE DECKS
        cm.UpdateDeck(pm);
        cm.UpdateDeck(em);

        // DISPLAY HEROES
        DisplayHeroes();

        // SCHEDULE ACTIONS
        vm.NewDelayedAction(() => Managers.AN_MAN.CombatIntro(), 1);
        vm.NewDelayedAction(() => CombatStart(), 1);
        vm.NewDelayedAction(() => StartCombatTurn(Managers.P_MAN, true), 2);

        void CombatStart()
        {
            um.CombatLogEntry($"<b>{TextFilter.Clrz_grn(pm.HeroScript.HeroShortName, false)}" +
                $" VS {TextFilter.Clrz_red(em.HeroScript.HeroName, false)}</b>");

            cm.ShuffleDeck(pm, false);
            cm.ShuffleDeck(em, false);

            for (int i = 0; i < GameManager.START_HAND_SIZE; i++)
                vm.NewDelayedAction(() => AllDraw(), 0.5f);

            // TUTORIAL!
            if (gm.IsTutorial)
            {
                vm.NewDelayedAction(() => um.CreateTutorialActionPopup(), 0);
                vm.NewDelayedAction(() => vm.PauseDelayedActions(true), 0);
                vm.NewDelayedAction(() => gm.Tutorial_Tooltip(1), 0);
            }
            gm.ResolveReputationEffects(1); // REPUTATION EFFECTS [RESOLVE ORDER = 1]
            PlayStartingUnits();
            vm.NewDelayedAction(() => Mulligan_Player(), 0.5f);
            vm.NewDelayedAction(() => em.Mulligan(), 0.5f);
            gm.ResolveReputationEffects(2); // REPUTATION EFFECTS [RESOLVE ORDER = 2]
            gm.ResolveReputationEffects(3); // REPUTATION EFFECTS [RESOLVE ORDER = 3]

            string cognitiveMagnifier = "Cognitive Magnifier";
            if (pm.GetAugment(cognitiveMagnifier))
            {
                vm.NewDelayedAction(() => CognitiveMagnifierEffect(), 0.25f);

                void CognitiveMagnifierEffect()
                {
                    Managers.AN_MAN.TriggerAugment(cognitiveMagnifier);
                    Managers.EF_MAN.StartEffectGroupList(new List<EffectGroup>
                    { cognitiveMagnifierEffect }, pm.HeroObject);
                }
            }

            string cryochamber = "Cryochamber";
            if (pm.GetAugment(cryochamber))
            {
                vm.NewDelayedAction(() => CryochamberEffect(), 0.25f);

                void CryochamberEffect()
                {
                    Managers.AN_MAN.TriggerAugment(cryochamber);
                    Managers.EF_MAN.StartEffectGroupList(new List<EffectGroup>
                    { cryochamberEffect }, pm.HeroObject);
                }
            }

            // TUTORIAL!
            if (gm.IsTutorial) vm.NewDelayedAction(() => gm.Tutorial_Tooltip(2), 0);

            void AllDraw()
            {
                cm.DrawCard(pm);
                cm.DrawCard(em);
            }

            void PlayStartingUnits()
            {
                var startingUnits = (em.HeroScript as EnemyHero).Reinforcements[em.ReinforcementGroup].StartingUnits;
                foreach (UnitCard card in startingUnits)
                {
                    var newCard = cm.NewCardInstance(card) as UnitCard;
                    vm.NewDelayedAction(() =>
                    Managers.EF_MAN.PlayCreatedUnit(newCard, false, new List<Effect>(), em.HeroObject), 0.5f);
                }
            }
        }

        void Mulligan_Player() =>
            Managers.EF_MAN.StartEffectGroupList(new List<EffectGroup> { mulliganEffect }, pm.HeroObject);

        void DisplayHeroes()
        {
            pm.HeroObject.GetComponent<HeroDisplay>().HeroScript = pm.HeroScript;
            em.HeroObject.GetComponent<HeroDisplay>().HeroScript = em.HeroScript;
        }

        void CombatAudio()
        {
            string soundtrack = (em.HeroScript as EnemyHero).IsBoss ?
                "Soundtrack_CombatBoss1" : "Soundtrack_Combat1";
            Managers.AU_MAN.StartStopSound(soundtrack, null, AudioManager.SoundType.Soundtrack);
            FunctionTimer.Create(() => Managers.AU_MAN.StartStopSound("SFX_StartCombat1"), 0.15f);
        }
    }

    /******
     * *****
     * ****** END_COMBAT
     * *****
     *****/
    public void EndCombat(bool playerWins)
    {
        if (playerWins && Managers.P_MAN.CurrentHealth > 0) // Make sure the player doesn't win at 0 health (pretty sure it's not possible)
        {
            Managers.AU_MAN.StartStopSound(null, Managers.EN_MAN.HeroScript.HeroLose);
            FunctionTimer.Create(() =>
            Managers.AU_MAN.StartStopSound(null, Managers.P_MAN.HeroScript.HeroWin), 2f);
        }
        else
        {
            Managers.AU_MAN.StartStopSound(null, Managers.P_MAN.HeroScript.HeroLose);
            FunctionTimer.Create(() =>
            Managers.AU_MAN.StartStopSound(null, Managers.EN_MAN.HeroScript.HeroWin), 2f);

            SaveLoad.DeletePlayerData(); // Delete saved PlayerData on defeat
        }

        Managers.EV_MAN.ClearDelayedActions();
        Managers.U_MAN.PlayerIsTargetting = false;
        Managers.EF_MAN.EffectsResolving = false;
        Managers.P_MAN.IsMyTurn = false;

        // Items
        foreach (var item in Managers.P_MAN.HeroItems) item.IsUsed = false;

        // Created Cards Played
        Managers.P_MAN.ResetForCombat();
        Managers.EN_MAN.ResetForCombat();

        FunctionTimer.Create(() => Managers.U_MAN.CreateCombatEndPopup(playerWins), 2f);
    }

    /******
     * *****
     * ****** START_COMBAT_TURN
     * *****
     *****/
    private void StartCombatTurn(HeroManager hero, bool isFirstTurn = false)
    {
        bool isPlayerTurn = hero == Managers.P_MAN;
        string logText = "\n" + (isPlayerTurn ? "[Your Turn]" : "[Enemy Turn]");

        Managers.EV_MAN.NewDelayedAction(() => TurnPopup(), 0);
        Managers.EV_MAN.NewDelayedAction(() => Managers.U_MAN.CombatLogEntry(logText), 0);

        if (isPlayerTurn)
        {
            Managers.EV_MAN.NewDelayedAction(() => PlayerTurnStart(), 2);
            Managers.EV_MAN.NewDelayedAction(() => TurnDraw(), 0.5f);

            string synapticStabilizer = "Synaptic Stabilizer";

            if (isFirstTurn && Managers.P_MAN.GetAugment(synapticStabilizer))
                Managers.EV_MAN.NewDelayedAction(() => SynapticStabilizerEffect(), 0.5f);

            void SynapticStabilizerEffect()
            {
                Managers.P_MAN.CurrentEnergy++;
                Managers.AN_MAN.ModifyHeroEnergyState(1, Managers.P_MAN.HeroObject);
                Managers.AN_MAN.TriggerAugment(synapticStabilizer);
                Managers.CA_MAN.SelectPlayableCards();
            }

            void PlayerTurnStart()
            {
                Managers.P_MAN.IsMyTurn = true;
                Managers.EN_MAN.IsMyTurn = false;
                Managers.P_MAN.HeroPowerUsed = false;

                int startEnergy = Managers.P_MAN.CurrentEnergy;
                if (Managers.P_MAN.EnergyPerTurn < Managers.P_MAN.MaxEnergy) Managers.P_MAN.EnergyPerTurn++;
                Managers.P_MAN.CurrentEnergy = Managers.P_MAN.EnergyPerTurn;

                int energyChange = Managers.P_MAN.CurrentEnergy - startEnergy;
                Managers.AN_MAN.ModifyHeroEnergyState(energyChange, Managers.P_MAN.HeroObject);

                Managers.P_MAN.TurnNumber++;

                // TUTORIAL!
                if (Managers.G_MAN.IsTutorial && Managers.P_MAN.EnergyPerTurn == 2) Managers.G_MAN.Tutorial_Tooltip(4);
            }

            void TurnDraw()
            {
                Managers.CA_MAN.DrawCard(Managers.P_MAN);
                Managers.CA_MAN.SelectPlayableCards();
            }
        }

        if (isPlayerTurn) Managers.EV_MAN.NewDelayedAction(() =>
        Managers.CA_MAN.TriggerPlayedUnits(CardManager.TRIGGER_TURN_START, hero), 0);
        else
        {
            Managers.P_MAN.IsMyTurn = false;
            Managers.EN_MAN.IsMyTurn = true;
            Managers.EV_MAN.NewDelayedAction(() => Managers.EN_MAN.StartEnemyTurn(), 1);
        }

        void TurnPopup()
        {
            Managers.AU_MAN.StartStopSound("SFX_NextTurn");
            Managers.U_MAN.CreateTurnPopup(isPlayerTurn);
        }
    }

    /******
     * *****
     * ****** END__COMBAT_TURN
     * *****
     *****/
    public void EndCombatTurn(HeroManager hero)
    {
        if (!Managers.G_MAN.IsTutorial && !Managers.G_MAN.IsCombatTest) GameLoader.SaveGame(); // Consider limiting this call by checking the time in between calls

        Managers.EV_MAN.NewDelayedAction(() =>
        Managers.CA_MAN.TriggerPlayedUnits(CardManager.TRIGGER_TURN_END, hero), 0);

        Managers.EV_MAN.NewDelayedAction(() => Managers.CO_MAN.RefreshAllUnits(), 0.5f);
        Managers.EV_MAN.NewDelayedAction(() => RemoveEffects(), 0);
        Managers.EV_MAN.NewDelayedAction(() => ResetTriggerCounts(), 0);
        Managers.EV_MAN.NewDelayedAction(() => Managers.CA_MAN.SelectPlayableCards(), 0); // To reset conditional card costs (i.e. based on units destroyed this turn)

        if (hero == Managers.EN_MAN) Managers.EV_MAN.NewDelayedAction(() => StartCombatTurn(Managers.P_MAN), 0.5f);
        else if (hero == Managers.P_MAN)
        {
            if (Managers.G_MAN.IsTutorial) // TUTORIAL!
                switch (Managers.P_MAN.EnergyPerTurn)
                {
                    case 1:
                        if (Managers.P_MAN.CurrentEnergy > 0) return;
                        else Managers.U_MAN.DestroyInfoPopup(UIManager.InfoPopupType.Tutorial);
                        break;
                    case 2:
                        if (!Managers.P_MAN.HeroPowerUsed || Managers.EN_MAN.PlayZoneCards.Count > 0) return;
                        else Managers.U_MAN.DestroyInfoPopup(UIManager.InfoPopupType.Tutorial);
                        break;
                }

            Managers.P_MAN.IsMyTurn = false;
            Managers.CA_MAN.SelectPlayableCards(true);
            Managers.EV_MAN.NewDelayedAction(() => StartCombatTurn(Managers.EN_MAN), 0.5f);
        }
        else Debug.LogError("INVALID PLAYER!");

        void RemoveEffects()
        {
            Managers.EF_MAN.RemoveTemporaryEffects();
            Managers.EF_MAN.RemoveGiveNextEffects(hero);
            Managers.EF_MAN.RemoveChangeNextCostEffects(hero);
            Managers.EF_MAN.RemoveModifyNextEffects(hero);

            Managers.P_MAN.DamageTaken_ThisTurn = 0;
            Managers.P_MAN.AlliesDestroyed_ThisTurn = 0;

            Managers.EN_MAN.DamageTaken_ThisTurn = 0;
            Managers.EN_MAN.AlliesDestroyed_ThisTurn = 0;
        }
        void ResetTriggerCounts()
        {
            foreach (var unit in Managers.P_MAN.PlayZoneCards) ResetTrigger(unit);
            foreach (var unit in Managers.EN_MAN.PlayZoneCards) ResetTrigger(unit);
            
            void ResetTrigger(GameObject unit)
            {
                var ucd = unit.GetComponent<UnitCardDisplay>();
                foreach (var ca in ucd.CurrentAbilities)
                {
                    if (ca is TriggeredAbility tra)
                    {
                        tra.TriggerCount = 0;
                        ucd.EnableTriggerIcon(tra.AbilityTrigger, true);
                    }
                    else if (ca is ModifierAbility ma)
                    {
                        ma.TriggerCount = 0;
                        ucd.EnableTriggerIcon(null, true);
                    }
                }
            }
        }
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
        Managers.EV_MAN.PauseDelayedActions(true);
        HeroManager hMan_Attacker = HeroManager.GetSourceHero(attacker, out HeroManager hMan_Defender);
        string logEntry = "";

        if (hMan_Attacker == Managers.P_MAN)
        {
            logEntry += "<b><color=\"green\">";

            if (Managers.G_MAN.IsTutorial && Managers.P_MAN.EnergyPerTurn == 2) // TUTORIAL!
            {
                if (Managers.P_MAN.HeroPowerUsed) Managers.G_MAN.Tutorial_Tooltip(6);
                else return;
            }
        }
        else logEntry += "<b><color=\"red\">";
        logEntry += GetUnitDisplay(attacker).CardName + "</b></color> ";

        logEntry += "attacked ";
        if (IsUnitCard(defender))
        {
            if (hMan_Defender == Managers.P_MAN) logEntry += "<b><color=\"green\">";
            else logEntry += "<b><color=\"red\">";
            logEntry += GetUnitDisplay(defender).CardName + "</b></color>.";
        }
        else
        {
            if (hMan_Attacker == Managers.P_MAN) logEntry += "the enemy hero.";
            else logEntry += "your hero.";
        }

        Managers.U_MAN.CombatLogEntry(logEntry);
        GetUnitDisplay(attacker).IsExhausted = true;

        if (CardManager.GetAbility(attacker, CardManager.ABILITY_STEALTH))
            GetUnitDisplay(attacker).RemoveCurrentAbility(CardManager.ABILITY_STEALTH);

        if (!CardManager.GetAbility(attacker, CardManager.ABILITY_RANGED))
            Managers.AN_MAN.UnitAttack(attacker, defender, IsUnitCard(defender));
        else
        {
            Managers.AU_MAN.PlayAttackSound(attacker);
            Managers.EF_MAN.CreateEffectRay(attacker.transform.position, defender,
                () => RangedAttackRay(), Managers.EF_MAN.DamageRayColor, EffectRay.EffectRayType.RangedAttack);
        }

        if (Managers.P_MAN.IsMyTurn) Managers.EV_MAN.NewDelayedAction(() => Managers.CA_MAN.SelectPlayableCards(), 0);

        void RangedAttackRay()
        {
            Strike(attacker, defender, true, false);
            Managers.EV_MAN.PauseDelayedActions(false);
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
            DealDamage(striker, defender, out bool strikerDealtDamage,
                out bool defenderDestroyed, isMelee);

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
                        Managers.CA_MAN.TriggerUnitAbility(striker, CardManager.TRIGGER_DEATHBLOW);
                }
                if (!defenderDestroyed && CardManager.GetTrigger
                    (defender, CardManager.TRIGGER_DEATHBLOW))
                {
                    if (strikerDestroyed)
                        Managers.CA_MAN.TriggerUnitAbility(defender, CardManager.TRIGGER_DEATHBLOW);
                }
            }
            else if (!defenderDestroyed && strikerDealtDamage)
            {
                HeroManager.GetSourceHero(striker, out HeroManager hMan_Enemy);
                Managers.CA_MAN.TriggerPlayedUnits(CardManager.TRIGGER_RETALIATE, hMan_Enemy);

                // Trigger Infiltrate BEFORE Retaliate, can cause Retaliate sources to be destroyed before triggering.
                if (CardManager.GetTrigger(striker, CardManager.TRIGGER_INFILTRATE))
                    Managers.CA_MAN.TriggerUnitAbility(striker, CardManager.TRIGGER_INFILTRATE);
            }

            if (!(!IsUnitCard(defender) && defenderDestroyed))
                Managers.EV_MAN.NewDelayedAction(() => Managers.U_MAN.UpdateEndTurnButton(), 0);
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
                ManagerHandler.CA_MAN.GetTrigger(striker, ManagerHandler.CA_MAN.TRIGGER_DEATHBLOW))
                    DeathblowTrigger(striker);
            }
            else if (attackerDealtDamage &&
                ManagerHandler.CA_MAN.GetTrigger(striker, ManagerHandler.CA_MAN.TRIGGER_INFILTRATE))
                InfiltrateTrigger(striker);
        }
        */

        void DealDamage(GameObject striker, GameObject defender,
            out bool dealtDamage, out bool defenderDestroyed, bool isMeleeAttacker)
        {
            var ucd = GetUnitDisplay(striker);
            int power = ucd.CurrentPower;

            // IsMeleeAttacker must be reversed here, the target of the attack will have the opposite designation
            TakeDamage(defender, power, out dealtDamage, out defenderDestroyed, !isMeleeAttacker);

            // Poisonous
            if (IsUnitCard(defender))
            {
                var defUcd = GetUnitDisplay(defender);
                if (dealtDamage && !defenderDestroyed)
                {
                    if (CardManager.GetAbility(striker, CardManager.ABILITY_POISONOUS))
                        defUcd.AddCurrentAbility(Managers.EF_MAN.PoisonAbility);
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

        Managers.AN_MAN.ShakeCamera(AnimationManager.Bump_Light);
        //ManagerHandler.AN_MAN.CreateParticleSystem(target, ParticleSystemHandler.ParticlesType.Damage, 1); // TESTING

        int targetValue;
        int newTargetValue;
        if (IsUnitCard(target)) targetValue = GetUnitDisplay(target).CurrentHealth;
        else if (target == Managers.P_MAN.HeroObject) targetValue = Managers.P_MAN.CurrentHealth;
        else if (target == Managers.EN_MAN.HeroObject) targetValue = Managers.EN_MAN.CurrentHealth;
        else
        {
            Debug.LogError("INVALID TARGET!");
            return;
        }

        if (targetValue < 1) return; // Don't deal damage to targets with 0 health
        newTargetValue = targetValue - damageValue;
        
        // Damage to heroes
        if (target == Managers.P_MAN.HeroObject)
        {
            Managers.P_MAN.DamageTaken_ThisTurn += damageValue;
            Managers.P_MAN.CurrentHealth = newTargetValue;

            Managers.AN_MAN.ModifyHeroHealthState(target, -damageValue);
            wasDamaged = true;
        }
        else if (target == Managers.EN_MAN.HeroObject)
        {
            Managers.EN_MAN.DamageTaken_ThisTurn += damageValue;
            Managers.EN_MAN.CurrentHealth = newTargetValue;

            Managers.AN_MAN.ModifyHeroHealthState(target, -damageValue);
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
                Managers.AN_MAN.UnitTakeDamageState(target, damageTaken, isMeleeAttacker);
                wasDamaged = true;
            }
        }

        if (newTargetValue < 1)
        {
            wasDestroyed = true;

            if (IsUnitCard(target)) DestroyUnit(target);
            else
            {
                Managers.AN_MAN.ShakeCamera(EZCameraShake.CameraShakePresets.Earthquake);
                Managers.AN_MAN.SetAnimatorBool(target, "IsDestroyed", true);
                EndCombat(target == Managers.EN_MAN.HeroObject);
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

        if (target == Managers.P_MAN.HeroObject)
        {
            targetValue = Managers.P_MAN.CurrentHealth;
            maxValue = Managers.P_MAN.MaxHealth;
        }
        else if (target == Managers.EN_MAN.HeroObject)
        {
            targetValue = Managers.EN_MAN.CurrentHealth;
            maxValue = Managers.EN_MAN.MaxHealth;
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

        Managers.AU_MAN.StartStopSound("SFX_StatPlus");
        int healthChange = newTargetValue - targetValue;

        if (IsUnitCard(target))
        {
            GetUnitDisplay(target).CurrentHealth = newTargetValue;
            Managers.AN_MAN.UnitStatChangeState(target, 0, healthChange, true);
        }
        else
        {
            if (target == Managers.P_MAN.HeroObject) Managers.P_MAN.CurrentHealth = newTargetValue;
            else if (target == Managers.EN_MAN.HeroObject) Managers.EN_MAN.CurrentHealth = newTargetValue;
            Managers.AN_MAN.ModifyHeroHealthState(target, healthChange);
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

        HeroManager hMan_Source = HeroManager.GetSourceHero(card, out HeroManager hMan_Enemy);

        if (!Managers.EF_MAN.UnitsToDestroy.Contains(card)) Managers.EF_MAN.UnitsToDestroy.Add(card);
        DestroyFX();

        // Remove from lists immediately for PlayCard effects triggered from Revenge
        hMan_Source.PlayZoneCards.Remove(card);
        hMan_Source.AlliesDestroyed_ThisTurn++; // TESTING

        Managers.EV_MAN.NewDelayedAction(() => Destroy(), 0.75f, true);
        Managers.EV_MAN.NewDelayedAction(() => DestroyTriggers(), 0, true);

        void DestroyFX()
        {
            if (card == null)
            {
                Debug.LogError("CARD IS NULL!");
                return;
            }
            Sound deathSound = GetUnitDisplay(card).UnitCard.UnitDeathSound;
            Managers.AN_MAN.DestroyUnitCardState(card);
        }
        void DestroyTriggers()
        {
            if (card == null)
            {
                Debug.LogError("CARD IS NULL!");
                return;
            }

            if (hMan_Source == null || hMan_Enemy == null)
            {
                Debug.LogError("MANAGERS ARE NULL!");
                return;
            }

            // Revenge
            Managers.CA_MAN.TriggerUnitAbility(card, CardManager.TRIGGER_REVENGE);

            // Marked
            if (CardManager.GetAbility(card, CardManager.ABILITY_MARKED))
                Managers.EV_MAN.NewDelayedAction(() => MarkedTrigger(), 0.5f, true);

            // Ally Destroyed
            Managers.EF_MAN.TriggerModifiers_SpecialTrigger
                (ModifierAbility.TriggerType.AllyDestroyed, hMan_Source.PlayZoneCards);

            // Enemy Destroyed
            Managers.EF_MAN.TriggerModifiers_SpecialTrigger
                (ModifierAbility.TriggerType.EnemyDestroyed, hMan_Enemy.PlayZoneCards);

            // Marked Enemy Destroyed
            if (CardManager.GetAbility(card, CardManager.ABILITY_MARKED))
            {
                Managers.EF_MAN.TriggerModifiers_SpecialTrigger
                    (ModifierAbility.TriggerType.MarkedEnemyDestroyed, hMan_Enemy.PlayZoneCards);
            }
            // Poisoned Enemy Destroyed
            if (CardManager.GetAbility(card, CardManager.ABILITY_POISONED))
            {
                Managers.EF_MAN.TriggerModifiers_SpecialTrigger
                    (ModifierAbility.TriggerType.PoisonedEnemyDestroyed, hMan_Enemy.PlayZoneCards);
            }

            void MarkedTrigger()
            {
                GetUnitDisplay(card).AbilityTriggerState(CardManager.ABILITY_MARKED);
                HeroManager.GetSourceHero(card, out HeroManager hMan_Enemy);
                Managers.CA_MAN.DrawCard(hMan_Enemy);
            }
        }
        void Destroy()
        {
            if (card == null)
            {
                Debug.LogError("CARD IS NULL!");
                return;
            }

            if (hMan_Source == null || hMan_Enemy == null)
            {
                Debug.LogError("MANAGERS ARE NULL!");
                return;
            }

            Managers.EF_MAN.UnitsToDestroy.Remove(card);

            UnitCard unitCard = card.GetComponent<CardDisplay>().CardScript as UnitCard;
            card.GetComponent<CardZoom>().DestroyZoomPopups();
            Managers.AU_MAN.StartStopSound(null, unitCard.UnitDeathSound);

            DiscardCard(hMan_Source.DiscardZoneCards);

            if (Managers.P_MAN.IsMyTurn) Managers.CA_MAN.SelectPlayableCards();

            void DiscardCard(List<Card> cardZone)
            {
                if (unitCard.BanishAfterPlay) Managers.CA_MAN.HideCard(card);
                else
                {
                    card.GetComponent<CardDisplay>().ResetCard();
                    cardZone.Add(Managers.CA_MAN.HideCard(card));
                }
            }
        }
    }
    #endregion
    #endregion
}
