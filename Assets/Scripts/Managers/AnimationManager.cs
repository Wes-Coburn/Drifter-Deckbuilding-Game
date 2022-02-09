using System.Collections;
using UnityEngine;

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
    private AnimationManager anMan;

    private Vector2 playerHandStart;

    private void Start()
    {
        uMan = UIManager.Instance;
        coMan = CombatManager.Instance;
        dMan = DialogueManager.Instance;
        auMan = AudioManager.Instance;
        anMan = AnimationManager.Instance;
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
                anim.SetBool(boolName, animBool); // TESTING
                return;
            }
        Debug.LogError("ANIMATOR NOT FOUND!");
    }

    /* HERO_ANIMATIONS */
    public void ModifyHeroHealthState(GameObject hero) =>
        ChangeAnimationState(hero, "Modify_Health");
    public void ModifyHeroEnergyState()
    {
        auMan.StartStopSound("SFX_EnergyRefill");
        ChangeAnimationState(coMan.PlayerHero, "Modify_Energy");
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
    /* UNIT_ANIMATIONS */
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

    // Stat Changes
    public void UnitTakeDamageState(GameObject unitCard) =>
        ChangeAnimationState(unitCard.GetComponent<UnitCardDisplay>().UnitStats, "Take_Damage");
    public void DestroyUnitCardState(GameObject unitCard) =>
        ChangeAnimationState(unitCard, "Destroyed");
    public void UnitStatChangeState(GameObject unitCard,
        bool isPowerChange, bool isHealthChange)
    {
        if (!coMan.IsUnitCard(unitCard))
        {
            Debug.LogError("TARGET IS NOT UNIT CARD!");
            return;
        }

        if (!isPowerChange && !isHealthChange) return;
        GameObject stats = unitCard.GetComponent<UnitCardDisplay>().UnitStats;
        SetAnimatorBool(stats, "IsDamaged", coMan.IsDamaged(unitCard));

        if (isPowerChange)
        {
            if (isHealthChange) ModifyAllUnitStatsState(stats);
            else ModifyUnitPowerState(stats);
        }
        else if (isHealthChange) ModifyUnitHealthState(stats);
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

        GameObject pFrame = pHD.HeroFrame;
        GameObject eFrame = eHD.HeroFrame;
        GameObject pStats = pHD.HeroStats;
        GameObject eStats = eHD.HeroStats;
        GameObject turBut = uMan.EndTurnButton;

        Vector2 turButStart = turBut.transform.position;
        Vector2 pFrameStart = pFrame.transform.position;
        Vector2 eFrameStart = eFrame.transform.position;
        Vector2 pStatsStart = pStats.transform.localPosition;
        Vector2 eStatsStart = eStats.transform.localPosition;
        float scaleSpeed = 0.1f;
        float fScale = 1;
        float fZoomScale = 1.5f;
        Vector2 scaleVec = new Vector2();
        turBut.SetActive(true);
        pStats.SetActive(true);
        eStats.SetActive(true);
        turBut.transform.localPosition = new Vector2(turButStart.x + 450, turButStart.y);
        pStats.transform.localPosition = new Vector2(pStatsStart.x, pStatsStart.y - 450);
        eStats.transform.localPosition = new Vector2(eStatsStart.x, eStatsStart.y + 450);

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

        // TESTING TESTING TESTING
        yield return new WaitForSeconds(1);
        uMan.SelectTarget(coMan.PlayerHero, true);
        PlayerManager.Instance.PlayerPowerSounds();
        yield return new WaitForSeconds(2);
        uMan.SelectTarget(coMan.PlayerHero, false);
        uMan.SelectTarget(coMan.EnemyHero, true);
        Sound enemyWinSound = EnemyManager.Instance.EnemyHero.HeroWin;
        auMan.StartStopSound(null, enemyWinSound);
        FunctionTimer.Create(() => uMan.SelectTarget(coMan.EnemyHero, false), 2);
        // TESTING TESTING TESTING

        EnemyHero eh = dMan.EngagedHero as EnemyHero;
        if (eh.IsBoss)
        {
            yield return new WaitForSeconds(2);
            uMan.CreateVersusPopup(true);
        }
        
        float delay = 3;
        foreach (Transform augTran in uMan.AugmentBar.transform)
        {
            augTran.gameObject.SetActive(true);
            anMan.SkybarIconAnimation(augTran.gameObject);
            auMan.StartStopSound("SFX_Trigger");
            yield return new WaitForSeconds(0.5f);
            delay -= 0.5f;
        }

        foreach (Transform itemTran in uMan.ItemBar.transform)
        {
            itemTran.gameObject.SetActive(true);
            anMan.SkybarIconAnimation(itemTran.gameObject);
            auMan.StartStopSound("SFX_Trigger");
            yield return new WaitForSeconds(0.5f);
            delay -= 0.5f;
        }

        if (delay > 0) yield return new WaitForSeconds(delay);
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

        // RETREAT
        do
        {
            distance = Vector2.Distance(attacker.transform.position, container.transform.position);
            attacker.transform.position =
                Vector3.MoveTowards(attacker.transform.position,
                container.transform.position,
                GetCurrentSpeed(distance));
            yield return new WaitForFixedUpdate();
        }
        while (distance > 0);
        container.GetComponent<CardContainer>().IsDetached = false;
        EventManager.Instance.PauseDelayedActions(false);
    }

    private readonly float minSpeed = 100;
    private readonly float maxSpeed = 200;
    //private readonly float speedControl = 0.05f;

    private float GetCurrentSpeed(float distance)
    {
        float speed = maxSpeed - (distance * 0.5f);
        //float speed = distance * speedControl;
        if (speed < minSpeed) speed = minSpeed;
        else if (speed > maxSpeed) speed = maxSpeed;
        return speed;
    }
}