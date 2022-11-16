using UnityEngine;
using TMPro;

public class RecruitUnitPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popupText;

    private UIManager uMan;
    private PlayerManager pMan;
    private GameManager gMan;
    private CardManager caMan;

    private UnitCard unitCard;

    private string PopupText
    {
        set
        {
            popupText.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }

    private void Awake()
    {
        uMan = UIManager.Instance;
        pMan = PlayerManager.Instance;
        gMan = GameManager.Instance;
        caMan = CardManager.Instance;
    }

    public UnitCard UnitCard
    {
        set
        {
            int aether = pMan.AetherCells;
            unitCard = value;
            string text = "Recruit <b><u>" + unitCard.CardName + "</u></b>" +
                " for <color=\"yellow\"><b>" + gMan.GetRecruitCost(unitCard, out _) +
                "</b></color> aether? (You have " + aether + " aether)";
            PopupText = text;
        }
    }

    public void ConfirmButton_OnClick()
    {
        caMan.AddCard(unitCard, GameManager.PLAYER, true);
        pMan.AetherCells -= gMan.GetRecruitCost(unitCard, out _);
        int recruitIndex = caMan.PlayerRecruitUnits.FindIndex(x => x.CardName == unitCard.CardName);
        if (recruitIndex != -1) caMan.PlayerRecruitUnits.RemoveAt(recruitIndex);
        else Debug.LogError("RECRUIT UNIT NOT FOUND!");

        bool isReady = false;
        int previousProgress = gMan.RecruitLoyalty;
        if (++gMan.RecruitLoyalty == GameManager.RECRUIT_LOYALTY_GOAL) isReady = true;
        else if (gMan.RecruitLoyalty > GameManager.RECRUIT_LOYALTY_GOAL) gMan.RecruitLoyalty = 0;
        uMan.CreateCardPagePopup(CardPageDisplay.CardPageType.RecruitUnit, false);
        FindObjectOfType<CardPageDisplay>().SetProgressBar(previousProgress, gMan.RecruitLoyalty, isReady);

        AnimationManager.Instance.CreateParticleSystem(gameObject, ParticleSystemHandler.ParticlesType.ButtonPress); // TESTING
    }

    public void CancelButton_OnClick() =>
        uMan.DestroyInteractablePopup(gameObject);
}
