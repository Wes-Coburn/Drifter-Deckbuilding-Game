using UnityEngine;

public class DragDrop : MonoBehaviour
{
    private PlayerManager pMan;
    private CombatManager coMan;
    private UIManager uMan;
    private AudioManager auMan;
    private GameObject dragArrow;
    private bool isDragging;
    private bool isOverDropZone;
    private GameObject startParent;
    private Vector2 startPosition;
    private int startIndex;
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
            transform.position = new Vector3(dragPoint.x, dragPoint.y, -2);
            transform.SetParent(UIManager.Instance.CurrentWorldSpace.transform, true);
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
        coMan.SetCardParent(gameObject, startParent.transform);
        transform.SetSiblingIndex(startIndex);
        transform.position = new Vector3(startPosition.x, startPosition.y, CombatManager.CARD_Z_POSITION);
        if (TryGetComponent(out ActionCardDisplay _)) IsPlayed = false;
        AnimationManager.Instance.RevealedHandState(gameObject);
        GetComponent<ChangeLayer>().HandLayer(); // TESTING
    }

    public void StartDrag()
    {
        uMan.DestroyZoomObjects();
        if (DraggingCard != null || ArrowIsDragging || !pMan.IsMyTurn || 
            CompareTag(CombatManager.ENEMY_CARD) || uMan.PlayerIsTargetting) return;
        FunctionTimer.StopTimer(CardZoom.ZOOM_CARD_TIMER);
        FunctionTimer.StopTimer(CardZoom.ABILITY_POPUP_TIMER);
        DraggingCard = gameObject;
        if (!IsPlayed)
        {
            isDragging = true;
            startParent = transform.parent.gameObject;
            startPosition = transform.position;
            startIndex = transform.GetSiblingIndex();
            GetComponent<ChangeLayer>().ZoomLayer();
            AnimationManager.Instance.RevealedDragState(gameObject);
            uMan.SetPlayerZoneOutline(true, false);
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
            dragArrow = Instantiate(CombatManager.Instance.DragArrowPrefab, 
                UIManager.Instance.CurrentWorldSpace.transform);
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
        if (Enemy != null && coMan.CanAttack(gameObject, Enemy))
            coMan.Attack(gameObject, Enemy);
        else
        {
            Debug.Log("EndDrag! (NO ATTACK) IsExhausted = " + 
                GetComponent<UnitCardDisplay>().IsExhausted);
        }
        foreach (GameObject enemyUnit in coMan.EnemyZoneCards)
            uMan.SelectTarget(enemyUnit, false);
        uMan.SelectTarget(coMan.EnemyHero, false);
    }
}