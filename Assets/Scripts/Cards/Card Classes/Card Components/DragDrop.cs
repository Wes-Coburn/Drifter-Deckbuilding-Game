﻿using UnityEngine;

public class DragDrop : MonoBehaviour
{
    private PlayerManager pMan;
    private CombatManager coMan;
    private UIManager uMan;
    private AudioManager auMan;
    private GameObject dragArrow;
    private bool isDragging;
    private bool isOverDropZone;
    //private int startIndex;
    private const string SFX_DRAG_CARD = "SFX_DragCard";

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
        isOverDropZone = false;
        isDragging = false;
        IsPlayed = false;
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 dragPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);            
            transform.position = new Vector2(dragPoint.x, dragPoint.y);
            transform.SetParent(uMan.CurrentCanvas.transform, true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDragging && !IsPlayed)
            if (collision.gameObject == coMan.PlayerZone)
            {
                isOverDropZone = true;
                uMan.SetPlayerZoneOutline(true, true);
            }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (isDragging && !IsPlayed)
            if (collision.gameObject == coMan.PlayerZone)
            {
                isOverDropZone = false;
                uMan.SetPlayerZoneOutline(true, false);
            }
    }

    private void ResetPosition()
    {
        GameObject container = GetComponent<CardDisplay>().CardContainer;
        transform.SetParent(container.transform, false);
        transform.localPosition = new Vector2(0, 0);
        if (TryGetComponent(out ActionCardDisplay _)) IsPlayed = false;
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

        if (!IsPlayed)
        {
            isDragging = true;
            AnimationManager.Instance.RevealedDragState(gameObject);
            uMan.SetPlayerZoneOutline(true, false);
        }
        else
        {
            if (!coMan.CanAttack(gameObject, null, true))
            {
                DraggingCard = null;
                return;
            }
            ArrowIsDragging = true;
            if (dragArrow != null) Destroy(dragArrow);
            dragArrow = Instantiate(coMan.DragArrowPrefab, 
                uMan.CurrentCanvas.transform);
            dragArrow.GetComponent<DragArrow>().SourceCard = gameObject;
            foreach (GameObject enemyUnit in coMan.EnemyZoneCards)
                if (coMan.CanAttack(gameObject, enemyUnit, true))
                    uMan.SelectTarget(enemyUnit, true);
            if (coMan.CanAttack(gameObject, coMan.EnemyHero, true)) 
                uMan.SelectTarget(coMan.EnemyHero, true);
        }
        auMan.StartStopSound(SFX_DRAG_CARD, null, AudioManager.SoundType.SFX, false, true);
    }

    public void EndDrag()
    {
        if (!isDragging && !ArrowIsDragging) return;
        auMan.StartStopSound(SFX_DRAG_CARD, null, AudioManager.SoundType.SFX, true);
        DraggingCard = null;
        // From Hand
        if (!IsPlayed)
        {
            isDragging = false;
            uMan.SetPlayerZoneOutline(false, false);
            if (isOverDropZone && coMan.IsPlayable(gameObject))
            {
                IsPlayed = true;
                coMan.PlayCard(gameObject);
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
                coMan.Attack(gameObject, enemy), 0.5f);
            }
            uMan.SelectTarget(Enemy, false);
            Enemy = null;
        }

        foreach (GameObject enemyUnit in coMan.EnemyZoneCards)
            uMan.SelectTarget(enemyUnit, false);
        uMan.SelectTarget(coMan.EnemyHero, false);
    }
}