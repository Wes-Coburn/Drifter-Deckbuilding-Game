using System.Collections;
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

    private Vector2 playerHandStart;
    private Color previousBarColor;
    private Color previousTextCountColor;

    [SerializeField] private GameObject valueChangerPrefab;
    [SerializeField] private GameObject particleSystemPrefab;
    [SerializeField] private GameObject particleSystem_BurstPrefab;

    public Coroutine ProgressBarRoutine { get; set; }
    private Coroutine TextCountRoutine { get; set; }

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
     * ****** VALUE_CHANGE_ANIMATION
     * *****
     *****/
    private void ValueChanger(Transform parent, int value)
    {
        GameObject valueChanger = Instantiate(valueChangerPrefab, parent);
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
        switch (particlesType)
        {
            case ParticleSystemHandler.ParticlesType.Attack:
                goto default;
            case ParticleSystemHandler.ParticlesType.ButtonPress:
                goto default;
            case ParticleSystemHandler.ParticlesType.Damage:
                goto default;
            case ParticleSystemHandler.ParticlesType.Drag:
                goto default;
            case ParticleSystemHandler.ParticlesType.Play:
                prefab = particleSystem_BurstPrefab;
                break;
            default:
                prefab = particleSystemPrefab;
                break;
        }
        GameObject particleSystem = Instantiate(prefab, uMan.CurrentWorldSpace.transform);
        ParticleSystemHandler psh = particleSystem.GetComponent<ParticleSystemHandler>();
        psh.StartParticles(parent, particlesType, stopDelay);
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
    public void ModifyHeroEnergyState(int energyChange)
    {
        auMan.StartStopSound("SFX_EnergyRefill");
        ChangeAnimationState(coMan.PlayerHero, "Modify_Energy");
        GameObject energyScore = coMan.PlayerHero.GetComponent<PlayerHeroDisplay>().HeroEnergy;
        ValueChanger(energyScore.transform, energyChange);
    }
    public void ReinforcementsState()
    {
        auMan.StartStopSound("SFX_Reinforcements");
        ChangeAnimationState(coMan.EnemyHero, "Reinforcements");
    }
    public void NextReinforcementsState()
    {
        auMan.StartStopSound("SFX_NextReinforcements");
        ChangeAnimationState(coMan.EnemyHero, "Next_Reinforcements");
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
    public void UnitTakeDamageState(GameObject unitCard, int damageValue)
    {
        ChangeAnimationState(unitCard.GetComponent<UnitCardDisplay>().UnitStats, "Take_Damage");
        GameObject healthScore = unitCard.GetComponent<UnitCardDisplay>().HealthScore;
        ValueChanger(healthScore.transform, -damageValue); // TESTING
    }
    public void DestroyUnitCardState(GameObject unitCard) =>
        ChangeAnimationState(unitCard, "Destroyed");
    public void UnitStatChangeState(GameObject unitCard, int powerChange, int healthChange, bool isHealing = false)
    {
        if (powerChange == 0 && healthChange == 0 && !isHealing) return;

        UnitCardDisplay ucd = unitCard.GetComponent<UnitCardDisplay>();
        GameObject stats = ucd.UnitStats;
        SetAnimatorBool(stats, "IsDamaged", coMan.IsDamaged(unitCard));

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
            Debug.LogWarning("HEAL!");
            ModifyUnitHealthState(stats);
            ModifyHealth();
        }

        void ModifyPower()
        {
            GameObject powerScore = ucd.PowerScore;
            ValueChanger(powerScore.transform, powerChange); // TESTING
        }
        void ModifyHealth()
        {
            GameObject healthScore = ucd.HealthScore;
            ValueChanger(healthScore.transform, healthChange); // TESTING
        }
    }
    private void ModifyUnitHealthState(GameObject card) =>
        ChangeAnimationState(card, "Modify_Health");
    private void ModifyUnitPowerState(GameObject card) =>
        ChangeAnimationState(card, "Modify_Power");
    private void ModifyAllUnitStatsState(GameObject card) =>
        ChangeAnimationState(card, "Modify_All");

    // Ability Trigger
    public void AbilityTriggerState(GameObject triggerIcon)
    {
        ChangeAnimationState(triggerIcon.GetComponent
            <AbilityIconDisplay>().AbilitySpriteObject, "Trigger");
    }

    // Icon Animation
    public void SkybarIconAnimation(GameObject icon)
    {
        ChangeAnimationState(icon, "Trigger");
    }

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
    public void ShiftPlayerHand(bool isUpShift) =>
        StartCoroutine(HandShiftNumerator(isUpShift));
    private IEnumerator HandShiftNumerator(bool isUpShift)
    {
        auMan.StartStopSound("SFX_ShiftHand");
        float distance;
        float yTarget;
        GameObject hand = coMan.PlayerHand;
        if (isUpShift)
        {
            yTarget = -350;
            playerHandStart = hand.transform.position;
        }
        else yTarget = playerHandStart.y;
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
    public void NewEngagedHero(bool isExitOnly) =>
        StartCoroutine(NewEngagedHeroNumerator(isExitOnly));
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
    public void CombatIntro() =>
        StartCoroutine(CombatIntroNumerator());
    private IEnumerator CombatIntroNumerator()
    {
        float distance;
        HeroDisplay pHD = coMan.PlayerHero.GetComponent<HeroDisplay>();
        HeroDisplay eHD = coMan.EnemyHero.GetComponent<HeroDisplay>();

        GameObject turBut = uMan.EndTurnButton;
        GameObject pFrame = pHD.HeroFrame;
        GameObject eFrame = eHD.HeroFrame;
        GameObject pStats = pHD.HeroStats;
        GameObject eStats = eHD.HeroStats;
        GameObject combatLog = uMan.CombatLog;

        Vector2 turButStart = turBut.transform.position;
        Vector2 pFrameStart = pFrame.transform.position;
        Vector2 eFrameStart = eFrame.transform.position;
        Vector2 pStatsStart = pStats.transform.localPosition;
        Vector2 eStatsStart = eStats.transform.localPosition;
        Vector2 combatLogStart = combatLog.transform.localPosition;

        float scaleSpeed = 0.1f;
        float fScale = 1;
        float fZoomScale = 1.5f;
        Vector2 scaleVec = new Vector2();
        turBut.SetActive(true);
        pStats.SetActive(true);
        eStats.SetActive(true);
        combatLog.SetActive(true);
        turBut.transform.localPosition = new Vector2(turButStart.x + 450, turButStart.y);
        pStats.transform.localPosition = new Vector2(pStatsStart.x, pStatsStart.y - 450);
        eStats.transform.localPosition = new Vector2(eStatsStart.x, eStatsStart.y + 450);
        combatLog.transform.localPosition = new Vector2(combatLogStart.x - 450, combatLogStart.y);

        uMan.SelectTarget(coMan.PlayerHero, true);
        PlayerManager.Instance.PlayerPowerSounds();

        do
        {
            distance = Vector2.Distance(pFrame.transform.position, eFrame.transform.position);
            pFrame.transform.position =
                Vector2.MoveTowards(pFrame.transform.position, eFrame.transform.position, 30);
            eFrame.transform.position =
                Vector2.MoveTowards(eFrame.transform.position, pFrame.transform.position, 30);
            if (fScale < fZoomScale) fScale += scaleSpeed;
            else if (fScale > fZoomScale) fScale = fZoomScale;
            scaleVec.Set(fScale, fScale);
            pFrame.transform.localScale = scaleVec;
            eFrame.transform.localScale = scaleVec;
            yield return new WaitForFixedUpdate();
        }
        while (distance > 700);

        uMan.CreateVersusPopup();
        uMan.ShakeCamera(EZCameraShake.CameraShakePresets.Bump);
        
        yield return new WaitForSeconds(0.5f);
        uMan.SelectTarget(coMan.PlayerHero, false);
        
        yield return new WaitForSeconds(0.5f);
        uMan.SelectTarget(coMan.EnemyHero, true);
        Sound enemyWinSound = EnemyManager.Instance.EnemyHero.HeroWin;
        auMan.StartStopSound(null, enemyWinSound);
        FunctionTimer.Create(() => uMan.SelectTarget(coMan.EnemyHero, false), 2);

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
            eFrame.transform.position =
                Vector2.MoveTowards(eFrame.transform.position, eFrameStart, 20);
            if (fScale > 1) fScale -= scaleSpeed;
            else if (fScale < 1) fScale = 1;
            scaleVec.Set(fScale, fScale);
            pFrame.transform.localScale = scaleVec;
            eFrame.transform.localScale = scaleVec;
            yield return new WaitForFixedUpdate();
        }
        while (distance > 0);

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

        if (enMan.EnemyHero.IsBoss)
        {
            float countDelay = 0.15f;
            CountingText(eHD.HeroHealthObject.GetComponent
                <TextMeshProUGUI>(), GameManager.ENEMY_STARTING_HEALTH, enMan.MaxEnemyHealth, countDelay);

            while (TextCountRoutine != null)
            {
                ModifyHeroHealthState(coMan.EnemyHero, 1);
                yield return new WaitForSeconds(countDelay);
            }
            enMan.EnemyHealth = enMan.MaxEnemyHealth;
        }
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

        ParticleSystemHandler particleHandler = CreateParticleSystem(attacker, ParticleSystemHandler.ParticlesType.Attack); // TESTING

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
        yield return new WaitForSeconds(0.1f); // TESTING

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

        particleHandler.StopParticles(); // TESTING
        container.GetComponent<CardContainer>().IsDetached = false;
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
                    phd.UltimateUsedIcon.SetActive(false); // TESTING
                    GameObject heroUltimate = phd.HeroUltimate;
                    ChangeAnimationState(heroUltimate, "Trigger");
                    auMan.StartStopSound("SFX_HeroUltimateReady");
                    break;
                case ProgressBarType.Recruit:
                    Debug.LogWarning("RECRUIT REWARD!");
                    //gMan.RecruitLoyalty = 0;
                    break;
                case ProgressBarType.Item:
                    Debug.LogWarning("SHOP REWARD!");
                    //gMan.ShopLoyalty = 0;
                    break;
            }
        }
        ProgressBarRoutine = null; // TESTING
    }
}