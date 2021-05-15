using UnityEngine;

public class DragDrop : MonoBehaviour
{
    /* CARD_MANAGER_DATA */
    private const string PLAYER_ZONE = CardManagerData.PLAYER_ZONE;
    private const string ENEMY_ZONE = CardManagerData.ENEMY_ZONE;
    private const string ENEMY_CARD = CardManagerData.ENEMY_CARD;
    private const string BACKGROUND = CardManagerData.BACKGROUND;

    /* GAME_MANAGER_DATA */
    private const string PLAYER = GameManagerData.PLAYER;
    private const string ENEMY = GameManagerData.ENEMY;

    /* MANAGERS */
    private PlayerManager playerManager;
    private CardManager cardManager;
    private UIManager UIManager;

    /* ZONES */
    private GameObject background;
    private GameObject playerZone;
    private GameObject enemyZone;

    /* STATIC CLASS VARIABLES */
    public static bool CardIsDragging = false;

    /* CLASS VARIABLES */
    private GameObject enemy;
    private bool isOverEnemy = false;
    private bool isOverDropZone = false;
    private GameObject startParent;
    private Vector2 startPosition;
    private int startIndex;

    private bool isDragging = false;
    public bool IsDragging
    {
        get => isDragging;
        private set
        {
            isDragging = value;
            CardIsDragging = IsDragging;
        }
    }
    private bool isPlayed = false;

    void Start()
    {
        playerManager = PlayerManager.Instance;
        cardManager = CardManager.Instance;
        UIManager = UIManager.Instance;

        background = GameObject.Find(BACKGROUND);
        playerZone = GameObject.Find(PLAYER_ZONE);
        enemyZone = GameObject.Find(ENEMY_ZONE);
    }

    void Update()
    {
        if (IsDragging)
        {
            Vector3 dragPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(dragPoint.x, dragPoint.y, -2);
            transform.SetParent(background.transform, true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collisionObject = collision.gameObject;
        GameObject collisionObjectParent = collisionObject.transform.parent.gameObject;
        if (!isPlayed)
        {
            if (collisionObject == playerZone) isOverDropZone = true;
        }
        else
        {
            if (collisionObject.CompareTag(ENEMY_CARD) && collisionObjectParent == enemyZone)
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
            if (collisionObject == playerZone) isOverDropZone = false;
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
        if (CompareTag(ENEMY_CARD) || !playerManager.IsMyTurn) return;

        IsDragging = true;
        startParent = transform.parent.gameObject;
        startPosition = transform.position;
        startIndex = transform.GetSiblingIndex();
        gameObject.GetComponent<ChangeLayer>().ZoomLayer();
    }

    public void EndDrag()
    {
        if (CompareTag(ENEMY_CARD) || !playerManager.IsMyTurn) return;

        IsDragging = false;
        if (!isPlayed)
        {
            if (isOverDropZone && cardManager.IsPlayable(gameObject))
            {
                isPlayed = true;
                if (gameObject.CompareTag("PlayerCard")) cardManager.PlayCard(gameObject, PLAYER);
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