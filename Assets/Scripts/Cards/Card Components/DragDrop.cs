using UnityEngine;

public class DragDrop : MonoBehaviour
{
    private PlayerManager pMan;
    private CardManager cMan;
    private UIManager uMan;
    private AudioManager auMan;
    private GameObject dragArrow;
    private bool isDragging;
    private bool isOverDropZone;
    private GameObject startParent;
    private Vector2 startPosition;
    private int startIndex;
    private const string SFX_DRAG_CARD = "SFX_DragCard";

    [SerializeField] private GameObject dragArrowPrefab;

    public bool IsPlayed { get; set; }
    public static GameObject DraggingCard;
    public static bool ArrowIsDragging;
    public static GameObject Enemy;

    void Start()
    {
        pMan = PlayerManager.Instance;
        cMan = CardManager.Instance;
        uMan = UIManager.Instance;
        auMan = AudioManager.Instance;
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
            if (collision.gameObject == cMan.PlayerZone)
            {
                isOverDropZone = true;
                uMan.SetPlayerZoneOutline(true, true);
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (isDragging && !IsPlayed)
        {
            if (collision.gameObject == cMan.PlayerZone)
            {
                isOverDropZone = false;
                uMan.SetPlayerZoneOutline(true, false);
            }
        }
    }

    private void ResetPosition()
    {
        cMan.SetCardParent(gameObject, startParent.transform);
        transform.SetSiblingIndex(startIndex);
        transform.position = new Vector3(startPosition.x, startPosition.y, CardManager.CARD_Z_POSITION);

        if (GetComponent<CardDisplay>() is ActionCardDisplay) IsPlayed = false;
        if (IsPlayed) AnimationManager.Instance.RevealedPlayState(gameObject);
        else AnimationManager.Instance.RevealedHandState(gameObject);
    }

    public void StartDrag()
    {
        uMan.DestroyZoomObjects();
        if (DraggingCard != null || ArrowIsDragging || !pMan.IsMyTurn || 
            CompareTag(CardManager.ENEMY_CARD) || uMan.PlayerIsTargetting) return;

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
            if (!cMan.CanAttack(gameObject, null))
            {
                DraggingCard = null;
                return;
            }
            ArrowIsDragging = true;
            if (dragArrow != null) Destroy(dragArrow);
            dragArrow = Instantiate(dragArrowPrefab, UIManager.Instance.CurrentWorldSpace.transform);
            dragArrow.GetComponent<DragArrow>().SourceCard = gameObject;

            foreach (GameObject enemyUnit in CardManager.Instance.EnemyZoneCards)
                if (cMan.CanAttack(gameObject, enemyUnit, true))
                    uMan.SelectTarget(enemyUnit, true);
            
            if (cMan.CanAttack(gameObject, cMan.EnemyHero, true)) 
                uMan.SelectTarget(cMan.EnemyHero, true);
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
            if (isOverDropZone && cMan.IsPlayable(gameObject))
            {
                IsPlayed = true;
                cMan.PlayCard(gameObject);
            }
            else ResetPosition();
            return;
        }

        // In Play
        ArrowIsDragging = false;
        Destroy(dragArrow);
        dragArrow = null;

        if (Enemy != null && cMan.CanAttack(gameObject, Enemy))
            cMan.Attack(gameObject, Enemy);
        else
        {
            Debug.Log("EndDrag! (NO ATTACK) IsExhausted = " + 
                GetComponent<UnitCardDisplay>().IsExhausted);
        }

        foreach (GameObject enemyUnit in cMan.EnemyZoneCards)
            uMan.SelectTarget(enemyUnit, false);
        
        uMan.SelectTarget(cMan.EnemyHero, false);
    }
}