using UnityEngine;

public class DragDrop : MonoBehaviour
{
    /* CARD_MANAGER_DATA */
    private const string ENEMY_CARD = CardManager.ENEMY_CARD;

    /* MANAGERS */
    private PlayerManager playerManager;
    private CardManager cardManager;
    private UIManager UIManager;

    /* STATIC CLASS VARIABLES */
    public static bool CardIsDragging;

    /* CLASS VARIABLES */
    private GameObject enemy;
    private bool isOverEnemy;
    private bool isOverDropZone;
    private GameObject startParent;
    private Vector2 startPosition;
    private int startIndex;

    private bool isDragging;
    public bool IsDragging
    {
        get => isDragging;
        private set
        {
            isDragging = value;
            CardIsDragging = IsDragging;
        }
    }
    public bool IsPlayed { get; set; }

    void Start()
    {
        playerManager = PlayerManager.Instance;
        cardManager = CardManager.Instance;
        UIManager = UIManager.Instance;

        CardIsDragging = false;
        isOverEnemy = false;
        isOverDropZone = false;
        IsDragging = false;
        IsPlayed = false;
    }

    void Update()
    {
        if (IsDragging)
        {
            Vector3 dragPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(dragPoint.x, dragPoint.y, -2);
            transform.SetParent(UIManager.Instance.CurrentWorldSpace.transform, true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsDragging) return;
        GameObject collisionObject = collision.gameObject;
        GameObject collisionObjectParent = collisionObject.transform.parent.gameObject;
        if (!IsPlayed)
        {
            if (collisionObject == cardManager.PlayerZone) 
                isOverDropZone = true;
        }
        else
        {
            if (collisionObjectParent == cardManager.EnemyZone || 
                collisionObject == cardManager.EnemyHero)
            {
                isOverEnemy = true;
                enemy = collisionObject;
                UIManager.SelectEnemy(enemy, true, true);
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        GameObject collisionObject = collision.gameObject;
        if (!IsPlayed)
        {
            if (collisionObject == cardManager.PlayerZone) 
                isOverDropZone = false;
        }
        else
        {
            if (collisionObject == enemy)
            {
                if (cardManager.CanAttack(gameObject, enemy, true)) 
                    UIManager.Instance.SelectEnemy(enemy, true);
                
                isOverEnemy = false;
                enemy = null;
            }
        }
    }

    private void ResetPosition()
    {
        if (enemy != null) // Unnecessary?
            UIManager.SelectEnemy(enemy, false);

        cardManager.SetCardParent(gameObject, startParent.transform);
        transform.SetSiblingIndex(startIndex);
        transform.position = new Vector3(startPosition.x, startPosition.y, CardManager.CARD_Z_POSITION);

        if (gameObject.GetComponent<CardDisplay>() is ActionCardDisplay) IsPlayed = false;
        if (IsPlayed) AnimationManager.Instance.RevealedPlayState(gameObject);
        else AnimationManager.Instance.RevealedHandState(gameObject);
    }

    public void StartDrag()
    {
        UIManager.DestroyZoomObjects();
        if (!playerManager.IsMyTurn || CardIsDragging || CompareTag(ENEMY_CARD) || 
            UIManager.PlayerIsTargetting) return;

        FunctionTimer.StopTimer(CardZoom.ZOOM_CARD_TIMER);
        FunctionTimer.StopTimer(CardZoom.ABILITY_POPUP_TIMER);

        IsDragging = true;
        startParent = transform.parent.gameObject;
        startPosition = transform.position;
        startIndex = transform.GetSiblingIndex();
        gameObject.GetComponent<ChangeLayer>().ZoomLayer();

        if (IsPlayed)
        {
            AnimationManager.Instance.DragPlayedState(gameObject);

            foreach(GameObject enemyUnit in CardManager.Instance.EnemyZoneCards)
                if (cardManager.CanAttack(gameObject, enemyUnit, true))
                    UIManager.SelectEnemy(enemyUnit, true);
            if (cardManager.CanAttack(gameObject, cardManager.EnemyHero, true)) 
                UIManager.SelectEnemy(cardManager.EnemyHero, true);
        }
        else AnimationManager.Instance.RevealedDragState(gameObject);
    }

    public void EndDrag()
    {
        if (!IsDragging || !playerManager.IsMyTurn || 
            CompareTag(ENEMY_CARD) || UIManager.PlayerIsTargetting) return;
        IsDragging = false;

        // From Hand
        if (!IsPlayed)
        {
            if (isOverDropZone && cardManager.IsPlayable(gameObject))
            {
                IsPlayed = true;
                cardManager.PlayCard(gameObject);
            }
            else ResetPosition();
            return;
        }
        // In Play
        foreach (GameObject enemyUnit in cardManager.EnemyZoneCards) 
            UIManager.SelectEnemy(enemyUnit, false);
        UIManager.SelectEnemy(cardManager.EnemyHero, false);

        if (isOverEnemy && cardManager.CanAttack(gameObject, enemy))
        {
            ResetPosition(); // NEEDS TO COME BEFORE ATTACK
            cardManager.Attack(gameObject, enemy);
        }
        else
        {
            Debug.Log("EndDrag! (NO ATTACK) IsExhausted = " + gameObject.GetComponent<UnitCardDisplay>().IsExhausted);
            ResetPosition();
        }
    }
}