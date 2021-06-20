using UnityEngine;

public class DragDrop : MonoBehaviour
{
    /* CARD_MANAGER_DATA */
    private const string PLAYER_CARD = CardManager.PLAYER_CARD;
    private const string ENEMY_CARD = CardManager.ENEMY_CARD;

    /* GAME_MANAGER_DATA */
    private const string PLAYER = GameManager.PLAYER;
    private const string ENEMY = GameManager.ENEMY;

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
    private bool isPlayed;

    void Start()
    {
        playerManager = PlayerManager.Instance;
        cardManager = CardManager.Instance;
        UIManager = UIManager.Instance;

        CardIsDragging = false;
        isOverEnemy = false;
        isOverDropZone = false;
        IsDragging = false;
        isPlayed = false;
    }

    void Update()
    {
        if (IsDragging)
        {
            Vector3 dragPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(dragPoint.x, dragPoint.y, -2);
            transform.SetParent(UIManager.Instance.CurrentBackground.transform, true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("OnCollisionEnter2D!");
        GameObject collisionObject = collision.gameObject;
        GameObject collisionObjectParent = collisionObject.transform.parent.gameObject;
        if (!isPlayed)
        {
            if (collisionObject == CardManager.Instance.PlayerZone) isOverDropZone = true;
        }
        else
        {
            if (collisionObjectParent == CardManager.Instance.EnemyZone || collisionObject == CardManager.Instance.EnemyChampion)
            {
                isOverEnemy = true;
                enemy = collisionObject;
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        GameObject collisionObject = collision.gameObject;
        if (!isPlayed)
        {
            if (collisionObject == CardManager.Instance.PlayerZone) isOverDropZone = false;
        }
        else
        {
            if (collisionObject == enemy)
            {
                isOverEnemy = false;
                enemy = null;
            }
        }
    }

    private void ResetPosition()
    {
        transform.position = startPosition;
        cardManager.SetCardParent(gameObject, startParent.transform);
        transform.SetSiblingIndex(startIndex);
    }

    public void StartDrag()
    {
        UIManager.DestroyAllZoomObjects();
        if (!playerManager.IsMyTurn || CompareTag(ENEMY_CARD)) return;
        IsDragging = true;

        startParent = transform.parent.gameObject;
        startPosition = transform.position;
        startIndex = transform.GetSiblingIndex();
        gameObject.GetComponent<ChangeLayer>().ZoomLayer();
    }

    public void EndDrag()
    {
        if (!IsDragging || !playerManager.IsMyTurn || CompareTag(ENEMY_CARD)) return;
        IsDragging = false;

        if (!isPlayed)
        {
            if (isOverDropZone && cardManager.IsPlayable(gameObject))
            {
                isPlayed = true;
                if (gameObject.CompareTag(PLAYER_CARD)) cardManager.PlayCard(gameObject, PLAYER);
                else cardManager.PlayCard(gameObject, ENEMY);
            }
            else ResetPosition();
        }
        else if (isOverEnemy && cardManager.CanAttack(gameObject))
        {
            ResetPosition();
            cardManager.Attack(gameObject, enemy);
        }
        else ResetPosition();
    }
}