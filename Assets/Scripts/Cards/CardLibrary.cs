using System.Collections.Generic;
using UnityEngine;

public class CardLibrary : MonoBehaviour // REMOVE THIS CLASS EVENTUALLY!!!
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

    [SerializeField] private GameObject FollowerCardPrefab;
    [SerializeField] private GameObject ActionCardPrefab;
    public List<Card> cardScripts;
    public GameObject GetCard(int cardID)
    {
        cardID--;
        Card cardScript = cardScripts[cardID];
        Card cardInstance;
        GameObject cardPrefab;

        if (cardScript is UnitCard)
        {
            cardPrefab = FollowerCardPrefab;
            cardInstance = ScriptableObject.CreateInstance<UnitCard>();
        }
        else if (cardScript is ActionCard)
        {
            cardPrefab = ActionCardPrefab;
            cardInstance = ScriptableObject.CreateInstance<ActionCard>();
        }
        else
        {
            Debug.LogError("Card Type NOT FOUND!");
            return null;
        }

        cardInstance.LoadCard(cardScript);

        cardPrefab = Instantiate(cardPrefab, new Vector3(0, 0, -1), Quaternion.identity);
        cardPrefab.GetComponent<CardDisplay>().CardScript = cardInstance;
        return cardPrefab;
    }
}
 