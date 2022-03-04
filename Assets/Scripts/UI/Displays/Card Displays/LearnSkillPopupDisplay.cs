using UnityEngine;
using TMPro;

public class LearnSkillPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popupText;

    private UIManager uMan;
    private PlayerManager pMan;
    private SkillCard skillCard;

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
    }

    public SkillCard SkillCard
    {
        set
        {
            int aether = pMan.AetherCells;
            skillCard = value;
            string text = "Learn " + skillCard.CardName +
                " for " + GameManager.LEARN_SKILL_COST +
                " aether? (You have " + aether + " aether)";
            PopupText = text;
        }
    }

    public void ConfirmButton_OnClick()
    {
        CardManager.Instance.AddCard(skillCard, GameManager.PLAYER);
        pMan.AetherCells -= GameManager.LEARN_SKILL_COST;
        uMan.CreateCardPagePopup(CardPageDisplay.CardPageType.LearnSkill, true, false);
    }

    public void CancelButton_OnClick() => 
        uMan.DestroyLearnSkillPopup();
}
