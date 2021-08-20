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
            if (collisionObject == CardManager.Instance.PlayerZone) 
                isOverDropZone = true;
        }
        else
        {
            if (collisionObjectParent == CardManager.Instance.EnemyZone || 
                collisionObject == CardManager.Instance.EnemyHero)
            {
                isOverEnemy = true;
                enemy = collisionObject;
                UIManager.Instance.SelectEnemy(enemy, true);
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        GameObject collisionObject = collision.gameObject;
        if (!IsPlayed)
        {
            if (collisionObject == CardManager.Instance.PlayerZone) 
                isOverDropZone = false;
        }
        else
        {
            if (collisionObject == enemy)
            {
                UIManager.Instance.SelectEnemy(enemy, false);
                isOverEnemy = false;
                enemy = null;
            }
        }
    }

    private void ResetPosition()
    {
        if (enemy != null) // Unnecessary?
            UIManager.Instance.SelectEnemy(enemy, false);

        cardManager.SetCardParent(gameObject, startParent.transform);
        transform.SetSiblingIndex(startIndex);

        transform.position = new Vector3(startPosition.x, startPosition.y, CardManager.CARD_Z_POSITION);
        //transform.position = startPosition;

        if (gameObject.GetComponent<CardDisplay>() is ActionCardDisplay) IsPlayed = false;
        if (IsPlayed) AnimationManager.Instance.RevealedPlayState(gameObject);
        else AnimationManager.Instance.RevealedHandState(gameObject);
    }

    public void StartDrag()
    {
        UIManager.DestroyZoomObjects();
        if (!playerManager.IsMyTurn || CardIsDragging || CompareTag(ENEMY_CARD) || 
            UIManager.Instance.PlayerIsTargetting) return;
        
        IsDragging = true;
        startParent = transform.parent.gameObject;
        startPosition = transform.position;
        startIndex = transform.GetSiblingIndex();
        gameObject.GetComponent<ChangeLayer>().ZoomLayer();

        if (IsPlayed) AnimationManager.Instance.DragPlayedState(gameObject);
        else AnimationManager.Instance.RevealedDragState(gameObject);
    }

    public void EndDrag()
    {
        if (!IsDragging || !playerManager.IsMyTurn || 
            CompareTag(ENEMY_CARD) || UIManager.Instance.PlayerIsTargetting) return;
        IsDragging = false;

        if (!IsPlayed)
        {
            if (isOverDropZone && cardManager.IsPlayable(gameObject))
            {
                IsPlayed = true;
                cardManager.PlayCard(gameObject);
            }
            else ResetPosition();
        }
        else if (isOverEnemy && cardManager.CanAttack(gameObject, enemy))
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