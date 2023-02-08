using UnityEngine;

public class DragDrop : MonoBehaviour
{
    private CardContainer container;
    private GameObject dragArrow;
    private ParticleSystemHandler particleHandler;
    private bool isOverDropZone;
    private bool isDragging;

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

    public int LastIndex { get; private set; }
    public bool IsPlayed { get; set; }

    public static GameObject DraggingCard;
    public static GameObject Enemy;
    public static bool ArrowIsDragging;

    void Start()
    {
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
            if (collision.gameObject == ManagerHandler.P_MAN.PlayZone)
            {
                isOverDropZone = true;
                ManagerHandler.U_MAN.SetPlayerZoneOutline(true, true);
            }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (IsDragging && !IsPlayed)
            if (collision.gameObject == ManagerHandler.P_MAN.PlayZone)
            {
                isOverDropZone = false;
                ManagerHandler.U_MAN.SetPlayerZoneOutline(true, false);
            }
    }

    private void ResetPosition()
    {
        transform.SetParent(CombatManager.Instance.CardZone.transform);

        transform.localPosition = container.transform.position;
        transform.SetSiblingIndex(LastIndex);
        IsPlayed = false;
        AnimationManager.Instance.RevealedHandState(gameObject);
    }

    public void StartDrag()
    {
        ManagerHandler.U_MAN.DestroyZoomObjects();
        if (!ManagerHandler.P_MAN.IsMyTurn || CompareTag(ManagerHandler.EN_MAN.CARD_TAG) || DraggingCard != null ||
            ArrowIsDragging || EffectManager.Instance.EffectsResolving ||
            EventManager.Instance.ActionsDelayed) return;

        FunctionTimer.StopTimer(CardZoom.ZOOM_CARD_TIMER);
        FunctionTimer.StopTimer(CardZoom.ABILITY_POPUP_TIMER);
        DraggingCard = gameObject;

        if (!IsPlayed)
        {
            IsDragging = true;
            LastIndex = transform.GetSiblingIndex();
            transform.SetParent(ManagerHandler.U_MAN.CurrentZoomCanvas.transform);

            ManagerHandler.U_MAN.SetPlayerZoneOutline(true, false);
            AnimationManager.Instance.RevealedDragState(gameObject);
            particleHandler = ManagerHandler.AN_MAN.CreateParticleSystem(gameObject,
                ParticleSystemHandler.ParticlesType.Drag);
        }
        else
        {
            if (!ManagerHandler.CO_MAN.CanAttack(gameObject, null))
            {
                DraggingCard = null;
                return;
            }

            ArrowIsDragging = true;
            if (dragArrow != null) Destroy(dragArrow);
            dragArrow = Instantiate(ManagerHandler.CA_MAN.DragArrowPrefab, ManagerHandler.U_MAN.CurrentCanvas.transform);
            dragArrow.GetComponent<DragArrow>().SourceCard = gameObject;

            foreach (GameObject enemyUnit in ManagerHandler.EN_MAN.PlayZoneCards)
                if (ManagerHandler.CO_MAN.CanAttack(gameObject, enemyUnit))
                    ManagerHandler.U_MAN.SelectTarget(enemyUnit, UIManager.SelectionType.Highlighted);

            foreach (GameObject playerUnit in ManagerHandler.P_MAN.PlayZoneCards)
                ManagerHandler.U_MAN.SelectTarget(playerUnit, UIManager.SelectionType.Disabled);

            if (ManagerHandler.CO_MAN.CanAttack(gameObject, ManagerHandler.EN_MAN.HeroObject))
                ManagerHandler.U_MAN.SelectTarget(ManagerHandler.EN_MAN.HeroObject, UIManager.SelectionType.Highlighted);

            particleHandler = ManagerHandler.AN_MAN.CreateParticleSystem(gameObject,
                ParticleSystemHandler.ParticlesType.MouseDrag);
        }
        ManagerHandler.AU_MAN.StartStopSound(SFX_DRAG_CARD, null, AudioManager.SoundType.SFX, false, true);
    }

    public void EndDrag()
    {
        if (!IsDragging && !ArrowIsDragging) return;

        DraggingCard = null;
        ManagerHandler.AU_MAN.StartStopSound(SFX_DRAG_CARD, null, AudioManager.SoundType.SFX, true);

        if (particleHandler != null)
        {
            particleHandler.StopParticles();
            particleHandler = null;
        }

        // From Hand
        if (!IsPlayed)
        {
            IsDragging = false;
            ManagerHandler.U_MAN.SetPlayerZoneOutline(false, false);

            if (isOverDropZone && ManagerHandler.CA_MAN.IsPlayable(gameObject))
            {
                // TUTORIAL!
                if (ManagerHandler.G_MAN.IsTutorial)
                {
                    switch (ManagerHandler.P_MAN.EnergyPerTurn)
                    {
                        case 1:
                            ManagerHandler.G_MAN.Tutorial_Tooltip(3);
                            break;
                        case 2:
                            if (!ManagerHandler.P_MAN.HeroPowerUsed)
                            {
                                ResetPosition();
                                return;
                            }
                            break;
                    }
                }

                IsPlayed = true;
                ManagerHandler.CA_MAN.PlayCard(gameObject);
                transform.SetParent(ManagerHandler.CO_MAN.CardZone.transform);
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
            if (ManagerHandler.CO_MAN.CanAttack(gameObject, Enemy, false))
            {
                GameObject enemy = Enemy;
                EventManager.Instance.NewDelayedAction(() =>
                ManagerHandler.CO_MAN.Attack(gameObject, enemy), 0.25f);
            }
            ManagerHandler.U_MAN.SelectTarget(Enemy, UIManager.SelectionType.Disabled);
            Enemy = null;
        }


        foreach (GameObject enemyUnit in ManagerHandler.EN_MAN.PlayZoneCards)
            ManagerHandler.U_MAN.SelectTarget(enemyUnit, UIManager.SelectionType.Disabled);

        ManagerHandler.U_MAN.SelectTarget(ManagerHandler.EN_MAN.HeroObject, UIManager.SelectionType.Disabled);
        ManagerHandler.CA_MAN.SelectPlayableCards(true);
        ManagerHandler.CA_MAN.SelectPlayableCards();
    }
}