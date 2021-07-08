using System.Collections.Generic;
using UnityEngine;

public class CardLibrary : MonoBehaviour
{
    /* SINGELTON_PATTERN */
    public static CardLibrary Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    /* CARD_PREFABS */
    [SerializeField] private GameObject FollowerCardPrefab;
    [SerializeField] private GameObject ActionCardPrefab;

    /* CARD_SCRIPTS_LIST */
    public List<Card> cardScripts;
    public GameObject GetCard(int cardID)
    {
        cardID--;
        Card cardScript = cardScripts[cardID];

        GameObject cardPrefab = null;
        if (cardScript is FollowerCard) cardPrefab = FollowerCardPrefab;
        else if (cardScript is ActionCard) cardPrefab = ActionCardPrefab;
        else Debug.LogError("Card Type NOT FOUND!");

        cardPrefab = Instantiate(cardPrefab, new Vector3(0, 0, -1), Quaternion.identity);
        cardPrefab.GetComponent<CardDisplay>().CardScript = cardScript;
        return cardPrefab;
    }
}
 