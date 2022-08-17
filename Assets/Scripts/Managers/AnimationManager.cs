﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimationManager : MonoBehaviour
{
    /* SINGELTON_PATTERN */
    public static AnimationManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private UIManager uMan;
    private CombatManager coMan;
    private DialogueManager dMan;
    private AudioManager auMan;
    private EnemyManager enMan;

    private Color previousBarColor;
    private Color previousTextCountColor;

    [SerializeField] private GameObject valueChangerPrefab;

    [Header("PARTICLE SYSTEMS")]
    [SerializeField] private GameObject particleSystemPrefab;
    [SerializeField] private GameObject particleSystem_BurstPrefab;
    [Header("PARTICLE SYSTEM COLORS")]
    [SerializeField] private Color attackColor;
    [SerializeField] private Color buttonPressColor;
    [SerializeField] private Color damageColor;
    [SerializeField] private Color dragColor;
    [SerializeField] private Color playColor;
    [SerializeField] private Color newCardColor;
    
    public Coroutine ProgressBarRoutine { get; private set; }
    private Coroutine TextCountRoutine { get; set; }
    private Coroutine ShiftHandRoutine { get; set; }

    private void Start()
    {
        uMan = UIManager.Instance;
        coMan = CombatManager.Instance;
        dMan = DialogueManager.Instance;
        auMan = AudioManager.Instance;
        enMan = EnemyManager.Instance;
    }

    public void ProgressBarRoutine_Stop()
    {
        if (ProgressBarRoutine != null)
        {
            AudioManager.Instance.StartStopSound("SFX_ProgressBar", null, AudioManager.SoundType.SFX, true);
            StopCoroutine(ProgressBarRoutine);
            ProgressBarRoutine = null;
        }
    }

    public void ChangeAnimationState(GameObject go, string state)
    {
        if (go == null)
        {
            Debug.LogError("GAMEOBJECT IS NULL!");
            return;
        }
        
        if (!go.activeSelf)
        {
            Debug.LogError("GAMEOBJECT IS INACTIVE!");
            return;
        }

        if (go.TryGetComponent(out Animator anim))
            if (anim.enabled)
            {
                anim.Play(state);
                return;
            }
        Debug.LogWarning("ANIMATOR NOT FOUND!");
    }

    public void SetAnimatorBool(GameObject go, string boolName, bool animBool)
    {
        if (go == null)
        {
            Debug.LogError("GAMOBJECT IS NULL!");
            return;
        }

        if (go.TryGetComponent(out Animator anim))
            if (anim.enabled)
            {
                anim.SetBool(boolName, animBool);
                return;
            }
        Debug.LogError("ANIMATOR NOT FOUND!");
    }

    /******
     * *****
     * ****** VALUE_CHANGE
     * *****
     *****/
    public void ValueChanger(Transform parent, int value, float yBuffer = 0)
    {
        GameObject valueChanger = Instantiate(valueChangerPrefab, parent);
        valueChanger.transform.localPosition = new Vector2(0, yBuffer);
        Transform newParent;
        if (yBuffer != 0)
        {
            newParent = uMan.UICanvas.transform;
            valueChanger.transform.localScale = new Vector2(2, 2);
        }
        else newParent = parent.parent.parent.parent;
        valueChanger.transform.SetParent(newParent);
        valueChanger.transform.SetAsLastSibling();

        string valueText = "+";
        bool isPositiveChange = true;
        if (value < 0)
        {
            isPositiveChange = false;
            valueText = "";
        }

        valueText += value;
        valueChanger.GetComponentInChildren<TextMeshProUGUI>().SetText(valueText);
        valueChanger.GetComponentInChildren<Animator>().SetBool("IsPositiveChange", isPositiveChange);
    }

    /******
     * *****
     * ****** PARTICLE_SYSTEM
     * *****
     *****/
    public ParticleSystemHandler CreateParticleSystem(GameObject parent,
        ParticleSystemHandler.ParticlesType particlesType, float stopDelay = 0)
    {
        GameObject prefab;
        Color startColor;
        ParticleSystem.MinMaxCurve startSize = 5;
        ParticleSystem.MinMaxCurve startLifetime;
        bool usePointerPosition = false;
        bool followPosition = true;

        switch (particlesType)
        {
            case ParticleSystemHandler.ParticlesType.Attack:
                prefab = particleSystemPrefab;
                startColor = attackColor;
                startSize = 10;
                startLifetime = 0.75f;
                break;
            case ParticleSystemHandler.ParticlesType.ButtonPress:
                prefab = particleSystemPrefab;
                startColor = buttonPressColor;
                startSize = 10;
                startLifetime = 5;
                usePointerPosition = true;
                followPosition = false;
                break;
            case ParticleSystemHandler.ParticlesType.Damage:
                prefab = particleSystem_BurstPrefab;
                startColor = damageColor;
                startLifetime = 1;
                break;
            case ParticleSystemHandler.ParticlesType.Drag:
                prefab = particleSystemPrefab;
                startColor = dragColor;
                startLifetime = 0.3f;
                break;
            case ParticleSystemHandler.ParticlesType.Explosion:
                prefab = particleSystem_BurstPrefab;
                startColor = damageColor;
                startSize = 20;
                startLifetime = 20;
                break;
            case ParticleSystemHandler.ParticlesType.MouseDrag:
                prefab = particleSystemPrefab;
                startColor = dragColor;
                startLifetime = 0.3f;
                usePointerPosition = true;
                break;
            case ParticleSystemHandler.ParticlesType.NewCard:
                prefab = particleSystemPrefab;
                startColor = newCardColor;
                startSize = 10;
                startLifetime = 1;
                break;
            case ParticleSystemHandler.ParticlesType.Play:
                prefab = particleSystem_BurstPrefab;
                startColor = playColor;
                startLifetime = 1;
                break;
            default:
                Debug.LogError("INVALIDE PARTICLES TYPE!");
                return null;
        }

        GameObject particleSystem = Instantiate(prefab, uMan.CurrentWorldSpace.transform);
        ParticleSystemHandler psh = particleSystem.GetComponent<ParticleSystemHandler>();
        psh.StartParticles(parent, startColor, startSize, startLifetime, stopDelay, usePointerPosition, followPosition);
        return psh;
    }

    /******
     * *****
     * ****** HERO_STATE_ANIMATIONS
     * *****
     *****/
    public void ModifyHeroHealthState(GameObject hero, int healthChange)
    {
        ChangeAnimationState(hero, "Modify_Health");
        GameObject healthScore = hero.GetComponent<HeroDisplay>().HeroHealthObject;
        ValueChanger(healthScore.transform, healthChange);
    }
    public void ModifyHeroEnergyState(int energyChange, GameObject hero, bool playSound = true)
    {
        if (playSound) auMan.StartStopSound("SFX_EnergyRefill");
        ChangeAnimationState(hero, "Modify_Energy");
        GameObject energyScore = hero.GetComponent<HeroDisplay>().HeroEnergyObject;
        ValueChanger(energyScore.transform, energyChange);
    }

    /******
     * *****
     * ****** CARD_STATE_ANIMATIONS
     * *****
     *****/
    public void RevealedHandState(GameObject card) =>
        ChangeAnimationState(card, "Revealed_Hand");
    public void RevealedDragState(GameObject card) =>
        ChangeAnimationState(card, "Revealed_Drag");
    public void RevealedPlayState(GameObject card) =>
        ChangeAnimationState(card, "Revealed_Play");
    public void PlayedState(GameObject card)
    {
        card.GetComponent<CardDisplay>().CardArt =
            card.GetComponent<CardDisplay>().CardScript.CardArt;
        ChangeAnimationState(card, "Played");
    }
    public void ZoomedState(GameObject card) =>
        ChangeAnimationState(card, "Zoomed");

    /******
     * *****
     * ****** STAT_CHANGES
     * *****
     *****/
    private void ModifyUnitHealthState(GameObject card) => ChangeAnimationState(card, "Modify_Health");
    private void ModifyUnitPowerState(GameObject card) => ChangeAnimationState(card, "Modify_Power");
    private void ModifyAllUnitStatsState(GameObject card) => ChangeAnimationState(card, "Modify_All");
    public void DestroyUnitCardState(GameObject unitCard) => ChangeAnimationState(unitCard, "Destroyed");

    public void UnitTakeDamageState(GameObject unitCard, int damageValue)
    {
        ChangeAnimationState(unitCard.GetComponent<UnitCardDisplay>().UnitStats, "Take_Damage");
        UnitCardDisplay ucd = unitCard.GetComponent<UnitCardDisplay>();
        GameObject stats = ucd.UnitStats;
        GameObject healthScore = ucd.HealthScore;
        ValueChanger(healthScore.transform, -damageValue);
        SetAnimatorBool(stats, "IsDamaged", coMan.IsDamaged(unitCard));
    }

    public void ShowStatChange(GameObject unitCard, StatChangeEffect sce, bool isRemoval)
    {
        bool isNegativeChange = false;
        int powerChange = sce.PowerChange;
        int healthChange = sce.HealthChange;

        if (sce.PowerChange < 0 || sce.HealthChange < 0) isNegativeChange = true;
        if (isRemoval)
        {
            isNegativeChange = !isNegativeChange;
            powerChange = -powerChange;
            healthChange = -healthChange;
        }

        if (!isNegativeChange) auMan.StartStopSound("SFX_StatPlus");
        else auMan.StartStopSound("SFX_StatMinus");
        UnitStatChangeState(unitCard, powerChange, healthChange);
    }

    public void UnitStatChangeState(GameObject unitCard, int powerChange, int healthChange, bool isHealing = false)
    {
        if (powerChange == 0 && healthChange == 0 && !isHealing) return;

        UnitCardDisplay ucd = unitCard.GetComponent<UnitCardDisplay>();
        GameObject stats = ucd.UnitStats;
        SetAnimatorBool(stats, "IsDamaged", coMan.IsDamaged(unitCard));
        SetAnimatorBool(stats, "PowerIsBuffed", coMan.PowerIsBuffed(unitCard));
        SetAnimatorBool(stats, "HealthIsBuffed", coMan.HealthIsBuffed(unitCard));

        if (powerChange != 0)
        {
            if (healthChange != 0)
            {
                ModifyAllUnitStatsState(stats);
                ModifyPower();
                ModifyHealth();
            }
            else
            {
                ModifyUnitPowerState(stats);
                ModifyPower();
            }
        }
        else if (healthChange != 0 || isHealing)
        {
            ModifyUnitHealthState(stats);
            ModifyHealth();
        }

        void ModifyPower()
        {
            GameObject powerScore = ucd.PowerScore;
            ValueChanger(powerScore.transform, powerChange);
        }
        void ModifyHealth()
        {
            GameObject healthScore = ucd.HealthScore;
            ValueChanger(healthScore.transform, healthChange);
        }
    }

    // Ability Trigger
    public void AbilityTriggerState(GameObject triggerIcon)
    {
        if (triggerIcon == null)
        {
            Debug.LogWarning("ICON IS NULL!");
            return;
        }

        triggerIcon.transform.SetAsLastSibling();
        ChangeAnimationState(triggerIcon.GetComponent<AbilityIconDisplay>().AbilitySpriteObject, "Trigger");
    }

    // Icon Animation
    public void SkybarIconAnimation(GameObject icon) => ChangeAnimationState(icon, "Trigger");

    /******
     * *****
     * ****** COUNTING_TEXT
     * *****
     *****/
    public void CountingText(TextMeshProUGUI text, int start, int end, float delay = 0.3f)
    {
        if (start == end)
        {
            Debug.LogError("START == END!");
            return;
        }
        if (TextCountRoutine != null) // TESTING
        {
            text.color = previousTextCountColor;
            StopCoroutine(TextCountRoutine);
        }
        TextCountRoutine = StartCoroutine(CountingTextNumerator(text, start, end, delay));
    }
    private IEnumerator CountingTextNumerator(TextMeshProUGUI text, int start, int end, float delay)
    {
        previousTextCountColor = text.color;
        text.color = uMan.HighlightedColor;
        int count = start;
        if (count < end)
        {
            while (count < end)
            {
                yield return new WaitForSeconds(delay);
                text.SetText(++count + "");
                auMan.StartStopSound("SFX_Counting");
            }
        }
        else
        {
            while (count > end)
            {
                yield return new WaitForSeconds(delay);
                text.SetText(--count + "");
                auMan.StartStopSound("SFX_Counting");
            }
        }

        yield return new WaitForSeconds(delay);
        text.color = previousTextCountColor;

        yield return new WaitForSeconds(delay);
        text.color = uMan.HighlightedColor;

        yield return new WaitForSeconds(delay);
        text.color = previousTextCountColor;

        TextCountRoutine = null;
    }

    /******
     * *****
     * ****** SHIFT_PLAYER_HAND
     * *****
     *****/
    public void ShiftPlayerHand(bool isUpShift)
    {
        if (ShiftHandRoutine != null)
        {
            StopCoroutine(ShiftHandRoutine);
            ShiftHandRoutine = null;
        }

        ShiftHandRoutine = StartCoroutine(ShiftHandNumerator(isUpShift));
    }
    private IEnumerator ShiftHandNumerator(bool isUpShift)
    {
        auMan.StartStopSound("SFX_ShiftHand");
        float distance;
        float yTarget;
        GameObject hand = coMan.PlayerHand;
        if (isUpShift) yTarget = -350;
        else yTarget = coMan.PlayerHandStart.y;
        Vector2 target = new Vector2(0, yTarget);

        do
        {
            distance = Vector2.Distance(hand.transform.position, target);
            hand.transform.position = Vector2.MoveTowards(hand.transform.position, target, 30);
            yield return new WaitForFixedUpdate();
        }
        while (distance > 0);
        uMan.DestroyZoomObjects();
    }

    /******
     * *****
     * ****** DIALOGUE_INTRO
     * *****
     *****/
    public void DialogueIntro() =>
        StartCoroutine(DialogueIntroNumerator());
    private IEnumerator DialogueIntroNumerator()
    {
        auMan.StartStopSound("SFX_PortraitClick");

        float distance;
        DialogueSceneDisplay dsp = dMan.DialogueDisplay;
        GameObject playerPortrait = dsp.PlayerHeroPortrait;
        GameObject npcPortrait = dsp.NPCHeroPortrait;
        Vector2 pPortStart = playerPortrait.transform.localPosition;
        Vector2 nPortStart = npcPortrait.transform.localPosition;
        playerPortrait.SetActive(true);
        DialoguePrompt prompt = dMan.EngagedHero.NextDialogueClip as DialoguePrompt;
        npcPortrait.SetActive(!prompt.HideNPC);
        playerPortrait.transform.localPosition = new Vector2(600, pPortStart.y);
        npcPortrait.transform.localPosition = new Vector2(-600, nPortStart.y);

        yield return new WaitForSeconds(0.5f);

        do
        {
            distance = Vector2.Distance(playerPortrait.transform.localPosition, pPortStart);
            playerPortrait.transform.localPosition =
                Vector2.MoveTowards(playerPortrait.transform.localPosition, pPortStart, 30);
            npcPortrait.transform.localPosition =
                Vector2.MoveTowards(npcPortrait.transform.localPosition, nPortStart, 30);
            yield return new WaitForFixedUpdate();
        }
        while (distance > 0);
        // PLAY SOUND
    }

    /******
     * *****
     * ****** NEW_ENGAGED_HERO
     * *****
     *****/
    public void NewEngagedHero(bool isExitOnly) => StartCoroutine(NewEngagedHeroNumerator(isExitOnly));
    private IEnumerator NewEngagedHeroNumerator(bool isExitOnly)
    {
        auMan.StartStopSound("SFX_PortraitClick");
        float distance;
        GameObject npcPortrait = dMan.DialogueDisplay.NPCHeroPortrait;
        Vector2 nPortStart = npcPortrait.transform.localPosition;
        Vector2 nPortEnd = new Vector2(-600, nPortStart.y);

        do
        {
            distance = Vector2.Distance(npcPortrait.transform.localPosition, nPortEnd);
            npcPortrait.transform.localPosition =
                Vector2.MoveTowards(npcPortrait.transform.localPosition, nPortEnd, 30);
            yield return new WaitForFixedUpdate();
        }
        while (distance > 0);

        npcPortrait.SetActive(!isExitOnly);

        dMan.DisplayCurrentHeroes();
        yield return new WaitForSeconds(0.5f);

        do
        {
            distance = Vector2.Distance(npcPortrait.transform.localPosition, nPortStart);
            npcPortrait.transform.localPosition =
                Vector2.MoveTowards(npcPortrait.transform.localPosition, nPortStart, 30);
            yield return new WaitForFixedUpdate();
        }
        while (distance > 0);

        if (!isExitOnly) dMan.DisplayDialoguePopup();
        dMan.AllowResponse = true;
    }

    /******
     * *****
     * ****** COMBAT_INTRO
     * *****
     *****/
    public void CombatIntro() => StartCoroutine(CombatIntroNumerator());
    private IEnumerator CombatIntroNumerator()
    {
        float distance;
        float scaleSpeed = 0.1f;
        float fScale = 1;
        float fZoomScale = 1.5f;
        int startBuffer = 600;

        GameObject turBut = uMan.EndTurnButton;
        GameObject combatLog = uMan.CombatLog;

        HeroDisplay pHD = coMan.PlayerHero.GetComponent<HeroDisplay>();
        GameObject pFrame = pHD.HeroFrame;
        GameObject pStats = pHD.HeroStats;
        GameObject pName = pHD.HeroNameObject;

        HeroDisplay eHD = coMan.EnemyHero.GetComponent<HeroDisplay>();
        GameObject eFrame = eHD.HeroFrame;
        GameObject eStats = eHD.HeroStats;
        GameObject eName = eHD.HeroNameObject;

        Vector2 turButStart = turBut.transform.position;
        Vector2 combatLogStart = combatLog.transform.localPosition;

        Vector2 pFrameStart = pFrame.transform.position;
        Vector2 pStatsStart = pStats.transform.localPosition;
        Vector2 pNameStart = pName.transform.localPosition;
        Vector2 pNameEnd = new Vector2(pNameStart.x + startBuffer, pNameStart.y);

        Vector2 eFrameStart = eFrame.transform.position;
        Vector2 eStatsStart = eStats.transform.localPosition;
        Vector2 eNameStart = eName.transform.localPosition;
        Vector2 eNameEnd = new Vector2(eNameStart.x - startBuffer, eNameStart.y);

        Vector2 scaleVec = new Vector2();
        turBut.SetActive(true);
        combatLog.SetActive(true);

        pStats.SetActive(true);
        pName.SetActive(true);

        eStats.SetActive(true);
        eName.SetActive(true);

        turBut.transform.localPosition = new Vector2(turButStart.x + startBuffer, turButStart.y);
        combatLog.transform.localPosition = new Vector2(combatLogStart.x - startBuffer, combatLogStart.y);

        pStats.transform.localPosition = new Vector2(pStatsStart.x, pStatsStart.y - startBuffer);
        pName.transform.localPosition = pNameEnd;

        eStats.transform.localPosition = new Vector2(eStatsStart.x, eStatsStart.y + startBuffer);
        eName.transform.localPosition = eNameEnd;

        uMan.SelectTarget(coMan.PlayerHero, UIManager.SelectionType.Highlighted);
        PlayerManager.Instance.PlayerPowerSounds();
        CreateParticleSystem(coMan.PlayerHero, ParticleSystemHandler.ParticlesType.Drag, 2);
        CreateParticleSystem(coMan.EnemyHero, ParticleSystemHandler.ParticlesType.Drag, 2);

        do
        {
            distance = Vector2.Distance(pFrame.transform.position, eFrame.transform.position);

            pFrame.transform.position =
                Vector2.MoveTowards(pFrame.transform.position, eFrame.transform.position, 30);
            pName.transform.localPosition =
                Vector2.MoveTowards(pName.transform.localPosition, pNameStart, 30);

            eFrame.transform.position =
                Vector2.MoveTowards(eFrame.transform.position, pFrame.transform.position, 30);
            eName.transform.localPosition =
                Vector2.MoveTowards(eName.transform.localPosition, eNameStart, 30);

            if (fScale < fZoomScale) fScale += scaleSpeed;
            else if (fScale > fZoomScale) fScale = fZoomScale;
            scaleVec.Set(fScale, fScale);
            pFrame.transform.localScale = scaleVec;
            eFrame.transform.localScale = scaleVec;
            yield return new WaitForFixedUpdate();
        }
        while (distance > 700);

        uMan.CreateVersusPopup();
        uMan.ShakeCamera(UIManager.Bump_Light);
        
        yield return new WaitForSeconds(0.5f);
        uMan.SelectTarget(coMan.PlayerHero, UIManager.SelectionType.Disabled);
        
        yield return new WaitForSeconds(0.5f);
        uMan.SelectTarget(coMan.EnemyHero, UIManager.SelectionType.Highlighted);
        Sound enemyWinSound = EnemyManager.Instance.EnemyHero.HeroWin;
        auMan.StartStopSound(null, enemyWinSound);
        FunctionTimer.Create(() =>
        uMan.SelectTarget(coMan.EnemyHero, UIManager.SelectionType.Disabled), 2);

        EnemyHero eh = dMan.EngagedHero as EnemyHero;
        if (eh.IsBoss)
        {
            yield return new WaitForSeconds(2);
            uMan.CreateVersusPopup(true);
        }

        foreach (Transform augTran in uMan.AugmentBar.transform)
        {
            augTran.gameObject.SetActive(true);
            SkybarIconAnimation(augTran.gameObject);
        }
        if (uMan.AugmentBar.transform.childCount > 0)
            auMan.StartStopSound("SFX_Trigger");

        yield return new WaitForSeconds(0.5f);

        foreach (Transform itemTran in uMan.ItemBar.transform)
        {
            itemTran.gameObject.SetActive(true);
            SkybarIconAnimation(itemTran.gameObject);
        }

        if (uMan.ItemBar.transform.childCount > 0)
            auMan.StartStopSound("SFX_Trigger");

        auMan.StartStopSound("SFX_PortraitClick");
        do
        {
            distance = Vector2.Distance(pFrame.transform.position, pFrameStart);

            pFrame.transform.position =
                Vector2.MoveTowards(pFrame.transform.position, pFrameStart, 20);
            pName.transform.localPosition =
                Vector2.MoveTowards(pName.transform.localPosition, pNameEnd, 20);

            eFrame.transform.position =
                Vector2.MoveTowards(eFrame.transform.position, eFrameStart, 20);
            eName.transform.localPosition =
                Vector2.MoveTowards(eName.transform.localPosition, eNameEnd, 20);

            if (fScale > 1) fScale -= scaleSpeed;
            else if (fScale < 1) fScale = 1;
            scaleVec.Set(fScale, fScale);
            pFrame.transform.localScale = scaleVec;
            eFrame.transform.localScale = scaleVec;
            yield return new WaitForFixedUpdate();
        }
        while (distance > 0);

        pName.SetActive(false);
        eName.SetActive(false);

        do
        {
            distance = Vector2.Distance(pStats.transform.localPosition, pStatsStart);
            turBut.transform.position =
                Vector2.MoveTowards(turBut.transform.position, turButStart, 20);
            pStats.transform.localPosition =
                Vector2.MoveTowards(pStats.transform.localPosition, pStatsStart, 20);
            eStats.transform.localPosition =
                Vector2.MoveTowards(eStats.transform.localPosition, eStatsStart, 20);
            combatLog.transform.localPosition =
                Vector2.MoveTowards(combatLog.transform.localPosition, combatLogStart, 20);
            yield return new WaitForFixedUpdate();
        }
        while (distance > 0);
    }

    /******
     * *****
     * ****** UNIT_ATTACK
     * *****
     *****/
    public void UnitAttack(GameObject attacker, GameObject defender, bool defenderIsUnit) => 
        StartCoroutine(AttackNumerator(attacker, defender, defenderIsUnit));
    
    private readonly float minSpeed = 100;
    private readonly float maxSpeed = 200;

    private float GetCurrentSpeed(float distance)
    {
        float speed = maxSpeed - (distance * 0.5f);
        if (speed < minSpeed) speed = minSpeed;
        else if (speed > maxSpeed) speed = maxSpeed;
        return speed;
    }

    private IEnumerator AttackNumerator(GameObject attacker,
        GameObject defender, bool defenderIsUnit = true)
    {
        EventManager.Instance.PauseDelayedActions(true);
        float distance;
        float bufferDistance;

        if (defenderIsUnit) bufferDistance = 150;
        else bufferDistance = 350;
        GameObject container = attacker.GetComponent<CardDisplay>().CardContainer;
        container.GetComponent<CardContainer>().IsDetached = true;
        attacker.transform.SetAsLastSibling();
        Vector2 defPos = defender.transform.position;

        ParticleSystemHandler particleHandler = CreateParticleSystem(attacker,
            ParticleSystemHandler.ParticlesType.Attack);

        // ATTACK
        do
        {
            distance = Vector2.Distance(attacker.transform.position, defPos);
            attacker.transform.position =
                Vector3.MoveTowards(attacker.transform.position,
                defPos, GetCurrentSpeed(distance));
            yield return new WaitForFixedUpdate();
        }
        while (distance > bufferDistance);

        coMan.PlayAttackSound(attacker);
        coMan.Strike(attacker, defender, true);
        yield return new WaitForSeconds(0.1f);

        // RETREAT
        do
        {
            distance = Vector2.Distance(attacker.transform.position, container.transform.position);
            attacker.transform.position =
                Vector3.MoveTowards(attacker.transform.position, container.transform.position,
                GetCurrentSpeed(distance));
            yield return new WaitForFixedUpdate();
        }
        while (distance > 0);

        particleHandler.StopParticles();
        container.GetComponent<CardContainer>().IsDetached = false;
        if (attacker.CompareTag(CombatManager.ENEMY_CARD)) // TESTING
            attacker.transform.SetAsFirstSibling();
        EventManager.Instance.PauseDelayedActions(false);
    }

    /******
     * *****
     * ****** SET_PROGRESS_BAR
     * *****
     *****/
    public enum ProgressBarType
    {
        Ultimate,
        Recruit,
        Item
    }
    public void SetProgressBar(ProgressBarType progressType, int currentProgress, int newProgress, bool isReady,
        GameObject progressBar, GameObject progressFill, int controlValue = 1)
    {
        if (ProgressBarRoutine != null)
        {
            progressFill.GetComponent<Image>().color = previousBarColor;
            StopCoroutine(ProgressBarRoutine);
        }
        ProgressBarRoutine = StartCoroutine(ProgressBarNumerator(progressType, currentProgress, newProgress,
            isReady, progressBar, progressFill, controlValue));
    }

    private IEnumerator ProgressBarNumerator(ProgressBarType progressType, int currentProgress, int newProgress,
        bool isReady, GameObject progressBar, GameObject progressFill, int controlValue)
    {
        Slider slider = progressBar.GetComponent<Slider>();
        slider.value = currentProgress + controlValue; // TESTING
        Image image = progressFill.GetComponent<Image>();
        previousBarColor = image.color;
        image.color = uMan.HighlightedColor;
        auMan.StartStopSound("SFX_ProgressBar", null, AudioManager.SoundType.SFX, false, true);

        int targetValue = newProgress + controlValue;
        if (slider.value < targetValue)
        {
            while (slider.value < targetValue)
            {
                slider.value += 0.02f;
                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            while (slider.value > targetValue)
            {
                slider.value -= 0.05f;
                yield return new WaitForFixedUpdate();
            }
        }
        
        image.color = previousBarColor;
        auMan.StartStopSound("SFX_ProgressBar", null, AudioManager.SoundType.SFX, true);

        if (isReady)
        {
            switch (progressType)
            {
                case ProgressBarType.Ultimate:
                    PlayerHeroDisplay phd = coMan.PlayerHero.GetComponent<PlayerHeroDisplay>();
                    phd.UltimateUsedIcon.SetActive(false);
                    GameObject heroUltimate = phd.HeroUltimate;
                    TriggerHeroPower(heroUltimate);
                    auMan.StartStopSound("SFX_HeroUltimateReady");
                    break;
                case ProgressBarType.Recruit:
                    Debug.Log("RECRUIT REWARD!");
                    break;
                case ProgressBarType.Item:
                    Debug.Log("SHOP REWARD!");
                    break;
            }
        }
        ProgressBarRoutine = null;
    }

    public void TriggerHeroPower(GameObject heroPower) => ChangeAnimationState(heroPower, "Trigger");
}