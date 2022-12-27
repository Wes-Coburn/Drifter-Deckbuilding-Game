using UnityEngine;

public class DragDrop : MonoBehaviour
{
    private PlayerManager pMan;
    private CombatManager coMan;
    private UIManager uMan;
    private AudioManager auMan;
    private AnimationManager anMan;
    private GameManager gMan;

    private CardContainer container;
    private GameObject dragArrow;
    private bool isOverDropZone;
    private bool isDragging;
    private int lastIndex;
    private ParticleSystemHandler particleHandler;

    private const string SFX_DRAG_CARD = "SFX_DragCard";

    private bool IsDragging
    {
        get => isDragging;
        set
        {
            isDragging = value;
            container.IsDetached = isDragging;
        }
    }

    public int LastIndex { get => lastIndex; }
    public bool IsPlayed { get; set; }

    public static GameObject DraggingCard;
    public static bool ArrowIsDragging;
    public static GameObject Enemy;

    void Start()
    {
        pMan = PlayerManager.Instance;
        coMan = CombatManager.Instance;
        uMan = UIManager.Instance;
        auMan = AudioManager.Instance;
        anMan = AnimationManager.Instance;
        gMan = GameManager.Instance;

        CardDisplay cd = GetComponent<CardDisplay>();
        if (cd.CardContainer != null)
            container = cd.CardContainer.GetComponent<CardContainer>();
        isOverDropZone = false;
        isDragging = false;
        IsPlayed = false;
    }

    void FixedUpdate()
    {
        if (IsDragging)
        {
            Vector3 dragPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);            
            transform.position = new Vector2(dragPoint.x, dragPoint.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsDragging && !IsPlayed)
            if (collision.gameObject == coMan.PlayerZone)
            {
                isOverDropZone = true;
                uMan.SetPlayerZoneOutline(true, true);
            }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (IsDragging && !IsPlayed)
            if (collision.gameObject == coMan.PlayerZone)
            {
                isOverDropZone = false;
                uMan.SetPlayerZoneOutline(true, false);
            }
    }

    private void ResetPosition()
    {
        transform.SetParent(coMan.CardZone.transform); // TESTING

        transform.localPosition = container.transform.position;
        transform.SetSiblingIndex(lastIndex);
        IsPlayed = false;
        AnimationManager.Instance.RevealedHandState(gameObject);
    }

    public void StartDrag()
    {
        uMan.DestroyZoomObjects();
        if (!pMan.IsMyTurn) return;
        if (CompareTag(CombatManager.ENEMY_CARD)) return;
        if (DraggingCard != null || ArrowIsDragging) return;

        if (EffectManager.Instance.EffectsResolving ||
            EventManager.Instance.ActionsDelayed) return;

        FunctionTimer.StopTimer(CardZoom.ZOOM_CARD_TIMER);
        FunctionTimer.StopTimer(CardZoom.ABILITY_POPUP_TIMER);
        DraggingCard = gameObject;
        //Enemy = null; // Unnecessary

        if (!IsPlayed)
        {
            IsDragging = true;
            AnimationManager.Instance.RevealedDragState(gameObject);
            uMan.SetPlayerZoneOutline(true, false);
            particleHandler = anMan.CreateParticleSystem(gameObject,
                ParticleSystemHandler.ParticlesType.Drag);

            lastIndex = transform.GetSiblingIndex();
            transform.SetParent(uMan.CurrentZoomCanvas.transform);
        }
        else
        {
            if (!coMan.CanAttack(gameObject, null))
            {
                DraggingCard = null;
                return;
            }

            ArrowIsDragging = true;
            if (dragArrow != null) Destroy(dragArrow);
            dragArrow = Instantiate(CardManager.Instance.DragArrowPrefab, uMan.CurrentCanvas.transform);
            dragArrow.GetComponent<DragArrow>().SourceCard = gameObject;

            foreach (GameObject enemyUnit in coMan.EnemyZoneCards)
                if (coMan.CanAttack(gameObject, enemyUnit))
                    uMan.SelectTarget(enemyUnit, UIManager.SelectionType.Highlighted);

            foreach (GameObject playerUnit in coMan.PlayerZoneCards)
                uMan.SelectTarget(playerUnit, UIManager.SelectionType.Disabled);

            if (coMan.CanAttack(gameObject, coMan.EnemyHero)) 
                uMan.SelectTarget(coMan.EnemyHero, UIManager.SelectionType.Highlighted);

            particleHandler = anMan.CreateParticleSystem(gameObject,
                ParticleSystemHandler.ParticlesType.MouseDrag);
        }
        auMan.StartStopSound(SFX_DRAG_CARD, null,
            AudioManager.SoundType.SFX, false, true);
    }

    public void EndDrag()
    {
        if (!IsDragging && !ArrowIsDragging) return;
        auMan.StartStopSound(SFX_DRAG_CARD, null, AudioManager.SoundType.SFX, true);
        DraggingCard = null;

        if (particleHandler != null)
        {
            particleHandler.StopParticles();
            particleHandler = null;
        }

        // From Hand
        if (!IsPlayed)
        {
            IsDragging = false;
            uMan.SetPlayerZoneOutline(false, false);
            if (isOverDropZone && coMan.IsPlayable(gameObject))
            {
                // TUTORIAL!
                if (gMan.IsTutorial)
                {
                    switch (pMan.EnergyPerTurn)
                    {
                        case 1:
                            gMan.Tutorial_Tooltip(3);
                            break;
                        case 2:
                            ResetPosition();
                            return;
                    }
                }

                IsPlayed = true;
                coMan.PlayCard(gameObject);
                transform.SetParent(coMan.CardZone.transform); // TESTING
            }
            else ResetPosition();
            return;
        }

        // From Play
        ArrowIsDragging = false;
        Destroy(dragArrow);
        dragArrow = null;
        if (Enemy != null)
        {
            if (coMan.CanAttack(gameObject, Enemy, false))
            {
                GameObject enemy = Enemy;
                EventManager.Instance.NewDelayedAction(() =>
                coMan.Attack(gameObject, enemy), 0.25f);
            }
            uMan.SelectTarget(Enemy, UIManager.SelectionType.Disabled);
            Enemy = null;
        }

        
        foreach (GameObject enemyUnit in coMan.EnemyZoneCards)
            uMan.SelectTarget(enemyUnit, UIManager.SelectionType.Disabled);

        uMan.SelectTarget(coMan.EnemyHero, UIManager.SelectionType.Disabled);
        coMan.SelectPlayableCards(true);
        coMan.SelectPlayableCards();
    }
}