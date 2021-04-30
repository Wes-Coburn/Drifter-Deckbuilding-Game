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

        // LOAD CARD SCRIPTS
        cardScripts.Add(CardScript1);
        cardScripts.Add(CardScript2);
        cardScripts.Add(CardScript3);
    }

    /* CARD_PREFABS */
    public GameObject HeroCardPrefab;
    //public GameObject ActionCardPrefab;

    /* CARD_SCRIPTS_LIST */
    private List<Card> cardScripts = new List<Card>();
    /* CARD_SCRIPTS */
    public HeroCard CardScript1; // cardID: 1
    public HeroCard CardScript2; // cardID: 2
    public ActionCard CardScript3; // cardID: 3

    public GameObject GetCard(int cardID)
    {
        cardID--;
        Card cardScript = cardScripts[cardID];

        GameObject cardPrefab = null;
        if (cardScript is HeroCard) cardPrefab = HeroCardPrefab;
        //else if (cardScript is ActionCard) cardPrefab = ActionPrefab; // ActionCardPrefab NEEDED

        cardPrefab = Instantiate(cardPrefab, new Vector3(0, 0, -1), Quaternion.identity);
        cardPrefab.GetComponent<CardDisplay>().CardScript = cardScript;
        return cardPrefab;
    }
}
