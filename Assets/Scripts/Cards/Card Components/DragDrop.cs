using UnityEngine;

public class DragDrop : MonoBehaviour
{
    /* MANAGERS */
    private PlayerManager playerManager;
    private CardManager cardManager;
    private UIManager UIManager;

    /* DRAG ARROW */
    [SerializeField] private GameObject dragArrowPrefab;
    private GameObject dragArrow;

    private bool isDragging;
    private bool isOverDropZone;
    private GameObject startParent;
    private Vector2 startPosition;
    private int startIndex;

    public bool IsPlayed { get; set; }
    public static GameObject DraggingCard;
    public static bool ArrowIsDragging;
    public static GameObject Enemy;

    void Start()
    {
        playerManager = PlayerManager.Instance;
        cardManager = CardManager.Instance;
        UIManager = UIManager.Instance;
        isOverDropZone = false;
        isDragging = false;
        IsPlayed = false;
        DraggingCard = null;
        ArrowIsDragging = false;
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
        {
            if (collision.gameObject == cardManager.PlayerZone)
            {
                isOverDropZone = true;
                UIManager.SetPlayerZoneOutline(true, true);
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (isDragging && !IsPlayed)
        {
            if (collision.gameObject == cardManager.PlayerZone)
            {
                isOverDropZone = false;
                UIManager.SetPlayerZoneOutline(true, false);
            }
        }
    }

    private void ResetPosition()
    {
        cardManager.SetCardParent(gameObject, startParent.transform);
        transform.SetSiblingIndex(startIndex);
        transform.position = new Vector3(startPosition.x, startPosition.y, CardManager.CARD_Z_POSITION);

        if (GetComponent<CardDisplay>() is ActionCardDisplay) IsPlayed = false;
        if (IsPlayed) AnimationManager.Instance.RevealedPlayState(gameObject);
        else AnimationManager.Instance.RevealedHandState(gameObject);
    }

    public void StartDrag()
    {
        UIManager.DestroyZoomObjects();
        if (DraggingCard != null || ArrowIsDragging || !playerManager.IsMyTurn || 
            CompareTag(CardManager.ENEMY_CARD) || UIManager.PlayerIsTargetting) return;

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
            UIManager.SetPlayerZoneOutline(true, false);
        }
        else
        {
            if (!cardManager.CanAttack(gameObject, null))
            {
                DraggingCard = null;
                return;
            }
            ArrowIsDragging = true;
            if (dragArrow != null) Destroy(dragArrow);
            dragArrow = Instantiate(dragArrowPrefab, UIManager.Instance.CurrentWorldSpace.transform);
            dragArrow.GetComponent<DragArrow>().SourceCard = gameObject;

            foreach (GameObject enemyUnit in CardManager.Instance.EnemyZoneCards)
                if (cardManager.CanAttack(gameObject, enemyUnit, true))
                    UIManager.SelectTarget(enemyUnit, true);
            
            if (cardManager.CanAttack(gameObject, cardManager.EnemyHero, true)) 
                UIManager.SelectTarget(cardManager.EnemyHero, true);
        }
    }

    public void EndDrag()
    {
        if (!isDragging && !ArrowIsDragging) return;

        DraggingCard = null;
        // From Hand
        if (!IsPlayed)
        {
            isDragging = false;
            UIManager.SetPlayerZoneOutline(false, false);
            if (isOverDropZone && cardManager.IsPlayable(gameObject))
            {
                IsPlayed = true;
                cardManager.PlayCard(gameObject);
            }
            else ResetPosition();
            return;
        }

        // In Play
        ArrowIsDragging = false;
        Destroy(dragArrow);
        dragArrow = null;

        if (Enemy != null && cardManager.CanAttack(gameObject, Enemy))
            cardManager.Attack(gameObject, Enemy);
        else
        {
            Debug.Log("EndDrag! (NO ATTACK) IsExhausted = " + 
                GetComponent<UnitCardDisplay>().IsExhausted);
        }

        foreach (GameObject enemyUnit in cardManager.EnemyZoneCards)
            UIManager.SelectTarget(enemyUnit, false);
        
        UIManager.SelectTarget(cardManager.EnemyHero, false);
    }
}