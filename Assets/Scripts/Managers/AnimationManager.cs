using EZCameraShake;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    #region FIELDS
    private Color previousBarColor;

    [SerializeField] private GameObject valueChangerPrefab;
    [Header("PARTICLE SYSTEMS"), SerializeField] private GameObject particleSystemPrefab;
    [SerializeField] private GameObject particleSystem_BurstPrefab;
    [Header("PARTICLE SYSTEM COLORS"), SerializeField] private Color attackColor;
    [SerializeField] private Color buttonPressColor, damageColor, dragColor, newCardColor;
    #endregion

    #region PROPERTIES
    public Coroutine ProgressBarRoutine { get; private set; }
    private Coroutine TextCountRoutine { get; set; }
    private Coroutine ShiftHandRoutine { get; set; }

    public static string CLOSE_SKYBAR_TIMER = "CloseSkybarTimer";
    #endregion

    #region METHODS
    #region UTILITY

    public void ProgressBarRoutine_Stop()
    {
        if (ProgressBarRoutine != null)
        {
            Managers.AU_MAN.StartStopSound("SFX_ProgressBar", null, AudioManager.SoundType.SFX, true);
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
    #endregion

    #region SYSTEMS
    /******
     * *****
     * ****** SHAKE_CAMERA
     * *****
     *****/
    public void ShakeCamera(CameraShakeInstance shake) => CameraShaker.Instance.Shake(shake);

    public static CameraShakeInstance Bump_Light
    {
        // TESTING Magnitude (normally 2.5f) Roughness (normally 4) FadeOutTime (normally 0.75)
        get => new(0.75f, 3, 0.1f, 0.5f)
        {
            PositionInfluence = Vector3.one * 0.15f,
            RotationInfluence = Vector3.one,
        };
    }
    /******
     * *****
     * ****** VALUE_CHANGE
     * *****
     *****/
    public void ValueChanger(Transform parent, int value, bool setToCanvas = true, float yBuffer = 0, float xBuffer = 0)
    {
        var valueChanger = Instantiate(valueChangerPrefab, parent);
        valueChanger.transform.localPosition = new Vector2(xBuffer, yBuffer);

        Transform newParent;
        if (yBuffer != 0)
        {
            newParent = Managers.U_MAN.UICanvas.transform;
            valueChanger.transform.localScale = new Vector2(2, 2);
        }
        else if (setToCanvas) newParent = Managers.U_MAN.CurrentCanvas.transform;
        else
        {
            newParent = parent.parent.parent.parent;
            newParent.SetAsLastSibling();
        }

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
        Color startColor = Color.white;
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
                stopDelay = 1; // TESTING
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
                startSize = 50;
                startLifetime = 5;
                //followPosition = false;
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
                startSize = 5;
                startLifetime = 0.3f;
                break;
            default:
                Debug.LogError("INVALIDE PARTICLES TYPE!");
                return null;
        }

        var particleSystem = Instantiate(prefab, Managers.U_MAN.CurrentWorldSpace.transform);
        var psh = particleSystem.GetComponent<ParticleSystemHandler>();
        psh.StartParticles(parent, startColor, startSize, startLifetime, stopDelay, usePointerPosition, followPosition);
        return psh;
    }

    /******
     * *****
     * ****** COUNTING_TEXT
     * *****
     *****/
    public class CountingTextObject
    {
        public CountingTextObject(TextMeshProUGUI text, int valueChange, Color newColor, TextMeshProUGUI altText = null, bool insertObject = false)
        {
            StartValue = int.Parse(text.text);
            currentValue = StartValue;
            Text = text;
            EndValue = StartValue + valueChange;
            this.newColor = newColor;
            defaultColor = Text.color;
            AltText = altText;

            if (insertObject) CountingTexts.Insert(0, this);
            else CountingTexts.Add(this);
        }

        private int currentValue;
        private Color newColor;
        private Color defaultColor;
        public TextMeshProUGUI Text { get; private set; }
        public int StartValue { get; private set; }
        public int EndValue { get; private set; }
        public TextMeshProUGUI AltText { get; private set; }

        public static List<CountingTextObject> CountingTexts = new List<CountingTextObject>();
        public static void ClearCountingTexts()
        {
            if (Instance.TextCountRoutine != null)
            {
                Instance.StopCoroutine(Instance.TextCountRoutine);
                Instance.TextCountRoutine = null;
            }

            foreach (var cto in CountingTexts) cto.DefaultColor();
            CountingTexts.Clear();
        }

        public bool IncrementValue()
        {
            if (currentValue < EndValue) Text.SetText(++currentValue + "");
            else Text.SetText(--currentValue + "");

            if (currentValue == EndValue) return true;
            return false;
        }
        public void NewColor()
        {
            Text.color = newColor;
            if (AltText != null) AltText.color = newColor;
        }
        public void DefaultColor()
        {
            Text.color = defaultColor;
            if (AltText != null) AltText.color = defaultColor;
        }
    }
    public void CountingText()
    {
        if (TextCountRoutine != null) // Should already be stopped
        {
            Debug.LogError("COROUTINE IS NOT NULL");
            StopCoroutine(TextCountRoutine);
            TextCountRoutine = null;
        }

        TextCountRoutine = StartCoroutine(CountingTextNumerator());
    }
    private void DefaultTextColor()
    {
        foreach (var cto in CountingTextObject.CountingTexts)
            cto.DefaultColor();
    }
    private IEnumerator CountingTextNumerator()
    {
        float delay = 0.05f;
        float interval = 0.3f;

        NewTextColor();

        while (true)
        {
            yield return new WaitForSeconds(delay);
            Managers.AU_MAN.StartStopSound("SFX_Counting");

            bool completed = true;
            foreach (var cto in CountingTextObject.CountingTexts)
            {
                if (!cto.IncrementValue()) completed = false;
            }

            if (completed) break;
        }

        DefaultTextColor();
        yield return new WaitForSeconds(interval);
        NewTextColor();
        yield return new WaitForSeconds(interval);
        DefaultTextColor();
        yield return new WaitForSeconds(interval);
        NewTextColor();
        yield return new WaitForSeconds(interval);
        DefaultTextColor();

        CountingTextObject.ClearCountingTexts();

        static void NewTextColor()
        {
            foreach (var cto in CountingTextObject.CountingTexts)
                cto.NewColor();
        }
    }

    /******
     * *****
     * ****** SET_PROGRESS_BAR
     * *****
     *****/
    public void SetProgressBar(int currentProgress, int newProgress,
        GameObject progressBar, GameObject progressFill, int controlValue = 1)
    {
        if (ProgressBarRoutine != null)
        {
            progressFill.GetComponent<Image>().color = previousBarColor;
            StopCoroutine(ProgressBarRoutine);
        }
        ProgressBarRoutine = StartCoroutine(ProgressBarNumerator(currentProgress, newProgress,
            progressBar, progressFill, controlValue));
    }

    private IEnumerator ProgressBarNumerator(int currentProgress, int newProgress,
        GameObject progressBar, GameObject progressFill, int controlValue)
    {
        Slider slider = progressBar.GetComponent<Slider>();
        slider.value = currentProgress + controlValue;
        Image image = progressFill.GetComponent<Image>();
        previousBarColor = image.color;
        image.color = Managers.U_MAN.HighlightedColor;
        Managers.AU_MAN.StartStopSound("SFX_ProgressBar", null, AudioManager.SoundType.SFX, false, true);

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
        Managers.AU_MAN.StartStopSound("SFX_ProgressBar", null, AudioManager.SoundType.SFX, true);
        ProgressBarRoutine = null;
    }
    #endregion

    #region HEROES
    /******
     * *****
     * ****** HERO_STATE_ANIMATIONS
     * *****
     *****/
    public void ModifyHeroHealthState(GameObject hero, int healthChange)
    {
        ChangeAnimationState(hero, "Modify_Health");
        var healthScore = hero.GetComponent<HeroDisplay>().HeroHealthObject;
        ValueChanger(healthScore.transform, healthChange);
    }
    public void ModifyHeroEnergyState(int energyChange, GameObject hero, bool playSound = true)
    {
        if (playSound) Managers.AU_MAN.StartStopSound("SFX_EnergyRefill");
        ChangeAnimationState(hero, "Modify_Energy");
        var energyScore = hero.GetComponent<HeroDisplay>().HeroEnergyObject;
        ValueChanger(energyScore.transform, energyChange);
    }
    public void TriggerHeroPower(GameObject heroPower) => ChangeAnimationState(heroPower, "Trigger");

    #endregion

    #region CARDS
    /******
     * *****
     * ****** CARD_STATE_ANIMATIONS
     * *****
     *****/
    public void HiddenHandState(GameObject card) =>
        ChangeAnimationState(card, "Hidden_Hand");
    public void RevealedHandState(GameObject card) =>
        ChangeAnimationState(card, "Revealed_Hand");
    public void RevealedDragState(GameObject card) =>
        ChangeAnimationState(card, "Revealed_Drag");
    public void RevealedPlayState(GameObject card) =>
        ChangeAnimationState(card, "Revealed_Play");
    public void ZoomedState(GameObject card) =>
        ChangeAnimationState(card, "Zoomed");
    public void PlayedUnitState(GameObject card) =>
        ChangeAnimationState(card, "Played_Unit");
    public void PlayedActionState(GameObject card) =>
        ChangeAnimationState(card, "Played_Action");
    public void ChangeCostState(GameObject card) =>
        ChangeAnimationState(card, "ChangeCost");

    /******
     * *****
     * ****** STAT_CHANGES
     * *****
     *****/
    private void ModifyUnitHealthState(GameObject card) => ChangeAnimationState(card, "Modify_Health");
    private void ModifyUnitPowerState(GameObject card) => ChangeAnimationState(card, "Modify_Power");
    private void ModifyAllUnitStatsState(GameObject card) => ChangeAnimationState(card, "Modify_All");
    public void DestroyUnitCardState(GameObject unitCard) => ChangeAnimationState(unitCard, "Destroyed");

    public void UnitTakeDamageState(GameObject unitCard, int damageValue, bool isMeleeAttacker)
    {
        ChangeAnimationState(unitCard.GetComponent<UnitCardDisplay>().UnitStats, "Take_Damage");
        var ucd = unitCard.GetComponent<UnitCardDisplay>();
        var stats = ucd.UnitStats;
        var healthScore = ucd.HealthScore;
        ValueChanger(healthScore.transform, -damageValue, isMeleeAttacker);
        SetAnimatorBool(stats, "IsDamaged", CombatManager.IsDamaged(unitCard));
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

        if (!isNegativeChange) Managers.AU_MAN.StartStopSound("SFX_StatPlus");
        else Managers.AU_MAN.StartStopSound("SFX_StatMinus");
        UnitStatChangeState(unitCard, powerChange, healthChange);
    }

    public void UnitStatChangeState(GameObject unitCard, int powerChange, int healthChange, bool showZero = false, bool setStatsOnly = false)
    {
        if (!setStatsOnly && !showZero && powerChange == 0 && healthChange == 0) return;

        var ucd = unitCard.GetComponent<UnitCardDisplay>();
        var stats = ucd.UnitStats;
        SetAnimatorBool(stats, "IsDamaged", CombatManager.IsDamaged(unitCard));
        SetAnimatorBool(stats, "PowerIsDebuffed", ucd.CurrentPower < ucd.UnitCard.StartPower);
        SetAnimatorBool(stats, "PowerIsBuffed", ucd.CurrentPower > ucd.UnitCard.StartPower);
        SetAnimatorBool(stats, "HealthIsBuffed", ucd.CurrentHealth > ucd.UnitCard.StartHealth);

        if (setStatsOnly) return;

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
        else if (healthChange != 0 || showZero)
        {
            ModifyUnitHealthState(stats);
            ModifyHealth();
        }

        void ModifyPower()
        {
            var powerScore = ucd.PowerScore;
            ValueChanger(powerScore.transform, powerChange);
        }
        void ModifyHealth()
        {
            var healthScore = ucd.HealthScore;
            ValueChanger(healthScore.transform, healthChange);
        }
    }

    public void AbilityTriggerState(GameObject triggerIcon)
    {
        if (triggerIcon == null)
        {
            Debug.LogWarning("ICON IS NULL!");
            return;
        }

        triggerIcon.transform.SetAsLastSibling();
        ChangeAnimationState(triggerIcon, "Trigger");
    }
    #endregion

    #region SKYBAR
    // Icon Animation
    public void SkybarIconAnimation(GameObject icon)
    {
        GameObject skybar = icon.transform.parent.parent.gameObject;

        if (skybar == Managers.U_MAN.AugmentsDropdown) Managers.U_MAN.AugmentsButton_OnClick(true);
        else if (skybar == Managers.U_MAN.ReputationsDropdown) Managers.U_MAN.ReputationsButton_OnClick(true);
        else
        {
            Trigger(); // TESTING
            return;
        }

        Trigger();
        FunctionTimer.Create(() => CloseSkybar(skybar), 1.5f, CLOSE_SKYBAR_TIMER);

        void Trigger() => ChangeAnimationState(icon, "Trigger");
        static void CloseSkybar(GameObject bar) => bar.SetActive(false);
    }

    public void TriggerAugment(string augmentName)
    {
        foreach (Transform icon in Managers.U_MAN.AugmentBar.transform)
        {
            var augmentIcon = icon.GetComponent<AugmentIcon>();
            if (augmentIcon.LoadedAugment.AugmentName == augmentName)
            {
                SkybarIconAnimation(icon.gameObject);
                Managers.AU_MAN.StartStopSound("SFX_Trigger");
                return;
            }
        }
    }
    #endregion

    #region DIALOGUE
    /******
     * *****
     * ****** DIALOGUE_INTRO
     * *****
     *****/
    public void DialogueIntro() =>
        StartCoroutine(DialogueIntroNumerator());
    private IEnumerator DialogueIntroNumerator()
    {
        Managers.AU_MAN.StartStopSound("SFX_PortraitClick");

        float distance;
        DialogueSceneDisplay dsp = Managers.D_MAN.DialogueDisplay;
        GameObject playerPortrait = dsp.PlayerHeroPortrait;
        GameObject npcPortrait = dsp.NPCHeroPortrait;
        Vector2 pPortStart = playerPortrait.transform.localPosition;
        Vector2 nPortStart = npcPortrait.transform.localPosition;
        playerPortrait.SetActive(true);
        DialoguePrompt prompt = Managers.D_MAN.EngagedHero.NextDialogueClip as DialoguePrompt;
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
        Managers.AU_MAN.StartStopSound("SFX_PortraitClick");
        float distance;
        GameObject npcPortrait = Managers.D_MAN.DialogueDisplay.NPCHeroPortrait;
        Vector2 nPortStart = npcPortrait.transform.localPosition;
        Vector2 nPortEnd = new(-600, nPortStart.y);

        do
        {
            distance = Vector2.Distance(npcPortrait.transform.localPosition, nPortEnd);
            npcPortrait.transform.localPosition =
                Vector2.MoveTowards(npcPortrait.transform.localPosition, nPortEnd, 30);
            yield return new WaitForFixedUpdate();
        }
        while (distance > 0);

        npcPortrait.SetActive(!isExitOnly);

        Managers.D_MAN.DisplayCurrentHeroes();
        yield return new WaitForSeconds(0.5f);

        do
        {
            distance = Vector2.Distance(npcPortrait.transform.localPosition, nPortStart);
            npcPortrait.transform.localPosition =
                Vector2.MoveTowards(npcPortrait.transform.localPosition, nPortStart, 30);
            yield return new WaitForFixedUpdate();
        }
        while (distance > 0);

        if (!isExitOnly) Managers.D_MAN.DisplayDialoguePopup();
        Managers.D_MAN.AllowResponse = true;
    }
    #endregion

    #region COMBAT
    /******
     * *****
     * ****** COMBAT_INTRO
     * *****
     *****/
    public void CombatIntro() => StartCoroutine(CombatIntroNumerator());
    private IEnumerator CombatIntroNumerator()
    {
        const float scaleSpeed = 0.1f;
        const float fZoomScale = 1.5f;
        const int startBuffer = 600;

        float distance;
        float fScale = 1;

        var turBut = Managers.U_MAN.EndTurnButton;
        var combatLog = Managers.U_MAN.CombatLog;

        var pHD = Managers.P_MAN.HeroObject.GetComponent<HeroDisplay>();
        var pBase = pHD.HeroBase;
        var pFrame = pHD.HeroFrame;
        var pStats = pHD.HeroStats;
        var pName = pHD.HeroNameObject;

        var eHD = Managers.EN_MAN.HeroObject.GetComponent<HeroDisplay>();
        var eBase = eHD.HeroBase;
        var eFrame = eHD.HeroFrame;
        var eStats = eHD.HeroStats;
        var eName = eHD.HeroNameObject;

        Vector2 turButStart = turBut.transform.position;
        Vector2 combatLogStart = combatLog.transform.localPosition;

        Vector2 pBaseStart = pBase.transform.localPosition;
        Vector2 pFrameStart = pFrame.transform.position;
        Vector2 pStatsStart = pStats.transform.localPosition;
        Vector2 pNameStart = pName.transform.localPosition;
        Vector2 pNameEnd = new(pNameStart.x + startBuffer, pNameStart.y);

        Vector2 eBaseStart = eBase.transform.localPosition;
        Vector2 eFrameStart = eFrame.transform.position;
        Vector2 eStatsStart = eStats.transform.localPosition;
        Vector2 eNameStart = eName.transform.localPosition;
        Vector2 eNameEnd = new(eNameStart.x - startBuffer, eNameStart.y);

        Vector2 scaleVec = new();
        turBut.SetActive(true);
        combatLog.SetActive(true);

        pBase.SetActive(true);
        pStats.SetActive(true);
        pName.SetActive(true);

        eBase.SetActive(true);
        eStats.SetActive(true);
        eName.SetActive(true);

        turBut.transform.localPosition = new Vector2(turButStart.x + startBuffer, turButStart.y);
        combatLog.transform.localPosition = new Vector2(combatLogStart.x - startBuffer, combatLogStart.y);

        pBase.transform.localPosition = new Vector2(pBaseStart.x, pBaseStart.y - startBuffer);
        pStats.transform.localPosition = new Vector2(pStatsStart.x, pStatsStart.y - startBuffer);
        pName.transform.localPosition = pNameEnd;

        eBase.transform.localPosition = new Vector2(eBaseStart.x, eBaseStart.y + startBuffer);
        eStats.transform.localPosition = new Vector2(eStatsStart.x, eStatsStart.y + startBuffer);
        eName.transform.localPosition = eNameEnd;

        Managers.P_MAN.PlayerPowerSounds();
        Managers.U_MAN.SelectTarget(Managers.P_MAN.HeroObject, UIManager.SelectionType.Highlighted);
        CreateParticleSystem(Managers.P_MAN.HeroObject, ParticleSystemHandler.ParticlesType.Drag, 2);
        CreateParticleSystem(Managers.EN_MAN.HeroObject, ParticleSystemHandler.ParticlesType.Drag, 2);

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

        Managers.U_MAN.CreateVersusPopup();
        ShakeCamera(Bump_Light);

        yield return new WaitForSeconds(0.5f);
        Managers.U_MAN.SelectTarget(Managers.P_MAN.HeroObject, UIManager.SelectionType.Disabled);

        yield return new WaitForSeconds(0.5f);
        Managers.U_MAN.SelectTarget(Managers.EN_MAN.HeroObject, UIManager.SelectionType.Highlighted);
        Managers.AU_MAN.StartStopSound(null, Managers.EN_MAN.HeroScript.HeroWin);
        FunctionTimer.Create(() =>
        Managers.U_MAN.SelectTarget(Managers.EN_MAN.HeroObject, UIManager.SelectionType.Disabled), 2);

        var eh = Managers.D_MAN.EngagedHero as EnemyHero;
        if (eh.IsBoss)
        {
            yield return new WaitForSeconds(2);
            Managers.U_MAN.CreateVersusPopup(true);
        }

        yield return ShowSkyBarChildren();

        Managers.AU_MAN.StartStopSound("SFX_PortraitClick");
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

            pBase.transform.localPosition =
                Vector2.MoveTowards(pBase.transform.localPosition, pBaseStart, 20);
            pStats.transform.localPosition =
                Vector2.MoveTowards(pStats.transform.localPosition, pStatsStart, 20);

            eBase.transform.localPosition =
                Vector2.MoveTowards(eBase.transform.localPosition, eBaseStart, 20);
            eStats.transform.localPosition =
                Vector2.MoveTowards(eStats.transform.localPosition, eStatsStart, 20);
            combatLog.transform.localPosition =
                Vector2.MoveTowards(combatLog.transform.localPosition, combatLogStart, 20);
            yield return new WaitForFixedUpdate();
        }
        while (distance > 0);
    }

    public IEnumerator ShowSkyBarChildren(float delay = 0)
    {
        yield return new WaitForSeconds(delay);

        // Reputation
        foreach (Transform repTran in Managers.U_MAN.ReputationBar.transform)
        {
            repTran.gameObject.SetActive(true);
            SkybarIconAnimation(repTran.gameObject);
        }

        Managers.AU_MAN.StartStopSound("SFX_Trigger");
        yield return new WaitForSeconds(0.5f);

        // Augments
        foreach (Transform augTran in Managers.U_MAN.AugmentBar.transform)
        {
            augTran.gameObject.SetActive(true);
            SkybarIconAnimation(augTran.gameObject);
        }

        if (Managers.U_MAN.AugmentBar.transform.childCount > 0)
        {
            Managers.AU_MAN.StartStopSound("SFX_Trigger");
            yield return new WaitForSeconds(0.5f);
        }

        // Items
        foreach (Transform itemTran in Managers.U_MAN.ItemBar.transform)
        {
            itemTran.gameObject.SetActive(true);
            SkybarIconAnimation(itemTran.gameObject);
        }

        if (Managers.U_MAN.ItemBar.transform.childCount > 0)
        {
            Managers.AU_MAN.StartStopSound("SFX_Trigger");
            yield return new WaitForSeconds(0.5f);
        }
    }

    /******
     * *****
     * ****** UNIT_ATTACK
     * *****
     *****/
    public void UnitAttack(GameObject attacker, GameObject defender, bool defenderIsUnit) =>
        StartCoroutine(AttackNumerator(attacker, defender, defenderIsUnit));

    private IEnumerator AttackNumerator(GameObject attacker,
        GameObject defender, bool defenderIsUnit = true)
    {
        const float minSpeed = 100;
        const float maxSpeed = 200;

        float distance;
        float bufferDistance = defenderIsUnit ? 150 : 350;
        var container = attacker.GetComponent<CardDisplay>().CardContainer;
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

        Managers.AU_MAN.PlayAttackSound(attacker);
        Managers.CO_MAN.Strike(attacker, defender, true, true);
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
        //attacker.transform.SetAsFirstSibling();
        Managers.EV_MAN.PauseDelayedActions(false);

        static float GetCurrentSpeed(float distance)
        {
            float speed = maxSpeed - (distance * 0.5f);
            if (speed < minSpeed) speed = minSpeed;
            else if (speed > maxSpeed) speed = maxSpeed;
            return speed;
        }
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
        float distance;
        float yTarget = isUpShift ? -350 : Managers.P_MAN.HandStart.y;

        Vector2 target = new(0, yTarget);
        var hand = Managers.P_MAN.HandZone;
        Managers.AU_MAN.StartStopSound("SFX_ShiftHand");

        foreach (var card in Managers.P_MAN.HandZoneCards)
            card.transform.SetAsLastSibling();

        foreach (var card in Managers.P_MAN.PlayZoneCards)
        {
            var ucd = card.GetComponent<UnitCardDisplay>();
            if (isUpShift) ucd.DisableVFX();
            else ucd.EnableVFX();
        }

        do
        {
            distance = Vector2.Distance(hand.transform.position, target);
            hand.transform.position = Vector2.MoveTowards(hand.transform.position, target, 30);
            yield return new WaitForFixedUpdate();
        }
        while (distance > 0);
        Managers.U_MAN.DestroyZoomObjects();
    }
    #endregion
    #endregion
}